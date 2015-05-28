using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DbBest.ZooPark
{
    class Program
    {
        protected static bool debug = true;

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


            ZooAnimalsRules[] Animals_FoodTypes;

            Animals_FoodTypes = new ZooAnimalsRules[] {
                    new ZooAnimalsRules( type: 1, canEatFood_1: 1, canEatFood_2: 0)
            };

            // 1 animal, eat 1 type of food, at storage is 1 item of food; different combinations at storage 
            TestCreateFoodTest(expectedResult: false, Animals: new int[] { 1 },
                AnimalsRulesList: Animals_FoodTypes,
                foodTypesAmounts: new int[] { 1 } );

            TestCreateFoodTest(expectedResult: true, Animals: new int[] { 1 },
                AnimalsRulesList: Animals_FoodTypes,
                foodTypesAmounts: new int[] { 2 });

            TestCreateFoodTest(expectedResult: false, Animals: new int[] { 1 },
                AnimalsRulesList: Animals_FoodTypes,
                foodTypesAmounts: new int[] { 1, 2 });





            Animals_FoodTypes = new ZooAnimalsRules[] {
                    new ZooAnimalsRules( type: 1, canEatFood_1: 0, canEatFood_2: 1)
            };


            // 1 animal, eat 1 type of food, at storage is 1 item of food; different combinations at storage 
            TestCreateFoodTest(expectedResult: false, Animals: new int[] { 1 },
                AnimalsRulesList: Animals_FoodTypes,
                foodTypesAmounts: new int[] { 1 });

            TestCreateFoodTest(expectedResult: false, Animals: new int[] { 1 },
                AnimalsRulesList: Animals_FoodTypes,
                foodTypesAmounts: new int[] { 1, 2 });

            TestCreateFoodTest(expectedResult: true, Animals: new int[] { 1 },
                AnimalsRulesList: Animals_FoodTypes,
                foodTypesAmounts: new int[] { 2, 1 });

            TestCreateFoodTest(expectedResult: false, Animals: new int[] { 1 },
                AnimalsRulesList: Animals_FoodTypes,
                foodTypesAmounts: new int[] { 0, 2 });




            Animals_FoodTypes = new ZooAnimalsRules[] {
                    new ZooAnimalsRules( type: 1, canEatFood_1: 0, canEatFood_2: 2)
            };


            // 1 animal, eat 1 type of food, at storage is 1 item of food; different combinations at storage 
            TestCreateFoodTest(expectedResult: false, Animals: new int[] { 1 },
                AnimalsRulesList: Animals_FoodTypes,
                foodTypesAmounts: new int[] { 1, 0 });

            TestCreateFoodTest(expectedResult: true, Animals: new int[] { 1 },
                AnimalsRulesList: Animals_FoodTypes,
                foodTypesAmounts: new int[] { 1, 2 });

            TestCreateFoodTest(expectedResult: false, Animals: new int[] { 1 },
                AnimalsRulesList: Animals_FoodTypes,
                foodTypesAmounts: new int[] { 2, 1 });

            TestCreateFoodTest(expectedResult: true, Animals: new int[] { 1 },
                AnimalsRulesList: Animals_FoodTypes,
                foodTypesAmounts: new int[] { 0, 2 });





            Animals_FoodTypes = new ZooAnimalsRules[] {
                    new ZooAnimalsRules( type: 1, canEatFood_1: 1, canEatFood_2: 2)
            };

            // 1 animal, eat 2 type of food; different combinations at storage 
            TestCreateFoodTest(expectedResult: false, Animals: new int[] { 1 },
                AnimalsRulesList: Animals_FoodTypes,
                foodTypesAmounts: new int[] { 1, 0 } );

            TestCreateFoodTest(expectedResult: false, Animals: new int[] { 1 },
                AnimalsRulesList: Animals_FoodTypes,
                foodTypesAmounts: new int[] { 0, 1 });

            TestCreateFoodTest(expectedResult: true, Animals: new int[] { 1 },
                AnimalsRulesList: Animals_FoodTypes,
                foodTypesAmounts: new int[] { 2, 0 });

            TestCreateFoodTest(expectedResult: true, Animals: new int[] { 1 },
                AnimalsRulesList: Animals_FoodTypes,
                foodTypesAmounts: new int[] { 1, 1 });

            TestCreateFoodTest(expectedResult: true, Animals: new int[] { 1 },
                AnimalsRulesList: Animals_FoodTypes,
                foodTypesAmounts: new int[] { 0, 2 });

            TestCreateFoodTest(expectedResult: true, Animals: new int[] { 1 },
                AnimalsRulesList: Animals_FoodTypes,
                foodTypesAmounts: new int[] { 1, 2 });




            // 2 animals
            TestCreateFoodTest(expectedResult: false, Animals: new int[] { 1, 1 },
                AnimalsRulesList: Animals_FoodTypes,
                foodTypesAmounts: new int[] { 0, 0 });

            TestCreateFoodTest(expectedResult: false, Animals: new int[] { 1, 1 },
                AnimalsRulesList: Animals_FoodTypes,
                foodTypesAmounts: new int[] { 0, 1 });

            TestCreateFoodTest(expectedResult: false, Animals: new int[] { 1, 1 },
                AnimalsRulesList: Animals_FoodTypes,
                foodTypesAmounts: new int[] { 0, 2 });

            TestCreateFoodTest(expectedResult: false, Animals: new int[] { 1, 1 },
                AnimalsRulesList: Animals_FoodTypes,
                foodTypesAmounts: new int[] { 1, 2 });

            TestCreateFoodTest(expectedResult: true, Animals: new int[] { 1, 1 },
                AnimalsRulesList: Animals_FoodTypes,
                foodTypesAmounts: new int[] { 1, 3 });

            TestCreateFoodTest(expectedResult: true, Animals: new int[] { 1, 1 },
                AnimalsRulesList: Animals_FoodTypes,
                foodTypesAmounts: new int[] { 2, 2 });

            TestCreateFoodTest(expectedResult: true, Animals: new int[] { 1, 1 },
                AnimalsRulesList: Animals_FoodTypes,
                foodTypesAmounts: new int[] { 2, 2 });

            TestCreateFoodTest(expectedResult: false, Animals: new int[] { 1, 1 },
                AnimalsRulesList: Animals_FoodTypes,
                foodTypesAmounts: new int[] { 0, 3 });

            TestCreateFoodTest(expectedResult: true, Animals: new int[] { 1, 1 },
                AnimalsRulesList: Animals_FoodTypes,
                foodTypesAmounts: new int[] { 0, 4 });


            // 2 animals, each eat 2 type of foods, 3 types of food, different combinations at storage   
            Animals_FoodTypes = new ZooAnimalsRules[] {
                    new ZooAnimalsRules( type: 1, canEatFood_1: 1, canEatFood_2: 2),
                    new ZooAnimalsRules( type: 2, canEatFood_1: 1, canEatFood_2: 3)
            };

            TestCreateFoodTest(expectedResult: true, Animals: new int[] { 1, 2 },
                AnimalsRulesList: Animals_FoodTypes,
                foodTypesAmounts: new int[] { 2, 0, 2 });

            TestCreateFoodTest(expectedResult: true, Animals: new int[] { 1, 2 },
                AnimalsRulesList: Animals_FoodTypes,
                foodTypesAmounts: new int[] { 1, 1, 2 });

            TestCreateFoodTest(expectedResult: true, Animals: new int[] { 1, 2 },
                AnimalsRulesList: Animals_FoodTypes,
                foodTypesAmounts: new int[] { 0, 2, 2 });

            TestCreateFoodTest(expectedResult: false, Animals: new int[] { 1, 2 },
                AnimalsRulesList: Animals_FoodTypes,
                foodTypesAmounts: new int[] { 1, 3, 0 });

            TestCreateFoodTest(expectedResult: true, Animals: new int[] { 1, 2 },
                AnimalsRulesList: Animals_FoodTypes,
                foodTypesAmounts: new int[] { 1, 2, 1 });

            TestCreateFoodTest(expectedResult: true, Animals: new int[] { 1, 2 },
                AnimalsRulesList: Animals_FoodTypes,
                foodTypesAmounts: new int[] { 2, 2, 0 });

            TestCreateFoodTest(expectedResult: true, Animals: new int[] { 1, 2 },
                AnimalsRulesList: Animals_FoodTypes,
                foodTypesAmounts: new int[] { 2, 1, 1 });

            TestCreateFoodTest(expectedResult: false, Animals: new int[] { 1, 2 },
                AnimalsRulesList: Animals_FoodTypes,
                foodTypesAmounts: new int[] { 2, 1, 0, 1 });



            // 3 animals
            Animals_FoodTypes = new ZooAnimalsRules[] {
                    new ZooAnimalsRules( type: 1, canEatFood_1: 1, canEatFood_2: 2),
                    new ZooAnimalsRules( type: 2, canEatFood_1: 1, canEatFood_2: 3),
                    new ZooAnimalsRules( type: 3, canEatFood_1: 2, canEatFood_2: 3)
            };

            TestCreateFoodTest(expectedResult: false, Animals: new int[] { 1, 2, 3 },
                AnimalsRulesList: Animals_FoodTypes,
                foodTypesAmounts: new int[] { 6, 0, 0 });

            TestCreateFoodTest(expectedResult: true, Animals: new int[] { 1, 2, 3 },
                AnimalsRulesList: Animals_FoodTypes,
                foodTypesAmounts: new int[] { 4, 2, 0 });

            TestCreateFoodTest(expectedResult: false, Animals: new int[] { 1, 2, 3 },
                AnimalsRulesList: Animals_FoodTypes,
                foodTypesAmounts: new int[] { 2, 2, 1, 1 });

            TestCreateFoodTest(expectedResult: true, Animals: new int[] { 1, 2, 3 },
                AnimalsRulesList: Animals_FoodTypes,
                foodTypesAmounts: new int[] { 2, 2, 2 });


            // more animals, 3 types
            Animals_FoodTypes = new ZooAnimalsRules[] {
                    new ZooAnimalsRules( type: 1, canEatFood_1: 1, canEatFood_2: 2),
                    new ZooAnimalsRules( type: 2, canEatFood_1: 1, canEatFood_2: 3),
                    new ZooAnimalsRules( type: 3, canEatFood_1: 2, canEatFood_2: 3)
            };

            TestCreateFoodTest(expectedResult: true, Animals: new int[] { 1, 1, 1, 2, 3 },
                AnimalsRulesList: Animals_FoodTypes,
                foodTypesAmounts: new int[] { 6, 2, 2 });

            TestCreateFoodTest(expectedResult: false, Animals: new int[] { 1, 1, 1, 2, 3 },
                AnimalsRulesList: Animals_FoodTypes,
                foodTypesAmounts: new int[] { 6, 2, 1, 1 });

            TestCreateFoodTest(expectedResult: true, Animals: new int[] { 1, 1, 1, 2, 3 },
                AnimalsRulesList: Animals_FoodTypes,
                foodTypesAmounts: new int[] { 3, 6, 1 });

        }


        /// <summary>
        /// create test for food task
        /// </summary>
        /// <param name="animals">array of animals: array item: animal type</param>
        /// <param name="animalsTypes"></param>
        /// <param name="ceils"></param>
        /// <param name="foodPackage"></param>
        /// <param name="foodTypes"></param>
        /// <param name="Animals">array with type of animal for each animal</param>
        /// <param name="AnimalsRulesList"></param>
        /// <param name="foodTypesAmounts"></param>
        /// <param name="logFile"></param>
        public static void TestCreateFoodTest(bool expectedResult, int[] Animals, ZooAnimalsRules[] AnimalsRulesList, int[] foodTypesAmounts, string logFile = "test.txt" 
        )
        {
            Zoo ZooInstance = null;

            int animals = Animals.Count();
            int animalsTypes = AnimalsRulesList.Count();
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

            bool taskResult = ZooInstance.FindFoodSolution_Permutation();
            ZooInstance.ShutDownWork();

            bool testSuccess = (taskResult == expectedResult);

            if ( debug && !testSuccess)
            {
                throw new Exception("Test Failed");
            }

            string message = testSuccess  ? "SUCCESS" : "FAILED";
            Console.WriteLine("\r\nTest " + message + " #############################\r\n\r\n");

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
