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

        public List<Animal> Animals;                    // list with animals that live at Zoo: index - id of animal;
        public Dictionary<int, ZooAnimalsRules> AnimalsRules;      // dict of rules for each type of Animal: <type of animal, animalRule>


        // ceil Model
        public int CeilsAmount { get; set; }
        public List<Ceil> Ceils;                        // list with ceils that are at Zoo: index - id of ceil; value: id of animal


        // food Model
        public int FoodPackagesAmount { get; set; }
        public int FoodTypesAmount { get; set; }       // amount of types of food

        public Dictionary<int, int> FoodStorage;       // list with amount of food packages of each type that are at Zoo: <type of food : amount of food>


        // results
        public List<List<uint>> CeilsResults;           // results for ceils: array of array of <id of ceil => animal id>
        public List<List<Animal>> FoodResults;          // results for foods: array of array of <id of animal : animal with food settings>


        // tools
        protected Random _rnd = null;




        #region init
        // init data methods

        public Zoo(int animals = 10, int animalsTypes = 3, int ceils = 15, int foodPackage = 40, int foodTypes = 3)
        {
            _rnd = new Random();

            Animals = new List<Animal>();
            Ceils = new List<Ceil>();
            FoodStorage = new Dictionary<int, int>();
            AnimalsRules = new Dictionary<int, ZooAnimalsRules>();

            CeilsResults = new List<List<uint>>();
            FoodResults = new List<List<Animal>>();

            GenerateZooModel(animals, animalsTypes, ceils, foodPackage, foodTypes);
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
            InitFood(FoodPackagesAmount, FoodTypesAmount);
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
        protected void InitFood(int count, int typesAmount)
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
        /// - what types of food can eat - 1 type or 2 types
        /// </summary>
        /// <param name="animalTypesAmount"></param>
        /// <param name="foodTypesAmount"></param>
        protected void InitAnimalRules(int animalTypesAmount, int foodTypesAmount)
        {
            bool applyLiveRule = false;
            bool applyFood_2Rule = false;

            int cantLiveWithType = 0;

            int Food_1Eat = 0;
            int Food_2Eat = 0;

            for (int i = 1; i <= animalTypesAmount; i++)
            {
                // generate rule with what animla type cant live
                applyLiveRule   = _rnd.Next(1, 3) > 1 ? true : false;  // randomly find - apply rule for animal type or not
                applyFood_2Rule = _rnd.Next(1, 3) > 1 ? true : false;

                if (applyLiveRule)
                {
                    cantLiveWithType = _rnd.Next(1, animalTypesAmount + 1);
                    cantLiveWithType = (cantLiveWithType != i) ? cantLiveWithType : 0;  // if selected type of animal with wich we cnat live is the same as animal itself type - reset rule
                }

                // generate rule what food can eat - 1st type and 2nd type
                Food_1Eat = _rnd.Next(1, foodTypesAmount + 1);
                if (applyFood_2Rule)
                {
                    Food_2Eat = _rnd.Next(1, foodTypesAmount + 1);
                    Food_2Eat = (Food_2Eat != Food_1Eat) ? Food_2Eat : 0;  // if selected type of food 2 is the same as food 1  - just reset rule
                }

                AnimalsRules[i] = new ZooAnimalsRules(i, cantLiveWithType, Food_1Eat, Food_2Eat );               
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

            sb.AppendFormat("Ceils; {0} items; types of animals: ==================================== \r\n", Ceils.Count );
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

                sb.AppendFormat("{0}: type: {1} | Amount: \t{2} \r\n", count, pair.Key, pair.Value );
            }

            return sb.ToString();
        }


        public string GetAnimasRulesDisplay()
        {
            StringBuilder sb = new StringBuilder();
            int count = 0;

            sb.AppendFormat("AnimalsRules: {0} rules;  ===================================== \r\n", AnimalsRules.Count );
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


        public void DisplayMessage(string message)
        {
            Console.WriteLine(message);
        }


        public int Run()
        {

            DisplayMessage("done");
            return 0;
        }



    }
}
