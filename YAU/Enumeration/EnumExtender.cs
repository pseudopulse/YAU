using RoR2;
using System.Collections.Generic;
using YAU;
using System.Reflection;
using System;
using System.Linq;
using YAU.Enumeration;
using UnityEngine;
using Object = System.Object;

namespace YAU.Enumeration {
    public class EnumExtender<T> where T : Enum
    {
        private static int current = -1;
        
        static EnumExtender()
        {
            if (typeof(T).IsEnum)
            {
                var underlyingType = Enum.GetUnderlyingType(typeof(T));
                if (underlyingType == typeof(int))
                {
                    current = -1;
                    foreach (var value in Enum.GetValues(typeof(T)))
                    {
                        int intValue = Convert.ToInt32(value);
                        if (intValue > current)
                        {
                            current = intValue;
                        }
                    }
                    current += 5;
                }
                else if (underlyingType == typeof(uint))
                {
                    current = 0;
                    foreach (var value in Enum.GetValues(typeof(T)))
                    {
                        uint uintValue = Convert.ToUInt32(value);
                        if (uintValue > current)
                        {
                            current = (int) (uintValue * 2);
                        }
                    }
                }
                else
                {
                    current = -1;
                    foreach (var value in Enum.GetValues(typeof(T)))
                    {
                        int intValue = Convert.ToInt32(value);
                        if (intValue > current)
                        {
                            current = intValue;
                        }
                    }
                    current += 5;
                }
            }
        }

        public static T Extend()
        {
            T result = (T) Enum.ToObject(typeof(T), current);
            current = (Enum.GetUnderlyingType(typeof(T)) == typeof(uint)) ? current * 2 : current + 1;
            return result;
        }
    }

    public static class EnumExtension {
        public static T Extend<T>() where T : Enum {
            return EnumExtender<T>.Extend();
        }
    }
}