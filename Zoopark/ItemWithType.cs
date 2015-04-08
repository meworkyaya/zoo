﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DbBest.ZooPark
{
    /// <summary>
    /// describe class that has type or id, and possible can have maximum range of types
    /// </summary>
    public class ItemWithType
    {
        private int _type;
        private int _maxType;

        public static int _maxTypeNotUsed = -1;

        public int Type
        {
            get
            {
                return _type;
            }
            set
            {
                if (!CheckTypeRange(value, MaxType))
                {
                    throw new Exception("new Type is bigger than MaxType");
                }

                _type = value;
            }
        }

        public int MaxType
        {
            get
            {
                return _maxType;
            }
            set
            {
                if (!CheckTypeRange( _type, value))
                {
                    throw new Exception("new MaxTyoe is smaller than curretn Type");
                }

                _maxType = value;
            }
        }

        protected bool CheckTypeRange(int ItemType, int ItemMaxType)
        {
            if (ItemMaxType != ItemWithType._maxTypeNotUsed)
            {
                return (ItemType <= MaxType);
            }
            return true;
        }


        public ItemWithType()
        {
            _type = 0;
            _maxType = ItemWithType._maxTypeNotUsed;
        }

        public ItemWithType(int ItemType, int ItemMaxType = -1)
        {
            if (!CheckTypeRange(ItemType, ItemMaxType))
            {
                throw new Exception("ItemType is bigger than MaxType");
            }

            _type = ItemType;
            _maxType = ItemMaxType;            
        }
    }
}
