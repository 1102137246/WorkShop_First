using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Workshop.Controllers
{
    public class DefaultController : Controller
    {
        // GET: Default
        public ActionResult Index()
        {
            Workshop.Models.Service service = new Models.Service();
            List<Models.Order> result = service.GetEmpData();
            List<SelectListItem> empData = new List<SelectListItem>();
            foreach (var item in result)
            {
                empData.Add(new SelectListItem() {
                    Text = item.EmployeeName,
                    Value = item.EmployeeID.ToString()
                });
            }
            ViewBag.empData = empData;
            result = service.GetShipperData();
            List<SelectListItem> shipperData = new List<SelectListItem>();
            foreach (var item in result)
            {
                shipperData.Add(new SelectListItem()
                {
                    Text = item.ShipperName,
                    Value = item.ShipperId.ToString()
                });
            }
            ViewBag.shipperData = shipperData;
            return View();
        }

        [HttpPost]
        public ActionResult Result(Models.Order order)
        {
            Models.Service service = new Models.Service();
            List<Workshop.Models.Order> result =  service.GetOrderData(order);
            ViewBag.result = result;
            return View();
        }
    }
}