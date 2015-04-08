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
        public uint AnimalTypesAmount { get; set; }     // amount of types of animal
        public uint AnimalsAmount { get; set; }         // mount of animals

        public List<Animal> Animals;                    // list with animals that live at Zoo: index - id of animal;
        public Dictionary<uint,ZooAnimalsRules> AnimalsRules;      // dict of rules for each type of Animal: <type of animal, animalRule>


        // ceil Model
        public uint CeilsAmount { get; set; }
        public List<Ceil> Ceils;                        // list with ceils that are at Zoo: index - id of ceil; value: id of animal


        // food Model
        public int FoodPackagesAmount { get; set; }
        public int FoodTypesAmount { get; set; }       // amount of types of food

        public Dictionary<int,int> FoodStorage;       // list with amount of food packages of each type that are at Zoo: <type of food : amount of food>


        // results
        public List<List<uint>> CeilsResults;           // results for ceils: array of array of <id of ceil => animal id>
        public List<List<Animal>> FoodResults;          // results for foods: array of array of <id of animal : animal with food settings>




        #region init

        public Zoo(uint animals = 10, uint animalsTypes = 3, uint ceils = 15, int foodPackage = 40, int foodTypes = 3)
        {
            Animals = new List<Animal>();
            Ceils = new List<Ceil>();
            FoodStorage = new Dictionary<int, int>();

            CeilsResults = new List<List<uint>>();
            FoodResults = new List<List<Animal>>();

            GenerateZooModel(animals, animalsTypes, ceils, foodPackage, foodTypes);
        }


        public void GenerateZooModel(uint animals = 10, uint animalsTypes = 3,  uint ceils = 15, int foodPackages = 40, int foodTypes = 3)
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
            InitAnimals(AnimalsAmount);
            InitCeils(CeilsAmount);
            InitFood(FoodPackagesAmount, FoodTypesAmount);
        }


        public void CreateFreshFoodTaskModel( uint animals = 10, int foodPackage = 40, int foodTypes = 3 ){
            GenerateZooModel(animals, 0, 0, foodPackage, foodTypes);
        }
        public void CreatePlacementTaskModel(uint animals = 10, uint animalsTypes = 3, uint ceils = 15 )
        {
            GenerateZooModel(animals, animalsTypes, ceils, 0, 0);
        }

        /// <summary>
        /// init animals
        /// </summary>
        /// <param name="count"></param>
        public void InitAnimals( uint count){
            for (int i = 0; i < count; i++)
            {
                Animals.Add(new Animal());
            }
        }

        /// <summary>
        /// init ceils
        /// </summary>
        /// <param name="count"></param>
        public void InitCeils( uint count)
        {
            for (int i = 0; i < count; i++)
            {
                Ceils.Add(new Ceil());
            }
        }

        /// <summary>
        /// init food:
        /// at each step we generate random amount of each type food; and find amount of food that left
        /// </summary>
        /// <param name="count"></param>
        public void InitFood(int count, int typesAmount)
        {
            int FoodTypeAmount = 0;
            int Left = count;
            Random rnd = new Random();

            for (int i = 1; i < typesAmount; i++)
            {
                FoodTypeAmount = rnd.Next(1, Left);
                FoodStorage[i] = FoodTypeAmount;
                Left -= FoodTypeAmount;
            }
            FoodStorage[typesAmount] = Left;    // add last item
        }

        #endregion



        public void DisplayMessage( string message){
            Console.WriteLine( message);
        }

        public int Run(){

            DisplayMessage( "done");            
            return 0;
        }
    }
}
