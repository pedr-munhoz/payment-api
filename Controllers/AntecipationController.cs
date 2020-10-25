using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using payment_api.Models.Service;

namespace payment_api.Controllers
{
    public class AntecipationController : BaseController
    {
        private readonly IDbService _paymentDbService;

        public AntecipationController(IDbService paymentDbService)
        {
            _paymentDbService = paymentDbService;
        }

        [HttpGet("avaliable")]
        public async Task<ActionResult> GetAvailablePayments()
        {
            var avaliablePayments = await _paymentDbService.GetAvailablePayments();
            return Ok(avaliablePayments);
        }
    }
}