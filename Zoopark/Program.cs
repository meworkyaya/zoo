using System;
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
            // RunFoodZoo();
            // RunCeilZoo();
            // RunRandomZoo();

            RunFoodTests(); // used for testing some parts of code
            // RunCeilTests(); // used for testing some parts of code

            Console.ReadLine();     // wait to see output
        }


        static void RunCeilTests()
        {
            int count = 0;
            Console.WriteLine("\r\nTEST create empty Zoo ##############");
            Zoo ZooInstance = new Zoo(animals: 0, ceils: 0, foodPackage: 0, animalsTypes: 0, foodTypes: 0, logFile: "test.txt");
            ZooInstance.ShutDownWork();
            Console.WriteLine("\r\nTest Done #############################\r\n\r\n");

            //Console.WriteLine("\r\nTEST inc numbers ====================");
            //ZooInstance.TestNumberWithBase(baseOfNumber: 2, numberOfDigits: 4);
            //ZooInstance.ShutDownWork();

            Zoo ZooInstance_2 = null;
            for (int i = 1; i < 7; i++)
            {
                count = i;
                Console.WriteLine("\r\nTEST create Zoo {0} animals {1} types {2} ceils 0 neighbour rule ##############", count, count, count);
                ZooInstance_2 = new Zoo(animals: count, ceils: count, foodPackage: 0, animalsTypes: count, foodTypes: 0, logFile: "test" + count + ".txt");
                ZooInstance_2.TestNxNxNCreate(count);
                ZooInstance_2.FindCeilSolution();
                ZooInstance_2.ShutDownWork();
                Console.WriteLine("\r\nTest Done #############################\r\n\r\n");
            }
        }

        static void RunFoodTests()
        {
            int count = 0;

            Zoo ZooInstance_2 = null;

            count = 0;
            Console.WriteLine("\r\nTEST Food 1 ##############");
            ZooInstance_2 = new Zoo(animals: count, ceils: 0, foodPackage: count, animalsTypes: count, foodTypes: count, logFile: "test.txt");
            ZooInstance_2.TestFood_1_Create(count);
            ZooInstance_2.FindFoodSolution_Permutation();
            ZooInstance_2.ShutDownWork();
            Console.WriteLine("\r\nTest Done #############################\r\n\r\n");
        }




        static void RunCeilZoo()
        {
            Zoo ZooInstance = new Zoo(animals: 10, ceils: 15, foodPackage: 0, animalsTypes: 3, foodTypes: 0, logFile: "test.txt");
            ZooInstance.DisplayZooDebugInfo();
            ZooInstance.FindCeilSolution();
            ZooInstance.ShutDownWork();
        }


        static void RunFoodZoo()
        {
            Zoo ZooInstance = new Zoo(animals: 10, ceils: 15, foodPackage: 40, animalsTypes: 3, foodTypes: 4, logFile: "test.txt");
            ZooInstance.DisplayZooDebugInfo();
            // ZooInstance.FindFoodSolution();
            ZooInstance.FindFoodSolution_Permutation();
            ZooInstance.ShutDownWork();
        }


        static void RunRandomZoo()
        {
            int animalLimit = 11;
            // ====== generate random zoo 
            Random rnd = new Random();
            int animals = rnd.Next(1, animalLimit);
            int ceils = animals + rnd.Next(1, 6); // more than animal
            int animalsTypes = animals - rnd.Next(1, animals - 1); // 1 <  animalsTypes <  animals

            int foodPackage = rnd.Next(1, 10);
            int foodTypes = rnd.Next(1, 10);

            Zoo ZooInstance = new Zoo(animals: animals, ceils: ceils, foodPackage: foodPackage, animalsTypes: animalsTypes, foodTypes: foodTypes, logFile: "test.txt");

            ZooInstance.DisplayZooDebugInfo();

            ZooInstance.FindCeilSolution();
            ZooInstance.ShutDownWork();
        }



    }
}
