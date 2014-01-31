using FlitBit.IoC;
using RedRocket.Persistence.Dapper.Tests.Dtos;
using RedRocket.Persistence.Dapper.Tests.Infrastructure;
using Xunit;

namespace RedRocket.Persistence.Dapper.Tests
{
    public class TableAnnotationTests : AbstractTests
    {
        [Fact]
        public void CanCreateTableAsIfDto()
        {
            using (Create.SharedOrNewContainer())
            {
                var country = Create.NewInit<ICountry>().Init(new { CountryCode = "USA" });
                Assert.NotNull(country);
                Assert.Equal("USA", country.CountryCode);
            }
        }
    }
}