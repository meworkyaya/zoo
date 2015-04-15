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

        public int TotalAmountFood
        {
            get
            {
                return _amountFood_1 + _amountFood_2;
            }
            protected set{}
        }

        private int _amountFood_1;
        private int _amountFood_2;
        public int AmountFood_1 { get { return _amountFood_1; } protected set { _amountFood_1 = value; } }
        public int AmountFood_2 { get { return _amountFood_2; } protected set { _amountFood_2 = value; } }

        public int TypeFood_1 { get; set; }
        public int TypeFood_2 { get; set; }

        public FoodBucket(int type_1, int type_2)
        {
            TypeFood_1 = type_1;
            TypeFood_2 = type_2;
        }

        public int fillFood(int type, int amount)
        {
            if (amount <= 0)
            {
                throw new Exception("amount must be > 0");
            }

            if (TypeFood_1 == type)
            {
                return fillFoodType_1(amount);
            }
            else if (TypeFood_2 == type)
            {
                return fillFoodType_2(amount);
            }
            else
            {
                throw new Exception("wrong type of food: dotn have such type in bucket");
            }
        }


        /// <summary>
        /// use food of type with amount
        /// </summary>
        /// <param name="amount"></param>
        /// <returns>amount of food that left from input after filling</returns>
        public int fillFoodType_1(int amount)
        {
            if (amount <= 0)
            {
                throw new Exception("amount must be > 0");
            }

            int CanUse = BucketsAmount - AmountFood_1;
            if (amount < CanUse)
            {
                AmountFood_1 += amount;
                return 0;
            }
            else
            {
                AmountFood_1 += CanUse;
                return (amount - CanUse);
            }
        }

        /// <summary>
        /// use food of type with amount
        /// </summary>
        /// <param name="amount"></param>
        /// <returns>amount of food that left from input after filling</returns>
        public int fillFoodType_2(int amount)
        {
            if (amount <= 0)
            {
                throw new Exception("amount must be > 0");
            }

            int CanUse = BucketsAmount - AmountFood_2;
            if (amount < CanUse)
            {
                AmountFood_2 += amount;
                return 0;
            }
            else
            {
                AmountFood_2 += CanUse;
                return (amount - CanUse);
            }
        }
    }
}
