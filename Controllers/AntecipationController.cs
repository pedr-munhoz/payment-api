using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using payment_api.Models;
using payment_api.Models.Service;

namespace payment_api.Controllers
{
    public class AntecipationController : BaseController
    {
        private readonly IPaymentDbService _paymentDbService;
        private readonly IAntecipationDbService _antecipationDbService;

        public AntecipationController(IPaymentDbService paymentDbService, IAntecipationDbService antecipationDbService)
        {
            _paymentDbService = paymentDbService;
            _antecipationDbService = antecipationDbService;
        }

        [HttpGet("avaliable-payments")]
        public async Task<ActionResult> GetAvailablePayments()
        {
            var avaliablePayments = await _paymentDbService.GetAvailablePayments();
            return Ok(avaliablePayments);
        }

        [HttpGet]
        public async Task<ActionResult> Get(string status)
        {
            if (status != null && status != "pending" && status != "analyzing" && status != "finished")
                return BadRequest("Possible values for status: 'null', 'pending', 'analyzing' or 'finished'");

            var antecipations = await _antecipationDbService.Get(status);

            return Ok(antecipations);
        }

        [HttpPost]
        public async Task<ActionResult> Create([FromBody] List<int> paymentIds)
        {
            var result = await _antecipationDbService.Create(paymentIds, DateTime.Now);

            if (!result.Success)
                return UnprocessableEntity(result.ErrorMessage);

            return Created("", result.Value);
        }

        [HttpPatch("start-analysis/{id:int}")]
        public async Task<ActionResult> StartAnalysis(int id)
        {
            var result = await _antecipationDbService.StartAnalysis(id, DateTime.Now);

            return Ok(result);
        }

        [HttpPatch("resolve-analysis")]
        public async Task<ActionResult> StartAnalysis([FromQuery] int id, bool approve, [FromBody] List<int> paymentIds)
        {
            var result = await _antecipationDbService.ResolvePaymentAntecipation(id, paymentIds, approve);

            return Ok(result);
        }
    }
}