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
            inputArgument = "foodtest";
            //inputArgument = "ceiltest";

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
                case "foodtest":
                    RunFoodTests(); // used for testing some parts of code
                    break;
                case "ceiltest":
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
            string help = @"Usage:
help:
    Zoopark             - display help;
    Zoopark help        - display help;

run:
    Zoopark food        - find solution for food task and display it;
    Zoopark ceil        - find solution for food task and display it;
    Zoopark random      - create random zoo and find solution for food and ceil task, and display it;

tests:
    Zoopark foodtest   - run predefined tests for food task;
    Zoopark ceiltest   - run predefined tests for ceil task;
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
            Zoo ZooInstance = null;

            int count = 0;

            /*
              
            //=== 1 animal, eat 1 type of food, at storage is 1 item of food - result : false
            count = 1;            
            Console.WriteLine("\r\nTEST Food 1 ##############");
            ZooInstance = new Zoo(animals: count, ceils: 0, foodPackage: count, animalsTypes: count, foodTypes: count, logFile: "test.txt");

            ZooInstance.TestSetAnimalsType(new int[] { 1 });  // tune test case
            ZooInstance.TestSetAnimalsFoodType(1, 1, 1, 0);
            ZooInstance.TestSetFoodStorage( new int[] { 1 } );

            ZooInstance.FindFoodSolution_Permutation();
            ZooInstance.ShutDownWork();
            Console.WriteLine("\r\nTest Done #############################\r\n\r\n");


            //=== 1 animal, eat 1 type of food, at storage is 2 item of food - result : true
            count = 1;
            Console.WriteLine("\r\nTEST Food 1 ##############");
            ZooInstance = new Zoo(animals: count, ceils: 0, foodPackage: 2, animalsTypes: count, foodTypes: count, logFile: "test.txt");

            ZooInstance.TestSetAnimalsType(new int[] { 1 });  // tune test case
            ZooInstance.TestSetAnimalsFoodType(1, 1, 1, 0);
            ZooInstance.TestSetFoodStorage(new int[] { 2 });

            ZooInstance.FindFoodSolution_Permutation();
            ZooInstance.ShutDownWork();
            Console.WriteLine("\r\nTest Done #############################\r\n\r\n");


            //=== 1 animal, eat 1 type of food, at storage is 1 item of his food, and 2 item of OTHER food - result : false
            Console.WriteLine("\r\nTEST Food 1 ##############");
            ZooInstance = new Zoo(animals: 1, ceils: 0, foodPackage: 1 + 2, animalsTypes: 2, foodTypes: 2, logFile: "test.txt");

            ZooInstance.TestSetAnimalsType(new int[] { 1 });  // tune test case
            ZooInstance.TestSetAnimalsFoodType(1, 1, 1, 0);
            ZooInstance.TestSetFoodStorage(new int[] { 1, 2 });

            ZooInstance.FindFoodSolution_Permutation();
            ZooInstance.ShutDownWork();
            Console.WriteLine("\r\nTest Done #############################\r\n\r\n");

            

            //=== 1 animal, eat 2 type of food, at storage is 1 item of his food - result : false
            Console.WriteLine("\r\nTEST Food 1 ##############");
            ZooInstance = new Zoo(animals: 1, ceils: 0, foodPackage: 1, animalsTypes: 1, foodTypes: 1, logFile: "test.txt");

            ZooInstance.TestSetAnimalsType(new int[] { 1 });  // tune test case
            ZooInstance.TestSetAnimalsFoodType(1, 1, 1, 1);
            ZooInstance.TestSetFoodStorage(new int[] { 1 });

            ZooInstance.FindFoodSolution_Permutation();
            ZooInstance.ShutDownWork();
            Console.WriteLine("\r\nTest Done #############################\r\n\r\n");



            //=== 1 animal, eat 2 type of food, at storage is 2 item of his food - result : true
            Console.WriteLine("\r\nTEST Food 1 ##############");
            ZooInstance = new Zoo(animals: 1, ceils: 0, foodPackage: 2, animalsTypes: 1, foodTypes: 1, logFile: "test.txt");

            ZooInstance.TestSetAnimalsType(new int[] { 1 });  // tune test case
            ZooInstance.TestSetAnimalsFoodType(1, 1, 1, 1);
            ZooInstance.TestSetFoodStorage(new int[] { 2 });

            ZooInstance.FindFoodSolution_Permutation();
            ZooInstance.ShutDownWork();
            Console.WriteLine("\r\nTest Done #############################\r\n\r\n");
             */


            //=== 1 animal, eat 2 type of food, at storage is 2 item of his food - result : true
            TestCreateFoodTest(animalsTypes: 1, 
                Animals: new int[] { 1 },                 
                AnimalsRulesList: new ZooAnimalsRules[] {
                    new ZooAnimalsRules( type: 1, canEatFood_1: 1, canEatFood_2: 2)
                }, 
                foodTypesAmounts: new int[] { 2, 0 });
        }


        /// <summary>
        /// create test for food task
        /// </summary>
        /// <param name="animals"></param>
        /// <param name="animalsTypes"></param>
        /// <param name="ceils"></param>
        /// <param name="foodPackage"></param>
        /// <param name="foodTypes"></param>
        /// <param name="Animals">array with type of animal for each animal</param>
        /// <param name="AnimalsRulesList"></param>
        /// <param name="foodTypesAmounts"></param>
        /// <param name="logFile"></param>
        public static void TestCreateFoodTest(int animalsTypes, int[] Animals, ZooAnimalsRules[] AnimalsRulesList, int[] foodTypesAmounts, string logFile = "test.txt" 
        )
        {
            Zoo ZooInstance = null;

            int animals = Animals.Count();
            int ceils = 0;
            int foodTypes = foodTypesAmounts.Count();

            int foodPackage = 0;
            foreach (var item in foodTypesAmounts)
            {
                foodPackage += item;
            }

            //=== 1 animal, eat 2 type of food, at storage is 2 item of his food - result : true
            Console.WriteLine("\r\nTEST Food ##############");
            ZooInstance = new Zoo(animals, animalsTypes, ceils, foodPackage, foodTypes, logFile);

            ZooInstance.TestSetAnimals(Animals);  // tune test case
            ZooInstance.TestSetAnimalsFoodType(AnimalsRulesList);
            ZooInstance.TestSetFoodStorage(foodTypesAmounts );

            ZooInstance.FindFoodSolution_Permutation();
            ZooInstance.ShutDownWork();
            Console.WriteLine("\r\nTest Done #############################\r\n\r\n");

            return;
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
