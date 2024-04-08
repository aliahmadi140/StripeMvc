using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Stripe;
using StripeMvc.Models;
using StripeMvc.Models.ViewModels;
using System.Diagnostics;

namespace StripeMvc.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
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
        public IActionResult Pricing()
        {
            StripeConfiguration.ApiKey = "sk_test_51OsOJ2JWWKtHpjwkP6AS8BBbgnwaTPLoizaPHCsMY1bkwWwhgFFQhY0lPDNQb8rIp77PMUYmT6L8JBxGqWcBIREh00Qgfj3DMX";

            var options = new CustomerSessionCreateOptions
            {
                Customer = "cus_PisI8IVPpmJVdq",
                Components = new CustomerSessionComponentsOptions
                {
                    PricingTable = new CustomerSessionComponentsPricingTableOptions
                    {
                        Enabled = true,
                    },
                },
            };
            var service = new CustomerSessionService();
            var a = service.Create(options);

            var c = new PricingViewModel { CustomerSecretKey = a.ClientSecret, Email = User?.Identity?.Name };

            return View(c);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
