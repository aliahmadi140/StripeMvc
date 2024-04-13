namespace StripeMvc.Models.ViewModels
{
    public class UserSubscriptionsViewModel
    {
        public string SubscriptionId { get; set; }

        public string ProductName { get; set; }

        public DateTime? StartDate { get; set; }

        public DateTime? EndDate { get; set; }

        public DateTime? TrialEnd { get; set; }

        public bool SubscriptionCanceled { get; set; }
    }
}
