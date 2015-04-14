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
            // RunZoo();
            RunRandomZoo();

            // RunTests(); // used for testing some parts of code

            Console.ReadLine();     // wait to see output
        }


        static void RunTests()
        {
            int count = 0;
            Console.WriteLine("\r\nTEST create empty Zoo ====================");
            Zoo ZooInstance = new Zoo(animals: 0, ceils: 0, foodPackage: 0, animalsTypes: 0, foodTypes: 0, logFile: "test.txt");
            ZooInstance.ShutDownWork();

            //Console.WriteLine("\r\nTEST inc numbers ====================");
            //ZooInstance.TestNumberWithBase(baseOfNumber: 2, numberOfDigits: 4);
            //ZooInstance.ShutDownWork();

            Console.WriteLine("\r\nTEST create Zoo 3 animals 3 types 3 ceils 0 neighbour rule ====================");
            count = 3;
            Zoo ZooInstance_2 = new Zoo(animals: count, ceils: count, foodPackage: 0, animalsTypes: count, foodTypes: 0, logFile: "test.txt");
            ZooInstance_2.TestNxNxNCreate(count);
            ZooInstance_2.Run();
            ZooInstance_2.ShutDownWork();

            Console.WriteLine("\r\nTEST create Zoo 4 animals 4 types 4 ceils 0 neighbour rule ====================");
            count = 4;
            Zoo ZooInstance_3 = new Zoo(animals: count, ceils: count, foodPackage: 0, animalsTypes: count, foodTypes: 0, logFile: "test.txt");
            ZooInstance_3.TestNxNxNCreate(count);
            ZooInstance_3.Run();
            ZooInstance_3.ShutDownWork();
        }





        static void RunZoo()
        {
            Zoo ZooInstance = new Zoo(animals: 10, ceils: 15, foodPackage: 40, animalsTypes: 3, foodTypes: 4, logFile: "test.txt");
            ZooInstance.DisplayZooDebugInfo();
            ZooInstance.Run();
            ZooInstance.ShutDownWork();
        }


        static void RunRandomZoo()
        {
            int animalLimit = 11;
            // ====== generate random zoo 
            Random rnd = new Random();
            int animals = rnd.Next(1, animalLimit);
            int ceils = animals + rnd.Next(1, 6); // more than animal
            int animalsTypes = animals - rnd.Next(1, animals); // 1 <  animalsTypes <  animals

            int foodPackage = rnd.Next(1, 10);
            int foodTypes = rnd.Next(1, 10);

            Zoo ZooInstance = new Zoo(animals: animals, ceils: ceils, foodPackage: foodPackage, animalsTypes: animalsTypes, foodTypes: foodTypes, logFile: "test.txt");

            ZooInstance.DisplayZooDebugInfo();

            ZooInstance.Run();
            ZooInstance.ShutDownWork();
        }



    }
}
