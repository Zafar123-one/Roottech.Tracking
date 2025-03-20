using System;
using System.Collections.Generic;

namespace Roottech.Tracking.Common.Repository.Interfaces
{
    public interface IRepository<T> : IDisposable where T : class
    {
        bool Add(T entity);
        bool Add(IEnumerable<T> entities);
        bool Merge(T entity);
        bool Merge(IEnumerable<T> entities);
        bool Delete(T entity);
        bool Delete(IEnumerable<T> entities);
        void Evict(T entity);
        void Evict(IEnumerable<T> entities);
    }
}