using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Stripe;
using StripeMvc.Interfaces;
using StripeMvc.Models.Dtos.UserDtos;
using Newtonsoft.Json;
namespace StripeMvc.Controllers
{

    [ApiController]
    public class WebhookController : ControllerBase
    {
        private readonly IUserService _userService;

        public WebhookController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpPost("stripe_webhooks")]
        public async Task<IActionResult> Webhook()
        {
            var json = await new StreamReader(HttpContext.Request.Body).ReadToEndAsync();
            Event stripeEvent;
            try
            {
                stripeEvent = EventUtility.ConstructEvent(
                    json,
                    Request.Headers["Stripe-Signature"],
               "whsec_4bcbebd4f3f4427ab1cf663c70cbd1b21bc201bfd53f15cd573855f0f77e0dcf"
                );

                  var rawData = Newtonsoft.Json.JsonConvert.SerializeObject(stripeEvent.Data);

                //  _logger.LogInformation($"Webhook notification with type: {stripeEvent.Type} found for {stripeEvent.Id}");

                PaymentIntent intent = null;

                switch (stripeEvent.Type)
                {
                    case "invoice.paid":
                        var data = (Invoice)stripeEvent.Data.Object;

                        break;

                    case "customer.created":
                        var customerData = (Customer)stripeEvent.Data.Object;
                        await _userService.UpdateUserStripeCustomerId(new UserInfroDto
                        {
                            Email = customerData.Email,
                            StripeCustomerId = customerData.Id
                        });
                        break;

                    case "payment_intent.payment_failed":
                        intent = (PaymentIntent)stripeEvent.Data.Object;
                        //    _logger.LogInformation("Failure: {ID}", intent.Id);

                        // Notify the customer that payment failed

                        break;
                    case "payment_intent.processing":
                        intent = (PaymentIntent)stripeEvent.Data.Object;
                        //   _logger.LogInformation("Succeeded: {ID}", intent.Id);

                        // Fulfil the customer's purchase

                        break;
                    default:
                        // Handle other event types

                        break;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine($"Something failed {e}");
                return BadRequest();
            }


            return Ok();
        }


       
    }
}
