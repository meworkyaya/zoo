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




        #region init

        public Zoo(int animals = 10, int animalsTypes = 3, int ceils = 15, int foodPackage = 40, int foodTypes = 3)
        {
            Animals = new List<Animal>();
            Ceils = new List<Ceil>();
            FoodStorage = new Dictionary<int, int>();

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
            Random rnd = new Random();
            int type = 0;
            for (int i = 0; i < count; i++)
            {
                type = rnd.Next(1, typeAmount);
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
            Random rnd = new Random();

            // !! attention: at cycle we dont step to last step; last setp we set after end of cycle becasue we cant set random value at last step
            for (int i = 1; i < typesAmount; i++)
            {
                FoodTypeAmount = rnd.Next(1, Left);
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


        protected void InitAnimalRules(int animalTypesAmount, int foodTypesAmount)
        {



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
