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
            string inputArgument = ""; // help

            if (args.Count() > 0)
            {
                inputArgument = args[0];
            }

            //======= run 
            //inputArgument = "food";
            //inputArgument = "ceil";
            //inputArgument = "random";

            //======= tests
            inputArgument = "foodTests";
            //inputArgument = "ceilTests";

            RunTask(inputArgument);

            Console.WriteLine("\r\nPress Enter to close window");
            Console.ReadLine();     // wait to see output
        }


        /// <summary>
        /// run task by selection
        /// </summary>
        /// <param name="task"></param>
        static void RunTask(string task )
        {
            switch (task)
            {
                // main run modes
                case "food":
                    RunFoodZoo(); // used for testing some parts of code
                    break;
                case "ceil":
                    RunCeilZoo(); // used for testing some parts of code
                    break;
                case "random":
                    RunRandomZoo(); // used for testing some parts of code
                    break;


                // tests
                case "foodTests":
                    RunFoodTests(); // used for testing some parts of code
                    break;
                case "ceilTests":
                    RunCeilTests(); // used for testing some parts of code
                    break;

                case "help":
                default:
                    DisplayHelp();
                    break;

            }
            return;
        }


        /// <summary>
        /// display help for usage
        /// </summary>
        static void DisplayHelp(){
            string help = @"Basic usage:
        Zoopark             - display help;
        Zoopark help        - display help;
        Zoopark food        - find solution for food task and display it;
        Zoopark ceil        - find solution for food task and display it;
        Zoopark random      - create random zoo and find solution for food and ceil task, and display it;

        Tests:
        Zoopark foodTests   - run predefined tests for food task;
        Zoopark ceilTests   - run predefined tests for ceil task;
";
            Console.WriteLine(help);
            return;
        }


        /// <summary>
        /// tests for ceil task
        /// </summary>
        static void RunCeilTests()
        {
            int count = 0;

            TestCreateEmptyZoo();

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


        /// <summary>
        /// test number with base
        /// </summary>
        public static void TestNumberWithBase()
        {
            Console.WriteLine("\r\nTEST inc numbers ====================");
            Zoo ZooInstance = new Zoo(animals: 0, ceils: 0, foodPackage: 0, animalsTypes: 0, foodTypes: 0, logFile: "test.txt");
            ZooInstance.TestNumberWithBase(baseOfNumber: 2, numberOfDigits: 4);
            ZooInstance.ShutDownWork();
        }


        /// <summary>
        /// create empty zoo for test
        /// </summary>
        public static void TestCreateEmptyZoo()
        {
            Console.WriteLine("\r\nTEST create empty Zoo ##############");
            Zoo ZooInstance = new Zoo(animals: 0, ceils: 0, foodPackage: 0, animalsTypes: 0, foodTypes: 0, logFile: "test.txt");
            ZooInstance.ShutDownWork();
            Console.WriteLine("\r\nTest Done #############################\r\n\r\n");
        }




        /// <summary>
        /// tests for food task
        /// </summary>
        static void RunFoodTests()
        {
            TestCreateEmptyZoo();

            int count = 0;

            Zoo ZooInstance = null;

            count = 0;
            Console.WriteLine("\r\nTEST Food 1 ##############");
            ZooInstance = new Zoo(animals: count, ceils: 0, foodPackage: count, animalsTypes: count, foodTypes: count, logFile: "test.txt");
            ZooInstance.TestFood_1_Create(count);
            ZooInstance.FindFoodSolution_Permutation();
            ZooInstance.ShutDownWork();
            Console.WriteLine("\r\nTest Done #############################\r\n\r\n");
        }



        /// <summary>
        /// solution for ceil task
        /// </summary>
        static void RunCeilZoo()
        {
            Zoo ZooInstance = new Zoo(animals: 10, ceils: 15, foodPackage: 0, animalsTypes: 3, foodTypes: 0, logFile: "test.txt");
            ZooInstance.DisplayZooDebugInfo();
            ZooInstance.FindCeilSolution();
            ZooInstance.ShutDownWork();
        }


        /// <summary>
        /// solution for food task
        /// </summary>
        static void RunFoodZoo()
        {
            Zoo ZooInstance = new Zoo(animals: 10, ceils: 15, foodPackage: 40, animalsTypes: 3, foodTypes: 4, logFile: "test.txt");
            ZooInstance.DisplayZooDebugInfo();
            // ZooInstance.FindFoodSolution();
            ZooInstance.FindFoodSolution_Permutation();
            ZooInstance.ShutDownWork();
        }


        /// <summary>
        /// create random zoo and find solution for ceil task and for food task
        /// </summary>
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
