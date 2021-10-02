using System.ComponentModel.DataAnnotations;

namespace MET.Data.Models.Profits
{
    public class CustomProfit : IProfit
    {
        [Key]
        public int Id { get; set; }
        public string PartNumber { get; set; }
        public double Profit { get; set; }
    }
}
