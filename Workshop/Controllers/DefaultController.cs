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
                empData.Add(new SelectListItem(){
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


        public ActionResult Insert()
        {
            Workshop.Models.Service service = new Models.Service();
            List<Models.Order> result = service.GetCustomerData();

            List<SelectListItem> customerData = new List<SelectListItem>();
            foreach (var item in result)
            {
                customerData.Add(new SelectListItem()
                {
                    Text = item.CustomerName,
                    Value = item.CustomerID.ToString()
                });
            }
            ViewBag.customerData = customerData;

            result = service.GetEmpData();
            List<SelectListItem> empData = new List<SelectListItem>();
            foreach (var item in result)
            {
                empData.Add(new SelectListItem()
                {
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

            result = service.GetProductData();
            List<SelectListItem> productData = new List<SelectListItem>();
            List<string> price = new List<string>();
            foreach (var item in result)
            {
                productData.Add(new SelectListItem()
                {
                    Text = item.ProductName,
                    Value = item.ProductID.ToString(),
                });
            }
            ViewBag.productData = productData;

            foreach (var item in result)
            {
                price.Add(item.UnitPrice);
            }
            ViewBag.price = price;

            return View();
        }

        [HttpPost]
        public JsonResult Insert(Models.Order order)
        {
            Models.Service service = new Models.Service();
            service.InsertOrder(order);
            return null;
        }

        [HttpGet]
        public ActionResult Revise(int OrderID)
        {
            Workshop.Models.Service service = new Models.Service();
            Workshop.Models.Order order = new Models.Order();
            order = service.ReviseOrder(OrderID);
            order.OrderID = OrderID;
            ViewBag.order = order;

            ViewBag.OrderDate = Convert.ToDateTime(order.OrderDate).ToString("yyyy-MM-dd");
            ViewBag.RequiredDate = Convert.ToDateTime(order.RequiredDate).ToString("yyyy-MM-dd");
            ViewBag.ShippedDate = Convert.ToDateTime(order.ShippedDate).ToString("yyyy-MM-dd");

            List<Models.Order> result = service.GetCustomerData();
            List<List<SelectListItem>> productList = new List<List<SelectListItem>>();
            List<SelectListItem> customerData = new List<SelectListItem>();
            foreach (var item in result)
            {
                customerData.Add(new SelectListItem()
                {
                    Text = item.CustomerName,
                    Value = item.CustomerID.ToString(),
                    Selected = item.CustomerID.Equals(order.CustomerID),
                });
            }
            ViewBag.customerData = customerData;

            result = service.GetEmpData();
            List<SelectListItem> empData = new List<SelectListItem>();
            foreach (var item in result)
            {
                empData.Add(new SelectListItem()
                {
                    Text = item.EmployeeName,
                    Value = item.EmployeeID.ToString(),
                    Selected = item.EmployeeID.Equals(order.EmployeeID),
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
                    Value = item.ShipperId.ToString(),
                    Selected = item.ShipperId.Equals(order.ShipperId),
                });
            }
            ViewBag.shipperData = shipperData;

            result = service.GetProductData();
            List<SelectListItem> productData = new List<SelectListItem>();
            List<string> price = new List<string>();
            for (int i = 0; i < order.ProductIdList.Count; i++)
            {
                foreach (var item in result)
                {
                    productData.Add(new SelectListItem()
                    {
                        Text = item.ProductName,
                        Value = item.ProductID.ToString(),
                        Selected = item.ProductID.Equals(order.ProductIdList[i]),
                    });
                }
                productList.Add(new List<SelectListItem>(productData));
                productData.Clear();
            }

            ViewBag.productData = productList;

            foreach (var item in result)
            {
                price.Add(item.UnitPrice);
            }
            ViewBag.price = price;
            return View();
        }

        [HttpPost]
        public JsonResult Revise(Models.Order order)
        {
            Models.Service service = new Models.Service();
            return null;

        }

        public void ReviseOrder(Models.Order order)
        {
        
        }

        [HttpGet]
        public JsonResult Delete(int orderId)
        {
            Models.Service service = new Models.Service();
            service.DeleteOrderById(orderId);
            return null;
        }


    }
}