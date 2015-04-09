using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DbBest.ZooPark
{
    public class Zoo
    {
        // common Model Part
        public int AnimalTypesAmount { get; set; }     // amount of types of animal
        public int AnimalsAmount { get; set; }         // mount of animals

        protected List<Animal> Animals;                    // list with animals that live at Zoo: index - id of animal;
        protected Dictionary<int, ZooAnimalsRules> AnimalsRules;      // dict of rules for each type of Animal: <type of animal, animalRule>

        protected int[,] AnimalsLivingWithRules;            // rules for animals - with what they can live; 0 - can live together; 1 - cant live together


        // ceil Model
        protected int CeilsAmount { get; set; }
        protected List<Ceil> Ceils;                        // list with ceils that are at Zoo: index - id of ceil; value: id of animal


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
            Ceils = new List<Ceil>();
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
            Ceils.Clear();
            FoodStorage.Clear();
            AnimalsRules.Clear();

            CeilsResults.Clear();
            FoodResults.Clear();

            // generate new lists
            InitAnimals(AnimalsAmount, AnimalTypesAmount);
            InitAnimalRules(AnimalTypesAmount, FoodTypesAmount);
            InitCeils(CeilsAmount);
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
        /// init ceils - just add empty ceils
        /// </summary>
        /// <param name="count"></param>
        protected void InitCeils(int count)
        {
            for (int i = 0; i < count; i++)
            {
                Ceils.Add(new Ceil());
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

            int randomGenerateChanceBorder = 2;   // how often generate rule: 2 -> 1/2; 3 -> 2/3; ... 4 -> 3/4; ..

            bool applyLiveRule = false;
            int cantLiveWithType = 0;

            // по умолчанию новый массив забивается нулями, так что эта инициализация не нужна
            // но оставим ее на всякий случай ))
            for (int i = 0; i <= animalTypesAmount; i++)
            {
                AnimalsLivingWithRules[i, 0] = 0;   // по умолчанию с пустой клеткой все могут жить.. 
                AnimalsLivingWithRules[0, i] = 0;   // и пустая клетка может жить со всеми
            }

            // for all types
            for (int i = 1; i <= animalTypesAmount; i++)
            {
                cantLiveWithType = 0;

                // generate rule with what animla type cant live
                applyLiveRule = _rnd.Next(1, randomGenerateChanceBorder + 1) > 1 ? true : false;  // randomly find - apply rule for animal type or not                

                // fill live together rule data
                if (applyLiveRule)
                {
                    cantLiveWithType = _rnd.Next(1, animalTypesAmount + 1);

                    // иногда животное не может жить со своим типом - например два быка ); поэтому фильтр ниже убираем
                    // cantLiveWithType = (cantLiveWithType != i) ? cantLiveWithType : 0;  // if selected type of animal with wich we cnat live is the same as animal itself type - reset rule

                    AnimalsLivingWithRules[i, cantLiveWithType] = 1;
                    AnimalsLivingWithRules[cantLiveWithType, i] = 1;    // если заяц не живет со львом, то лев не живет с зайцем тоже )
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

        public void LogMessage(string message)
        {
            if (LogFile != null)
            {
                LogFile.WriteLine(message);
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


        public string GetCeilsDisplay()
        {
            StringBuilder sb = new StringBuilder();
            int count = 0;

            sb.AppendFormat("Ceils; {0} items; types of animals: ==================================== \r\n", Ceils.Count);
            foreach (var pair in Ceils)
            {
                count++;

                // sb.AppendFormat("{0}: animal : {1} | Amount: \t{2} \r\n", count, pair.Type);
                sb.AppendFormat("{0} ", pair.Type);
            }
            sb.Append("\r\n");

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



        public string GetDisplayZooDebugInfo()
        {
            string spacer = "\r\n";

            string sr = GetAnimasRulesDisplay();
            string sf = GetFoodDisplay();
            string sa = GetAnimalsDisplay();
            string sc = GetCeilsDisplay();

            string result = sr + spacer + sf + spacer + sa + spacer + sc;

            return result;
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
        /// 
        /// </summary>
        /// <param name="successLimit"></param>
        /// <returns></returns>
        public bool findCeilSolutionByNumberBase(int successLimit)
        {
            int TotalAnimalTypesAmount = AnimalTypesAmount + 1;     // у нас общее число типов животных: Animal Types Amount + Нет Животного
            int CheckedCeilsAmount = CeilsAmount + 1;               // используем дополнительную клетку (разряд числа) для проверки что перебор прошел все варианты
            NumberWithBase nb = new NumberWithBase(TotalAnimalTypesAmount, CheckedCeilsAmount);     // число с нашим набором вариантов

            int i;

            // выставляем начальный вариант с которого будем перебирать, меньшие варианты нам не нужны
            // допустим у нас 5 животных 3 типов; для этого всем битам от 0 до ( 5 - 1 - 1) ставим значение 3 (старший тип животного).
            for (i = 0; i < AnimalsAmount - 2; i++)
            {
                nb.setBit(i, AnimalTypesAmount);
            }

            int HighBitIndex = CeilsAmount;
            int current, next;

            long count = 0;
            long displaySteps = 100 * 1000;

            DisplayMessage("Begin ceil placing search ... =========================\r\n");

            int currentCantLiveType = 0, nextCantLiveType = 0;


            do
            {
                nb.inc();   // inc number - create next combination
                count++;

                // check rules

                // we dont need check last animal we will check last animal rule at previous animal step
                for (i = 0; i < CeilsAmount - 1; i++)
                {
                    current = nb.getBit(i);
                    next = nb.getBit(i + 1);

                    // currentCantLiveType = CantLiveTogether[current];    // curretn cant live with this
                    // nextCantLiveType = CantLiveTogether[next];          // next cant live with this - they can differ )

                    // check type for living for 'current' point of view
                    if (currentCantLiveType != 0 && currentCantLiveType == next) // these two cant live together - check new combination
                    {
                        // failed combination
                        goto NextCombination;
                    }

                    // check type for living for 'next' point of view
                    if (nextCantLiveType != 0 && nextCantLiveType == current) // these two cant live together - check new combination
                    {
                        // failed combination
                        goto NextCombination;
                    }
                }

                // DisplayMessage( nb.DisplayBits());

                NextCombination: ;

                // display workign status
                if (count % displaySteps == 0)
                {
                    Console.WriteLine("\rcount: {0}", count);
                }

            } while (nb.getBit(HighBitIndex) == 0);

            return false;
        }




        public bool findCeilSolutionByUniquePermutation(int successLimit)
        {
            int TotalAnimalTypesAmount = AnimalTypesAmount + 1;     // у нас общее число типов животных: Animal Types Amount + Нет Животного
            int CheckedCeilsAmount = CeilsAmount + 1;               // используем дополнительную клетку (разряд числа) для проверки что перебор прошел все варианты
            NumberWithBase nb = new NumberWithBase(TotalAnimalTypesAmount, CheckedCeilsAmount);     // число с нашим набором вариантов

            int i;

            // выставляем начальный вариант с которого будем перебирать, меньшие варианты нам не нужны
            // допустим у нас 5 животных 3 типов; для этого всем битам от 0 до ( 5 - 1 - 1) ставим значение 3 (старший тип животного).
            for (i = 0; i < AnimalsAmount - 2; i++)
            {
                nb.setBit(i, AnimalTypesAmount);
            }

            int HighBitIndex = CeilsAmount;
            int current, next;

            long count = 0;
            long displaySteps = 100 * 1000;

            DisplayMessage("Begin ceil placing search ... =========================\r\n");

            int currentCantLiveType = 0, nextCantLiveType = 0;


            // =======================  some dirty algoritm

            List<int> AllAnimalsThatLeft = GetAllAnimalsAsListInt();
            List<int> UniqueTypes = GetUniqueTypes( AllAnimalsThatLeft );
            
            int CurrentStep = 1;

            foreach( var Type in UniqueTypes ){
                int NewCurrentStep = CurrentStep - 1;

                if (NewCurrentStep > 0)
                {
                    NewAnimalsThatLeft = AllAnimalsThatLeft.Remove(Type);
                    DoAllPermutations(NewCurrentStep, NewAnimalsThatLeft);
                }
                else
                {
                    // check variation
                }
            }











            do
            {
                nb.inc();   // inc number - create next combination
                count++;

                // check rules

                // we dont need check last animal we will check last animal rule at previous animal step
                for (i = 0; i < CeilsAmount - 1; i++)
                {
                    current = nb.getBit(i);
                    next = nb.getBit(i + 1);

                    // currentCantLiveType = CantLiveTogether[current];    // curretn cant live with this
                    // nextCantLiveType = CantLiveTogether[next];          // next cant live with this - they can differ )

                    // check type for living for 'current' point of view
                    if (currentCantLiveType != 0 && currentCantLiveType == next) // these two cant live together - check new combination
                    {
                        // failed combination
                        goto NextCombination;
                    }

                    // check type for living for 'next' point of view
                    if (nextCantLiveType != 0 && nextCantLiveType == current) // these two cant live together - check new combination
                    {
                        // failed combination
                        goto NextCombination;
                    }
                }

                // DisplayMessage( nb.DisplayBits());

                NextCombination: ;

                // display workign status
                if (count % displaySteps == 0)
                {
                    Console.WriteLine("\rcount: {0}", count);
                }

            } while (nb.getBit(HighBitIndex) == 0);

            return false;
        }


        public bool MakePermutation(int CurrentStep, List<int> FreeItems)
        {
            bool result = false;

            List<int> UniqueTypes = GetUniqueTypes(FreeItems);

            int NewCurrentStep = CurrentStep - 1;

            foreach (var Type in UniqueTypes)
            {
                if (NewCurrentStep > 0)
                {
                    NewItemsThatLeft = AllAnimalsThatLeft.Remove(Type);
                    DoAllPermutations(NewCurrentStep, NewAnimalsThatLeft);
                }
                else
                {
                    // check variation
                }
            }


            if (step > 0)
            {
                // make next step for permutation
            }
            else
            {
                // have ready 
            }
            return result;
        }



        /// <summary>
        /// return all animals list as List<int> from List<Animal>
        /// </summary>
        /// <returns></returns>
        public List<int> GetAllAnimalsAsListInt()
        {
            List<int> result = new List<int>();
            foreach (var item in Animals)
            {
                result.Add(item.Type);
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



        public int Run()
        {
            findCeilSolutionByNumberBase(100);

            DisplayMessage("done");
            return 0;
        }



    }
}
