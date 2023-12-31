﻿using System.ComponentModel.DataAnnotations;

namespace MET.Data.Models.Profits
{
    public class ManufacturerProfit : IProfit, IProviderProfit, IProfitKey
    {
        [Key]
        public int Id { get; set; }

        public string Manufacturer { get; set; }

        public Providers Provider { get; set; }

        public double Profit { get; set; }
        string IProfitKey.ProfitKey => Manufacturer;
    }
}
