﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DbBest.ZooPark
{
    class Program
    {
        static void Main(string[] args)
        {
            // RunTests();
            RunZoo();

            Console.ReadLine();     // wait to see output
        }


        static void RunTests()
        {
            Zoo ZooInstance = new Zoo(animals: 3, ceils: 1, foodPackage: 3, animalsTypes: 2, foodTypes: 1, logFile: "test.txt" );

            ZooInstance.TestNumberWithBase();
            ZooInstance.ShutDownWork();
        }

        static void RunZoo()
        {
            Zoo ZooInstance = new Zoo(animals: 10, ceils: 15, foodPackage: 40, animalsTypes: 3, foodTypes: 4);
            ZooInstance.DisplayZooDebugInfo();
            ZooInstance.Run();
        }


        static void RunRandomZoo()
        {
            // ====== generate random zoo 
            //Random rnd = new Random();
            //int foodPackage = rnd.Next(1, 10);
            //int foodTypes = rnd.Next(1, 10);

            //Zoo ZooInstance = new Zoo(animals: 5, ceils: 7, foodPackage: foodPackage, animalsTypes: 2, foodTypes: foodTypes);

            // ZooInstance.DisplayZooDebugInfo();

            // ZooInstance.Run();
        }



    }
}
