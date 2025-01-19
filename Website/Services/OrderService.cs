using App.Domain;
using App.Repository;
using Entities.ViewModels;
using Microsoft.IdentityModel.Tokens;
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
        SoOrder? GetId(int id);
        List<SoItem>? GetItem(int orderId);
        VMOrderRequest Update(VMOrderRequest request);
    }
    public class OrderService : IOrderService
    {
        private readonly IRepository<SoOrder> _order;
        private readonly IRepository<SoItem> _item;
        private readonly IRepository<ComCustomer> _cust;
        public OrderService(
            IRepository<SoOrder> order, 
            IRepository<SoItem> item,
            IRepository<ComCustomer> cust)
        {
            _item = item;
            _cust = cust;
            _order = order;
        }
        public List<SoOrder> GetList(VMMasterSearchForm crit)
        {
            var table = _order.TableNoTracking;
            var custTable = _cust.TableNoTracking;
            var list = (from o in table
                       join p in custTable on o.ComCustomerId equals p.ComCustomerId
                       select new SoOrder()
                       {
                           SoOrderId = o.SoOrderId,
                           OrderNo = o.OrderNo,
                           OrderDate = o.OrderDate,
                           Address = o.Address,
                           ComCustomerId = o.ComCustomerId,
                           _ComCustomerName = p.CustomerName
                       }).ToList();
            if (crit.searchDate != null)
            {
                list.Where(w => w.OrderDate == crit.searchDate).ToList();
            }
            if (!crit.searchText.IsNullOrEmpty())
            {
                list.Where(w => w.OrderNo.ToUpper().Contains(crit.searchText.ToUpper().Trim())).ToList();
            }

            return list;
        }
        public SoOrder? GetId(int id)
        {
            var data = _order.TableNoTracking.Where(w => w.SoOrderId == id).FirstOrDefault();
            return data != null ? data : null;
        }
        public List<SoItem>? GetItem(int orderId)
        {
            var list = _item.TableNoTracking.Where(w => w.SoOrderId == orderId).ToList();
            return list;
        }
        public VMOrderRequest Store(VMOrderRequest request)
        {
            if (request != null && request.Items.Count > 0) 
            {
                SoOrder data = new SoOrder();
                data.OrderNo = "";
                data.OrderDate = request.OrderDate;
                data.Address = request.Address;
                data.ComCustomerId = Convert.ToInt32(request.ComCustomerId);
                var addData =_order.Add(data);

                data.OrderNo = $"ORD-00{addData.SoOrderId}-{DateTime.Today.ToString("ddMMyyyy")}";
                _order.Update(data);

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
                    }
                    
                }
                _order.SaveChangesAsync();
                _item.SaveChangesAsync();
            }
            else
            {
                
            }
            return request;
        }
        public VMOrderRequest Update(VMOrderRequest request)
        {
            if (request != null && request.Items.Count > 0)
            {
                var existingOrder = _order.TableNoTracking.Where(w => w.SoOrderId == request.SoOrderId).FirstOrDefault();
                var order = new SoOrder()
                {
                    SoOrderId = existingOrder.SoOrderId,
                    OrderNo = existingOrder.OrderNo,
                    OrderDate = request.OrderDate,
                    ComCustomerId = Convert.ToInt32(request.ComCustomerId),
                    Address = request.Address
                };
                _order.Update(order);
                if (request.Items.Count > 0)
                {
                    List<VMOrderItemDto> itemList = request.Items;
                    foreach (VMOrderItemDto l in itemList)
                    {
                        SoItem itemData = new SoItem();
                        var existingItem = _item.TableNoTracking.Where(w => w.SoItemId == l.ItemId).FirstOrDefault();
                        if (existingItem == null)
                        {
                            itemData.ItemName = l.Name;
                            itemData.SoOrderId = existingOrder.SoOrderId;
                            itemData.Quantity = l.Qty;
                            itemData.Price = Convert.ToInt32(l.Price);
                            _item.Add(itemData);
                        }
                        else
                        {
                            itemData.SoItemId = l.ItemId ?? 0;
                            itemData.ItemName = l.Name;
                            itemData.SoOrderId = existingItem.SoOrderId;
                            itemData.Quantity = l.Qty;
                            itemData.Price = Convert.ToInt32(l.Price);
                            _item.Update(itemData);
                        }
                    }

                }
                _item.SaveChangesAsync();
                _order.SaveChangesAsync();
            }
            return request;
        }
    }
}
