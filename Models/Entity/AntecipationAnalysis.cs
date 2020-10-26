using System;

namespace payment_api.Models
{
    public class AntecipationAnalysis : DbEntity
    {
        public DateTime? StartDate { get; set; }

        public DateTime? EndDate { get; set; }

        public string FinalStatus { get; set; }

        public int AntecipationId { get; set; }
    }
}