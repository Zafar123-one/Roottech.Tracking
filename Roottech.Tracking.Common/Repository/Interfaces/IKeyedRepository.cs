using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using NHibernate;
using NHibernate.Criterion;
using Roottech.Tracking.Common.Entities;

namespace Roottech.Tracking.Common.Repository.Interfaces
{
    public interface IKeyedRepository<TKey, T> : IRepository<T> where T : class, IKeyed<TKey>
    {
        IQueryable<T> All();
        T FindBy(TKey id);
        bool Delete(TKey id); // Its suppose to be in IRepository but coz of TKey
        T FindBy(Expression<Func<T, bool>> expression);
        IQueryable<T> FilterBy(Expression<Func<T, bool>> expression);
        IEnumerable<T> Future();
        IEnumerable<T> Future(Expression<Func<T, bool>> expression);
        IFutureValue<TFType> FutureValue<TFType>(IProjection projection);
        IFutureValue<TFType> FutureValue<TFType>(Expression<Func<T, bool>> expression, IProjection projection);
        IEnumerable<T> GetNamedQueryFuture(string queryName);
        IList<T> GetNamedQuery(string queryName);
        IQueryOver<T, T> QueryOver();
        IQueryOver<T, T> QueryOver(Expression<Func<T>> alias);
        IEnumerable<T> QueryOver(Expression<Func<T, bool>> expression, IList<Expression<Func<T, object>>> expressionProperties, int? take, int? skip);
        IQueryable<T> QueryOver(Expression<Func<T, bool>> expression, IList<Expression<Func<T, object>>> expressionProperties);
        IQueryOver<T, T> QueryOver(IList<Expression<Func<T, object>>> expressionProperties);
        void EnableFilter(string filterName, string parameterName, object value);
         
    }
}