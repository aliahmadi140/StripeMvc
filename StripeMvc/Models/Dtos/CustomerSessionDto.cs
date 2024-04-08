namespace StripeMvc.Models.Dtos
{

    public class CustomerSessionDto
    {
        public string Object { get; set; }
        public string Client_Secret { get; set; }
        public Components Components { get; set; }
        public int Created { get; set; }
        public string Customer { get; set; }
        public int Expires_at { get; set; }
        public bool Livemode { get; set; }
    }

    public class BuyButton
    {
        public bool Enabled { get; set; }
    }

    public class Components
    {
        public BuyButton Buy_Button { get; set; }
        public PricingTable Pricing_Table { get; set; }
    }

    public class PricingTable
    {
        public bool Enabled { get; set; }
    }


}

