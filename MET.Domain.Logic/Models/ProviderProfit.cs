namespace MET.Domain.Logic.Models
{
    public class ProviderProfit
    {
        public ProviderProfit(Providers provider)
        {
            Provider = provider;
        }

        public Providers Provider { get; private set; }

        public double Profit { get; set; }
    }
}
