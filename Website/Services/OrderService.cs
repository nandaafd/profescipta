using App.Data;
using App.Domain;
using App.Repository;
using Entities.ViewModels;
using Microsoft.EntityFrameworkCore.Storage;
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
        bool Store(VMOrderRequest request);
        SoOrder? GetId(int id);
        List<SoItem>? GetItem(int orderId);
        bool Update(VMOrderRequest request);
        bool Delete(int id);
    }
    public class OrderService : IOrderService
    {
        private readonly IRepository<SoOrder> _order;
        private readonly IRepository<SoItem> _item;
        private readonly IRepository<ComCustomer> _cust;
        private readonly EfDbContext _dbContext;
        public OrderService(
            IRepository<SoOrder> order, 
            IRepository<SoItem> item,
            IRepository<ComCustomer> cust,
            EfDbContext dbContext)
        {
            _item = item;
            _cust = cust;
            _order = order;
            _dbContext = dbContext;
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
                list = list.Where(w => w.OrderDate == crit.searchDate).ToList();
            }
            if (!crit.searchText.IsNullOrEmpty())
            {
                list = list.Where(w => w.OrderNo.ToUpper().Contains(crit.searchText.ToUpper().Trim())).ToList();
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
        public bool Store(VMOrderRequest request)
        {
            using (IDbContextTransaction dbTran = _dbContext.Database.BeginTransaction())
            {
                try
                {
                    if (request != null && request.Items.Count > 0)
                    {
                        SoOrder data = new SoOrder();
                        data.OrderNo = "";
                        data.OrderDate = request.OrderDate;
                        data.Address = request.Address;
                        data.ComCustomerId = Convert.ToInt32(request.ComCustomerId);
                        var addData = _order.Add(data);

                        data.OrderNo = $"ORD-00{addData.SoOrderId}-{DateTime.Today.ToString("ddMMyyyy")}";
                        _order.Update(data);

                        if (request.Items.Count > 0)
                        {
                            List<VMOrderItemDto> itemList = request.Items;
                            foreach (VMOrderItemDto l in itemList)
                            {
                                SoItem itemData = new SoItem();

                                itemData.ItemName = l.Name;
                                itemData.SoOrderId = addData.SoOrderId;
                                itemData.Quantity = l.Qty;
                                itemData.Price = Convert.ToInt32(l.Price);
                                _item.Add(itemData);
                            }

                        }
                        else
                        {
                            throw new Exception();
                        }
                        _order.SaveChangesAsync();
                        _item.SaveChangesAsync();
                        dbTran.CommitAsync();
                    }
                    else
                    {
                        dbTran.RollbackAsync();
                        throw new Exception();
                    }
                }
                catch (Exception ex)
                {
                    return false;
                }

                return true;
            }
                
        }
        public bool Update(VMOrderRequest request)
        {
            using (IDbContextTransaction dbTran = _dbContext.Database.BeginTransaction())
            {
                try
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
                            _item.ExecuteSqlCommand($"delete from SO_ITEM where SO_ORDER_ID = {existingOrder.SoOrderId}");
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
                        else
                        {
                            throw new Exception();
                        }
                        _item.SaveChangesAsync();
                        _order.SaveChangesAsync();
                        dbTran.CommitAsync();
                    }
                    else
                    {
                        throw new Exception();
                    }
                }
                catch (Exception ex)
                {
                    dbTran.RollbackAsync();
                    return false;
                }

                return true;
            }
        }
        public bool Delete(int id)
        {
            using (IDbContextTransaction dbTran = _dbContext.Database.BeginTransaction())
            {
                try
                {
                    if (id != 0)
                    {
                        var orderData = _order.TableNoTracking.Where(w => w.SoOrderId == id).FirstOrDefault();
                        var itemData = _item.TableNoTracking.Where(w => w.SoOrderId == id).ToList();
                        if (orderData != null)
                        {
                            _order.Delete(orderData);
                            if (itemData.Count > 0)
                            {
                                foreach (var item in itemData)
                                {
                                    _item.Delete(item);
                                }
                            }
                            else
                            {
                                throw new Exception();
                            }
                        }
                        else
                        {
                            throw new Exception();
                        }
                        _order.SaveChangesAsync();
                        _item.SaveChangesAsync();
                        dbTran.CommitAsync();
                    }
                    else
                    {
                        throw new Exception();
                    }
                }
                catch (Exception ex)
                {
                    dbTran.RollbackAsync();
                    return false;
                }
                return true;
            }
                
        }
    }
}
