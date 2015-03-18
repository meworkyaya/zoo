using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DbBest.ZooPark
{
    /// <summary>
    /// 4 types of animals
    /// </summary>
    enum AnimalTypes { Alpha, Beta, Cann  }


    /// <summary>
    /// food types
    /// </summary>
    enum FoodTypes { FoodOne, FoodTwo } 


    public class Zoo
    {
        public uint AnimalsAmount { get; set; }
        public uint CeilsAmount { get; set; }
        public uint FoodPackageAmount { get; set; }


        public Zoo(uint animals = 10, uint ceils = 15, uint foodPackage = 40)
        {
            this.AnimalsAmount        = animals;
            this.CeilsAmount          = ceils;
            this.FoodPackageAmount    = foodPackage;
        }


        public void DisplayMessage( string message){
            Console.WriteLine( message);
        }

        public int Run(){

            DisplayMessage( "done");            
            return 0;
        }
    }
}
