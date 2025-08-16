using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventory_Reorder_Advisor
{
    class Item
    {
        public string Code { get; }
        public string Name { get; }
        public double UnitPrice { get; }
        public int CurrentStock { get; }
        public int MinStock { get; }
        public int[] SalesLast7Days { get; }

        public Item(string code, string name, double unitPrice, int currentStock, int minStock, int[] salesLast7Days)
        {
            Code = code;
            Name = name;
            UnitPrice = unitPrice;
            CurrentStock = currentStock;
            MinStock = minStock;
            SalesLast7Days = salesLast7Days;
        }

        public bool NeedsReorder(int leadTime)
        {
            double avgDailySales = ComputeMovingAverage();
            double stockNeeded = avgDailySales * leadTime;
            return stockNeeded > CurrentStock;
        }

        private double ComputeMovingAverage()
        {
            return SalesLast7Days.Average();
        }

        public double UrgencyScore(int leadTime)
        {
            double avgDailySales = ComputeMovingAverage();
            return (avgDailySales * leadTime) - CurrentStock;
        }
    }
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.Write("Enter the number of items: ");
            int itemCount = int.Parse(Console.ReadLine());
            List<Item> items = new List<Item>();

            for (int i = 0; i < itemCount; i++)
            {
                Console.Write($"Enter code for item {i + 1}: ");
                string code = Console.ReadLine();

                Console.Write($"Enter name for item {i + 1}: ");
                string name = Console.ReadLine();

                double unitPrice = GetPositiveDouble($"Enter unit price for item {i + 1}: ");
                int currentStock = GetPositiveInt($"Enter current stock for item {i + 1}: ");
                int minStock = GetPositiveInt($"Enter minimum stock for item {i + 1}: ");

                int[] salesLast7Days = GetSalesData();

                items.Add(new Item(code, name, unitPrice, currentStock, minStock, salesLast7Days));
            }
            Console.Write("Enter lead time (in days): ");
            int leadTime = int.Parse(Console.ReadLine());

            var reorderList = items.Where(item => item.NeedsReorder(leadTime))
                                    .OrderBy(item => item.UrgencyScore(leadTime))
                                    .ToList();

            Console.WriteLine("\nReorder List:");
            foreach (var item in reorderList)
            {
                Console.WriteLine($"{item.Code} - {item.Name}: Urgency Score = {item.UrgencyScore(leadTime):F2}");
            }
        }
        static double GetPositiveDouble(string prompt)
        {
            double value;
            while (true)
            {
                Console.Write(prompt);
                if (double.TryParse(Console.ReadLine(), out value) && value >= 0)
                {
                    return value;
                }
                Console.WriteLine("Invalid input. Please enter a positive number.");
            }
        }
        static int GetPositiveInt(string prompt)
        {
            int value;
            while (true)
            {
                Console.Write(prompt);
                if (int.TryParse(Console.ReadLine(), out value) && value >= 0)
                {
                    return value;
                }
                Console.WriteLine("Invalid input. Please enter a positive integer.");
            }
        }
        static int[] GetSalesData()
        {
            int[] sales = new int[7];
            for (int i = 0; i < 7; i++)
            {
                sales[i] = GetPositiveInt($"Enter sales for day {i + 1}: ");
            }
            return sales;
        }
    }
}
