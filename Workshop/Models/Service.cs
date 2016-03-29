using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Data.SqlClient;

namespace Workshop.Models
{
    public class Service
    {
        public List<Models.Order> GetEmpData() {
            DataTable dt = new DataTable();
            string str = @"SELECT EmployeeID, FirstName+' '+LastName as EmployeeName 
                            FROM HR.Employees";
            using (SqlConnection conn = new SqlConnection(this.GetConnectionString()))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand(str, conn);
                SqlDataAdapter dad = new SqlDataAdapter(cmd);
                dad.Fill(dt);
                conn.Close();
            }
            return this.ConvertEmpToList(dt);
        }

        public List<Models.Order> GetShipperData() {
            DataTable dt = new DataTable();
            string str = @"SELECT ShipperID,CompanyName
                            FROM Sales.Shippers";
            using (SqlConnection conn = new SqlConnection(this.GetConnectionString()))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand(str, conn);
                SqlDataAdapter dad = new SqlDataAdapter(cmd);
                dad.Fill(dt);
                conn.Close();
            }
            return this.ConvertShipperToList(dt);
        }

        public List<Models.Order> GetOrderData(Models.Order order)
        {
            DataTable dt = new DataTable();
            string str = @"SELECT A.OrderID,B.CompanyName AS CustomerName,A.OrderDate,A.ShippedDate
                            FROM Sales.Orders AS A JOIN Sales.Customers AS B ON A.CustomerID=B.CustomerID
                            WHERE (A.OrderID=@OrderID OR @OrderID=0) AND (B.CompanyName=@CustomerName OR @CustomerName IS NULL)
                            AND (A.EmployeeID=@EmployeeID OR @EmployeeID=0)
                            AND (A.ShipperID=@ShipperID OR @ShipperID=0)
                            AND (A.OrderDate=@OrderDate OR @OrderDate IS NULL)
                            AND (A.ShippedDate=@ShippedDate OR @ShippedDate IS NULL)
                            AND (A.RequiredDate=@RequiredDate OR @RequiredDate IS NULL)";
            using (SqlConnection conn = new SqlConnection(this.GetConnectionString()))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand(str, conn);
                cmd.Parameters.Add(new SqlParameter("@OrderID", order.OrderID));
                cmd.Parameters.Add(new SqlParameter("@CustomerName", order.CustomerName == null ? (object)DBNull.Value : order.CustomerName));
                cmd.Parameters.Add(new SqlParameter("@EmployeeID", order.EmployeeID));
                cmd.Parameters.Add(new SqlParameter("@ShipperID", order.ShipperId));
                cmd.Parameters.Add(new SqlParameter("@OrderDate", order.OrderDate == null ? (object)DBNull.Value : order.OrderDate));
                cmd.Parameters.Add(new SqlParameter("@ShippedDate", order.ShippedDate == null ? (object)DBNull.Value : order.ShippedDate));
                cmd.Parameters.Add(new SqlParameter("@RequiredDate", order.RequiredDate == null ? (object)DBNull.Value : order.RequiredDate));
                SqlDataAdapter dad = new SqlDataAdapter(cmd);
                dad.Fill(dt);
                conn.Close();
            }
            return this.ConvertOrderToList(dt);
        }

        private List<Order> ConvertOrderToList(DataTable dt)
        {
            List<Models.Order> list = new List<Order>();
            foreach (DataRow item in dt.Rows)
            {
                list.Add(new Order
                {
                    OrderID = (int)item["OrderID"],
                    CustomerName = item["CustomerName"].ToString(),
                    OrderDate = (DateTime)item["OrderDate"],
                    ShippedDate = item["ShippedDate"] == DBNull.Value ? (DateTime?)null : (DateTime)item["ShippedDate"]
                });
            }
            return list;
        }

        private List<Order> ConvertShipperToList(DataTable dt)
        {
            List<Models.Order> list = new List<Order>();
            foreach (DataRow item in dt.Rows)
            {
                list.Add(new Order
                {
                    ShipperId = (int)item["ShipperID"],
                    ShipperName = item["CompanyName"].ToString(),
                });
            }
            return list;


        }

        private List<Order> ConvertEmpToList(DataTable dt)
        {
            List<Models.Order> list = new List<Order>();
            foreach (DataRow item in dt.Rows)
            {
                list.Add(new Order {
                    EmployeeID = (int)item["EmployeeID"],
                    EmployeeName = item["EmployeeName"].ToString(),
                });
            }
            return list;
        }

        private string GetConnectionString()
        {
            return System.Configuration.ConfigurationManager.ConnectionStrings["DefaultDB"].ConnectionString.ToString();
        }

    }
    
}