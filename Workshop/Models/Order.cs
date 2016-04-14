using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Workshop.Models
{
    public class Order
    {
        /// <summary>
        /// 訂單編號
        /// </summary>
        public int OrderID {get;set;}

        /// <summary>
        /// 客戶代號
        /// </summary>
        public int CustomerID {get;set;}

        /// <summary>
        /// 客戶名稱
        /// </summary>
        public string CustomerName { get; set; }

        /// <summary>
        /// 業務(員工)代號
        /// </summary>
        public int EmployeeID {get;set;}

        /// <summary>
        /// 業務(員工姓名)
        /// </summary>
        public string EmployeeName { get; set; }

        /// <summary>
        /// 商品代號
        /// </summary>
        public int ProductID { get; set; }

        /// <summary>
        /// 商品名稱
        /// </summary>
        public string ProductName { get; set; }

        /// <summary>
        /// 商品單價
        /// </summary>
        public string UnitPrice { get; set; }

        /// <summary>
        /// 商品數量
        /// </summary>
        public int Qty { get; set; }

        /// <summary>
        /// 訂單日期
        /// </summary>
        public DateTime ? OrderDate {get;set;}

        /// <summary>
        /// 需要日期
        /// </summary>
        public DateTime? RequiredDate { get; set; }

        /// <summary>
        /// 出貨日期
        /// </summary>
        public DateTime?  ShippedDate { get; set; }

        /// <summary>
        /// 出貨公司代號
        /// </summary>
        public int ShipperId {get;set;}

        /// <summary>
        /// 出貨公司名稱
        /// </summary>
        public string ShipperName { get; set; }

        /// <summary>
        /// 運費
        /// </summary>
        public string Freight {get;set;}

        /// <summary>
        /// 出貨說明
        /// </summary>
        public string ShipName {get;set;}

        /// <summary>
        /// 出貨地址
        /// </summary>
        public string ShipAddress {get;set;}

        /// <summary>
        /// 出貨城市
        /// </summary>
        public string ShipCity {get;set;}

        /// <summary>
        /// 出貨地區
        /// </summary>
        public string ShipRegion {get;set;}

        /// <summary>
        /// 郵遞區號
        /// </summary>
        public string ShipPostalCode {get;set;}

        /// <summary>
        /// 出貨國家
        /// </summary>
        public string ShipCountry { get; set; }

        public List<int> ProductIdList { get; set; }

        public List<string> UnitPriceList { get; set; }

        public List<string> QtyList { get; set; }
    }
}
