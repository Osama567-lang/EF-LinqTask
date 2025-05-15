using EF_LinqTask.Data;
using Microsoft.EntityFrameworkCore;

namespace LinqTask
{
    internal class Program
    {
        static void Main(string[] args)
        {
            ApplicationDbContext context = new ApplicationDbContext();

            #region (List all customers' first and last names along with their email addresses. )
            var Customers = context.Customers;
            foreach (var customer in Customers)
            {
                Console.WriteLine($"Name : {customer.FirstName} {customer.LastName} , Email {customer.Email}");
            }
            #endregion


            #region (Retrieve all orders processed by a specific staff member (e.g., staff_id = 3).)
            var OrdersByStaff3 = context.Orders.Include(o => o.Staff).Where(o => o.StaffId == 3);
            foreach (var o in OrdersByStaff3)
            {
                Console.WriteLine($"Orders : {o.OrderId} , Staff Id {o.StaffId}");
            }
            #endregion


            #region (Get all products that belong to a category named "Mountain Bikes")
            var MountainBikesProducts = context.Products.Include(p => p.Category)
                .Where(p => p.Category.CategoryName == "Mountain Bikes");

            foreach (var product in MountainBikesProducts)
            {
                Console.WriteLine($"Product: {product.ProductName}, Category: {product.Category.CategoryName}");
            }
            #endregion


            #region (Count the total number of orders per store.)
            var orderCountsPerStore = context.Orders
                .GroupBy(o => o.Store.StoreName)
                .Select(s => new
                {
                    StoreName = s.Key,
                    OrderCount = s.Count()
                });

            foreach (var order in orderCountsPerStore)
            {
                Console.WriteLine($"Store: {order.StoreName}, Total Orders: {order.OrderCount}");
            }

            #endregion


            #region (List all orders that have not been shipped yet (shipped_date is null).)
            var OrdersNotShippedYet = context.Orders.Where(o => o.OrderDate == null);

            foreach (var order in OrdersNotShippedYet)
            {
                Console.WriteLine($"OrderId: {order.OrderId}, Order Date: {order.OrderDate}");
            }
            #endregion


            #region (Display each customer’s full name and the number of orders they have placed.)
            var OrderByCustomer = context.Customers.Include(c => c.Orders)
                .Select(c => new
                {
                    c.FirstName,
                    c.LastName,
                    OrderCount = c.Orders.Count
                });
            foreach (var item in OrderByCustomer)
            {
                Console.WriteLine($"FirstName : {item.FirstName} , LastName : {item.LastName} , Num Of Orders : {item.OrderCount}");
            }
            #endregion


            #region (List all products that have never been ordered (not found in order_items). 
            var ProductsNeverOrdered = context.Products.Where(p => !p.OrderItems.Any());
            foreach (var product in ProductsNeverOrdered)
            {
                Console.WriteLine($"Product ID: {product.ProductId}, Name: {product.ProductName}");
            }
            #endregion


            #region ( Display products that have a quantity of less than 5 in any store stock.)
            var lowStocks = context.Stocks
            .Include(s => s.Product)
            .Include(s => s.Store)
            .Where(s => s.Quantity < 5);
            foreach (var stock in lowStocks)
            {
                Console.WriteLine($"Product: {stock.Product.ProductName}, Store: {stock.Store.StoreName}, Quantity: {stock.Quantity}");
            }
            #endregion


            #region ( Retrieve the first product from the products table.)
            var FirstProduct = context.Products.FirstOrDefault();
            if (FirstProduct != null)
            {
                Console.WriteLine($"Product ID: {FirstProduct.ProductId}, Name: {FirstProduct.ProductName}");
            }
            else
            {
                Console.WriteLine("No products found.");
            }
            #endregion


            #region (Retrieve all products from the products table with a certain model year.)
            var ModelYearProducts = context.Products.Select(p => new
            {
                p.ProductName,
                p.ModelYear
            });
            foreach (var p in ModelYearProducts)
            {
                Console.WriteLine($"Name: {p.ProductName}, Model Year: {p.ModelYear}");
            }
            #endregion


            #region ( Display each product with the number of times it was ordered. )
            var productsWithOrderCount = context.Products
            .Select(p => new
            {
                p.ProductName,
                OrderCount = p.OrderItems.Count
            });
            foreach (var product in productsWithOrderCount)
            {
                Console.WriteLine($"Product: {product.ProductName}, Times Ordered: {product.OrderCount}");
            }
            #endregion


            #region (Count the number of products in a specific category.)
            var productCountsByCategory = context.Products
            .GroupBy(p => p.Category.CategoryName)
            .Select(g => new
            {
                CategoryName = g.Key,
                Count = g.Count()
            });
            foreach (var item in productCountsByCategory)
            {
                Console.WriteLine($"Category: {item.CategoryName}, Product Count: {item.Count}");
            }
            #endregion


            #region ( Calculate the average list price of products. )
            var averagePrice = context.Products
            .Average(p => p.ListPrice);
            Console.WriteLine($"Average List Price: {averagePrice}");
            #endregion


            #region ( Retrieve a specific product from the products table by ID. )
            Console.Write("Please enter Product ID: ");
            string? input = Console.ReadLine();

            if (int.TryParse(input, out int productId))
            {
                var product = context.Products
                    .Where(p => p.ProductId == productId)
                    .Select(p => new
                    {
                        p.ProductId,
                        p.ProductName
                    })
                    .FirstOrDefault();

                if (product != null)
                {
                    Console.WriteLine($"Product found: ID = {product.ProductId}, Name = {product.ProductName}");
                }
                else
                {
                    Console.WriteLine("Product with this ID does not exist.");
                }
            }
            else
            {
                Console.WriteLine("Invalid input. Please enter a valid number.");
            }
            #endregion


            #region (-List all products that were ordered with a quantity greater than 3 in any order. )
            var productsWithLargeOrders = context.OrderItems
                .Include(oi => oi.Product)
                .Where(oi => oi.Quantity > 3)
                .Select(oi => oi.Product);
            if (productsWithLargeOrders.Any())
            {
                foreach (var product in productsWithLargeOrders)
                {
                    Console.WriteLine($"Product ID: {product.ProductId}, Name: {product.ProductName}");
                }
            }
            else
            {
                Console.WriteLine("No products were ordered with quantity greater than 3.");
            }
            #endregion


            #region (Display each staff member’s name and how many orders they processed.)
            var ordersCountByStaff = context.Staffs
            .Select(s => new
            {
                StaffName = s.FirstName + " " + s.LastName,
                OrdersCount = s.Orders.Count()
            });
            foreach (var item in ordersCountByStaff)
            {
                Console.WriteLine($"Staff: {item.StaffName}, Orders Processed: {item.OrdersCount}");
            }
            #endregion


            #region ( List active staff members only (active = true) along with their phone numbers.)
            var activeStaff = context.Staffs
            .Where(s => s.Active == 1)
            .Select(s => new
            {
                FullName = s.FirstName + " " + s.LastName,
                PhoneNumber = s.Phone
            });
            foreach (var staff in activeStaff)
            {
                Console.WriteLine($"Name: {staff.FullName}, Phone: {staff.PhoneNumber}");
            }
            #endregion


            #region (List all products with their brand name and category name. )
            var productsWithDetails = context.Products
            .Include(p => p.Brand)
            .Include(p => p.Category)
            .Select(p => new
            {
                p.ProductName,
                p.Brand.BrandName,
                p.Category.CategoryName
            });
            foreach (var product in productsWithDetails)
            {
                Console.WriteLine($"Product: {product.ProductName}, Brand: {product.BrandName}, Category: {product.CategoryName}");
            }
            #endregion


            #region (Retrieve orders that are completed.)
            var completedOrders = context.Orders
            .Where(o => o.OrderStatus == 2);
            foreach (var order in completedOrders)
            {
                Console.WriteLine($"Order ID: {order.OrderId}, Status: {order.OrderStatus}");
            }

            var AnotherWaycompletedOrders = context.Orders
            .Where(o => o.OrderStatus == 2) // أو (byte)OrderStatusEnum.Completed
            .Select(o => new
            {
                o.OrderId,
                o.OrderStatus,
                StatusDescription = o.OrderStatus == 1 ? "Pending" :
                                    o.OrderStatus == 2 ? "Completed" :
                                    o.OrderStatus == 3 ? "Cancelled" : "Unknown"
            });
            foreach (var order in completedOrders)
            {
                Console.WriteLine($"Order ID: {order.OrderId}, Status: {order.OrderStatus}, Description: Completed");
            }

            #endregion


            #region ( List each product with the total quantity sold (sum of quantity from order_items))
            var productsWithTotalSold = context.Products
            .Select(p => new
            {
                p.ProductId,
                p.ProductName,
                TotalQuantitySold = p.OrderItems.Sum(oi => (int?)oi.Quantity) ?? 0
            });
            foreach (var product in productsWithTotalSold)
            {
                Console.WriteLine($"Product ID: {product.ProductId}, Name: {product.ProductName}, Total Quantity Sold: {product.TotalQuantitySold}");
            }
            #endregion
        }
    }
}
