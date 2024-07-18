using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderSystem.Core.Entities.Core
{
    public class Product : BaseEntity
    {
        public Product()
        {
            InsertDate = DateTime.Now;
            UpdateDate = DateTime.Now;
            DeleteDate = null;
        }

        public string Name { get; set; }
        public decimal Price { get; set; }
        public int Stock { get; set; }
    }
}
