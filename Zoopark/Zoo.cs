using System;
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
        public int AnimalsAmount { get; set; }         // mount of animals

        protected List<Animal> Animals;                    // list with animals that live at Zoo: index - id of animal;
        protected Dictionary<int, ZooAnimalsRules> AnimalsRules;      // dict of rules for each type of Animal: <type of animal, animalRule>

        protected int[,] AnimalsLivingWithRules;            // rules for animals - with what they can live; 0 - can live together; 1 - cant live together
        protected static int CantLiveWithFlag = 1;          // flag when cant live with


        // ceil Model
        protected int CeilsAmount { get; set; }

        protected long FailCount = 0;
        protected long SuccessCount = 0;
        protected long AttemptCount = 0;
        protected long DisplaySteps = 1000 * 10;



        // food Model
        protected int FoodPackagesAmount { get; set; }
        protected int FoodTypesAmount { get; set; }       // amount of types of food

        protected Dictionary<int, int> FoodStorage;       // list with amount of food packages of each type that are at Zoo: <type of food : amount of food>


        // results
        public List<List<uint>> CeilsResults;           // results for ceils: array of array of <id of ceil => animal id>
        public List<List<Animal>> FoodResults;          // results for foods: array of array of <id of animal : animal with food settings>


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

        public Zoo(int animals = 10, int animalsTypes = 3, int ceils = 15, int foodPackage = 40, int foodTypes = 3, string logFile = "")
        {
            CreateLogFile(logFile);

            _rnd = new Random();

            Animals = new List<Animal>();
            FoodStorage = new Dictionary<int, int>();
            AnimalsRules = new Dictionary<int, ZooAnimalsRules>();

            CeilsResults = new List<List<uint>>();
            FoodResults = new List<List<Animal>>();

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

            CeilsResults.Clear();
            FoodResults.Clear();

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
            int FoodTypeAmount = 0;
            int Left = count;

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
                applyLiveRule = ( rndNext > 1 ) ? true : false;

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
        protected void CreateLogFile(string fileName)
        {
            CloseLogFile();

            bool result = false;
            if (!String.IsNullOrWhiteSpace(fileName))
            {
                try
                {
                    LogFile = new System.IO.StreamWriter(fileName);
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

        public void LogMessage(string message, string Preffix = null )
        {
            if (LogFile != null)
            {
                LogFile.WriteLine( Preffix + message);
            }
        }

        #endregion



        #region display debug

        public string GetAnimalsDisplay()
        {
            StringBuilder sb = new StringBuilder();
            int count = 0;

            sb.AppendFormat("Animals; {0} items ==================================== \r\n", Animals.Count);
            foreach (Animal a in Animals)
            {
                count++;

                sb.AppendFormat("{0}: \t{1}", count, a.DisplayAnimal(1));
                sb.Append("\r\n");
            }


            return sb.ToString();
        }



        public string GetFoodDisplay()
        {
            StringBuilder sb = new StringBuilder();
            int count = 0;

            sb.AppendFormat("Food Types: {0} types; Food Storage: {1} items ================= \r\n", FoodTypesAmount, FoodPackagesAmount);
            foreach (var pair in FoodStorage)
            {
                count++;

                sb.AppendFormat("{0}: type: {1} | Amount: \t{2} \r\n", count, pair.Key, pair.Value);
            }

            return sb.ToString();
        }


        public string GetAnimasRulesDisplay()
        {
            StringBuilder sb = new StringBuilder();
            int count = 0;

            sb.AppendFormat("AnimalsRules: {0} rules;  ===================================== \r\n", AnimalsRules.Count);
            foreach (var pair in AnimalsRules)
            {
                count++;

                sb.AppendFormat("{0}: {1} \r\n", count, pair.Value.DisplayRule());
            }

            return sb.ToString();
        }


        public string GetAnimasLivingRulesDisplay()
        {
            StringBuilder sb = new StringBuilder();

            sb.AppendFormat("Animals Living Rules: {0} rules;  ===================================== \r\n", AnimalsLivingWithRules.Length );

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

            sb.AppendLine( GetAnimalsDisplay() );
            sb.AppendLine( GetAnimasLivingRulesDisplay() );

            // sb.AppendLine( GetAnimasRulesDisplay() );
            // sb.AppendLine( GetFoodDisplay() );

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
        /// у нас есть животные некого числа типов
        /// размещение их по клеткам означает что в клетке есть некий тип.
        /// 
        /// тогда например для 4 типов животных мы получааем для 5 клеток набор:
        /// 1 3 0 2 1
        /// 
        /// т.е. эти наборы формируют разные числа по базису числа типов животных ( по базису 4 в данном примере)
        /// перебор по числам от 0 0 0 0 0 до  4 4 4 4 4  даст нам все варианты расположения животных без дупликатов
        /// 0 - отсутствие животного
        /// 
        /// в общем случае получаем: 
        /// расположение по клеткам:
        ///  A B C D E F G H ... 
        ///  где A-H - тип животного
        ///  
        /// мы создаем новый вариант с помощью инкремента - и проверяем его на правила
        /// 
        /// </summary>
        /// <param name="successLimit"></param>
        public void findCeilSolutionByUniquePermutation(int successLimit)
        {
            int TotalAnimalTypesAmount = AnimalTypesAmount + 1;     // у нас общее число типов животных: Animal Types Amount + Нет Животного
            int HighBitIndex = CeilsAmount;

            DisplayMessage("Begin ceil placing search ... =========================");

            AttemptCount = 0;
            SuccessCount = 0;
            FailCount = 0;


            // =======================  some dirty algoritm

            // we are using array becase theoreticelly they give litle better performance, must create some benchmarks for this
            List<int> CurrentItems = new List<int>();       // partial result with already placed items/animals
            List<int> AnimalsThatLeft = GetAllAnimalsAndEmptyCeilsAsListInt();

            int CurrentStep = CeilsAmount;

            MakePermutation(CurrentStep, ref CurrentItems, ref AnimalsThatLeft);

            DisplayMessage("\r\nDone =========================");
            DisplayMessage("Result: Success: " + SuccessCount + ";     Failed: " + FailCount );
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
            AttemptCount++;

            if (AttemptCount % DisplaySteps == 0)
            {
                Console.Write("\rFunc calls: {0}: Finded: {1}; Failed: {2}", AttemptCount, SuccessCount, FailCount);
            }


            if (CurrentStep == 0)
            {
                SuccessCount++;
                // DisplayMessage( Zoo.DisplayListInt( ref CurrentItems));
                LogMessage(Zoo.DisplayListInt(ref CurrentItems));
                return; // stop recursion
            }

            if (AnimalsThatLeft.Count == 0)
            {
                DisplayMessage( "Error: wrong place");
                // this is no more items - so all items placed // stop recursion
                return;
            }

            // get only unique types from current list of items
            List<int> UniqueTypes = AnimalsThatLeft.Distinct().ToList(); // it must have at least one item - because source list already has at least one item

            int NewCurrentStep = CurrentStep - 1;

            CurrentItems.Add(0);    // add new item as placeholder
            int CurrentNewItemIndex = CurrentItems.Count  - 1;
            int ItemIndex = 0;

            foreach (var Item in UniqueTypes)
            {
                // ========= try apply placement rule for animal
                if (CurrentItems.Count >= 2) // if have previous item; previous item will be 2nd from edn - because we placed placeholder for new item
                {
                    //if (!CheckLivingRuleForPair(CurrentItems[CurrentItems.Count - 2], Item))    // check rule for previous item and current item
                    if (AnimalsLivingWithRules[CurrentItems[CurrentItems.Count - 2], Item] == Zoo.CantLiveWithFlag)  // for optimization remove function call
                    {
                        FailCount++;

                        LogMessage(Zoo.DisplayListWithoutLastInt(ref CurrentItems), "Fail:\r\nTried Add last: " + Item + " => " );

                        continue; // rule does not passed - skip to next item
                    }
                }

                // clone new list 
                List<int> NewAnimalsThatLeft = new List<int>(AnimalsThatLeft); // wil have new list of items - copy of AnimalsThatLeft without items of current type

                // create new list without current item
                ItemIndex = NewAnimalsThatLeft.FindLastIndex(x => x == Item);
                NewAnimalsThatLeft.RemoveAt(ItemIndex);

                CurrentItems[CurrentNewItemIndex] = Item;

                MakePermutation(CurrentStep: NewCurrentStep, CurrentItems: ref CurrentItems, AnimalsThatLeft: ref NewAnimalsThatLeft);
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

        public void Tests()
        {
            // TestNumberWithBase();
        }

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

        #endregion




        public void DisplayMessage(string message, bool dontLogMessageEvenWhenHaveLogging = false)
        {
            Console.WriteLine(message);
            if (!dontLogMessageEvenWhenHaveLogging)
            {
                LogMessage(message);
            }
        }

        public static string DisplayListInt( ref List<int> Items ){
            StringBuilder sb = new StringBuilder();
            foreach( var Item in Items){
                sb.AppendFormat("{0} ", Items);
            }
            return sb.ToString();
        }

        public static string DisplayListWithoutLastInt(ref List<int> Items)
        {
            StringBuilder sb = new StringBuilder();
            int Length = Items.Count;
            for(int i=0; i < Length - 1; i++)
            {
                sb.AppendFormat("{0} ", Items[i]);
            }
            return sb.ToString();
        }


        public int Run()
        {
            findCeilSolutionByUniquePermutation(100);

            DisplayMessage("done");
            return 0;
        }



    }
}
