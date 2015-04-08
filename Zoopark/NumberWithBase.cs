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

        protected int min = 2;
        protected int max = 100;


        public NumberWithBase(int baseOfNumber, int length)
        {

            if ( (baseOfNumber < min) || (baseOfNumber >  max))
            {
                throw new Exception("baseOfNumber must be in range " + min +  " and " + max );
            }

            _base = baseOfNumber;
            bits = new int[length];
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
                    break;
                }
                else
                {
                    bits[i] = _base - 1;
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
                throw new Exception("value must be in range " + min + " and " + (_base - 1));
            }

            if (index >= bits.Length)
            {
                throw new Exception("index must be in range 0 and " + (bits.Length - 1));
            }

            bits[index] = value;
        }

    }
}
