using RedRocket.Persistence.Dapper.Infrastructure.Attributes;

namespace RedRocket.Persistence.Dapper.Tests.Dtos
{
    [Table("Country")]
    public interface ICountry
    {
        [PrimaryKey("Country_Code")]
        string CountryCode { get; set; }

        [Column("Country_Name")]
        string Name { get; set; }
    }
}