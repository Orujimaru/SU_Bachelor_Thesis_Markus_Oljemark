using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Orujin.Core.GameHelp
{
    public static class DynamicArray
    {
        #region Dynamic object array
        public static object[] ObjectArray(object a)
        {
            object[] parameters = new object[1];
            parameters[0] = a;
            return parameters;
        }

        public static object[] ObjectArray(object a, object b)
        {
            object[] parameters = new object[2];
            parameters[0] = a;
            parameters[1] = b;
            return parameters;
        }

        public static object[] ObjectArray(object a, object b, object c)
        {
            object[] parameters = new object[3];
            parameters[0] = a;
            parameters[1] = b;
            parameters[2] = c;
            return parameters;
        }

        public static object[] ObjectArray(object a, object b, object c, object d)
        {
            object[] parameters = new object[4];
            parameters[0] = a;
            parameters[1] = b;
            parameters[2] = c;
            parameters[3] = d;
            return parameters;
        }

        public static object[] ObjectArray(object a, object b, object c, object d, object e)
        {
            object[] parameters = new object[5];
            parameters[0] = a;
            parameters[1] = b;
            parameters[2] = c;
            parameters[3] = d;
            parameters[4] = e;
            return parameters;
        }

        public static object[] ObjectArray(object a, object b, object c, object d, object e, object f)
        {
            object[] parameters = new object[6];
            parameters[0] = a;
            parameters[1] = b;
            parameters[2] = c;
            parameters[3] = d;
            parameters[4] = e;
            parameters[5] = f;
            return parameters;
        }

        public static object[] ObjectArray(object a, object b, object c, object d, object e, object f, object g)
        {
            object[] parameters = new object[7];
            parameters[0] = a;
            parameters[1] = b;
            parameters[2] = c;
            parameters[3] = d;
            parameters[4] = e;
            parameters[5] = f;
            parameters[6] = g;
            return parameters;
        }

        public static object[] ObjectArray(object a, object b, object c, object d, object e, object f, object g, object h)
        {
            object[] parameters = new object[8];
            parameters[0] = a;
            parameters[1] = b;
            parameters[2] = c;
            parameters[3] = d;
            parameters[4] = e;
            parameters[5] = f;
            parameters[6] = g;
            parameters[7] = h;
            return parameters;
        }

        public static object[] ObjectArray(object a, object b, object c, object d, object e, object f, object g, object h, object i)
        {
            object[] parameters = new object[9];
            parameters[0] = a;
            parameters[1] = b;
            parameters[2] = c;
            parameters[3] = d;
            parameters[4] = e;
            parameters[5] = f;
            parameters[6] = g;
            parameters[7] = h;
            parameters[8] = i;
            return parameters;
        }

        public static object[] ObjectArray(object[] array, object a)
        {
            object[] parameters = new object[array.Count() + 1];
            for (int x = 0; x < array.Count(); x++)
            {
                parameters[x] = array[x];
            }
            parameters[array.Count()] = a;
            return parameters;
        }
        #endregion Dynamic object array
    }
}
