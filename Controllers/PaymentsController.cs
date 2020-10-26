using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using payment_api.Models;
using payment_api.Models.Service;

namespace payment_api.Controllers
{
    public class PaymentsController : BaseController
    {
        private readonly IPaymentDbService _paymentService;

        private readonly IPaymentProcessService _paymentProcessService;

        public PaymentsController(IPaymentDbService paymentService, IPaymentProcessService paymentProcessService)
        {
            _paymentService = paymentService;
            _paymentProcessService = paymentProcessService;
        }

        [HttpGet("{nsu:int}")]
        public async Task<ActionResult> Get(int nsu)
        {
            var entity = await _paymentService.Get(nsu);
            if (entity == null)
                return NotFound($"No payment found for Nsu = {nsu}");
            return Ok(entity);
        }

        [HttpPost]
        public async Task<ActionResult> Post([FromBody] PaymentRequest paymentRequest)
        {
            var result = await _paymentProcessService.ProcessPayment(paymentRequest, DateTime.Now);

            if (!result.Success)
                return UnprocessableEntity(result);

            return Created($"/{result.Value.Id}", result.Value);
        }
    }
}