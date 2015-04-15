using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DbBest.ZooPark
{
    /// <summary>
    /// describe bucket of food with food_1 and food_2 types
    /// </summary>
    public class FoodBucket
    {
        public int BucketsAmount { get; set; }  // amount of items of such bucket

        public int TotalAmountFood { get; set; }
        public int AmountFood_1 { get; set; }
        public int AmountFood_2 { get; set; }

        public int TypeFood_1 { get; set; }
        public int TypeFood_2 { get; set; }

        public FoodBucket( int type_1, int type_2){
            TypeFood_1 = type_1;
            TypeFood_2 = type_2;
        }
    }
}
