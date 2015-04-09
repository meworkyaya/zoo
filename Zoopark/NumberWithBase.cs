using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DbBest.ZooPark
{
    /// <summary>
    /// class allows use of number with any base; 
    /// </summary>
    public class NumberWithBase
    {
        protected int _base;
        protected int _length;

        protected int[] bits;

        public static readonly int MinBaseOfNumber = 2;
        public static readonly int MaxBaseOfNumber = 1000;


        /// <summary>
        /// 
        /// </summary>
        /// <param name="baseOfNumber"></param>
        /// <param name="numberOfDigits"></param>
        public NumberWithBase(int baseOfNumber, int numberOfDigits)
        {
            if ( (baseOfNumber < MinBaseOfNumber) || (baseOfNumber >  MaxBaseOfNumber))
            {
                throw new Exception("baseOfNumber must be in range " + MinBaseOfNumber +  " and " + MaxBaseOfNumber );
            }

            _base = baseOfNumber;
            bits = new int[numberOfDigits];
        }


        /// <summary>
        /// increment current number
        /// </summary>
        public void inc(){
            int i = 0;
            for (i = 0; i < bits.Length; i++)
            {
                bits[i] += 1;

                if (bits[i] < _base)
                {
                    return;
                }
                else
                {
                    bits[i] = 0;
                }
            }

            if (i == bits.Length)
            {
                throw new Exception("overflow happen");
            }

        }

        public int getBit(int index){
            return bits[index];
        }

        public void setBit(int index, int value)
        {
            if (value >= _base)
            {
                throw new Exception("value must be in range " + MinBaseOfNumber + " and " + (_base - 1));
            }

            if (index >= bits.Length)
            {
                throw new Exception("index must be in range 0 and " + (bits.Length - 1));
            }

            bits[index] = value;
        }


        public string GetBitsString()
        {
            StringBuilder sb = new StringBuilder();
            for (int i = bits.Length - 1; i >= 0; i-- ) {
                sb.AppendFormat("{0} ", bits[i]);
            }
            return sb.ToString();
        }

    }
}
