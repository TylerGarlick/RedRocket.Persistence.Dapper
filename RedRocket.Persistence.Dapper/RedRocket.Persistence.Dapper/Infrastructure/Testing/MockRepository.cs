using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace RedRocket.Persistence.Dapper.Infrastructure.Testing
{
    public class MockRepository<T> : IBasicRepository<T>
    {
        protected List<T> Entities { get; set; }

        public MockRepository()
        {
            Entities = Enumerable.Empty<T>().ToList();
        }

        public MockRepository(IEnumerable<T> seeds)
        {
            Entities = seeds.ToList();
        }

        public IQueryable<T> All()
        {
            return Entities.AsQueryable();
        }

        public IEnumerable<T> Query(Func<T, bool> predicate)
        {
            return All().Where(predicate);
        }

        public T FindByKey(Expression<Func<T, bool>> predicate)
        {
            return All().AsQueryable().SingleOrDefault(predicate);
        }

        public T Add(T entity)
        {
            Entities.Add(entity);
            return entity;
        }

        public T Update(T entity)
        {
            Entities.Remove(entity);
            Entities.Add(entity);
            return entity;
        }

        public void Delete(T entity)
        {
            Entities.Remove(entity);
        }
    }
}