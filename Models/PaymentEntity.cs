using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace payment_api.Models
{
    public class PaymentEntity : DbEntity
    {
        public PaymentEntity()
        {
            var installments = new List<PaymentInstallmentEntity>();
            installments.Add(new PaymentInstallmentEntity());
            PaymentInstallments = installments;
        }

        public DateTime TransactionDate { get; set; }

        public DateTime? AprovalDate { get; set; }

        public DateTime? CancelDate { get; set; }

        public bool Anticipated { get; set; }

        public bool Approved { get; set; }

        public float RawValue { get; set; }

        public float LiquidValue { get; set; }

        public float Tax { get; set; }

        public List<PaymentInstallmentEntity> PaymentInstallments { get; set; }

        public int PaymentInstallmentCount { get => PaymentInstallments.Count; }

        public string CreditCard { get; set; }
    }
}