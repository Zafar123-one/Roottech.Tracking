using System;
using System.Data;
using NHibernate;
using Roottech.Tracking.Common.Repository.Interfaces;

namespace Roottech.Tracking.Common.Repository.Implemenations
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ISessionFactory _sessionFactory;
        private readonly IInterceptor _dataBindingIntercepter;
        private ITransaction _transaction;

        public ISession Session { get; private set; }

        public UnitOfWork(ISessionFactory sessionFactory)
        {
            _sessionFactory = sessionFactory;
            OpenNewSession();
            BeginTransaction();
        }

        public UnitOfWork(ISessionFactory sessionFactory, IInterceptor dataBindingIntercepter)
        {
            _sessionFactory = sessionFactory;
            _dataBindingIntercepter = dataBindingIntercepter;
            OpenNewSession();
            BeginTransaction();
        }

        ~UnitOfWork()
        {
            Dispose();
        }

        private void OpenNewSession()
        {
            Session = _dataBindingIntercepter == null
                ? _sessionFactory.GetCurrentSession() ?? _sessionFactory.OpenSession()
                : _sessionFactory.OpenSession(_dataBindingIntercepter);
            Session.FlushMode = FlushMode.Auto;
        }

        public void Dispose()
        {
            lock (Session) if (Session.IsOpen) Session.Close();

            GC.SuppressFinalize(this);
        }

        public void BeginTransaction()
        {
            if (!Session.IsOpen) OpenNewSession();
            _transaction = Session.BeginTransaction(IsolationLevel.ReadCommitted);
        }

        public void Commit()
        {
            if (!_transaction.IsActive) throw new InvalidOperationException("No active transation");
            _transaction.Commit();
        }

        public void Rollback()
        {
            if (_transaction.IsActive) _transaction.Rollback();
        }
         
    }
}