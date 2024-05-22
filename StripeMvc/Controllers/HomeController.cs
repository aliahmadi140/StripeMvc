using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Stripe;
using StripeMvc.Interfaces;
using StripeMvc.Models;
using StripeMvc.Models.ViewModels;
using System.Diagnostics;
using System.Net;

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
            //var webHookOptions = new WebhookEndpointCreateOptions
            //{
            //    EnabledEvents = new List<string> { "*" },
            //    Url = "http://localhost:4242/stripe_webhooks",
            //};
            //var webhookService = new WebhookEndpointService();
            //webhookService.Create(webHookOptions);

            //var handler = new HttpClientHandler
            //{
            //    Proxy = new WebProxy("http://localhost:4242/"),
            //    UseProxy = true,
            //};
            //var httpClient = new HttpClient(handler);

            //var stripeClient = new StripeClient(
            //    StripeConfiguration.ApiKey,
            //    httpClient: new SystemNetHttpClient(httpClient),
            //    apiBase: "web.dev.namava.ir"
            //);
            //StripeConfiguration.StripeClient = stripeClient;

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

        public async Task<IActionResult> UserSubscriptions()
        {
            var result = new List<UserSubscriptionsViewModel>();

            StripeConfiguration.ApiKey = "sk_test_51OsOJ2JWWKtHpjwkP6AS8BBbgnwaTPLoizaPHCsMY1bkwWwhgFFQhY0lPDNQb8rIp77PMUYmT6L8JBxGqWcBIREh00Qgfj3DMX";
            var service = new SubscriptionService();

            SubscriptionListOptions options = new();

            options.Customer = "cus_Q3X3IRcXsPeD92";
            options.Status = "active";


            StripeList<Subscription> subscriptions = service.List(options);


            foreach (var item in subscriptions)
            {
                result.Add(new UserSubscriptionsViewModel
                {
                    EndDate = item.CurrentPeriodEnd,
                    StartDate = item.CurrentPeriodStart,
                    TrialEnd = item.TrialEnd,
                    ProductName = GetPriceInfo(item.Items.FirstOrDefault().Price.Id),
                    SubscriptionId = item.Id,
                    SubscriptionCanceled = item.CancelAt != null ? true : false,
                });

            }

            return View(result);
        }

        public async Task<IActionResult> SuccessPyment([FromQuery] string id)
        {

           
            try
            {
                StripeConfiguration.ApiKey = "sk_test_51OsOJ2JWWKtHpjwkP6AS8BBbgnwaTPLoizaPHCsMY1bkwWwhgFFQhY0lPDNQb8rIp77PMUYmT6L8JBxGqWcBIREh00Qgfj3DMX";

                var service = new Stripe.Checkout.SessionService();
                var result = service.Get(id);

                bool isPaid;
                if (result.PaymentStatus == "paid")
                {
                    isPaid = true;
                }

                var subscription = new SubscriptionService();
                var a = subscription.Get(result.SubscriptionId);
                var name = a.Items.FirstOrDefault().Price.Nickname;
            }
            catch (Exception e)
            {

                throw;
            }

            return Ok();
        }

        public async Task<IActionResult> DeleteSubscription(string subscriptionId)
        {
            StripeConfiguration.ApiKey = "sk_test_51OsOJ2JWWKtHpjwkP6AS8BBbgnwaTPLoizaPHCsMY1bkwWwhgFFQhY0lPDNQb8rIp77PMUYmT6L8JBxGqWcBIREh00Qgfj3DMX";
             

            var options = new SubscriptionUpdateOptions { CancelAtPeriodEnd = true, };
            var service = new SubscriptionService();
            service.Update(subscriptionId, options);

            //var service = new SubscriptionService();
            //service.Cancel(subscriptionId);





            return View("Index");
        }


        private string GetPriceInfo(string priceId)
        {

            var service = new PriceService();
            Price priceInfo = service.Get(priceId);

            return priceInfo.Nickname;
        }
    }
}
