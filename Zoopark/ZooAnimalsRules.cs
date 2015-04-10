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
        public int CanEatFood_1 { get; set; }
        public int CanEatFood_2 { get; set; }

        public ZooAnimalsRules(int type, int canEatFood_1, int canEatFood_2) : base( type )
        {
            CanEatFood_1 = canEatFood_1;
            CanEatFood_2 = canEatFood_2;
        }


        public string DisplayRule()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("type: {0} | Food 1: \t{1} | Food 2: \t{2}", Type, CanEatFood_1, CanEatFood_2);
            return sb.ToString();
        }

    }
}
