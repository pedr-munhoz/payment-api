using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace payment_api.Models
{
    public class AntecipationEntity : DbEntity
    {
        public DateTime SolicitationDate { get; set; }

        public AntecipationAnalysis Analysis { get; set; }

        public double SolicitedValue { get; set; }

        public double? AntecipatedValue { get; set; }

        [NotMapped]
        public List<PaymentEntity> SolicitedPayments { get; set; } = new List<PaymentEntity>();
    }
}