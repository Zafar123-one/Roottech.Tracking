using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using NHibernate;
using NHibernate.Criterion;
using NHibernate.Linq;
using NHibernate.Persister.Entity;
using NHibernate.Transform;
using Roottech.Tracking.Common.Entities;
using Roottech.Tracking.Common.Repository.Interfaces;

namespace Roottech.Tracking.Common.Repository.Implemenations
{
    public class Repository<TKey, T> : IKeyedRepository<TKey, T> where T : class, IKeyed<TKey>
    {
        private readonly ISession _session;

        public Repository(ISessionFactory sessionFactory)
        {
            _session = sessionFactory.GetCurrentSession();
        }

        ~Repository()
        {
            Dispose();
        }

        public void Dispose()
        {
            //lock (_session) if (_session.IsOpen) _session.Close(); Commented coz NHibernateSessionModule was getting session.closed when committing the transaction.
            GC.SuppressFinalize(this);
        }

        #region IRepository<T> Members

        public bool Add(T entity)
        {
            _session.Save(entity);
            return true;
        }

        public bool Add(IEnumerable<T> entities)
        {
            return entities.Aggregate(true, (current, entity) => Add(entity) && current);
        }

        ///The stateless session is especially useful for reporting situations or for batch processing. 
        ///Use IStatelessSession to delete or update multiple objects. It will be faster because the identity map will not slow down the session/operations.
        public bool Merge(T entity)
        {
            if (entity != null)
                if (entity.Id != null)
                {
                    if (FindBy(entity.Id) == null) return false;
                    //_session.Update(entity);
                    _session.Merge(entity);
                }
            return true;
        }

        public bool Merge(IEnumerable<T> entities)
        {
            return entities.Aggregate(true, (current, entity) => Merge(entity) && current);
        }

        public bool Delete(TKey id)
        {
            T toDelete = FindBy(id);
            if (toDelete == null) return false;
            _session.Delete(toDelete);
            return true;
        }

        public bool Delete(T entity)
        {
            T toDelete = FindBy(entity.Id);
            if (toDelete == null) return false;
            _session.Delete(toDelete);
            return true;
        }

        public bool Delete(IEnumerable<T> entities)
        {
            return entities.Aggregate(true, (current, entity) => Delete(entity) && current);
        }

        public void Evict(T entity)
        {
            _session.Evict(entity);
        }

        public void Evict(IEnumerable<T> entities)
        {
            _session.Evict(entities);
        }

        #endregion

        #region IKeyedRepository<TKey, T> Members

        public IQueryable<T> All()
        {
            var queryable = _session.Query<T>();
            if (Cacheable(typeof(T).FullName)) return queryable;
            return queryable.Cacheable().CacheRegion("DPW.EACCS");//.AsQueryable();// .Linq<T>();
        }

        public T FindBy(TKey id)
        {
            ///When using the Load method NHibernate only instantiates a proxy for the given entity
            //return _session.Load<T>(id);
            ///When using the Get method NHibernate loads this record and instantiates a fully populated entity in memory and immediately puts this entity into the entity map (or first level cache) 
            return _session.Get<T>(id);
        }

        public T FindBy(Expression<Func<T, bool>> expression)
        {
            return FilterBy(expression).Single();
        }

        public IQueryable<T> FilterBy(Expression<Func<T, bool>> expression)
        {
            var queryOver = _session.QueryOver<T>().Where(expression);
            if (Cacheable(typeof(T).FullName)) return queryOver.List<T>().AsQueryable();
            return queryOver.Cacheable().CacheRegion("DPW.EACCS").List<T>().AsQueryable();
        }

        private bool Cacheable(string entityName)
        {
            return ((SingleTableEntityPersister)(_session.SessionFactory.GetClassMetadata(entityName))).Cache == null;
        }

        public IEnumerable<T> Future()
        {
            var queryOver = _session.QueryOver<T>();
            if (Cacheable(typeof(T).FullName)) return queryOver.Future<T>();
            return queryOver.Cacheable().CacheRegion("DPW.EACCS").Future<T>();
        }

        public IEnumerable<T> Future(Expression<Func<T, bool>> expression)
        {
            var queryOver = _session.QueryOver<T>().Where(expression);
            if (Cacheable(typeof(T).FullName)) return queryOver.Future<T>();
            return queryOver.Cacheable().CacheRegion("DPW.EACCS").Future<T>();
        }

        public IFutureValue<TFType> FutureValue<TFType>(IProjection projection)
        {
            var queryOver = _session.QueryOver<T>().Select(projection);
            if (Cacheable(typeof(T).FullName)) return queryOver.FutureValue<TFType>();
            return queryOver.Cacheable().CacheRegion("DPW.EACCS").FutureValue<TFType>();
        }

        public IFutureValue<TFType> FutureValue<TFType>(Expression<Func<T, bool>> expression, IProjection projection)
        {
            var queryOver = _session.QueryOver<T>().Where(expression).Select(projection);
            if (Cacheable(typeof(T).FullName)) return queryOver.FutureValue<TFType>();
            return queryOver.Cacheable().CacheRegion("DPW.EACCS").FutureValue<TFType>();
        }

        public IEnumerable<T> GetNamedQueryFuture(string queryName)
        {
            return _session.GetNamedQuery(queryName).SetCacheable(true).SetCacheRegion("DPW.EACCS").Future<T>();
        }

        public IList<T> GetNamedQuery(string queryName)
        {
            return _session.GetNamedQuery(queryName).SetCacheable(true).SetCacheRegion("DPW.EACCS").List<T>();
        }

        public IQueryOver<T, T> QueryOver()
        {
            return _session.QueryOver<T>();
        }

        public IQueryOver<T, T> QueryOver(Expression<Func<T>> alias)
        {
            return _session.QueryOver(alias);
        }

        public IEnumerable<T> QueryOver(Expression<Func<T, bool>> expression, IList<Expression<Func<T, object>>> expressionProperties, int? take, int? skip)
        {
            var projectionList = Projections.ProjectionList();
            foreach (var propertyLambda in expressionProperties)
            {
                var memberExpression = propertyLambda.Body as MemberExpression ??
                                       ((MemberExpression)((UnaryExpression)propertyLambda.Body).Operand);

                projectionList.Add(Projections.Property(propertyLambda).As(memberExpression.Member.Name));
            }

            var queryOver = _session.QueryOver<T>();
            if (expression != null) queryOver.Where(expression);
            if (skip != null) queryOver.Take((int)take).Skip((int)skip);
            return queryOver.Select(Projections.Distinct(projectionList))
                .TransformUsing(Transformers.AliasToBean<T>()).Future<T>();
        }

        public IQueryable<T> QueryOver(Expression<Func<T, bool>> expression, IList<Expression<Func<T, object>>> expressionProperties)
        {
            var projectionList = Projections.ProjectionList();
            foreach (var propertyLambda in expressionProperties)
            {
                var memberExpression = propertyLambda.Body as MemberExpression ??
                                       ((MemberExpression)((UnaryExpression)propertyLambda.Body).Operand);

                projectionList.Add(Projections.Property(propertyLambda).As(memberExpression.Member.Name));
            }

            var queryOver = _session.QueryOver<T>();
            if (expression != null) queryOver.Where(expression);
            //if (skip != null) queryOver.Take((int)take).Skip((int)skip);
            return queryOver.Select(Projections.Distinct(projectionList))
                .TransformUsing(Transformers.AliasToBean<T>()).List<T>().AsQueryable();
        }

        public IQueryOver<T, T> QueryOver(IList<Expression<Func<T, object>>> expressionProperties)
        {
            var projectionList = Projections.ProjectionList();
            foreach (var propertyLambda in expressionProperties)
            {
                var memberExpression = propertyLambda.Body as MemberExpression ??
                                       ((MemberExpression)((UnaryExpression)propertyLambda.Body).Operand);

                projectionList.Add(Projections.Property(propertyLambda).As(memberExpression.Member.Name));
            }

            return _session.QueryOver<T>().Select(Projections.Distinct(projectionList))
                .TransformUsing(Transformers.AliasToBean<T>());
        }

        public void EnableFilter(string filterName, string parameterName, object value)
        {
            _session.EnableFilter(filterName).SetParameter(parameterName, value);
        }

        #endregion
    }
}