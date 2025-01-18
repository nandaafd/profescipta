using App.Domain;
using App.Repository;
using Entities.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services
{
    public interface IOrderService
    {
        List<SoOrder> GetList(VMMasterSearchForm crit);
        VMOrderRequest Store(VMOrderRequest request);
    }
    public class OrderService : IOrderService
    {
        private readonly IRepository<SoOrder> _order;
        private readonly IRepository<SoItem> _item;
        public OrderService(IRepository<SoOrder> order, IRepository<SoItem> item)
        {
            _item = item;
            _order = order;
        }
        public List<SoOrder> GetList(VMMasterSearchForm crit)
        {
            var list = _order.TableNoTracking.ToList();
            return list;
        }
        public VMOrderRequest Store(VMOrderRequest request)
        {
            if (request != null) 
            {
                SoOrder data = new SoOrder();
                data.OrderNo = "";
                data.OrderDate = request.OrderDate;
                data.Address = request.Address;
                data.ComCustomerId = Convert.ToInt32(request.ComCustomerId);
                var addData =_order.Add(data);

                data.OrderNo = $"ORD-00{addData.SoOrderId}-{DateTime.Today.ToString("ddMMyyyy")}";
                _order.Update(data);
                _order.SaveChangesAsync();

                if (request.Items.Count > 0)
                {
                    List<VMOrderItemDto> itemList = request.Items;
                    foreach(VMOrderItemDto l in itemList)
                    {
                        SoItem itemData = new SoItem();
                        itemData.ItemName = l.Name;
                        itemData.SoOrderId = addData.SoOrderId;
                        itemData.Quantity = l.Qty;
                        itemData.Price = Convert.ToInt32(l.Price);
                        _item.Add(itemData);
                        _item.SaveChangesAsync();
                    }
                    
                }
            }
            return request;
        }
    }
}
