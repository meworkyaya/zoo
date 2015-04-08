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
        public uint FoodPackagesAmount { get; set; }
        public uint FoodTypesAmount { get; set; }       // amount of types of food

        public Dictionary<uint,uint> FoodStorage;       // list with amount of food packages of each type that are at Zoo: <type of food : amount of food>


        // results
        public List<List<uint>> CeilsResults;           // results for ceils: array of array of <id of ceil => animal id>
        public List<List<Animal>> FoodResults;          // results for foods: array of array of <id of animal : animal with food settings>




        #region init

        public Zoo(uint animals = 10, uint animalsTypes = 3, uint ceils = 15, uint foodPackage = 40, uint foodTypes = 3)
        {
            Animals = new List<Animal>();
            Ceils = new List<Ceil>();
            FoodStorage = new Dictionary<uint, uint>();

            GenerateZooModel(animals, animalsTypes, ceils, foodPackage, foodTypes);
        }


        public void GenerateZooModel(uint animals = 10, uint animalsTypes = 3,  uint ceils = 15, uint foodPackages = 40, uint foodTypes = 3)
        {
            // ceils model
            AnimalsAmount = animals;
            AnimalTypesAmount = animalsTypes;
            CeilsAmount = ceils;

            // food model
            FoodPackagesAmount = foodPackages;
            FoodTypesAmount = foodTypes;

            Animals.Clear();
            Ceils.Clear();
            FoodStorage.Clear();

            InitAnimals(AnimalsAmount);
            InitCeils(CeilsAmount);
            InitFood(FoodPackagesAmount);
        }


        public void CreateFreshFoodTaskModel( uint animals = 10, uint foodPackage = 40, uint foodTypes = 3 ){
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
        /// init food
        /// </summary>
        /// <param name="count"></param>
        public void InitFood(uint count)
        {
            for (int i = 0; i < count; i++)
            {
                FoodStorage.Add(new FoodType());
            }
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
