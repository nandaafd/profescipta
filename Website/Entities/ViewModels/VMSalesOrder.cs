using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.ViewModels
{
    public class VMOrderRequest
    {
        public string OrderNo { get; set; }
        public DateTime OrderDate { get; set; }
        public string ComCustomerId { get; set; }
        public string Address { get; set; }
        public List<VMOrderItemDto> Items { get; set; }
    }

    public class VMOrderItemDto
    {
        public string Name { get; set; }
        public int Qty { get; set; }
        public decimal Price { get; set; }
        public decimal Total { get; set; }
    }
}
