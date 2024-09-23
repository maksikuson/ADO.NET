using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _02_CRUDInterface
{
    internal class Product
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
        public int Quantity { get; set; }
        public int CostPrice { get; set; }
        public string Producer { get; set; }
        public int Price { get; set; }

        public override string ToString()
        {
            return $"Name :  {Name,-16}, Producer:  {Producer,-14}, Cost price {CostPrice,14}";
        }
    }
}
