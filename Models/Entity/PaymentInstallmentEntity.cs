using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace payment_api.Models
{
    public class PaymentInstallmentEntity : DbEntity
    {
        [Required(ErrorMessage = "PaymentId is required")]
        public int PaymentId { get; set; }

        [Required(ErrorMessage = "RawValue is required")]
        public double RawValue { get; set; }

        [Required(ErrorMessage = "LiquidValue is required")]
        public double LiquidValue { get; set; }

        [Required(ErrorMessage = "InstallmentNumber is required")]
        public int InstallmentNumber { get; set; }
        public double? AnticipatedValue { get; set; }

        [Required(ErrorMessage = "DueDate is required")]
        public DateTime DueDate { get; set; }

        public DateTime? AnticipatedTranferDate { get; set; }
    }
}