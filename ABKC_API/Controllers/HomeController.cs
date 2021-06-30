using System.Collections.Generic;
using System.Threading.Tasks;
using CoreApp.Controllers.Api;
using CoreApp.Interfaces;
using CoreDAL.Interfaces;
using CoreDAL.Models.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BullsBluffCore.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly ITransactionService _transService;
        private readonly IABKCUserService _userService;

        public HomeController(ITransactionService transService, IABKCUserService userService)
        {
            _transService = transService;
            _userService = userService;
        }

        public IActionResult Index() => View();

        [HttpPost]
        public async Task<ActionResult> Charge(string stripeToken, string stripeEmail)
        {
            var payment = new RegistrationPaymentRequest
            {
                amount = 5,
                tokenId = stripeToken,
                registrations = new List<PaymentItemDTO>()
            };
            var user = await _userService.GetUserFromOktaLogin(stripeEmail);
            await _transService.FinalizeTransaction(payment, user);

            return View();
        }
    }
}