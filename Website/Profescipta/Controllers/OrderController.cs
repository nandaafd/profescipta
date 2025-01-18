using App.Domain;
using App.Services;
using Azure.Core;
using Entities.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Services;

namespace Profescipta.Controllers
{
    public class OrderController : Controller
    {
        private readonly IComCustomerService _customerService;
        private readonly IOrderService _orderService;
        public OrderController(IComCustomerService customerService, IOrderService orderService)
        {
            _customerService = customerService;
            _orderService = orderService;
        }
        public IActionResult Index()
        {
            return View();
        }
        public async Task<IActionResult> Add()
        {
            var crit = new VMMasterSearchForm();
            ViewData["Cust"] = await _customerService.GetList(crit);
            var model = new SoOrder();
            return View(model);
        }
        public IActionResult Store([FromBody] VMOrderRequest request)
        {
            if (request == null || string.IsNullOrEmpty(request.ComCustomerId) || !request.Items.Any())
            {
                return BadRequest("Invalid data.");
            }
            var process = _orderService.Store(request);
            if (process == null)
            {
                return BadRequest("Create data failed.");
            }
            else
            {
                return RedirectToAction("Index");
            }
        }

    }
}
