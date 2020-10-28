using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using payment_api.Models;
using payment_api.Models.Service;

namespace payment_api.Controllers
{
    public class AnticipationController : BaseController
    {
        private readonly IPaymentDbService _paymentDbService;
        private readonly IAnticipationService _anticipationDbService;

        public AnticipationController(IPaymentDbService paymentDbService, IAnticipationService anticipationDbService)
        {
            _paymentDbService = paymentDbService;
            _anticipationDbService = anticipationDbService;
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

            var anticipations = await _anticipationDbService.Get(status);

            return Ok(anticipations);
        }

        [HttpPost]
        public async Task<ActionResult> Create([FromBody] List<int> paymentIds)
        {
            var result = await _anticipationDbService.Create(paymentIds, DateTime.Now);

            if (!result.Success)
                return UnprocessableEntity(result.ErrorMessage);

            return Created($"/{result.Value.Id}", result.Value);
        }

        [HttpPatch("start-analysis/{id:int}")]
        public async Task<ActionResult> StartAnalysis(int id)
        {
            var result = await _anticipationDbService.StartAnalysis(id, DateTime.Now);

            if (!result.Success)
            {
                if (result.UnprocessableEntity)
                    return UnprocessableEntity(result.ErrorMessage);

                return NotFound(result.ErrorMessage);
            }

            return Ok(result);
        }

        [HttpPatch("{id:int}/approve")]
        public async Task<ActionResult> ApprovePaymentAnticipation(int id, [FromBody] List<int> paymentIds)
        {
            var result = await _anticipationDbService.ResolvePaymentAnticipation(id, paymentIds, true, DateTime.Now);

            if (!result.Success)
            {
                if (result.UnprocessableEntity)
                    return UnprocessableEntity(result.ErrorMessage);

                return NotFound(result.ErrorMessage);
            }

            return Ok(result.Value);
        }

        [HttpPatch("{id:int}/reject")]
        public async Task<ActionResult> RejectPaymentAnticipation(int id, [FromBody] List<int> paymentIds)
        {
            var result = await _anticipationDbService.ResolvePaymentAnticipation(id, paymentIds, false, DateTime.Now);

            if (!result.Success)
            {
                if (result.UnprocessableEntity)
                    return UnprocessableEntity(result.ErrorMessage);

                return NotFound(result.ErrorMessage);
            }

            return Ok(result.Value);
        }
    }
}