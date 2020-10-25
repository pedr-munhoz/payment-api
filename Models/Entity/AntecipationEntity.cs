using System;
using System.Collections.Generic;

namespace payment_api.Models
{
    public class AntecipationEntity : DbEntity
    {
        public DateTime SolicitationDate { get; set; }

        public AntecipationAnalysis Analysis { get; set; }

        public double SolicitedValue { get; set; }

        public double AntecipatedValue { get; set; }

        public HashSet<PaymentEntity> SolicitedPayments { get; set; }
    }
}