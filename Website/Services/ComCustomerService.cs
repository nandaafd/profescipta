using App.Domain;
using App.Repository;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Services
{
    public interface IComCustomerService
    {
        Task<List<Domain.ComCustomer>> GetList(VMMasterSearchForm crit);
        Domain.ComCustomer GetId(int id);
        Domain.ComCustomer Update(ComCustomer request);
        bool Delete(int id);
    }
    public class ComCustomerService : IComCustomerService
    {
        private readonly IRepository<Domain.ComCustomer> _cust;
        public ComCustomerService(IRepository<Domain.ComCustomer> cust)
        {
            _cust = cust;
        }
        public async Task<List<Domain.ComCustomer>> GetList(VMMasterSearchForm crit)
        {
            List<Domain.ComCustomer> data = await _cust.TableNoTracking.ToListAsync();
            return data;
        }
        public Domain.ComCustomer GetId(int id)
        {
            Domain.ComCustomer data = _cust.TableNoTracking.Where(w => w.ComCustomerId == id).FirstOrDefault();
            return data;
        }
        public Domain.ComCustomer Update(ComCustomer request)
        {
            if (request.ComCustomerId == 0)
            {
                _cust.Add(request);
                _cust.SaveChangesAsync();
            }
            else
            {
                _cust.Update(request);
                _cust.SaveChangesAsync();
            }
            return request;
        }
        public bool Delete(int id)
        {
            bool result = true;
            try
            {
                var data = GetId(id);
                if (data != null)
                {
                    _cust.Delete(data);
                }
            }
            catch 
            {
                result = false;
            }
            return result;
        }
    }
}
