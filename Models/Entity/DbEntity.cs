using System.ComponentModel.DataAnnotations;

namespace payment_api.Models
{
    /// <summary>
    /// Defines mandatory information for every model that is persisted in the database
    /// </summary>
    public abstract class DbEntity
    {
        public int Id { get; set; }
    }
}