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
        public List<Models.Order> GetProductData()
        {
            DataTable dt = new DataTable();
            string str = @"SELECT ProductID,ProductName,UnitPrice
                            FROM Production.Products";
            using (SqlConnection conn = new SqlConnection(this.GetConnectionString()))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand(str, conn);
                SqlDataAdapter dad = new SqlDataAdapter(cmd);
                dad.Fill(dt);
                conn.Close();
            }
            return this.ConvertProductToList(dt);
        }

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

        public List<Models.Order> GetCustomerData() {
            DataTable dt = new DataTable();
            string str = @"SELECT CustomerID,CompanyName
                            FROM Sales.Customers";
            using (SqlConnection conn = new SqlConnection(this.GetConnectionString()))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand(str, conn);
                SqlDataAdapter dad = new SqlDataAdapter(cmd);
                dad.Fill(dt);
                conn.Close();
            }
            return this.ConvertCustomerToList(dt);
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

        public void DeleteOrderById(int orderId)
        {
            string str = @"DELETE FROM Sales.OrderDetails
                            WHERE OrderID = @OrderID
                            DELETE FROM Sales.Orders
                            WHERE OrderID = @OrderID";
            using (SqlConnection conn = new SqlConnection(this.GetConnectionString()))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand(str, conn);
                cmd.Parameters.Add(new SqlParameter("@OrderID", orderId));
                cmd.ExecuteNonQuery();
                conn.Close();
            }
        }

        public void InsertOrder(Models.Order order)
        {
            
            string str = @"INSERT INTO Sales.Orders(
				                        CustomerID, EmployeeID, OrderDate, RequiredDate,
				                        ShippedDate, ShipperID, Freight, ShipCountry,
				                        ShipCity, ShipRegion, ShipPostalCode, ShipAddress, ShipName)
                            VALUES( 
                                        @CustomerID, @EmployeeID, @OrderDate, @RequiredDate,
				                        @ShippedDate, @ShipperID, @Freight, @ShipCountry,
				                        @ShipCity, @ShipRegion, @ShipPostalCode, @ShipAddress, @ShipName)
                            SELECT SCOPE_IDENTITY() OrderID";

            string strDetail = @"INSERT INTO Sales.OrderDetails(
				                        OrderID,ProductID,UnitPrice,Qty,Discount)
                            VALUES(
				                        @OrderID,@ProductID,@UnitPrice,@Qty,@Discount)";
            string orderID = "";
            using (SqlConnection conn = new SqlConnection(this.GetConnectionString()))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand(str, conn);
                cmd.Parameters.Add(new SqlParameter("@CustomerID", order.CustomerID));
                cmd.Parameters.Add(new SqlParameter("@EmployeeID", order.EmployeeID));
                cmd.Parameters.Add(new SqlParameter("@OrderDate", order.OrderDate));
                cmd.Parameters.Add(new SqlParameter("@RequiredDate", order.RequiredDate));
                cmd.Parameters.Add(new SqlParameter("@ShippedDate", order.ShippedDate ?? (object)DBNull.Value));
                cmd.Parameters.Add(new SqlParameter("@ShipperID", order.ShipperId));
                cmd.Parameters.Add(new SqlParameter("@Freight", order.Freight ?? string.Empty));
                cmd.Parameters.Add(new SqlParameter("@ShipCountry", order.ShipCountry ?? string.Empty));
                cmd.Parameters.Add(new SqlParameter("@ShipCity", order.ShipCity ?? string.Empty));
                cmd.Parameters.Add(new SqlParameter("@ShipRegion", order.ShipRegion ?? (object)DBNull.Value));
                cmd.Parameters.Add(new SqlParameter("@ShipPostalCode", order.ShipPostalCode ?? (object)DBNull.Value));
                cmd.Parameters.Add(new SqlParameter("@ShipAddress", order.ShipAddress ?? string.Empty));
                cmd.Parameters.Add(new SqlParameter("@ShipName", order.ShipName ?? string.Empty));
                orderID = cmd.ExecuteScalar().ToString();
                for (int i = 0; i < order.ProductIdList.Count; i++)
                {
                    cmd = new SqlCommand(strDetail, conn);
                    cmd.Parameters.Add(new SqlParameter("@OrderID", orderID));
                    cmd.Parameters.Add(new SqlParameter("@ProductID", order.ProductIdList[i]));
                    cmd.Parameters.Add(new SqlParameter("@UnitPrice", order.UnitPriceList[i]));
                    cmd.Parameters.Add(new SqlParameter("@Qty", order.QtyList[i]));
                    cmd.Parameters.Add(new SqlParameter("@Discount", "0"));
                    cmd.ExecuteNonQuery();
                }
                conn.Close();
            }
        }

        public Models.Order ReviseOrder(int orderID)
        {
            DataTable dt = new DataTable();
            DataTable dtDetail = new DataTable();
            string str = @"SELECT CustomerID, EmployeeID, OrderDate, RequiredDate,
				                        ShippedDate, ShipperID, Freight, ShipCountry,
				                        ShipCity, ShipRegion, ShipPostalCode, ShipAddress, ShipName
										FROM Sales.Orders
										WHERE OrderID=@OrderID";

            string strDetail = @"SELECT ProductID,UnitPrice,Qty
                                        FROM Sales.OrderDetails
                                        WHERE OrderID=@OrderID";
            using (SqlConnection conn = new SqlConnection(this.GetConnectionString()))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand(str, conn);
                cmd.Parameters.Add(new SqlParameter("@OrderID", orderID));
                SqlDataAdapter dad = new SqlDataAdapter(cmd);
                dad.Fill(dt);
                cmd = new SqlCommand(strDetail, conn);
                cmd.Parameters.Add(new SqlParameter("@OrderID", orderID));
                dad = new SqlDataAdapter(cmd);
                dad.Fill(dtDetail);
                conn.Close();
            }
            return this.ConvertReviseToList(dt, dtDetail);
        }

        public void Update(Models.Order order) {
            DataTable dt = new DataTable();
            string sql = @"SELECT
                                ProductID
                            FROM
                                Sales.OrderDetails
                            WHERE
                                OrderID = @OrderID";
            string updateOrderSql = @"UPDATE Sales.Orders
                                    SET CustomerID = @CustomerID,
                                        EmployeeID = @EmployeeID,
	                                    OrderDate = @OrderDate,
	                                    RequiredDate = @RequiredDate,
	                                    ShippedDate = @ShippedDate,
	                                    ShipperID = @ShipperID,
	                                    Freight = @Freight,
	                                    ShipCountry = @ShipCountry,
	                                    ShipCity = @ShipCity,
	                                    ShipRegion = @ShipRegion,
	                                    ShipPostalCode = @ShipPostalCode,
	                                    ShipAddress = @ShipAddress,
	                                    ShipName = @ShipName
                                    WHERE OrderID = @OrderID";
            string updateOrderDetailSql = @"UPDATE Sales.OrderDetails
                                            SET ProductID = @ProductID,
                                                UnitPrice = @UnitPrice,
	                                            Qty = @Qty
                                            WHERE OrderID = @OrderID AND ProductID = @ProductID";
            string InsertOrderDetailSql = @"INSERT INTO Sales.OrderDetails (
			                                    OrderID,
			                                    ProductID,
		                                        UnitPrice,
                                                Qty,
		                                        Discount
                                            )
                                            VALUES (
                                                @OrderID,
                                                @ProductID,
                                                @UnitPrice,
                                                @Qty,
                                                @Discount
                                            )";
            string DeleteOrderDetailSql = @"DELETE FROM Sales.OrderDetails WHERE OrderID = @OrderID AND ProductID = @ProductID";
            using (SqlConnection conn = new SqlConnection(this.GetConnectionString()))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand(updateOrderSql, conn);
                cmd.Parameters.Add(new SqlParameter("@OrderID", order.OrderID));
                cmd.Parameters.Add(new SqlParameter("@CustomerID", order.CustomerID));
                cmd.Parameters.Add(new SqlParameter("@EmployeeID", order.EmployeeID));
                cmd.Parameters.Add(new SqlParameter("@OrderDate", order.OrderDate));
                cmd.Parameters.Add(new SqlParameter("@RequiredDate", order.RequiredDate));
                cmd.Parameters.Add(new SqlParameter("@ShippedDate", order.ShippedDate ?? (object)DBNull.Value));
                cmd.Parameters.Add(new SqlParameter("@ShipperID", order.ShipperId));
                cmd.Parameters.Add(new SqlParameter("@Freight", order.Freight ?? string.Empty));
                cmd.Parameters.Add(new SqlParameter("@ShipCountry", order.ShipCountry ?? string.Empty));
                cmd.Parameters.Add(new SqlParameter("@ShipCity", order.ShipCity ?? string.Empty));
                cmd.Parameters.Add(new SqlParameter("@ShipRegion", order.ShipRegion ?? (object)DBNull.Value));
                cmd.Parameters.Add(new SqlParameter("@ShipPostalCode", order.ShipPostalCode ?? (object)DBNull.Value));
                cmd.Parameters.Add(new SqlParameter("@ShipAddress", order.ShipAddress ?? string.Empty));
                cmd.Parameters.Add(new SqlParameter("@ShipName", order.ShipName ?? string.Empty));
                cmd.ExecuteNonQuery();
                cmd = new SqlCommand(sql, conn);
                cmd.Parameters.Add(new SqlParameter("@OrderID", order.OrderID));
                SqlDataAdapter sqlAdapter = new SqlDataAdapter(cmd);
                sqlAdapter.Fill(dt);
                conn.Close();
            }
            List<int> orderDetail = new List<int>();
            foreach (DataRow row in dt.Rows)
            {
                orderDetail.Add((int)row["ProductID"]);
            }
            for (int i = 0; i < orderDetail.Count; i++)
            {
                if (!order.ProductIdList.Contains(orderDetail[i]))
                {
                    using (SqlConnection conn = new SqlConnection(this.GetConnectionString()))
                    {
                        conn.Open();
                        SqlCommand cmd = new SqlCommand(DeleteOrderDetailSql, conn);
                        cmd.Parameters.Add(new SqlParameter("@OrderID", order.OrderID));
                        cmd.Parameters.Add(new SqlParameter("@ProductID", orderDetail[i]));
                        cmd.ExecuteNonQuery();
                        conn.Close();
                    }
                }
            }
            for (int i = 0; i < order.ProductIdList.Count; i++)
            {
                if (orderDetail.Contains(order.ProductIdList[i]))
                {
                    using (SqlConnection conn = new SqlConnection(this.GetConnectionString()))
                    {
                        conn.Open();
                        SqlCommand cmd = new SqlCommand(updateOrderDetailSql, conn);
                        cmd.Parameters.Add(new SqlParameter("@OrderID", order.OrderID));
                        cmd.Parameters.Add(new SqlParameter("@ProductID", order.ProductIdList[i]));
                        cmd.Parameters.Add(new SqlParameter("@UnitPrice", order.UnitPriceList[i]));
                        cmd.Parameters.Add(new SqlParameter("@Qty", order.QtyList[i]));
                        cmd.ExecuteNonQuery();
                        conn.Close();
                    }
                }
                else if (!orderDetail.Contains(order.ProductIdList[i]))
                {
                    using (SqlConnection conn = new SqlConnection(this.GetConnectionString()))
                    {
                        conn.Open();
                        SqlCommand cmd = new SqlCommand(InsertOrderDetailSql, conn);
                        cmd.Parameters.Add(new SqlParameter("@OrderID", order.OrderID));
                        cmd.Parameters.Add(new SqlParameter("@ProductID", order.ProductIdList[i]));
                        cmd.Parameters.Add(new SqlParameter("@UnitPrice", order.UnitPriceList[i]));
                        cmd.Parameters.Add(new SqlParameter("@Qty", order.QtyList[i]));
                        cmd.Parameters.Add(new SqlParameter("@Discount", "0"));
                        cmd.ExecuteNonQuery();
                        conn.Close();
                    }
                }
            }
        }

        private Models.Order ConvertReviseToList(DataTable dt, DataTable dtDetail)
        {
            Models.Order order = new Order();
            foreach (DataRow item in dt.Rows)
            {
                order.CustomerID = (int)item["CustomerID"];
                order.EmployeeID = (int)item["EmployeeID"];
                order.OrderDate = (DateTime)item["OrderDate"];
                order.RequiredDate = (DateTime)item["RequiredDate"];
                order.ShippedDate = (DateTime?)item["ShippedDate"];
                order.ShipperId = (int)item["ShipperId"];
                order.Freight = item["Freight"].ToString();
                order.ShipCountry = item["ShipCountry"].ToString();
                order.ShipCity = item["ShipCity"].ToString();
                order.ShipRegion = item["ShipRegion"].ToString();
                order.ShipPostalCode = item["ShipPostalCode"].ToString();
                order.ShipAddress = item["ShipAddress"].ToString();
                order.ShipName = item["ShipName"].ToString();
            }
            order.ProductIdList = new List<int>();
            order.UnitPriceList = new List<string>();
            order.QtyList = new List<string>();
            foreach (DataRow item in dtDetail.Rows)
            {
                order.ProductIdList.Add((int)item["ProductID"]);
                order.UnitPriceList.Add(item["UnitPrice"].ToString());
                order.QtyList.Add(item["Qty"].ToString());
            }
            return order;
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

        private List<Order> ConvertCustomerToList(DataTable dt)
        {
            List<Models.Order> list = new List<Order>();
            foreach (DataRow item in dt.Rows)
            {
                list.Add(new Order
                {
                    CustomerID = (int)item["CustomerID"],
                    CustomerName = item["CompanyName"].ToString(),
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

        private List<Order> ConvertProductToList(DataTable dt)
        {
            List<Models.Order> list = new List<Order>();
            foreach (DataRow item in dt.Rows)
            {
                list.Add(new Order
                {
                    ProductID = (int)item["ProductID"],
                    ProductName = item["ProductName"].ToString(),
                    UnitPrice = item["UnitPrice"].ToString(),
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