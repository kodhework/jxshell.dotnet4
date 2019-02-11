using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.InteropServices;

namespace jxshell.dotnet4
{
	[ComVisible(true)]
	public class methodDescriptor : memberDescriptor
	{
		public List<MethodBase> baseMethods = new List<MethodBase>();

		public string name;

		public int maxParameterCount;

		public int methodOrder;

		public bool isGenericMethod;

		public int genericParameterCount;

		public methodDescriptor()
		{
		}

		public ConstructorInfo getConstructorForParameters(ref object[] parameters)
		{
			memberDescriptor.convertParameters(ref parameters);
			List<MethodBase> list = new List<MethodBase>(0);
			bool flag = false;
			foreach (MethodBase baseMethod in this.baseMethods)
			{
				ParameterInfo[] parameters2 = baseMethod.GetParameters();
				if ((int)parameters2.Length != (int)parameters.Length)
				{
					continue;
				}
				bool flag2 = true;
				bool flag3 = false;
				for (int i = 0; i < (int)parameters2.Length; i++)
				{
					ParameterInfo parameterInfo = parameters2[i];
					if (parameters[i] == null)
					{
						flag3 = true;
					}
					else if (!parameterInfo.ParameterType.IsAssignableFrom(parameters[i].GetType()))
					{
						flag2 = false;
					}
				}
				if (!flag2)
				{
					continue;
				}
				list.Add(baseMethod);
				flag = (flag ? true : flag3 & flag2);
			}
			if (flag && list.Count > 0 || list.Count == 0)
			{
				throw new Exception("No se puede determinar la mejor coincidencia para la ejecución del método.");
			}
			return (ConstructorInfo)list[0];
		}

		public MethodBase getGenericMethodForParameters(Type[] arguments, ref object[] parameters)
		{
			return this.getMethodForParameters(ref parameters, arguments);
		}

        public object invokeMetavalueForParameters(object[] args , object target)
        {
            object m0 = getMethodForParameters(ref args, null);
            if (m0 is MethodInfo)
            {
                var m1 = (MethodInfo)m0;
                var name = m1.Name;
                var invoker1 = new invoker();
                if (m1.ReturnType == typeof(void))
                {
                    invoker1.invokeMethodVoid(target, name, args);
                    return null;
                }
                var value = invoker1.invokeMethod(target, name, args);
                if (null == value)
                    return null;
                return metaObject.getFromObject(value, target);
            }
            else
            {
                var c1 = (ConstructorInfo)m0;
                var value= c1.Invoke(args);
                if (null == value)
                    return null;
                return metaObject.getFromObject(value, target);
            }
        }

        public object invokeMetavalueGenericForParameters(Type[] gargs, object[] args, object target)
        {
            var m1 = (MethodInfo)getMethodForParameters(ref args, gargs);
            var name = m1.Name;
            var invoker1 = new invoker();
            if (m1.ReturnType == typeof(void))
            {
                invoker1.invokeMethodVoid(target, name, args);
                return null;
            }
            var value = invoker1.invokeMethod(target, name, args);
            if (null == value)
                return null;
            return metaObject.getFromObject(value, target);
        }



        public MethodBase getMethodForParameters(ref object[] parameters, Type[] arguments = null)
		{
			memberDescriptor.convertParameters(ref parameters);
			List<MethodBase> list = new List<MethodBase>(0);
			bool flag = false;
			for (int i = 0; i < this.baseMethods.Count; i++)
			{
				MethodBase methodBase = this.baseMethods[i];
				bool flag2 = true;
				if (arguments != null)
				{
					if ((int)methodBase.GetGenericArguments().Length == (int)arguments.Length)
					{
						methodBase = ((MethodInfo)methodBase).MakeGenericMethod(arguments);
					}
					else
					{
						flag2 = false;
					}
				}
				if (flag2)
				{
					ParameterInfo[] parameters2 = methodBase.GetParameters();
					if ((int)parameters2.Length == (int)parameters.Length)
					{
						bool flag3 = true;
						bool flag4 = false;
						for (int j = 0; j < (int)parameters2.Length; j++)
						{
							ParameterInfo parameterInfo = parameters2[j];
							if (parameters[j] == null)
							{
								flag4 = true;
							}
							else
							{
								Type type = parameters[j].GetType();
								if (!parameterInfo.ParameterType.IsAssignableFrom(type))
								{
									flag3 = false;
								}
							}
						}
						if (flag3)
						{
							list.Add(methodBase);
							i = 99999;
							flag = (flag ? true : flag4 & flag3);
						}
					}
				}
			}
			if (flag && list.Count > 1 || list.Count == 0)
			{
				throw new Exception("No se puede determinar la mejor coincidencia para la ejecución del método.");
			}
			return list[0];
		}
	}
}