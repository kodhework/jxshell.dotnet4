using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;

namespace jxshell.net6
{
	[ComVisible(true)]
	public class propertyDescriptor : memberDescriptor
	{
		public List<PropertyInfo> properties = new List<PropertyInfo>();

		public string name;
		public int maxParameterCount;
		public int propertyOrder;
		public propertyDescriptor()
		{
		}

        public object getPropertyMetavalueForParameters(object[] args, object target)
        {
            var p = getPropertyForParameters(ref args);
            var name = p.Name;
            var invoker1 = new invoker();
            var value = invoker1.invokeProperty(target, name, args);
            return metaObject.getFromObject(value, target);

        }

        public object setPropertyMetavalueForParameters(object[] args, object target, object value)
        {
            var p = getPropertyForParameters(ref args);
            var name = p.Name;
            var invoker1 = new invoker();
            invoker1.invokePropertySet(target, name, value, args);
            return null;

        }


        public PropertyInfo getPropertyForParameters(ref object[] parameters)
		{
			memberDescriptor.convertParameters(ref parameters);
			List<PropertyInfo> list = new List<PropertyInfo>(0);
			bool flag = false;
			StringBuilder stringBuilder = new StringBuilder();
			foreach (PropertyInfo property in this.properties)
			{
				stringBuilder.Length = 0;
				ParameterInfo[] indexParameters = property.GetIndexParameters();
				if ((int)indexParameters.Length != (int)parameters.Length)
				{
					continue;
				}
				bool flag2 = true;
				bool flag3 = false;
				for (int i = 0; i < (int)indexParameters.Length; i++)
				{
					ParameterInfo parameterInfo = indexParameters[i];
					if (parameters[i] == null)
					{
						flag3 = true;
					}
					else if (!parameterInfo.ParameterType.IsAssignableFrom(parameters[i].GetType()))
					{
						stringBuilder.Append(",").Append(parameterInfo.ParameterType.ToString());
						flag2 = false;
					}
				}
				if (!flag2)
				{
					continue;
				}
				list.Add(property);
				flag = (flag ? true : flag3 & flag2);
			}
			if (flag && list.Count > 0 || list.Count == 0)
			{
				string str = "";
				if (stringBuilder.Length > 0)
				{
					str = string.Concat("Una de las sobrecargas admite estos tipos es: ", stringBuilder.ToString());
				}
				throw new Exception(string.Concat("No se puede determinar la mejor coincidencia para la ejecución de la propiedad. ", str));
			}
			return list[0];
		}
	}
}