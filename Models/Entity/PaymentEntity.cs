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

        [Required(ErrorMessage = "TransactionDate is required")]
        public DateTime TransactionDate { get; set; }

        public DateTime? AprovalDate { get; set; }

        public DateTime? CancelDate { get; set; }

        [Required(ErrorMessage = "Anticipated is required")]
        public bool Anticipated { get; set; }

        [Required(ErrorMessage = "Approved is required")]
        public bool Approved { get; set; }

        [Required(ErrorMessage = "RawValue is required")]
        public float RawValue { get; set; }

        [Required(ErrorMessage = "LiquidValue is required")]
        public float LiquidValue { get; set; }

        [Required(ErrorMessage = "Tax is required")]
        public float Tax { get; set; }

        [Required(ErrorMessage = "PaymentInstallments is required")]
        public List<PaymentInstallmentEntity> PaymentInstallments { get; set; }

        [Required(ErrorMessage = "CreditCard is required")]
        public string CreditCard { get; set; }

        public int PaymentInstallmentCount { get => PaymentInstallments.Count; }

        public int Nfu { get => Id; }
    }
}