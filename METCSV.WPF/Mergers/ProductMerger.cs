using METCSV.WPF.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace METCSV.WPF.Mergers
{
    class ProductMerger
    {
        IEnumerable<Product> _metProducts;
        HashSet<string> _hiddenProducts;

        public void SetMetProducts(IEnumerable<Product> products)
        {
            _metProducts = products;
        }

        private void removeHiddenProducts(List<Product> products)
        {
            //LOG("Usuwam ukryte produkty"); //todo log this
            int countAtBegining = products.Count;

            CreateHiddenProductsCollection();
            
            for (int i = 0; i < products.Count; i++)
            {
                if (_hiddenProducts.Contains(products[i].SymbolSAP))
                {
                    products.RemoveAt(i);
                    i--;
                }
            }
            
            //log Database.Log.log("Usunięto " + (countAtBegining - products.Count).ToString()); //todo log or move it to return
        }

        private void CreateHiddenProductsCollection()
        {
            if (_hiddenProducts == null)
            {
                HashSet<string> hiddenProducts = new HashSet<string>();

                foreach (var prod in _metProducts)
                {
                    if (prod.Hidden)
                    {
                        hiddenProducts.Add(prod.SymbolSAP);
                    }
                }

                _hiddenProducts = hiddenProducts;
            }
        }
    }
}
