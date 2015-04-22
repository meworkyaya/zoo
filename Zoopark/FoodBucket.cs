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
        public static int FoodPerBucket = 2;

        public int BucketsAmount { get; set; }  // amount of items of such bucket
        public bool IsTaked { get; set; }       // does bucket is used or not

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

        public void SetAmount_1(int amount){
            _amountFood_1 = amount;
        }
        public void SetAmount_2(int amount)
        {
            _amountFood_2 = amount;
        }

        public int TypeFood_1 { get; set; }
        public int TypeFood_2 { get; set; }

        public FoodBucket(int type_1, int type_2)
        {
            TypeFood_1 = type_1;
            TypeFood_2 = type_2;
        }

        public int fillFood(int type, int amount)
        {
            if (TypeFood_1 == type)
            {
                return FillFoodType_1(amount);
            }
            else if (TypeFood_2 == type)
            {
                return FillFoodType_2(amount);
            }
            else
            {
                throw new Exception("wrong type of food: dotn have such type in bucket");
            }
        }


        protected int FillValue(ref int filled, int amount, int CanUse )
        {
            if (amount <= 0)
            {
                throw new Exception("amount must be > 0");
            }

            // int CanUse = maxUsed - filled;
            if (amount < CanUse)
            {
                filled += amount;
                return 0;
            }
            else
            {
                filled += CanUse;
                return (amount - CanUse);
            }
        }


        /// <summary>
        /// use food of type with amount
        /// </summary>
        /// <param name="amount"></param>
        /// <returns>amount of food that left from input after filling</returns>
        public int FillFoodType_1(int amount)
        {
            return FillValue(ref _amountFood_1, amount, FoodBucket.FoodPerBucket * BucketsAmount - _amountFood_1);
        }

        /// <summary>
        /// use food of type with amount
        /// </summary>
        /// <param name="amount"></param>
        /// <returns>amount of food that left from input after filling</returns>
        public int FillFoodType_2(int amount)
        {
            return FillValue(ref _amountFood_2, amount, FoodBucket.FoodPerBucket * BucketsAmount - _amountFood_2);
        }


        public int NeedFood()
        {
            int CanHave = FoodBucket.FoodPerBucket * BucketsAmount - TotalAmountFood;
            return CanHave;
        }


        public int NeedFoodType_1()
        {
            int CanHave = FoodBucket.FoodPerBucket * BucketsAmount - AmountFood_2;
            return ( CanHave - AmountFood_1 );
        }
        public int NeedFoodType_2()
        {
            int CanHave = FoodBucket.FoodPerBucket * BucketsAmount - AmountFood_1;
            return (CanHave - AmountFood_2);
        }


        public int PushFood_1(int amount)
        {
            return FillValue(ref _amountFood_1, amount, FoodBucket.FoodPerBucket * BucketsAmount - TotalAmountFood);
        }
        public int PushFood_2(int amount)
        {
            return FillValue(ref _amountFood_2, amount, FoodBucket.FoodPerBucket * BucketsAmount - TotalAmountFood);
        }


        protected int ExchangeInner(int amount, ref int from, ref int too)
        {
            int changed = 0;
            if (amount <= from)
            {
                changed = amount;
            } else {
                changed = from;
            }

            from -= changed;
            too += changed;
            return changed;
        }


        public int ExchangeFood(int amount, int typeToGet)
        {
            if (amount <= 0)
            {
                throw new Exception("amount must be > 0");
            }

            if (typeToGet == TypeFood_1)
            {
                return ExchangeInner(amount, ref _amountFood_2, ref _amountFood_1);
            }
            else if (typeToGet == TypeFood_2)
            {
                return ExchangeInner(amount, ref _amountFood_1, ref _amountFood_2);
            }
            else
            {
                throw new Exception("wrong type of FoodBucket");
            }
        }

        
    }
}
