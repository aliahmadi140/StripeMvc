using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Stripe;
using StripeMvc.Interfaces;
using StripeMvc.Models;
using StripeMvc.Models.ViewModels;
using System.Diagnostics;

namespace StripeMvc.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IUserService _userService;


        public HomeController(ILogger<HomeController> logger, IUserService userService)
        {
            _logger = logger;
            _userService = userService;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [Authorize]
        public async Task<IActionResult> Pricing()
        {
            StripeConfiguration.ApiKey = "sk_test_51OsOJ2JWWKtHpjwkP6AS8BBbgnwaTPLoizaPHCsMY1bkwWwhgFFQhY0lPDNQb8rIp77PMUYmT6L8JBxGqWcBIREh00Qgfj3DMX";
            var stripeCustomerId = await _userService.GetStripeCustomerIdByEmail(User?.Identity?.Name);
            CustomerSession customerSession = new();
            if (stripeCustomerId != null)
            {
                var options = new CustomerSessionCreateOptions
                {
                    Customer = stripeCustomerId,
                    Components = new CustomerSessionComponentsOptions
                    {
                        PricingTable = new CustomerSessionComponentsPricingTableOptions
                        {
                            Enabled = true,
                        },
                    },
                };
                var service = new CustomerSessionService();
                customerSession = service.Create(options);
            }
            var c = new PricingViewModel
            {
                CustomerSecretKey = customerSession.ClientSecret,
                Email = User?.Identity?.Name
            };

            return View(c);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
