﻿using System.ComponentModel.DataAnnotations;

namespace MET.Data.Models.Profits
{
    public class CategoryProfit : IProfit
    {
        [Key]
        public int Id { get; set; }

        public string Category { get; set; }

        public Providers Provider { get; set; }

        public double Profit { get; set; }
    }
}
