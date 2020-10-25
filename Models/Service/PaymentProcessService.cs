using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace payment_api.Models.Service
{
    public class PaymentProcessService : IPaymentProcessService
    {
        private const double FixTax = 0.90;
        private readonly IDbService _dbService;
        private readonly IValidationService _validationService;

        public PaymentProcessService(IDbService dbService, IValidationService validationService)
        {
            _dbService = dbService;
            _validationService = validationService;
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
                };

                return _validationService.Validate(rejectedPayment, false);
            }

            var payment = new PaymentEntity
            {
                TransactionDate = transactionDate,
                Approved = true,
                RawValue = request.RawValue,
                LiquidValue = request.RawValue - FixTax,
                Tax = FixTax,
                CreditCard = request.CreditCard.Substring(12)
            };

            for (int i = 1; i <= request.PaymentInstallmentCount; i++)
            {
                payment.PaymentInstallments.Add(new PaymentInstallmentEntity
                {
                    RawValue = request.RawValue / request.PaymentInstallmentCount,
                    LiquidValue = payment.LiquidValue / request.PaymentInstallmentCount,
                    InstallmentNumber = i,
                    DueDate = transactionDate.AddDays(i * 30),
                });
            }

            var result = _validationService.Validate(payment, true);

            await _dbService.Create(payment);

            if (!result.CreationResult.Success)
                return result;

            foreach (var installment in result.CreationResult.Value.PaymentInstallments)
            {
                installment.PaymentId = result.CreationResult.Value.Id;
            }

            return result;
        }
    }
}