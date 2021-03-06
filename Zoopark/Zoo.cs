﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DbBest.ZooPark
{
    public class Zoo
    {
        // model config important parameters:
        public int LivingRulePossibility = 6;           // how often generate living rule: 2 -> 1/2; 3 -> 2/3; ... 4 -> 3/4; ..

        // common Model Part
        public int AnimalTypesAmount { get; set; }     // amount of types of animal
        public int AnimalsAmount { get; set; }         // amount of animals

        protected List<Animal> Animals;                    // list with animals that live at Zoo: index - id of animal;
        protected Dictionary<int, ZooAnimalsRules> AnimalsRules;      // dict of rules for each type of Animal: <type of animal, animalRule>


        // ceil Model
        protected int[,] AnimalsLivingWithRules;            // rules for animals - with what animal type they can live; 0 - can live together; 1 - cant live together
        protected static int CantLiveWithFlag = 1;          // flag when cant live with

        protected int CeilsAmount { get; set; }         // total amount of ceils

        protected long FailCount = 0;
        protected long SuccessCount = 0;
        protected long AttemptCount = 0;
        protected long DisplaySteps = 1000 * 10;



        // food Model
        protected int FoodPackagesAmount { get; set; }
        protected int FoodTypesAmount { get; set; }       // amount of types of food

        protected Dictionary<int, int> FoodStorage;       // list with amount of food packages of each type that are at Zoo: <type of food : amount of food>
        protected int FoodAmountForOneAnimal = 2;         // amount of food needed for each animal

        Dictionary<int, int> FoodWorkStorage;      // work copy of foodstorage
        Dictionary<int, int> FoodRequirements;      // work copy of foodstorage
        Dictionary<int, int> AnimalsOfTypes;   // amount of animals of each type
        List<FoodBucket> FoodBuckets;



        // results - not used now
        protected StringBuilder FoodResults_1;             // results for food - part 1 - for animals that eat only 1 type of food;
        // public List<List<uint>> CeilsResults;           // results for ceils: array of array of <id of ceil => animal id>
        // public List<List<Animal>> FoodResults;          // results for foods: array of array of <id of animal : animal with food settings>


        #region logging

        // logging features
        protected string _logFilename;
        public string LogFileName
        {
            get
            {
                return _logFilename;
            }
            set
            {
                CreateLogFile(value);
            }
        }                      // path to log where store log data

        protected System.IO.StreamWriter LogFile;          // log file handler

        #endregion


        // tools
        protected Random _rnd = null;




        #region init
        // init data methods

        public Zoo(int animals = 10, int animalsTypes = 3, int ceils = 15, int foodPackage = 40, int foodTypes = 3, string logFile = "", bool keepOldLog = true )
        {
            CreateLogFile(logFile, keepOldLog);

            _rnd = new Random();

            Animals = new List<Animal>();
            FoodStorage = new Dictionary<int, int>();
            AnimalsRules = new Dictionary<int, ZooAnimalsRules>();

            FoodResults_1 = new StringBuilder();
            // CeilsResults = new List<List<uint>>();
            // FoodResults = new List<List<Animal>>();

            GenerateZooModel(animals, animalsTypes, ceils, foodPackage, foodTypes);
        }


        /// <summary>
        /// shutdown work of class; now just close log
        /// </summary>
        public void ShutDownWork()
        {
            CloseLogFile();
        }



        public void GenerateZooModel(int animals = 10, int animalsTypes = 3, int ceils = 15, int foodPackages = 40, int foodTypes = 3)
        {
            // ceils model
            AnimalsAmount = animals;
            AnimalTypesAmount = animalsTypes;
            CeilsAmount = ceils;

            // food model
            FoodPackagesAmount = foodPackages;
            FoodTypesAmount = foodTypes;

            // clear lists
            Animals.Clear();
            FoodStorage.Clear();
            AnimalsRules.Clear();

            FoodResults_1.Clear();
            // CeilsResults.Clear();
            // FoodResults.Clear();

            // generate new lists
            InitAnimals(AnimalsAmount, AnimalTypesAmount);
            InitAnimalRules(AnimalTypesAmount, FoodTypesAmount);
            InitFoodStorage(FoodPackagesAmount, FoodTypesAmount);
        }


        public void CreateFreshFoodTaskModel(int animals = 10, int foodPackage = 40, int foodTypes = 3)
        {
            GenerateZooModel(animals, 0, 0, foodPackage, foodTypes);
        }
        public void CreatePlacementTaskModel(int animals = 10, int animalsTypes = 3, int ceils = 15)
        {
            GenerateZooModel(animals, animalsTypes, ceils, 0, 0);
        }

        /// <summary>
        /// init animals
        /// </summary>
        /// <param name="count"></param>
        protected void InitAnimals(int count, int typeAmount)
        {
            int type = 0;
            for (int i = 0; i < count; i++)
            {
                type = _rnd.Next(1, typeAmount + 1);
                Animals.Add(new Animal(type, AnimalTypesAmount));
            }
        }

        /// <summary>
        /// init food: 
        /// each item contains amount of food of this food type; 
        /// at each step we generate random amount of each type food; and find amount of food that left
        /// </summary>
        /// <param name="count"></param>
        protected void InitFoodStorage(int count, int typesAmount)
        {
            if (typesAmount == 0)   // fix for 0 food model
            {
                return;
            }

            int FoodTypeAmount = 0;
            int Left = count;

            // add food storage for food type 0 - this type set to animal when  animal dont eat second type of food
            FoodStorage[0] = 0;

            // !! attention: at cycle we dont step to last step; last setp we set after end of cycle becasue we cant set random value at last step
            for (int i = 1; i < typesAmount; i++)
            {
                FoodTypeAmount = _rnd.Next(0, Left);
                FoodStorage[i] = FoodTypeAmount;
                Left -= FoodTypeAmount;
            }
            FoodStorage[typesAmount] = Left;    // add last item

            // debug check integrity: must have at total count packages
            DebugInitFoodAssert(count, typesAmount);
        }

        /// <summary>
        /// debug check integrity: must have at total count packages
        /// </summary>
        public void DebugInitFoodAssert(int count, int typesAmount)
        {
            int assertTotal = 0;
            for (int i = 1; i <= typesAmount; i++)
            {
                assertTotal += FoodStorage[i];
            }
            if (assertTotal != count)
            {
                throw new Exception("logic error: total food is not equal ");
            }
        }


        /// <summary>
        /// generate rules for animal types:
        /// - with what animal type cant live
        /// - what types of food can eat - 1 type or 2 typess
        /// </summary>
        /// <param name="animalTypesAmount"></param>
        /// <param name="foodTypesAmount"></param>
        protected void InitAnimalRules(int animalTypesAmount, int foodTypesAmount)
        {
            InitAnimalLiveRules(animalTypesAmount);
            InitAnimalFoodRules(animalTypesAmount, foodTypesAmount);
        }

        /// <summary>
        /// generate rules for animal types:
        /// - with what animal type cant live
        /// </summary>
        /// <param name="animalTypesAmount"></param>
        protected void InitAnimalLiveRules(int animalTypesAmount)
        {
            AnimalsLivingWithRules = new int[animalTypesAmount + 1, animalTypesAmount + 1]; // we need add rules for types pairs like :  { Animal Type, No Animal }

            bool applyLiveRule = false;
            int cantLiveWithType = 0;

            // по умолчанию новый массив забивается нулями, так что эта инициализация не нужна
            // но оставим ее на всякий случай ))
            for (int i = 0; i <= animalTypesAmount; i++)
            {
                AnimalsLivingWithRules[i, 0] = 0;   // по умолчанию с пустой клеткой все могут жить.. 
                AnimalsLivingWithRules[0, i] = 0;   // и пустая клетка может жить со всеми
            }

            int rndNext = 0;

            // for all types
            for (int i = 1; i <= animalTypesAmount; i++)
            {
                cantLiveWithType = 0;

                // generate rule with what animla type cant live
                rndNext = _rnd.Next(1, LivingRulePossibility + 1);  // randomly find - apply rule for animal type or not                
                // LogMessage(rndNext.ToString(), "random: ");
                applyLiveRule = (rndNext > 1) ? true : false;

                // fill live together rule data
                if (applyLiveRule)
                {
                    cantLiveWithType = _rnd.Next(1, animalTypesAmount + 1);

                    // иногда животное не может жить со своим типом - например два быка ); поэтому фильтр ниже убираем
                    // cantLiveWithType = (cantLiveWithType != i) ? cantLiveWithType : 0;  // if selected type of animal with wich we cnat live is the same as animal itself type - reset rule

                    AnimalsLivingWithRules[i, cantLiveWithType] = Zoo.CantLiveWithFlag;
                    AnimalsLivingWithRules[cantLiveWithType, i] = Zoo.CantLiveWithFlag;    // если заяц не живет со львом, то лев не живет с зайцем тоже )
                }

            }
        }



        /// <summary>
        /// generate rules for animal types:
        /// - what types of food can eat - 1 type or 2 typess
        /// </summary>
        protected void InitAnimalFoodRules(int animalTypesAmount, int foodTypesAmount)
        {
            int randomGenerateChanceBorder = 2;   // how often generate rule

            bool applyFood_2Rule = false;
            int Food_1Eat = 0;
            int Food_2Eat = 0;

            for (int i = 1; i <= animalTypesAmount; i++)
            {
                // generate rule what food can eat - 1st type and 2nd type
                Food_1Eat = _rnd.Next(1, foodTypesAmount + 1);

                applyFood_2Rule = _rnd.Next(1, randomGenerateChanceBorder + 1) > 1 ? true : false;
                if (applyFood_2Rule)
                {
                    Food_2Eat = _rnd.Next(1, foodTypesAmount + 1);
                    Food_2Eat = (Food_2Eat != Food_1Eat) ? Food_2Eat : 0;  // if selected type of food 2 is the same as food 1  - just reset rule
                }
                else
                {
                    Food_2Eat = 0;
                }
                AnimalsRules[i] = new ZooAnimalsRules(i, Food_1Eat, Food_2Eat);
            }
        }




        #endregion


        #region logging implementation

        /// <summary>
        /// create log file
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        protected void CreateLogFile(string fileName, bool keepOldLog = true )
        {
            CloseLogFile();

            bool result = false;
            if (!String.IsNullOrWhiteSpace(fileName))
            {
                try
                {
                    LogFile = new System.IO.StreamWriter(fileName, keepOldLog);
                    LogFile.AutoFlush = true;
                    result = (LogFile != null);
                }
                catch (System.IO.IOException ex)
                {
                    result = false;
                    DisplayMessage("error: cant create log file: " + ex.Message);
                    LogFile = null;
                }
            }
            _logFilename = (result) ? fileName : null;
        }

        /// <summary>
        /// close log file
        /// </summary>
        protected void CloseLogFile()
        {
            if (LogFile != null)
            {
                LogFile.Close();
                LogFile = null;
                _logFilename = null;
            }
        }

        public void LogMessage(string message, string preffix = null)
        {
            if (LogFile != null)
            {
                LogFile.WriteLine(preffix + message);
            }
        }

        #endregion



        #region display debug

        public string GetAnimalsDisplay()
        {
            StringBuilder sb = new StringBuilder();
            int count = 0;

            sb.AppendFormat("Animals: {0}; Animal Types: {1}, Ceils: {2} \r\n", Animals.Count, AnimalTypesAmount, CeilsAmount);

            sb.Append("Animal List: {");
            foreach (Animal a in Animals)
            {
                count++;
                sb.Append( a.Type + ", " );
            }
            sb.Append("}\r\n");

            return sb.ToString();
        }



        public string GetFoodDisplay()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("Food: Types: {0}; Storage: {1} items; format: {{type: amount}},... \r\n", FoodTypesAmount, FoodPackagesAmount);
            sb.Append(GetFoodContainerDisplay(FoodStorage));
            return sb.ToString();
        }


        public string GetFoodWorkDisplay()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(GetFoodContainerDisplay(FoodWorkStorage));
            return sb.ToString();
        }


        protected string GetFoodContainerDisplay( Dictionary<int, int> container){
            StringBuilder sb = new StringBuilder();
            int count = 0;
            int PadLeft = 3;

            sb.Append("FoodStorage: ");
            foreach (var pair in container)
            {
                sb.AppendFormat("{{{0}: {1}}} ", pair.Key, pair.Value.ToString().PadLeft(PadLeft));
                count += pair.Value;
            }
            sb.AppendFormat("Total: {0}", count );

            return sb.ToString();
        }


        public string GetAnimasRulesDisplay()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("Food Rules: format: {type: food_1, food_2}, ...\r\n");
            foreach (var pair in AnimalsRules)
            {
                sb.AppendFormat("{0}, ", pair.Value.DisplayRule());
            }
            sb.AppendLine();

            return sb.ToString();
        }


        public string GetAnimasLivingRulesDisplay()
        {
            StringBuilder sb = new StringBuilder();

            sb.AppendFormat("Animals Living Rules: {0} rules; \r\n", AnimalsLivingWithRules.Length);

            int PadLeft = 3;

            // ====== head row 
            // first column
            sb.Append("    ");
            for (int i = 0; i <= AnimalTypesAmount; i++)
            {
                sb.AppendFormat("{0}", i.ToString().PadLeft(PadLeft));
            }
            sb.AppendFormat("\r\n");

            sb.Append("    ");
            for (int i = 0; i <= AnimalTypesAmount; i++)
            {
                sb.AppendFormat("{0}", "----".ToString().PadLeft(PadLeft));
            }
            sb.AppendFormat("\r\n");

            // inner part with first row
            for (int i = 0; i <= AnimalTypesAmount; i++)
            {
                // first column
                sb.AppendFormat("{0}|", i.ToString().PadLeft(PadLeft));

                for (int j = 0; j <= AnimalTypesAmount; j++)
                {
                    // data
                    sb.AppendFormat("{0}", AnimalsLivingWithRules[i, j].ToString().PadLeft(PadLeft));
                }
                sb.AppendFormat("\r\n");
            }
            return sb.ToString();
        }




        public string GetDisplayZooDebugInfo()
        {
            StringBuilder sb = new StringBuilder();

            sb.AppendLine("========================================");
            sb.AppendLine(GetAnimalsDisplay());
            sb.AppendLine(GetAnimasLivingRulesDisplay());

            sb.AppendLine(GetAnimasRulesDisplay());
            sb.AppendLine(GetFoodDisplay());
            sb.AppendLine("========================================");

            return sb.ToString();
        }


        public void DisplayZooDebugInfo()
        {
            DisplayMessage(GetDisplayZooDebugInfo());
        }

        #endregion



        #region ceil solution

        public bool findCeilSolution(int successLimit)
        {
            return false;
        }




        /// <summary>
        /// алгоритм:
        /// 
        /// варианты:
        /// 1) лучший: у нас есть животные N штук, M типов, X клеток
        /// 
        /// введение
        /// берем первое животное - N вариантов
        /// но в этих N вариантах есть M повторяющихся типов, перебор по которым создает дубликаты вариантов.
        /// поэтому для перебора нам надо выбрать только животных уникальных типов
        /// 
        /// Итого:
        /// - выбираем из N животных M животных уникальных типов
        /// - и каждого из этих животных садим в 1ю клетку
        /// - смотрим - есть у животного соседи слева? если есть - смотрим ограничение - может мы эти животные посадить вместе?
        /// - если можем - садим, и делаем перебор для оставшихся животных (рекурсия)
        /// - если не можем - обрасываем этот вариант
        /// - пустая клетка работает как дополнительный тип животного
        /// 
        /// оценка числа вариантов: перебор из N элементов из которых M уникальных - число вариантов порядка N! / (N-M)!
        /// и еще отсеиваются варианты для которых срабатывает ограничение на соседей        /// 
        /// 
        /// возможный вариант улучшения быстродействия - векторизация - выделить начальные цепочки которые можно считать в разных потоках
        /// один поток на ядро процессора
        /// 
        /// - глубина рекурсии равна числу клеток
        /// 
        /// 
        /// 
        /// 2) более худший алгоритм:
        /// - каждая клетка - разряд числа
        /// - тип животного - бит числа в этом разряде.. 3 типа животных - число по базе 4 ( пустая клетка + 4 значения одного разряда числа)
        /// перебор всех битов от минимального числа до максимального даст нам все варианты
        /// 
        /// для этого алгоритма число просматриваемых вариантов намного больше чем дял 1го, но не требуется рекурсия и намного легче делать алгоритм.
        /// алгоритм не реализовывал.. 
        /// 
        /// 
        /// </summary>
        /// <param name="successLimit">not used now, can use if want limit number of success variants</param>
        public void findCeilSolutionByUniquePermutation(int successLimit)
        {
            DisplayMessage("===============================================");
            DisplayMessage("Begin ceil placing search ... ");

            AttemptCount = 0;
            SuccessCount = 0;
            FailCount = 0;

            List<int> CurrentItems = new List<int>();       // partial result with already placed items/animals
            List<int> AnimalsThatLeft = GetAllAnimalsAndEmptyCeilsAsListInt();  // all not placed yet animals

            MakePermutation(CurrentStep: CeilsAmount, CurrentItems: ref CurrentItems, AnimalsThatLeft: ref AnimalsThatLeft);
            
            DisplayMessage("\r\nResult: Success: " + SuccessCount + ";     Failed: " + FailCount);
            DisplayMessage("Logfile: " + LogFileName);
            DisplayMessage("Done");
            DisplayMessage("================================================\r\n");
        }


        /// <summary>
        /// recurse procedure that make permutations;
        /// stack size for 64 bit systems is about 1Mb but this can be changed at app settings
        /// from stack size and size of stack vars depends amount of steps that are allowed
        /// http://stackoverflow.com/questions/823724/stack-capacity-in-c-sharp
        /// https://msdn.microsoft.com/en-us/library/windows/desktop/ms686774(v=vs.85).aspx
        /// </summary>
        /// <param name="CurrentStep"></param>
        /// <param name="CurrentItems"></param>
        /// <param name="AnimalsThatLeft"></param>
        public void MakePermutation(int CurrentStep, ref List<int> CurrentItems, ref List<int> AnimalsThatLeft)
        {
            AttemptCount++; // calculate number of calls of functoin

            if (AttemptCount % DisplaySteps == 0)  // sometimes display status
            {
                Console.Write("\rFunc calls: {0}: Finded: {1}; Failed: {2}", AttemptCount, SuccessCount, FailCount);
            }


            if (CurrentStep == 0)   // we added items to all positions - so have success result, return back from stack of calls
            {
                SuccessCount++;
                // DisplayMessage( Zoo.DisplayListInt( ref CurrentItems));  // display success result to console
                LogMessage(Zoo.DisplayListInt(ref CurrentItems));           // log success result
                return; // stop recursion
            }

            if (AnimalsThatLeft.Count == 0)     // if dont have more animals - return; must be only if some logical error trapped at code
            {
                DisplayMessage("Error: wrong place");
                // this is no more items - so all items placed // stop recursion
                return;
            }

            // get only unique types from current list of items // for small optimization moved out of method
            List<int> UniqueTypes = AnimalsThatLeft.Distinct().ToList(); // it must have at least one item - because source list already has at least one item

            int NewCurrentStep = CurrentStep - 1;   // next level will have one item less at list

            CurrentItems.Add(0);    // add new item as placeholder - will change it later with item values
            int CurrentNewItemIndex = CurrentItems.Count - 1;
            int ItemIndex = 0;

            foreach (var Item in UniqueTypes)
            {
                // ========= try apply different rules for current variant: apply placement rule for animal
                if (CurrentItems.Count >= 2) // if have previous item; previous item will be 2nd from end - because we placed placeholder for new item
                {
                    //if (!CheckLivingRuleForPair(CurrentItems[CurrentItems.Count - 2], Item))    // check rule for previous item and current item
                    if (AnimalsLivingWithRules[CurrentItems[CurrentItems.Count - 2], Item] == Zoo.CantLiveWithFlag)  // for optimization remove function call
                    {
                        FailCount++;

                        LogMessage(Zoo.DisplayListWithoutLastInt(ref CurrentItems), "\t\t\t\t\t\t\t\t\t\tFail: Tried Add as last item: " + Item + " at list => ");

                        continue; // rule does not passed - skip to next item
                    }
                }

                // clone new list 
                List<int> NewAnimalsThatLeft = new List<int>(AnimalsThatLeft); // will have new list of items - copy of AnimalsThatLeft without items of current type

                // create new list without current item
                ItemIndex = NewAnimalsThatLeft.FindLastIndex(x => x == Item);
                NewAnimalsThatLeft.RemoveAt(ItemIndex);

                CurrentItems[CurrentNewItemIndex] = Item;   // add current item to list of selected items

                MakePermutation(CurrentStep: NewCurrentStep, CurrentItems: ref CurrentItems, AnimalsThatLeft: ref NewAnimalsThatLeft);  // make permutations for items that left
            }

            CurrentItems.RemoveAt(CurrentNewItemIndex); // remove item that we added for this level of permutationss

            return;
        }


        /// <summary>
        /// checking rule for living for 2 items; 
        /// </summary>
        /// <param name="first"></param>
        /// <param name="second"></param>
        /// <returns></returns>
        public bool CheckLivingRuleForPair(int first, int second)
        {
            return (AnimalsLivingWithRules[first, second] != Zoo.CantLiveWithFlag);
        }




        /// <summary>
        /// return all animals list as List<int> from List<Animal>
        /// </summary>
        /// <returns></returns>
        public List<int> GetAllAnimalsAndEmptyCeilsAsListInt()
        {
            int EmptyAnimalType = 0;

            List<int> result = new List<int>();
            foreach (var item in Animals)
            {
                result.Add(item.Type);
            }

            // add emppty ceils as animal type 0
            for (int i = 1; i <= CeilsAmount - AnimalsAmount; i++)
            {
                result.Add(EmptyAnimalType);
            }
            return result;
        }

        #endregion



        #region test

        /// <summary>
        /// использовался для старта тестов
        /// </summary>
        public void Tests()
        {
            // TestNumberWithBase();
        }

        /// <summary>
        /// тест класса чисел по произвольной базе
        /// </summary>
        /// <param name="baseOfNumber"></param>
        /// <param name="numberOfDigits"></param>
        public void TestNumberWithBase(int baseOfNumber, int numberOfDigits)
        {
            string bits;
            NumberWithBase nb = new NumberWithBase(baseOfNumber, numberOfDigits);
            try
            {
                do
                {
                    bits = nb.GetBitsString();
                    DisplayMessage(bits);
                    nb.inc();
                } while (true);
            }
            catch (Exception ex)
            {
                Console.WriteLine("catched: " + ex.Message);
            }
        }


        /// <summary>
        /// создает набор животных - по одному животному разного типа
        /// </summary>
        /// <param name="count"></param>
        public void TestNxNxNCreate(int count)
        {
            Animals.Clear();
            int type = 0;
            for (int i = 0; i < count; i++)
            {
                type = i + 1;
                Animals.Add(new Animal(type, AnimalTypesAmount));
            }
            TestInitAnimalLiveRules(count);
        }

        
        /// <summary>
        /// создает тестовые правила расселения животных - все могут жить со всеми
        /// </summary>
        /// <param name="animalTypesAmount"></param>
        protected void TestInitAnimalLiveRules(int animalTypesAmount)
        {
            AnimalsLivingWithRules = new int[animalTypesAmount + 1, animalTypesAmount + 1]; // we need add rules for types pairs like :  { Animal Type, No Animal }

            for (int i = 0; i <= animalTypesAmount; i++)
            {
                AnimalsLivingWithRules[i, 0] = 0;   // по умолчанию с пустой клеткой все могут жить.. 
                AnimalsLivingWithRules[0, i] = 0;   // и пустая клетка может жить со всеми
            }
        }



        /// <summary>
        /// create basic structures for count animals
        /// </summary>
        /// <param name="count"></param>
        public void TestFoodCreateNAnimals(int count, int animalTypesAmount, int foodStorage)
        {
            AnimalTypesAmount = animalTypesAmount;

            Animals.Clear();
            for (int i = 0; i < count; i++)
            {
                Animals.Add(new Animal(type: 0, maxType: animalTypesAmount));
            }
        }


        /// <summary>
        /// set list of animals for test
        /// </summary>
        /// <param name="AnimalsList"></param>
        public void TestSetAnimals(int[] AnimalsList)
        {
            for (int i = 0; i < AnimalsList.Count(); i++)
            {
                Animals[i] = new Animal( AnimalsList[i] );
            }
            return;
        }




        /// <summary>
        /// set food types that can eat animals
        /// </summary>
        /// <param name="indexStart"></param>
        /// <param name="indexEnd"></param>
        /// <param name="foodType_1"></param>
        /// <param name="foodType_2"></param>
        public void TestSetAnimalsFoodType(ZooAnimalsRules[] AnimalsRulesList )
        {
            foreach (var item in AnimalsRulesList)
            {
                AnimalsRules[item.Type] = item;
            }
            return;
        }


        /// <summary>
        /// set food amounts of different type for food storage
        /// </summary>
        /// <param name="foodAmounts"></param>
        public void TestSetFoodStorage(int[] foodAmounts)
        {
            // set all food amount to 0; 
            foreach ( int key in FoodStorage.Keys.ToList())
            {
                FoodStorage[key] = 0;
            }

            int count = 0;
            for (int i = 1; i <= foodAmounts.Count(); i++)
            {
                count += foodAmounts[i - 1];
                FoodStorage[i] = foodAmounts[i - 1];
            }

            // debug check integrity: must have at total count packages
            int foodTypesAmount = FoodStorage.Count() - 1;
            DebugInitFoodAssert(count, foodTypesAmount );

            return;
        }




        public void TestFood_1_Create(int count)
        {
            Animals.Clear();
            int type = 0;
            for (int i = 0; i < count; i++)
            {
                type = i;
                Animals.Add(new Animal(type, AnimalTypesAmount));
            }
            TestInitAnimalLiveRules(count);
        }


        #endregion




        public void DisplayMessage(string message, bool dontLogMessageEvenWhenHaveLogging = false)
        {
            Console.WriteLine(message);
            if (!dontLogMessageEvenWhenHaveLogging)
            {
                LogMessage(message);
            }
        }

        public static string DisplayListInt(ref List<int> Items)
        {
            StringBuilder sb = new StringBuilder();
            foreach (var Item in Items)
            {
                sb.AppendFormat("{0} ", Item);
            }
            return sb.ToString();
        }

        public static string DisplayListWithoutLastInt(ref List<int> Items)
        {
            StringBuilder sb = new StringBuilder();
            int Length = Items.Count;
            for (int i = 0; i < Length - 1; i++)
            {
                sb.AppendFormat("{0} ", Items[i]);
            }
            return sb.ToString();
        }


        public int FindCeilSolution()
        {
            findCeilSolutionByUniquePermutation(100);
            return 0;
        }



        #region food solution 2


        // алгоритм:
        // 
        // 1) у нас есть склад еды
        // 2) со склада мы раздали еду животным которые едят еду только одного типа. Если еды не хватило- стопим
        // 3) в этой точке у нас есть склад с едой, и животные которые едят еду обязательно двух типов. 
        // наборы еды могут быть разные, примеры: 3 животных по { 1, 2} ,  4 животных по { 1, 3 }, 2 животных по { 1, 3} и тд.            

        // 4) вся еда группируется по уникальным корзинам (бакетам), каждый бакет имеет: 
        // - тип еды 1, 
        // - тип еды 2,
        // - требуемое число пар еды для этого бакета
        // - сколько еды помещено каждого типа

        // 5) каждый бакет может находиться в 2 * N состояних, где N - максимальное число еды в бакете
        // если у нас в зоопарке M типов  пар еды ( бакетов) то общее число вариантов порядка: (2 * N ) ^ M / const
        // если в зоопарке X животных, то N может варьироваться от 1 до X
        // т.е. максимальное число вариантов перебора меньше чем (2 * X) ^ M

        // 6) сам перебор:
        // берем бакет, заполняем его едой для вариантов от 0 до 2*N, забранную еду забираем со склада, 
        // и рекурсивно ищем варианты для оставшихся бакетов для оставшейся еды
        // ограничение при переборе - на складе должна быть еда
        // если при заполнении последнего бакета еда на складе не ушла в минус - этот вариант верен
        // после этого возвращаемся и смотрим другую комбинацию

        // 7) глубина рекурсии равна числу разных типов бакетов


        /// <summary>
        /// решение для задачи питания
        /// метод: выбрасываем тех которые едят 1 еду
        /// группируем наборы еды из 2х элементов
        /// после группировки делаем переборы по распределению еды  по наборам еды
        /// </summary>
        /// <returns></returns>
        public bool FindFoodSolution_Permutation()
        {
            bool result = false;

            FoodWorkStorage = new Dictionary<int, int>(FoodStorage);    // work copy of foodstorage
            FoodBuckets = new List<FoodBucket>();

            FoodCreateLists_Permutations( ref FoodWorkStorage, ref FoodBuckets);  // create required lists of data

            DisplayMessage("Food after feed 1 type animals");
            DisplayMessage(GetFoodWorkDisplay());

            // check that we have food for animals that eat only one type of food 
            if (!FoodCheckThatEatOnly_1_Food(FoodWorkStorage))
            {
                return false;
            }

            // add feed 1 animals result 
            FoodPrepareResult_1(FoodStorage, FoodWorkStorage);

            // minimal cheсks passed - try feed 2-mode animals

            AttemptCount = 0;
            DisplaySteps = 100;
            SuccessCount = 0;
            FailCount = 0;

            int CurrentStep = FoodBuckets.Count;
            List<FoodBucket> CurrentBucketList = new List<FoodBucket>();  // current is empty at start
            List<FoodBucket> LeftBucketList = new List<FoodBucket>(FoodBuckets);    // left buckets are full at start

            MakeFoodPermutation(CurrentStep, ref CurrentBucketList, ref LeftBucketList, ref FoodWorkStorage);

            result = (SuccessCount > 0);

            DisplayMessage("Result: Success: " + SuccessCount + ";     Failed: " + FailCount);
            DisplayMessage("logfile: " + LogFileName );
            DisplayMessage("\r\nSearch Food Solution Done\r\n============================================");


            return result;
        }



        /// <summary>
        /// prepare display food result for animals that eat only 1 food
        /// we have too samples of storage - one - source, second copy of it with deducted food
        /// just find differences between them, and save diff as used food
        /// </summary>
        public void FoodPrepareResult_1( Dictionary<int, int> FoodStorage, Dictionary<int, int> FoodWorkStorage)
        {
            int delta = 0;
            string descr;
            foreach( var item in FoodStorage ){
                delta = item.Value - FoodWorkStorage[item.Key];
                if ( delta > 0) {
                    descr = string.Format("{0} => {1}, ", item.Key, delta);
                    FoodResults_1.Append( descr );
                }
            }
            return;
        }


        public void FoodDisplayResult(List<FoodBucket> buckets)
        {
            string res = "Food Success: ";
            if (FoodResults_1.Length > 0)
            {
                res += "Animals that eat 1 food (food type => amount): " + FoodResults_1.ToString() + "\r\n";
            }

            if (buckets.Count() > 0)
            {
                res += "animals that eat 2 foods: buckets: (type => amount): ";
                foreach (var item in buckets)
                {
                    res += string.Format("({0} => {1}, {2} => {3}), ", item.TypeFood_1, item.AmountFood_1, item.TypeFood_2, item.AmountFood_2);
                }
            }

            res += "\r\n";

            DisplayMessage(res);
        }




        /// <summary>
        /// recurse procedure that make food permutations;
        /// stack size for 64 bit systems is about 1Mb but this can be changed at app settings
        /// from stack size and size of stack vars depends amount of steps that are allowed
        /// http://stackoverflow.com/questions/823724/stack-capacity-in-c-sharp
        /// https://msdn.microsoft.com/en-us/library/windows/desktop/ms686774(v=vs.85).aspx
        /// </summary>
        /// <param name="CurrentStep"></param>
        /// <param name="CurrentItems"></param>
        /// <param name="AnimalsThatLeft"></param>
        public void MakeFoodPermutation(int CurrentStep, ref List<FoodBucket> CurrentBucketList, ref List<FoodBucket> LeftBucketList, ref Dictionary<int, int> FoodWorkStorage)
        {
            AttemptCount++; // calculate number of calls of functoin

            if (AttemptCount % DisplaySteps == 0)  // sometimes display status
            {
                Console.Write("\rFunc calls: {0}: Finded: {1}; Failed: {2}", AttemptCount, SuccessCount, FailCount);
            }


            if (CurrentStep == 0)   // we filled all buckets - so have success result, return back from stack of calls
            {
                SuccessCount++;
                // DisplayMessage("Success");  // display success result to console
                FoodDisplayResult(CurrentBucketList);
                return; // stop recursion
            }

            if (LeftBucketList.Count == 0)     // if dont have more free bucket - return; must be only if some logical error trapped at code
            {
                DisplayMessage("Error: wrong place");
                // this is no more items - so all items placed // stop recursion
                return;
            }

            int Type_1 = 0;
            int Type_2 = 0;

            int StepsCount = LeftBucketList.Count;

            int MinMax = 0;
            int TotalAmountToPlace = 0;
            int Amount_1_ToPlace = 0;
            int Amount_2_ToPlace = 0;

            for (int i = 0; i < StepsCount ; i++)
            {
                if (LeftBucketList[i].IsTaked)
                {
                    continue;   // skip buckets that are taked at previous levels
                }

                FoodBucket Bucket = LeftBucketList[i];  // select bucket
                TotalAmountToPlace = Bucket.BucketsAmount * FoodAmountForOneAnimal;   // amount of food that we must place

                Type_1 = Bucket.TypeFood_1;
                Type_2 = Bucket.TypeFood_2;

                // check - do we have amount of food at storage? amout of food at storage must be >= than amount of food in bucket
                if ((FoodWorkStorage[Type_1] + FoodWorkStorage[Type_2]) < TotalAmountToPlace)
                {
                    // amount of food at storage is less than needed - fail for this permutation
                    FailCount++;
                    return;
                }

                // amount of food at storage is enough for this step - try variants
                Bucket.IsTaked = true;
                CurrentBucketList.Add(Bucket);

                MinMax = FoodWorkStorage[Type_1] < TotalAmountToPlace ? FoodWorkStorage[Type_1] : TotalAmountToPlace;  // we cant take more than exists at storage, so this is maximal amount of food type 1 that we can take; if have many at storage - maximum is what can fit at bucket

                // fill bucket by different amount of food
                for (Amount_1_ToPlace = 0; Amount_1_ToPlace <= MinMax; Amount_1_ToPlace++)
                {
                    Amount_2_ToPlace = TotalAmountToPlace - Amount_1_ToPlace;
                    if (Amount_2_ToPlace > FoodWorkStorage[Type_2])
                    {
                        FailCount++;
                        continue;  // dont have more food of type 2  at storage; try next combination of: food type 1 : food type 2
                    }

                    // change bucket state
                    Bucket.SetAmount_1(Amount_1_ToPlace);
                    Bucket.SetAmount_2(Amount_2_ToPlace);

                    // change food storage state
                    FoodWorkStorage[Type_1] -= Amount_1_ToPlace;
                    FoodWorkStorage[Type_2] -= Amount_2_ToPlace;

                    // DisplayMessage(GetFoodWorkDisplay());

                    MakeFoodPermutation(CurrentStep - 1, ref CurrentBucketList, ref LeftBucketList, ref FoodWorkStorage);

                    // restore food storage state
                    FoodWorkStorage[Type_1] += Amount_1_ToPlace;
                    FoodWorkStorage[Type_2] += Amount_2_ToPlace;
                }

                // restore previous state for buckets
                Bucket.IsTaked = false; // restore status of taked bucket
                CurrentBucketList.RemoveAt(CurrentBucketList.Count - 1);    // remove bucket from result combination that we already tried
            }

            return;
        }






        /// <summary>
        /// 1) вычитаем еду для животных 1 го типа еды
        /// 2) создаем группы бакетов разного типа { еда1, еда 2 }
        /// </summary>
        /// <param name="WorkFoodStorage"></param>
        /// <param name="foodBuckets"></param>
        protected void FoodCreateLists_Permutations(ref Dictionary<int, int> WorkFoodStorage, ref List<FoodBucket> foodBuckets)
        {
            int FoodType = 0;
            int index = 0;

            FoodBucket newBucket;

            foreach (var item in Animals)
            {
                
                if (AnimalsRules[item.Type].CanEatFood_1 == 0 && AnimalsRules[item.Type].CanEatFood_2 == 0)
                {
                    // skip this type of animals - they dont eat at all )

                }
                // at first check animals that eat only one type of food: deduct all food for animals that eat only one type of food
                else if (AnimalsRules[item.Type].CanEatFood_2 == 0)
                {
                    FoodType = AnimalsRules[item.Type].CanEatFood_1;
                    WorkFoodStorage[FoodType] -= FoodAmountForOneAnimal;
                }
                else if (AnimalsRules[item.Type].CanEatFood_1 == 0)
                {
                    FoodType = AnimalsRules[item.Type].CanEatFood_2;
                    WorkFoodStorage[FoodType] -= FoodAmountForOneAnimal;
                } else {
                    // animals that eat 2 types of food - add at food buckets; each bucket contais pairs: { type_1, type_2 } for each type of food

                    // try find bucket - if cant - create it; if can find - increase it amount
                    index = FindFoodBucketIndex(AnimalsRules[item.Type].CanEatFood_1, AnimalsRules[item.Type].CanEatFood_2);
                    if (index >= 0)
                    {
                        FoodBuckets[index].BucketsAmount += 1;                        
                    }
                    else
                    {   
                        // dont finded bucket - create it
                        newBucket = new FoodBucket(AnimalsRules[item.Type].CanEatFood_1, AnimalsRules[item.Type].CanEatFood_2);
                        newBucket.BucketsAmount = 1;
                        FoodBuckets.Add(newBucket);
                    }
                }
            }
            return;
        }



        #endregion









        #region food solution - dont finished version

        // ВНИМАНИЕ - вариант не закончен

        // алгоритм:
        // 
        // у нас есть набор бакетов еды: каждый бакет имеет: 
        // - тип еды 1, 
        // - тип еды 2,
        // - число таких бакетов
        // - сколько еды помещено каждого типа

        // используем идею системы сообщающихся сосудов - если в сообщающийся сосуд залить излишек - из второй части его лишнее выльется.. 
        // каждый бакет - связываем с набором других бакетов в которые мы можем залить еду - втолкнув еду в первый бакет

        // в конечном результате все бакеты должны быть заполнены доверху

        // мы стараемся накормить животных сбалансированно
        // 1) проходим по бакетам и заполняем до половины каждого корма своего типа, если корма не хватает - заливаем что есть
        // 2) после этого будут заполнены половины кормов которых с избытком, не будет заполнены половины которых не хватает, на складе останутся избыточные корма
        // наша цель - сбалансировать избыточные корма разного типа так чтобы их хватило на все бакеты с неполной второй половиной
        // 3) после этого смотрим бакеты где вторая половина не заполнена и начинаем вталкивать избыточные корма в них            
        // 4) может создаться взаимная блокировка: 
        // если какого то корма (1) не хватает, связанного корма (2) тоже нет, но есть излишек парного корма (3) для связанного (2) 
        // - то надо втолкнуть корм (3) чтобы освободить корм (2), и заполнить им пустоту для корма (1)

        // пример: система тип-тип (колво, колво):  1-2 (1,1)  : 2-3 (0,1) : 1-3 (0,2) ; и на складе 1 ед типа 1.
        // 2й бакет 2-3 пуст. но на складе есть излишек типа 1.
        // тогда корм типа 1 можно втолкнуть в бакет 1-3, освободить корм типа 2, и втолкнуть его в бакет 2-3.                        
        //
        // ситуацию с блокировкой решаем проходом по пустым бакетам и пытаемся найти излишек для связанного корма заменив его на излишек на складе


        /// <summary>
        /// один из вариантов решения распределения еды, до конца не добивал.. 
        /// может хорошо работать для большого числа животных и большого числа разных типво животных
        /// </summary>
        /// <returns></returns>
        public bool FindFoodSolution()
        {
            FoodWorkStorage = new Dictionary<int, int>(FoodStorage);    // work copy of foodstorage
            FoodRequirements = new Dictionary<int, int>();              // work copy of foodstorage
            AnimalsOfTypes = new Dictionary<int, int>();                // amount of animals of each type
            FoodBuckets = new List<FoodBucket>();

            FoodCreateLists(ref AnimalsOfTypes, ref FoodWorkStorage, ref FoodRequirements, ref FoodBuckets);  // create required lists of data

            DisplayMessage(GetFoodWorkDisplay());

            // check that we have food for animals that eat only one type of food 
            if (!FoodCheckThatEatOnly_1_Food(FoodWorkStorage))
            {
                return false;
            }

            // now we have WorkFoodStorage with only food for animals that eat 2 types of food
            // check minimal requirement list of food for them
            if (!FoodCheckMinimalRequirements(FoodRequirements, FoodWorkStorage))
            {
                return false;
            }

            // minimal cheсks passed - try feed 2-mode animals

            FoodFillBucketsByHalf();
            FoodPushToEmptyBuckets();


            // check solution
            bool FoodNeedFood = FoodBucketsNeedFood();
            string message = !FoodNeedFood ? "Food Solution Exists" : "Food Solution Dont Finded";

            DisplayMessage(message);



            DisplayMessage("Search Food Solution Done");

            return true;
        }


        /// <summary>
        /// заполняем бакеты по половине - для сбаласнированного питания - это наша база  с которой мы работаем
        /// </summary>
        protected void FoodFillBucketsByHalf() {
            int FoodType = 0;
            int FoodAmount = 0;
            foreach (var item in FoodBuckets)
            {
                // try fill 1st food
                FoodType = item.TypeFood_1;
                FoodAmount = FoodWorkStorage[FoodType];
                
                if (FoodAmount > 0)
                {
                    FoodWorkStorage[FoodType] = item.FillFoodType_1(FoodAmount);
                }

                // try fill 2nd food
                FoodType = item.TypeFood_2;
                FoodAmount = FoodWorkStorage[FoodType];

                if (FoodAmount > 0)
                {
                    FoodWorkStorage[FoodType] = item.FillFoodType_2(FoodAmount);
                }
            }
            return;
        }


        /// <summary>
        ///  1) пытаемся излишками заполнить пустые места еды, если излишки есть - заполняем
        ///  2) ищем ситуации блокировок - когда излишек на складе есть - но он не того типа что нужен.
        /// а нужный тип занят.
        /// но мы можем занятый нужный тип высвободить поменяв на излишек со склада
        /// </summary>
        protected void FoodPushToEmptyBuckets() {
            int FoodType_1 = 0;
            int FoodType_2 = 0;

            FoodBucket item;

            int i = 0;
            for (i = 0; i < FoodBuckets.Count; i++)
            {
                item = FoodBuckets[i];

                if (!(item.NeedFood() > 0)) // если бакет заполнен - ничего с ним не делаем
                {
                    continue;
                }

                item = FoodBuckets[i];
                FoodType_1 = item.TypeFood_1;
                FoodType_2 = item.TypeFood_2;


                // 1) пытаемся излишками заполнить пустые места еды, если излишки есть - заполняем
                // if need food - try push it from food 2
                if (FoodWorkStorage[FoodType_1] > 0)
                {
                    FoodWorkStorage[FoodType_1] = item.PushFood_1(FoodWorkStorage[FoodType_1]);
                }

                // if again need food - try push it from food 1
                if (item.NeedFood() > 0)
                {
                    if (FoodWorkStorage[FoodType_2] > 0)
                    {
                        FoodWorkStorage[FoodType_2] = item.PushFood_2(FoodWorkStorage[FoodType_2]);
                    }
                }
                


                // 2) ищем ситуации блокировок - когда излишек на складе есть - но он не того типа что нужен.
                // а нужный тип занят.
                // но мы можем занятый нужный тип высвободить поменяв на свободный излишек со склада
                if (item.NeedFood() > 0)
                {
                    // ищем пары для используемых типов продуктов, и излишки этих пар есть на складе
                    bool result = FoodGetLinkedFreeFoodTypes(i);
                }
            }
            return;
        }




        public bool FoodGetLinkedFreeFoodTypes(int index)
        {
            FoodBucket target = FoodBuckets[index];
            bool result = false;

            FoodBucket item;
            for (int i = 0; i < FoodBuckets.Count; i++ )
            {
                if (i == index) // dont use at search current bucket - we want fill it - not take from it )
                {
                    continue;
                }

                item = FoodBuckets[i];

                result = FoodTryExchange(TakedNeededType: item.TypeFood_1, TakedLinkedType: item.TypeFood_2, 
                        TargetNeededType: target.TypeFood_1, TakedBucketIndex: i, TargetBucketIndex: index);
                if (result)
                {
                    return true;
                }
                //result = FoodTryExchange(ForType: item.TypeFood_1, ForType_2: item.TypeFood_2, FromType: target.TypeFood_2, FromBucket: i, ToBucket: index);
                //if (result)
                //{
                //    return true;
                //}
                //result = FoodTryExchange(ForType: item.TypeFood_2, ForType_2: item.TypeFood_1, FromType: target.TypeFood_1, FromBucket: i, ToBucket: index);
                //if (result)
                //{
                //    return true;
                //}
                //result = FoodTryExchange(ForType: item.TypeFood_2, ForType_2: item.TypeFood_1, FromType: target.TypeFood_2, FromBucket: i, ToBucket: index);
                //if (result)
                //{
                //    return true;
                //}

                if (!(item.NeedFood() > 0)) // если получилось - 2 тип еды не пробуем
                {
                    return true;
                }

            }

            return false;
        }



        protected bool FoodTryExchange( int TakedNeededType, int TakedLinkedType, int TargetNeededType, int TakedBucketIndex, int TargetBucketIndex)
        {
            if (TakedNeededType != TargetNeededType)  // типы не совпадают
            {
                return false;
            }
            
            if (FoodWorkStorage[TakedLinkedType] <= 0) // нет свободных излишков на складе связанного типа
            {
                return false;
            }

            FoodBucket TakedItem = FoodBuckets[TakedBucketIndex];
            FoodBucket TargetItem = FoodBuckets[TargetBucketIndex];

            int RequiredFoodAmount = TargetItem.NeedFood();

            int CanChange = 0;

            if (FoodWorkStorage[TakedLinkedType] > RequiredFoodAmount)
            {
                CanChange = TakedItem.ExchangeFood(RequiredFoodAmount, TakedLinkedType); // ????

                FoodWorkStorage[TakedLinkedType] -= CanChange;



                // free TakedNeededType from TakedBucketIndex item by fill by TakedLinkedType
                // ..

                return true;    // only here we fully fill needed amount
            }
            else
            {
                int TakedAmount = FoodWorkStorage[TakedLinkedType];
                FoodWorkStorage[TakedLinkedType] = 0;

                // free TakedAmount from TakedBucketIndex item by fill by TakedLinkedType
                // ... 
            }

            return false; 

        }




        protected bool FoodBucketsNeedFood() { 
            foreach (var item in FoodBuckets)
            {
                if (item.NeedFood() > 0)
                {
                    return true;
                }
            }
            return false;
        }



        protected void FoodCreateLists(ref Dictionary<int, int> AnimalsOfTypes, ref Dictionary<int, int> WorkFoodStorage, ref Dictionary<int, int> FoodRequirements,
            ref List<FoodBucket> foodBuckets)
        {
            int FoodType = 0;
            int index = 0;

            foreach (var item in Animals)
            {
                // 1) create list: amount of animals of each type  
                Zoo.DicAddOrUpdate(dic: AnimalsOfTypes, key: item.Type, addOrNewValue: 1);     // AnimalsOfTypes[item.Type] += 1;


                // 2) and at first check animals that eat only one type of food: deduct all food for animals that eat only one type of food
                if (AnimalsRules[item.Type].CanEatFood_2 == 0)
                {
                    FoodType = AnimalsRules[item.Type].CanEatFood_1;
                    WorkFoodStorage[FoodType] -= FoodAmountForOneAnimal;
                }

                else
                {
                    // 3) create food requirement list for animals that can eat 2 food types
                    FoodType = AnimalsRules[item.Type].CanEatFood_1;
                    Zoo.DicAddOrUpdate(dic: FoodRequirements, key: FoodType, addOrNewValue: 1);    // FoodRequirements[FoodType] += 1;

                    FoodType = AnimalsRules[item.Type].CanEatFood_2;
                    Zoo.DicAddOrUpdate(dic: FoodRequirements, key: FoodType, addOrNewValue: 1);    // FoodRequirements[FoodType] += 1;

                    // 4) create food buckets
                    index = FindFoodBucketIndex(AnimalsRules[item.Type].CanEatFood_1, AnimalsRules[item.Type].CanEatFood_2);
                    if (index < 0)
                    {
                        FoodBuckets.Add(new FoodBucket(AnimalsRules[item.Type].CanEatFood_1, AnimalsRules[item.Type].CanEatFood_2));
                    }
                    else
                    {
                        FoodBuckets[index].BucketsAmount += 1;
                    }
                }
            }
            return;
        }


        protected int FindFoodBucketIndex(int FoodType_1, int FoodType_2)
        {
            FoodBucket item;
            for (int i = 0; i < FoodBuckets.Count; i++)
            {
                item = FoodBuckets[i];
                if ((item.TypeFood_1 == FoodType_1 && item.TypeFood_2 == FoodType_2) ||
                    (item.TypeFood_2 == FoodType_1 && item.TypeFood_1 == FoodType_2))
                {
                    return i;
                }
            }
            return -1;
        }


        /// <summary>
        /// add or update value at dictionnary<int,int>
        /// </summary>
        /// <param name="dic"></param>
        /// <param name="key"></param>
        /// <param name="addOrNewValue"></param>
        public static void DicAddOrUpdate(Dictionary<int, int> dic, int key, int addOrNewValue)
        {
            int val;
            if (dic.TryGetValue(key, out val))
            {
                // yay, value exists!
                dic[key] = val + addOrNewValue;
            }
            else
            {
                // darn, lets add the value
                dic.Add(key, addOrNewValue);
            }
        }


        protected bool FoodCheckThatEatOnly_1_Food(Dictionary<int, int> WorkFoodStorage)
        {
            // check - if we dont have some food for animals that eat one type of food
            Dictionary<int, int> DontHaveFood = new Dictionary<int, int>();
            foreach (var item in WorkFoodStorage)
            {
                if (item.Value < 0)
                {
                    Zoo.DicAddOrUpdate(dic: DontHaveFood, key: item.Key, addOrNewValue: item.Value);    // DontHaveFood[item.Key] += item.Value;                    
                }
            }

            if (DontHaveFood.Count > 0)
            {
                StringBuilder sb = new StringBuilder();
                foreach (var item in DontHaveFood)
                {
                    sb.AppendFormat("\r\nHave delta {0} item(s) of food type {1}", item.Value, item.Key);
                }
                DisplayMessage("Food Check 1: we cant feed  animals that eat only 1 food type: " + sb.ToString() + "\r\n");
                return false;
            }

            return true;
        }



        protected bool FoodCheckMinimalRequirements(Dictionary<int, int> FoodRequirements, Dictionary<int, int> WorkFoodStorage)
        {
            // check - if we dont have some food for animals that eat two type of food
            Dictionary<int, int> DontHaveFood = new Dictionary<int, int>();
            int HaveFoodAmount = 0;
            int RequiredFoodAmount = 0;

            foreach (var item in FoodRequirements)
            {
                HaveFoodAmount += WorkFoodStorage[item.Key];
                RequiredFoodAmount += item.Value;
                // use for status display
                Zoo.DicAddOrUpdate(dic: DontHaveFood, key: item.Key, addOrNewValue: item.Value - HaveFoodAmount);    // DontHaveFood[item.Key] += item.Value;
            }

            if (HaveFoodAmount < RequiredFoodAmount)
            {
                StringBuilder sb = new StringBuilder();
                foreach (var item in DontHaveFood)
                {
                    sb.AppendFormat("\r\nHave {0} item of food type {1}", item.Value, item.Key);
                }
                DisplayMessage("Food Check 2: we cant feed animals that eat 2 food type. Have total: " + HaveFoodAmount.ToString() + "; Need Total: " + RequiredFoodAmount.ToString());
                DisplayMessage("Details for food types: " + sb.ToString());
                return false;
            }

            return true;
        }

        #endregion



    }


}
