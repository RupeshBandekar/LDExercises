﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventStoreSaleExercise
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("1 - Salesman");
            Console.WriteLine("2 - Inventory Manager");
            Console.WriteLine("3 - Director");
            var input = Console.ReadLine();
            if (input == "1")
            {
                while (true)
                {
                    Console.WriteLine("Please enter your sales details");
                    Console.WriteLine("Product Name:");
                    string productName = Console.ReadLine();
                    Console.WriteLine("Quantity:");
                    string quantity = Console.ReadLine();
                    Console.WriteLine("Price:");
                    string price = Console.ReadLine();

                    Sales objSales = new Sales(productName, Convert.ToInt32(quantity), Convert.ToDecimal(price));
                    objSales.AddSale(objSales);
                }
            }
            else if (input == "2")
            {
                
            }
            else if (input == "3")
            {
                
            }
        }
    }
}
