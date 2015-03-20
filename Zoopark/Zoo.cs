using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DbBest.ZooPark
{
    /// <summary>
    /// types of animals
    /// </summary>
    public enum AnimalTypes { Alpha, Beta, Cann  }

    /// <summary>
    /// food types
    /// </summary>
    public enum FoodTypes { FoodOne, FoodTwo, FoodThree, FoodForth }

    public enum CeilTypes { CeilOne, CeilTwo } 


    public class Zoo
    {
        public uint AnimalsAmount { get; set; }


        #region ceilModel
        // for model of animal palcement to ceils
        public uint AnimalTypesAmount { get; set; }     // amount of types of animal
        public uint CeilsAmount { get; set; }

        #endregion


        #region foodModel
        // for model of giving food to animals

        public uint FoodPackagesAmount { get; set; }

        public uint FoodTypesAmount { get; set; }     // amount of types of food

        #endregion


        public List<Animal> Animals;            // list with animals that live at Zoo
        public List<Ceil> Ceils;                // list with ceils that are at Zoo
        public List<Food> FoodPackages;         // list with food packages that are at Zoo

        #region init

        public Zoo(uint animals = 10, uint animalsTypes = 3, uint ceils = 15, uint foodPackage = 40, uint foodTypes = 3)
        {
            Animals = new List<Animal>();
            Ceils = new List<Ceil>();
            FoodPackages = new List<Food>();

            SetupZooModel(animals, animalsTypes, ceils, foodPackage, foodTypes);
        }


        public void SetupZooModel(uint animals = 10, uint animalsTypes = 3,  uint ceils = 15, uint foodPackages = 40, uint foodTypes = 3)
        {
            // ceils model
            AnimalsAmount = animals;
            AnimalTypesAmount = animalsTypes;
            CeilsAmount = ceils;

            // food model
            FoodPackagesAmount = foodPackages;
            FoodTypesAmount = foodTypes;

            InitAnimals(AnimalsAmount);
            InitCeils(CeilsAmount);
            InitFood(FoodPackagesAmount);
        }


        public void CreateFreshFoodTaskModel( uint animals = 10, uint foodPackage = 40, uint foodTypes = 3 ){
            SetupZooModel(animals, 0, 0, foodPackage, foodTypes);
        }
        public void CreatePlacementTaskModel(uint animals = 10, uint animalsTypes = 3, uint ceils = 15, )
        {
            SetupZooModel(animals, animalsTypes, ceils, 0, 0);
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
                FoodPackages.Add(new Food());
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
