using Dal.Configurations;
using Dal.Entities;
using Microsoft.EntityFrameworkCore;

namespace Dal;

public class MainContext : DbContext
{
    public DbSet<BotUser> Users { get; set; }
    public DbSet<UserInfo> UserInfos { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlServer("Data Source=localhost\\SQLEXPRESS;User ID=sa;Password=1;Initial Catalog=UserRegistration;TrustServerCertificate=True;");
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new BotUserConfiguration());
        modelBuilder.ApplyConfiguration(new UserInfoConfiguration());
    }
}

/*
 * **1. Count how many products each customer has ordered:**

SELECT c.CustomerID, c.Name, COUNT(oi.OrderItemID) AS TotalProductsOrdered
FROM Customers c
LEFT JOIN Orders o ON c.CustomerID = o.CustomerID
LEFT JOIN OrderItems oi ON o.OrderID = oi.OrderID
GROUP BY c.CustomerID, c.Name;


**2. Find the total amount spent by each customer on all their orders:**

SELECT c.CustomerID, c.Name, COALESCE(SUM(oi.Quantity * oi.Price), 0) AS TotalSpent
FROM Customers c
LEFT JOIN Orders o ON c.CustomerID = o.CustomerID
LEFT JOIN OrderItems oi ON o.OrderID = oi.OrderID
GROUP BY c.CustomerID, c.Name;


**3. Get the most expensive product ordered:**

SELECT TOP 1 p.ProductID, p.Name, p.Price
FROM Products p
ORDER BY p.Price DESC;

**4. List all orders placed in the last 30 days:**

SELECT * FROM Orders
WHERE OrderDate >= DATEADD(DAY, -30, GETDATE());

**5. Find the customer who has placed the most orders:**

SELECT TOP 1 c.CustomerID, c.Name, COUNT(o.OrderID) AS TotalOrders
FROM Customers c
JOIN Orders o ON c.CustomerID = o.CustomerID
GROUP BY c.CustomerID, c.Name
ORDER BY TotalOrders DESC;

**6. List products that have not been ordered yet:**

SELECT * FROM Products
WHERE ProductID NOT IN (SELECT DISTINCT ProductID FROM OrderItems);

**7. Find the total revenue for each product:**

SELECT p.ProductID, p.Name, SUM(oi.Quantity * oi.Price) AS TotalRevenue
FROM Products p
JOIN OrderItems oi ON p.ProductID = oi.ProductID
GROUP BY p.ProductID, p.Name;

**8. Get the total number of products ordered by each customer:**

SELECT c.CustomerID, c.Name, COALESCE(SUM(oi.Quantity), 0) AS TotalProductsOrdered
FROM Customers c
LEFT JOIN Orders o ON c.CustomerID = o.CustomerID
LEFT JOIN OrderItems oi ON o.OrderID = oi.OrderID
GROUP BY c.CustomerID, c.Name;

**9. Find customers who haven’t placed any orders:**

SELECT * FROM Customers
WHERE CustomerID NOT IN (SELECT DISTINCT CustomerID FROM Orders);

**10. Get the top 5 most expensive products:**

SELECT TOP 5 * FROM Products ORDER BY Price DESC;

**11. List the top 3 customers who have spent the most:**

SELECT TOP 3 c.CustomerID, c.Name, SUM(oi.Quantity * oi.Price) AS TotalSpent
FROM Customers c
JOIN Orders o ON c.CustomerID = o.CustomerID
JOIN OrderItems oi ON o.OrderID = oi.OrderID
GROUP BY c.CustomerID, c.Name
ORDER BY TotalSpent DESC;

**12. List all orders and the total quantity of products per order:**

SELECT o.OrderID, SUM(oi.Quantity) AS TotalQuantity
FROM Orders o
JOIN OrderItems oi ON o.OrderID = oi.OrderID
GROUP BY o.OrderID;

**13. Find the number of products ordered by each customer, filtering for those who have ordered more than 5 products:**

SELECT c.CustomerID, c.Name, SUM(oi.Quantity) AS TotalProducts
FROM Customers c
JOIN Orders o ON c.CustomerID = o.CustomerID
JOIN OrderItems oi ON o.OrderID = oi.OrderID
GROUP BY c.CustomerID, c.Name
HAVING SUM(oi.Quantity) > 5;

**14. Get the products with the highest quantity ordered:**

SELECT TOP 1 p.ProductID, p.Name, SUM(oi.Quantity) AS TotalQuantity
FROM Products p
JOIN OrderItems oi ON p.ProductID = oi.ProductID
GROUP BY p.ProductID, p.Name
ORDER BY TotalQuantity DESC;

**15. List the total number of orders placed each month in 2024:**

SELECT MONTH(OrderDate) AS OrderMonth, COUNT(OrderID) AS TotalOrders
FROM Orders
WHERE YEAR(OrderDate) = 2024
GROUP BY MONTH(OrderDate)
ORDER BY OrderMonth;

**16. Find the customer who has placed the most expensive order:**

SELECT TOP 1 c.CustomerID, c.Name, SUM(oi.Quantity * oi.Price) AS OrderTotal
FROM Customers c
JOIN Orders o ON c.CustomerID = o.CustomerID
JOIN OrderItems oi ON o.OrderID = oi.OrderID
GROUP BY c.CustomerID, c.Name
ORDER BY OrderTotal DESC;

**17. List products and their total quantity sold, ordered by the most sold:**

SELECT p.ProductID, p.Name, SUM(oi.Quantity) AS TotalQuantity
FROM Products p
JOIN OrderItems oi ON p.ProductID = oi.ProductID
GROUP BY p.ProductID, p.Name
ORDER BY TotalQuantity DESC;

**18. Find customers who have spent more than \$500:**

SELECT c.CustomerID, c.Name, SUM(oi.Quantity * oi.Price) AS TotalSpent
FROM Customers c
JOIN Orders o ON c.CustomerID = o.CustomerID
JOIN OrderItems oi ON o.OrderID = oi.OrderID
GROUP BY c.CustomerID, c.Name
HAVING SUM(oi.Quantity * oi.Price) > 500;

**19. Get the average number of products ordered per order:**

SELECT AVG(TotalQuantity) AS AvgProductsPerOrder
FROM (
    SELECT o.OrderID, SUM(oi.Quantity) AS TotalQuantity
    FROM Orders o
    JOIN OrderItems oi ON o.OrderID = oi.OrderID
    GROUP BY o.OrderID
) AS OrderQuantities;


**20. Find the orders where total amount exceeds \$1000:**

SELECT o.OrderID, SUM(oi.Quantity * oi.Price) AS OrderTotal
FROM Orders o
JOIN OrderItems oi ON o.OrderID = oi.OrderID
GROUP BY o.OrderID
HAVING SUM(oi.Quantity * oi.Price) > 1000;

**21. Find the most recent order placed by "Alice Smith":**

SELECT TOP 1 * FROM Orders
WHERE CustomerID = (SELECT CustomerID FROM Customers WHERE Name = 'Alice Smith')
ORDER BY OrderDate DESC;

**22. List products ordered by "John Doe", ordered by total quantity:**

SELECT p.Name, SUM(oi.Quantity) AS TotalQuantity
FROM Products p
JOIN OrderItems oi ON p.ProductID = oi.ProductID
JOIN Orders o ON oi.OrderID = o.OrderID
JOIN Customers c ON o.CustomerID = c.CustomerID
WHERE c.Name = 'John Doe'
GROUP BY p.Name
ORDER BY TotalQuantity DESC;

**23. Find the total number of orders and products ordered by each customer in 2024:**

SELECT c.CustomerID, c.Name, COUNT(o.OrderID) AS TotalOrders, SUM(oi.Quantity) AS TotalProducts
FROM Customers c
JOIN Orders o ON c.CustomerID = o.CustomerID
JOIN OrderItems oi ON o.OrderID = oi.OrderID
WHERE YEAR(o.OrderDate) = 2024
GROUP BY c.CustomerID, c.Name;

**24. Har bir mahsulotning narxi, buyurtma qilingan miqdori va umumiy daromadi**

SELECT p.Name, p.Price, SUM(oi.Quantity) AS TotalQuantity, SUM(oi.Quantity \* oi.Price) AS TotalRevenue
FROM Products p
JOIN OrderItems oi ON p.ProductID = oi.ProductID
GROUP BY p.Name, p.Price;

**25. Oxirgi haftada buyurtma qilingan mahsulotlar va ularning umumiy daromadi**

SELECT p.Name, SUM(oi.Quantity) AS TotalQuantity, SUM(oi.Quantity \* oi.Price) AS TotalRevenue
FROM Products p
JOIN OrderItems oi ON p.ProductID = oi.ProductID
JOIN Orders o ON oi.OrderID = o.OrderID
WHERE o.OrderDate >= DATEADD(DAY, -7, GETDATE())
GROUP BY p.Name;




1. Count how many products each customer has ordered:
2. Find the total amount spent by each customer on all their orders: 
3. Get the most expensive product ordered:
4. List all orders placed in the last 30 days:
5. Find the customer who has placed the most orders:
6. List products that have not been ordered yet:
7. Find the total revenue for each product:
8. Get the total number of products ordered by each customer:
9. Find customers who haven’t placed any orders:
10. Get the top 5 most expensive products:
11. List the top 3 customers who have spent the most:
12. List all orders and the total quantity of products per order:
13. Find the number of products ordered by each customer, and filter for customers who have ordered more than 5 products:
14. Get the products with the highest quantity ordered:
15. List the total number of orders placed each month in 2024:
16. Find the customer who has placed the most expensive order:
17. List products and their total quantity sold, ordered by the most sold:
18. Find customers who have spent more than $500:
19. Get the average number of products ordered per order:
20. Find the orders where total amount exceeds $1000:
21. Find the most recent order placed by customer "Alice Smith":
22. List products ordered by customer "John Doe", ordered by the total quantity of each product:
23. Find the total number of orders and products ordered by each customer in 2024:
24. List all products with the price, quantity ordered, and total revenue for each product:
25. Find the products ordered in the last week and calculate the total revenue for each: 

1. Har bir mijoz qancha mahsulot buyurtma qilganligini hisoblang:
2. Har bir mijozning barcha buyurtmalariga sarflagan umumiy summasini toping:
3. Buyurtma qilingan eng qimmat mahsulotni oling:
4. Oxirgi 30 kun ichida berilgan barcha buyurtmalarni sanab o‘ting:
5. Eng ko'p buyurtma bergan mijozni toping:
6. Hali buyurtma qilinmagan mahsulotlarni sanab o'ting:
7. Har bir mahsulot uchun umumiy daromadni toping:
8. Har bir mijoz tomonidan buyurtma qilingan mahsulotlarning umumiy sonini oling:
9. Hech qanday buyurtma bermagan mijozlarni toping:
10. Eng qimmat 5 ta mahsulotni oling:
11. Eng ko'p pul sarflagan 3 ta mijozlarni sanab o'ting:
12. Barcha buyurtmalarni va buyurtma bo'yicha mahsulotning umumiy miqdorini sanab o'ting:
13. Har bir mijoz tomonidan buyurtma qilingan mahsulotlar sonini toping va 5 dan ortiq mahsulot buyurtma qilgan mijozlar uchun filtrlang:
14. Eng ko'p buyurtma qilingan mahsulotlarni oling:
15. 2024-yilda har oyda berilgan jami buyurtmalar sonini sanab o‘ting:
16. Eng qimmat buyurtma bergan mijozni toping:
17. Eng ko'p sotilganlar bo'yicha buyurtma qilingan mahsulotlar va ularning umumiy sotilgan miqdorini sanab o'ting:
18. 500 dollardan ortiq pul sarflagan mijozlarni toping:
19. Bir buyurtma bo'yicha buyurtma qilingan mahsulotlarning o'rtacha sonini oling:
20. Umumiy miqdori $1000 dan oshadigan buyurtmalarni toping:
21. “Elis Smit” mijozi tomonidan berilgan eng so‘nggi buyurtmani toping:
22. Har bir mahsulotning umumiy miqdori bo'yicha buyurtma qilingan "Jon Doe" mijozi tomonidan buyurtma qilingan mahsulotlar ro'yxati:
23. 2024 yilda har bir mijoz tomonidan buyurtma qilingan jami buyurtmalar va mahsulotlar sonini toping:
24. Barcha mahsulotlarni narxi, buyurtma qilingan miqdori va har bir mahsulot uchun umumiy daromadi bilan sanab o'ting:
25. O'tgan haftada buyurtma qilingan mahsulotlarni toping va har birining umumiy daromadini hisoblang:
 * */
