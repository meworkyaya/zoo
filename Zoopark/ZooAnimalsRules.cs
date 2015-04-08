using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DbBest.ZooPark
{
    /// <summary>
    /// type: type of animal
    /// </summary>
    public class ZooAnimalsRules : ItemWithType
    {
        public int CantLiveWith { get; set; }
        public int CanEatFood_1 { get; set; }
        public int CanEatFood_2 { get; set; }

        public ZooAnimalsRules(int type, int cantLiveWith, int canEatFood_1, int canEatFood_2) : base( type )
        {
            CantLiveWith = cantLiveWith;
            CanEatFood_1 = canEatFood_1;
            CanEatFood_2 = canEatFood_2;
        }

    }
}
