using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using payment_api.Models.Result;

namespace payment_api.Models.Service
{
    public class PaymentProcessService : IPaymentProcessService
    {
        private const double FixTax = 0.90;
        private readonly IPaymentDbService _paymentDbService;
        private readonly IResultService _resultService;

        private readonly IPaymentInstallmentDbService _paymentInstallmentDbService;

        public PaymentProcessService(IPaymentDbService paymentDbService, IResultService resultService, IPaymentInstallmentDbService paymentInstallmentDbService)
        {
            _paymentDbService = paymentDbService;
            _resultService = resultService;
            _paymentInstallmentDbService = paymentInstallmentDbService;
        }

        public async Task<PaymentProcessResult> ProcessPayment(PaymentRequest request, DateTime transactionDate)
        {
            if (request.CreditCard.Substring(0, 4) == "5999")
            {
                return _resultService.GenerateFailedResult();
            }

            var payment = new PaymentEntity
            {
                TransactionDate = transactionDate,
                Approved = true,
                RawValue = request.RawValue,
                LiquidValue = request.RawValue - FixTax,
                Tax = FixTax,
                CreditCard = request.CreditCard.Substring(12),
                PaymentInstallmentCount = request.PaymentInstallmentCount
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

            var result = _resultService.GenerateResult(entity, true);

            return result;
        }
    }
}