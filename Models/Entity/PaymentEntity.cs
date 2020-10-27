using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace payment_api.Models
{
    public class PaymentEntity : DbEntity
    {
        public DateTime TransactionDate { get; set; }

        public DateTime? AprovalDate { get; set; }

        public DateTime? CancelDate { get; set; }

        public bool? Anticipated { get; set; }

        public bool Approved { get; set; }

        public double RawValue { get; set; }

        public double LiquidValue { get; set; }

        public double Tax { get; set; }

        [NotMapped]
        public List<PaymentInstallmentEntity> PaymentInstallments { get; set; } = new List<PaymentInstallmentEntity>();

        public string CreditCard { get; set; }

        public int PaymentInstallmentCount { get; set; }

        [NotMapped]
        public int Nsu { get => Id; }

        public int? AnticipationId { get; set; }
    }
}