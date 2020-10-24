using System;

namespace payment_api.Models
{
    public class PaymentInstallmentEntity : DbEntity
    {
        public int PaymentId { get; set; }
        public float RawValue { get; set; }
        public float LiquidValue { get; set; }
        public int InstallmentNumber { get; set; }
        public float? AnticipatedValue { get; set; }
        public DateTime DueDate { get; set; }
        public DateTime? AntecipatedTranfer { get; set; }
    }
}