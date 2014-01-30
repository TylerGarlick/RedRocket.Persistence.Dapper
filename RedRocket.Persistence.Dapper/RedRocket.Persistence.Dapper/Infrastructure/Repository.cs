using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Dapper;
using RedRocket.Persistence.Dapper.Infrastructure.Configuration;
using RedRocket.Persistence.Dapper.Infrastructure.Expressions;

namespace RedRocket.Persistence.Dapper.Infrastructure
{
    public class Repository<T> : IBatchRepository<T>
    {
        public IConnection Connection { get; set; }

        public Repository()
        {
            Connection = Db.Current;
        }

        public Repository(IConnection connection)
        {
            Connection = connection;
        }

        public IQueryable<T> All()
        {
            using (var connection = Connection.Open())
            {
                var query = new Linq<T>();
                var sql = query.ToSql().ToQuery();
                return connection.Query<T>(sql).AsQueryable();
            }
        }

        public IQueryable<T> All(string sql)
        {
            using (var connection = Connection.Open())
                return connection.Query<T>(sql).AsQueryable();
        }

        public IQueryable<T> All(IEnumerable<Linq<T>.OrderByExpression> sortingExpression)
        {
            using (var connection = Connection.Open())
            {
                var query = new Linq<T>();

                var orderByExpressions = sortingExpression as IList<Linq<T>.OrderByExpression> ?? sortingExpression.ToList();
                if (orderByExpressions.Any())
                    query.OrderByExpressions.AddRange(orderByExpressions);

                var sql = query.ToSql().ToQuery();
                return connection.Query<T>(sql).AsQueryable();
            }
        }

        public IQueryable<T> EntitiesBy(string sql)
        {
            using (var connection = Connection.Open())
                return connection.Query<T>(sql).AsQueryable();
        }

        public IQueryable<T> EntitiesBy(Expression<Func<T, bool>> predicate, IEnumerable<Linq<T>.OrderByExpression> sortingExpression)
        {
            using (var connection = Connection.Open())
            {
                var query = new Linq<T>().Where(predicate);

                var orderByExpressions = sortingExpression as IList<Linq<T>.OrderByExpression> ?? sortingExpression.ToList();
                if (sortingExpression != null && orderByExpressions.Any())
                    query.OrderByExpressions.AddRange(orderByExpressions);

                var sql = query.ToSql().ToQuery();
                return connection.Query<T>(sql).AsQueryable();
            }
        }

        public IQueryable<T> EntitiesBy(Expression<Func<T, bool>> predicate)
        {
            using (var connection = Connection.Open())
            {
                var query = new Linq<T>().Where(predicate);
                var sql = query.ToSql().ToQuery();
                return connection.Query<T>(sql).AsQueryable();
            }
        }

        public T FindByKey(Expression<Func<T, bool>> predicate)
        {
            using (var connection = Connection.Open())
            {
                var query = new Linq<T>().Where(predicate);
                var sql = query.ToSql().ToQuery();
                return connection.Query<T>(sql).SingleOrDefault();
            }
        }

        public T Add(T entity)
        {
            using (var transaction = Connection.OpenAndCreateTransaction())
            {
                var insertQuery = new LinqInsert<T>(entity);
                var sql = insertQuery.ToSql().ToQuery();
                var rows = transaction.Connection.Execute(sql, entity);
                if (rows > 0)
                    transaction.Commit();
                else
                    transaction.Rollback();

                return entity;
            }
        }

        public T Update(T entity)
        {
            using (var transaction = Connection.OpenAndCreateTransaction())
            {
                var updateQuery = new LinqUpdate<T>(entity);
                var sql = updateQuery.ToSql().ToQuery();
                var rows = transaction.Connection.Execute(sql, entity);
                if (rows == 1)
                    transaction.Commit();
                else
                    transaction.Rollback();

                return entity;
            }
        }

        public void Delete(T entity)
        {
            using (var transaction = Connection.OpenAndCreateTransaction())
            {
                var updateQuery = new LinqDelete<T>(entity);
                var sql = updateQuery.ToSql().ToQuery();
                var rows = transaction.Connection.Execute(sql, entity);
                if (rows == 1)
                    transaction.Commit();
                else
                    transaction.Rollback();
            }
        }

        public void BatchAdd(IEnumerable<T> entities)
        {
            throw new NotImplementedException();
        }

        public void BatchUpdate(IEnumerable<T> entities)
        {
            throw new NotImplementedException();
        }

        public void BatchDelete(IEnumerable<T> entities)
        {
            throw new NotImplementedException();
        }
    }
}