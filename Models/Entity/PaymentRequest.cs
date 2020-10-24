using System.ComponentModel.DataAnnotations;

namespace payment_api.Models
{
    public class PaymentRequest
    {
        [Required]
        public float RawValue { get; set; }
        [Required]
        public int PaymentInstallmentCount { get; set; }
        [Required]
        public string CreditCard { get; set; }
    }
}