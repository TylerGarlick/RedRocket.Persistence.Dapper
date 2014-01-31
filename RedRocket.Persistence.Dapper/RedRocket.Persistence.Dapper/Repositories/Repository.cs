using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Dapper;
using RedRocket.Persistence.Dapper.Infrastructure;
using RedRocket.Persistence.Dapper.Infrastructure.Configuration;
using RedRocket.Persistence.Dapper.Infrastructure.Expressions;

namespace RedRocket.Persistence.Dapper.Repositories
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

        public IEnumerable<T> All()
        {
            using (var connection = Connection.Open())
            {
                var query = new Linq<T>();
                var sql = query.Render().ToSql();
                LogSqlQuery(sql);
                return connection.Query<T>(sql);
            }
        }

        public IEnumerable<T> All(IEnumerable<Linq<T>.OrderByExpression> sortingExpression)
        {
            using (var connection = Connection.Open())
            {
                var query = new Linq<T>();

                var orderByExpressions = sortingExpression as IList<Linq<T>.OrderByExpression> ?? sortingExpression.ToList();
                if (orderByExpressions.Any())
                    query.OrderByExpressions.AddRange(orderByExpressions);

                var sql = query.Render().ToSql();
                LogSqlQuery(sql);

                return connection.Query<T>(sql);
            }
        }

        public IEnumerable<T> EntitiesBy(Expression<Func<T, bool>> predicate, IEnumerable<Linq<T>.OrderByExpression> sortingExpression)
        {
            using (var connection = Connection.Open())
            {
                var query = new Linq<T>().Where(predicate);

                var orderByExpressions = sortingExpression as IList<Linq<T>.OrderByExpression> ?? sortingExpression.ToList();
                if (sortingExpression != null && orderByExpressions.Any())
                    query.OrderByExpressions.AddRange(orderByExpressions);

                var result = query.Render();
                LogSqlQuery(result.ToSql());
                return connection.Query<T>(result.ToSql(), result.Parameters.ToDynamicThenObject());
            }
        }

        public IEnumerable<T> EntitiesBy(Expression<Func<T, bool>> predicate)
        {
            using (var connection = Connection.Open())
            {
                var query = new Linq<T>().Where(predicate).Render();
                LogSqlQuery(query.ToSql());
                return connection.Query<T>(query.ToSql(), query.Parameters.ToDynamicThenObject());
            }
        }

        public T FindByKey(Expression<Func<T, bool>> predicate)
        {
            using (var connection = Connection.Open())
            {
                var query = new Linq<T>().Where(predicate).Render();
                LogSqlQuery(query.ToSql());
                return connection.Query<T>(query.ToSql(), query.Parameters.ToDynamicThenObject()).SingleOrDefault();
            }
        }

        public T Add(T entity)
        {
            using (var transaction = Connection.OpenAndCreateTransaction())
            {
                var insertQuery = new LinqInsert<T>(entity);
                var sql = insertQuery.Render().ToSql();
                var rows = transaction.Connection.Execute(sql, entity);
                if (rows == 0)
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
                var sql = updateQuery.Render().ToSql();
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
                var deleteQuery = new LinqDelete<T>(entity).Render();
                LogSqlQuery(deleteQuery.ToSql());
                var rows = transaction.Connection.Execute(deleteQuery.ToSql(), deleteQuery.Parameters.ToDynamicThenObject());

                if (rows > 0)
                    transaction.Commit();
                else
                    transaction.Rollback();
            }

        }

        public void BulkAdd(IEnumerable<T> entities)
        {
            using (var transaction = Connection.OpenAndCreateTransaction())
            {
                var enumeratedEntities = entities as IList<T> ?? entities.ToList();

                var insertQuery = new LinqInsert<T>(enumeratedEntities.First());
                var sql = insertQuery.Render().ToSql();
                var rows = transaction.Connection.Execute(sql, entities);

                if (rows == enumeratedEntities.Count())
                    transaction.Commit();
                else
                    transaction.Rollback();
            }
        }

        public void BulkInsert(IEnumerable<T> entities)
        {
            var enumeratedEntities = entities as IList<T> ?? entities.ToList();
            if (enumeratedEntities.Any())
            {
                using (var transaction = Connection.OpenAndCreateTransaction())
                {
                    var updateQuery = new LinqUpdate<T>(enumeratedEntities.First());
                    var sql = updateQuery.Render().ToSql();
                    var rows = transaction.Connection.Execute(sql, enumeratedEntities);
                    if (enumeratedEntities.Count() == rows)
                        transaction.Commit();
                    else
                        transaction.Rollback();
                }
            }
        }

        public void BulkDelete(IEnumerable<T> entities)
        {
            var enumeratedEntities = entities as IList<T> ?? entities.ToList();
            if (enumeratedEntities.Any())
            {
                using (var transaction = Connection.OpenAndCreateTransaction())
                {
                    var deleteQuery = new LinqDelete<T>();

                    var sql = deleteQuery.Render().ToSql();
                    var rows = transaction.Connection.Execute(sql, enumeratedEntities);
                    if (enumeratedEntities.Count() == rows)
                        transaction.Commit();
                    else
                        transaction.Rollback();
                }
            }
        }

        void LogSqlQuery(string sql)
        {
            Logger.Write(sql);
        }
    }
}