using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
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

        [HttpGet("avaliable")]
        public async Task<ActionResult> GetAvailablePayments()
        {
            var avaliablePayments = await _paymentDbService.GetAvailablePayments();
            return Ok(avaliablePayments);
        }

        [HttpGet]
        public async Task<ActionResult> Get(string status)
        {
            var antecipations = await _antecipationDbService.Get();
            return Ok(antecipations);
        }
    }
}