using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DbBest.ZooPark
{
    public class Animal : ItemWithType
    {
        private int _food_1;
        private int _food_2;

        public bool isFeeded
        {
            get
            {
                return (_food_1 >= 0 && _food_2 > 0);
            }
        }

        public int Food_1
        {
            get
            {
                return _food_1;
            }
            set
            {
                _food_1 = value;
            }
        }
        public int Food_2
        {
            get
            {
                return _food_2;
            }
            set
            {
                _food_2 = value;
            }
        }

        public Animal()
        {
            _food_1 = -1;
            _food_2 = -1;
        }

    }
}
