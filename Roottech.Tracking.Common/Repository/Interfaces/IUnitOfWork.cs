using System;
using NHibernate;

namespace Roottech.Tracking.Common.Repository.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        ISession Session { get; }
        void BeginTransaction();
        void Commit();
        void Rollback();
         
    }
}