using System.Collections.Generic;
using MET.Data.Models;

namespace MET.Domain.Logic.Models
{
    public class Products
    {
        public ICollection<Product> MetProducts{ get; set; }
        public ICollection<Product> AbProducts{ get; set; }
        public ICollection<Product> TechDataProducts{ get; set; }
        public ICollection<Product> LamaProducts{ get; set; }
        public ICollection<Product> MetCustomProducts{ get; set; }

        public ICollection<Product> AbProductsOld{ get; set; }
        public ICollection<Product> TechDataProductsOld{ get; set; }
        public ICollection<Product> LamaProductsOld{ get; set; }
    }
}
