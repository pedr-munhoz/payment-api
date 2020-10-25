using System.ComponentModel.DataAnnotations;

namespace payment_api.Models
{
    public class PaymentRequest
    {
        [Required]
        public double RawValue { get; set; }

        [Required]
        public int PaymentInstallmentCount { get; set; }

        [Required]
        [RegularExpression("^[0-9]{16}$", ErrorMessage = "CreditCard must contain exactly 16 numeric characters")]
        public string CreditCard { get; set; }
    }
}