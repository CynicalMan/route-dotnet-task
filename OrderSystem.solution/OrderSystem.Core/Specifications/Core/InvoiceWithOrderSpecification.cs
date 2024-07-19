using OrderSystem.Core.Entities.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderSystem.Core.Specifications.Core
{
    public class InvoiceWithOrderSpecification : BaseSpecifications<Invoice>
    {

        public InvoiceWithOrderSpecification()
            : base()
        {
            Includes.Add(i => i.Order);
            AddOrderby(i => i.InsertDate);
        }

        public InvoiceWithOrderSpecification(int invoiceId)
            : base(i => i.Id == invoiceId)
        {
            Includes.Add(i => i.Order);
            AddOrderby(i => i.InsertDate);
        }
    }
}
