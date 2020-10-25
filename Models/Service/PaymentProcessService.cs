using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace payment_api.Models.Service
{
    public class PaymentProcessService : IPaymentProcessService
    {
        private const double FixTax = 0.90;
        private readonly IPaymentDbService _dbService;

        public PaymentProcessService(IPaymentDbService dbService)
        {
            _dbService = dbService;
        }

        public async Task<PaymentProcessResult> ProcessPayment(PaymentRequest request, DateTime transactionDate)
        {
            if (request.CreditCard.Substring(0, 4) == "5999")
            {
                var rejectedPayment = new PaymentEntity
                {
                    Approved = false,
                    TransactionDate = transactionDate,
                    Anticipated = false,
                    RawValue = request.RawValue,
                    LiquidValue = request.RawValue - FixTax,
                    Tax = FixTax,
                    CreditCard = request.CreditCard.Substring(12),
                    PaymentInstallments = new List<PaymentInstallmentEntity>()
                };

                return new PaymentProcessResult(rejectedPayment, false);
            }

            var payment = new PaymentEntity
            {
                TransactionDate = transactionDate,
                Anticipated = false,
                Approved = true,
                RawValue = request.RawValue,
                LiquidValue = request.RawValue - FixTax,
                Tax = FixTax,
                CreditCard = request.CreditCard.Substring(12)
            };

            var installments = new List<PaymentInstallmentEntity>();
            for (int i = 1; i <= request.PaymentInstallmentCount; i++)
            {
                installments.Add(new PaymentInstallmentEntity
                {
                    RawValue = request.RawValue / request.PaymentInstallmentCount,
                    LiquidValue = payment.LiquidValue / request.PaymentInstallmentCount,
                    InstallmentNumber = i,
                    DueDate = transactionDate.AddDays(i * 30),
                });
            }
            payment.PaymentInstallments = installments;

            var result = await _dbService.Create(payment);

            if (!result.Success)
                return new PaymentProcessResult(result, false);

            foreach (var installment in result.Value.PaymentInstallments)
            {
                installment.PaymentId = result.Value.Id;
            }

            return new PaymentProcessResult(result, true);
        }
    }
}