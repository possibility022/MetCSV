using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

namespace METCSV.Generator
{
    [Serializable]
    public class ProductGroup
    {
        List<Product> products;
        List<Product> freshProducts = new List<Product>();

        public ProductGroup()
        {
            products = new List<Product>();
            freshProducts = new List<Product>();
        }

        public void addProduct(Product p)
        {
            products.Add(p);
            freshProducts.Add(p);
        }

        public void removeProduct(Product p)
        {
            products.Remove(p);
            freshProducts.Remove(p);
        }

        public List<Product> getList()
        {
            return products;
        }

        public bool updateProductIfInGroup(Product p)
        {
            for (int i = 0; i < products.Count; i++)
            {
                if ((p.NazwaDostawcy == products[i].NazwaDostawcy) &&
                    (p.SymbolSAP == products[i].SymbolSAP) &&
                    (p.NazwaProduktu == products[i].NazwaProduktu) &&
                    (p.NazwaProducenta == products[i].NazwaProducenta))
                {
                    updateProduct(p, i);

                    return true;
                }
            }

            return false;
        }

        private void updateProduct(Product p, int index)
        {
            products[index] = p;
            freshProducts.Add(p);
        }

        public static ProductGroup loadFromByte(byte[] bytes)
        {
            MemoryStream memStream = new MemoryStream();
            BinaryFormatter binForm = new BinaryFormatter();
            memStream.Write(bytes, 0, bytes.Length);
            memStream.Seek(0, SeekOrigin.Begin);
            ProductGroup obj = (ProductGroup)binForm.Deserialize(memStream);

            obj.freshProducts = new List<Product>();

            return obj;
        }

        public byte[] exportToBytes()
        {
            BinaryFormatter bf = new BinaryFormatter();
            using (MemoryStream ms = new MemoryStream())
            {
                bf.Serialize(ms, this);
                return ms.ToArray();
            }
            //throw new NotImplementedException();
        }

        public bool manufacturerInGroup(string manufacturer)
        {
            if(products != null)
                for(int i = 0; i < products.Count; i++)
                {
                    if (products[i].NazwaProducenta == manufacturer)
                        return true;
                }

            if(freshProducts != null)
                for (int i = 0; i < products.Count; i++)
                {
                    if (freshProducts[i].NazwaProducenta == manufacturer)
                        return true;
                }

            return false;
        }
    }
}
