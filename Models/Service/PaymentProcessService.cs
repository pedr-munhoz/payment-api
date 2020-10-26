using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace payment_api.Models.Service
{
    public class PaymentProcessService : IPaymentProcessService
    {
        private const double FixTax = 0.90;
        private readonly IPaymentDbService _paymentDbService;
        private readonly IValidationService _validationService;

        private readonly IPaymentInstallmentDbService _paymentInstallmentDbService;

        public PaymentProcessService(IPaymentDbService paymentDbService, IValidationService validationService, IPaymentInstallmentDbService paymentInstallmentDbService)
        {
            _paymentDbService = paymentDbService;
            _validationService = validationService;
            _paymentInstallmentDbService = paymentInstallmentDbService;
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
                    PaymentInstallmentCount = request.PaymentInstallmentCount
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

            await _paymentDbService.Create(payment);

            for (int i = 1; i <= request.PaymentInstallmentCount; i++)
            {
                await _paymentInstallmentDbService.Create(new PaymentInstallmentEntity
                {
                    PaymentId = payment.Id,
                    RawValue = request.RawValue / request.PaymentInstallmentCount,
                    LiquidValue = payment.LiquidValue / request.PaymentInstallmentCount,
                    InstallmentNumber = i,
                    DueDate = transactionDate.AddDays(i * 30),
                });
            }

            var entity = await _paymentDbService.Get(payment.Id);

            var result = _validationService.Validate(entity, true);

            if (!result.CreationResult.Success)
                return result;

            return result;
        }
    }
}