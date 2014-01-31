using System.Linq;
using Dapper;
using FlitBit.IoC;
using RedRocket.Persistence.Dapper.Infrastructure.Expressions;
using RedRocket.Persistence.Dapper.Tests.Dtos;
using RedRocket.Persistence.Dapper.Tests.Infrastructure;
using Xunit;

namespace RedRocket.Persistence.Dapper.Tests.Repositories
{
    public class SelectRepositoryIntegrationTests : AbstractTests
    {
        [Fact]
        public void CanGetAllEntitiesUsingLinq()
        {
            using (Create.SharedOrNewContainer())
            using (var db = Db.NewConnection())
            {
                var repository = SetupReadOnlyRepository();
                var words = repository.EntitiesBy(w => w.Word.StartsWith("hat"));
                Logger.Write("Total Words: {0}".P(words.Count()));
                Assert.NotNull(words);
            }
        }

        [Fact]
        public void CanGetAllEntitiesUsingLinqAndSort()
        {
            using (Create.SharedOrNewContainer())
            using (var db = Db.NewConnection())
            {
                var repository = SetupReadOnlyRepository();
                var words = repository.EntitiesBy(w => w.Word.StartsWith("hat"), new[] { new Linq<IWord>.OrderByExpression() { Ascending = true, Expression = w => w.Word } }.AsEnumerable());
                Logger.Write("Total Words: {0}".P(words.Count()));
                Assert.NotNull(words);
            }
        }

        [Fact]
        public void CanGetSingleEntityById()
        {
            using (Create.SharedOrNewContainer())
            using (var db = Db.NewConnection())
            {
                var repository = SetupReadOnlyRepository();
                var word = repository.FindByKey(w => w.Id == 7524972);
                Assert.Equal(7524972, word.Id);
                Assert.Equal(word.Word, "a peck of troubles");
            }
        }

        [Fact]
        public void CanGetAllEntitiesUsingRawSql()
        {
            using (Create.SharedOrNewContainer())
            using (var db = Db.NewConnection())
            {
                var words = db.Query<IWord>("SELECT * FROM wrd_word_m WHERE word like @word", new { word = "hat%" });
                Logger.Write("Total Words: {0}".P(words.Count()));
                Assert.NotNull(words);
            }
        }


        IRepository<IWord> SetupReadOnlyRepository()
        {
            var repository = Create.New<IRepository<IWord>>();
            Assert.NotNull(repository);
            return repository;
        }
    }
}