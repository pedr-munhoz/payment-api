using System;
using System.ComponentModel.DataAnnotations;

namespace payment_api.Models
{
    public class PaymentInstallmentEntity : DbEntity
    {
        [Required(ErrorMessage = "PaymentId is required")]
        public int PaymentId { get; set; }

        [Required(ErrorMessage = "RawValue is required")]
        public float RawValue { get; set; }

        [Required(ErrorMessage = "LiquidValue is required")]
        public float LiquidValue { get; set; }

        [Required(ErrorMessage = "InstallmentNumber is required")]
        public int InstallmentNumber { get; set; }
        public float? AnticipatedValue { get; set; }

        [Required(ErrorMessage = "DueDate is required")]
        public DateTime DueDate { get; set; }

        public DateTime? AntecipatedTranfer { get; set; }
    }
}