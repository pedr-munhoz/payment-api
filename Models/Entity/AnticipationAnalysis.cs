using System;

namespace payment_api.Models
{
    public class AnticipationAnalysis : DbEntity
    {
        public DateTime? StartDate { get; set; }

        public DateTime? EndDate { get; set; }

        public string FinalStatus { get; set; }

        public int AnticipationId { get; set; }
    }
}