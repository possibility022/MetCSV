using System;
using System.ComponentModel.DataAnnotations;

namespace MET.Data.Models
{
    public class IgnoreCategory
    {
        [Key]
        public int Id { get; set; }

        public string Provider { get; set; }

        public string CategoryName { get; set; }

        public Providers ProviderAsType() => Enum.Parse<Providers>(Provider);
    }
}
