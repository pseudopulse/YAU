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
    internal class EnumExtender<T> where T : Enum {
        private static int mult = 100;

        public static T Extend() {
            System.Array values = Enum.GetValues(typeof(T));
            Object val = values.GetValue(values.Length - 1);
            int toCast = -1;
            mult++;
            Debug.Log(Enum.GetUnderlyingType(typeof(T)));
            if (Enum.GetUnderlyingType(typeof(T)) == typeof(Int32)) {
                int i = (int)val;
                i += mult;
                return (T)(object)(i);
            }
            else if (Enum.GetUnderlyingType(typeof(T)) == typeof(UInt32)) {
                uint u = (uint)val;
                u *= 2;
                return (T)(object)(u);
            }
            else {
                return (T)(object)toCast;
            }
        }
    }

    public static class EnumExtension {
        public static T Extend<T>() where T : Enum {
            return EnumExtender<T>.Extend();
        }
    }
}