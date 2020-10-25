using System;

namespace payment_api.Models
{
    public class AntecipationAnalysis : DbEntity
    {
        public DateTime? StartDate { get; set; }

        public DateTime? EndDate { get; set; }

        public AnalysisResultStatus? FinalStatus { get; set; }
    }
}