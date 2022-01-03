using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace jxshell.net6
{
    [ComVisible(true)]
    public class metaObject
    {
        public bool isstatic;
        public bool isthis;
        public typeDescriptor typeDescriptor;
        public object value;
        public string name;

        public static object getFromObject(object o, object self = null)
        {
            if (o is wrapper)
            {
                return o;
            }
            if (o == null || o is DBNull)
            {
                return null;
            }
            if (o is long)
            {
                return (double)((long)o);
            }
            if (o.GetType().IsPrimitive || o is string)
            {
                return o;
            }

            var m = new metaObject();
            m.value = o;
            Type t = o.GetType();
            string name = typeDescriptor.getNameForType(t);
            m.name = name;
            m.isstatic = false;
            m.isthis = o == self;
            return m;
        }
    }
}
