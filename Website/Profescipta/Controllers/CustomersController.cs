using App.Domain;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace Profescipta.Controllers
{
    public class CustomersController : Controller
    {
        private App.Services.IComCustomerService _customerService;
        public CustomersController(App.Services.IComCustomerService customerService)
        {
            _customerService = customerService;
        }
        public async Task<IActionResult> Index()
        {
            var crit = new VMMasterSearchForm();
            List<App.Domain.ComCustomer> model = await _customerService.GetList(crit);
            return View("Customers", model);
        }
        public IActionResult Edit(int id = 0) 
        {
            var dml = id == 0 ? "I" : "U";
            var model = new ComCustomer();

            if (dml == "I")
            {
                model.ComCustomerId = 0;
            }
            else if (dml == "U")
            {
                model = GetId(id);
            }
            return View("Customers.iud", model);
        }
        protected ComCustomer GetId(int id)
        {
            var data = _customerService.GetId(id);
            return data;
        }
        public IActionResult Update(ComCustomer request)
        {
            if (!request.CustomerName.IsNullOrEmpty())
            {
                var proc = _customerService.Update(request);
                
                if (proc == null)
                {
                    return BadRequest();
                }
                else
                {
                    return RedirectToAction("Index");
                }
            }
            else
            {
                return BadRequest();
            }
        }
        public IActionResult Delete(int id)
        {
            bool result;
            if (id != 0)
            {
                var proc = _customerService.Delete(id);
                if (!proc)
                {
                    return BadRequest();
                }
                else
                {
                    return RedirectToAction("Index");
                }
            }
            else
            {
                return BadRequest();
            }
        }
    }
}
