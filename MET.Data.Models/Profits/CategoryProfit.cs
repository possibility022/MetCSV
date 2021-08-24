using System.ComponentModel.DataAnnotations;

namespace MET.Data.Models.Profits
{
    public class CategoryProfit
    {
        [Key]
        public int Id { get; set; }

        public string Category { get; set; }

        public MET.Domain.Providers Provider { get; set; }

        public double Profit { get; set; }
    }
}
