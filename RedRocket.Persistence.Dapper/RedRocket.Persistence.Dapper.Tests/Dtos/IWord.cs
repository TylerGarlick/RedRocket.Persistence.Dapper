using System;
using RedRocket.Persistence.Dapper.Infrastructure.Attributes;

namespace RedRocket.Persistence.Dapper.Tests.Dtos
{
    [Table("wrd_word_m")]
    public interface IWord
    {
        [Column("word_id")]
        Int64 Id { get; set; }
        
        [Column("lang_code")]
        string LanguageCode { get; set; }

        [Column("word")]
        string Word { get; set; }
    }
}