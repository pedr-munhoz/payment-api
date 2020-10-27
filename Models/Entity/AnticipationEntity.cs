using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace payment_api.Models
{
    public class AnticipationEntity : DbEntity
    {
        public DateTime SolicitationDate { get; set; }

        [NotMapped]
        public AnticipationAnalysis Analysis { get; set; }

        public double SolicitedValue { get; set; }

        public double? AnticipatedValue { get; set; }

        [NotMapped]
        public List<PaymentEntity> SolicitedPayments { get; set; } = new List<PaymentEntity>();
    }
}