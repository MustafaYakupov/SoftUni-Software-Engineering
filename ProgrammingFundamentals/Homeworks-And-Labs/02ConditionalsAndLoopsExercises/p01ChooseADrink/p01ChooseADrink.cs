﻿using System;

namespace p01ChooseADrink
{
    class p01ChooseADrink
    {
        static void Main(string[] args)
        {
            string profession = Console.ReadLine().ToLower();

            switch (profession)
            {
                case "athlete":;
                    Console.WriteLine("Water");
                    break;
                case "businessman":
                case "businesswoman":
                    Console.WriteLine("Coffee");
                    break;
                case "softuni student":
                    ;
                    Console.WriteLine("Beer");
                    break;
                default:
                    ;
                    Console.WriteLine("Tea");
                    break;


            }
        }
    }
}
