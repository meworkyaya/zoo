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
    enum AnimalTypes { Alpha, Beta, Cann  }

    /// <summary>
    /// food types
    /// </summary>
    enum FoodTypes { FoodOne, FoodTwo, FoodThree, FoodForth }

    enum CeilTypes { CeilOne, CeilTwo } 


    public class Zoo
    {
        public uint AnimalsAmount { get; set; }
        public uint CeilsAmount { get; set; }
        public uint FoodPackagesAmount { get; set; }


        public List<Animal> Animals;            // list with animals that live at Zoo
        public List<Ceil> Ceils;                // list with ceils that are at Zoo
        public List<Food> FoodPackages;         // list with food packages that are at Zoo

        #region init

        public Zoo(uint animals = 10, uint ceils = 15, uint foodPackage = 40)
        {
            this.AnimalsAmount        = animals;
            this.CeilsAmount          = ceils;
            this.FoodPackagesAmount    = foodPackage;

            Animals = new List<Animal>();
            Ceils = new List<Ceil>();
            FoodPackages = new List<Food>();

            InitAnimals(AnimalsAmount);
            InitCeils(CeilsAmount);
            InitFood(FoodPackagesAmount);
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
