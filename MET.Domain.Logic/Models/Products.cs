using System.Collections.Generic;

namespace MET.Domain.Logic.Models
{
    public class Products
    {
        public ICollection<Product> MetProducts{ get; set; }
        public ICollection<Product> AbProducts{ get; set; }
        public ICollection<Product> TechDataProducts{ get; set; }
        public ICollection<Product> LamaProducts{ get; set; }
        public ICollection<Product> MetCustomProducts{ get; set; }

        public ICollection<Product> AbProducts_Old{ get; set; }
        public ICollection<Product> TechDataProducts_Old{ get; set; }
        public ICollection<Product> LamaProducts_Old{ get; set; }
    }
}
