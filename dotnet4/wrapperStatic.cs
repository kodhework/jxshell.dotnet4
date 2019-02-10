using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace jxshell.dotnet4
{
	[ComVisible(true)]
	public class wrapperStatic : wrapperBase
	{
		internal static Dictionary<Type, wrapperStatic> wrappersStatic;

		static wrapperStatic()
		{
			wrapperStatic.wrappersStatic = new Dictionary<Type, wrapperStatic>();
		}

		public wrapperStatic(Type t, typeDescriptor td)
		{
			this.wrappedObject = t;
			this.wrappedType = t;
			this.typeD = td;
		}

		public override object __getProperty(string property, params object[] args)
		{
			object staticProperty = this.typeD.getStaticProperty(null, property, args);
			return wrapper.getFromObject(staticProperty);
		}

		public override object __invokeMethod(string method, params object[] args)
		{
			object o = this.typeD.invokeStaticMethod(null, method, args);
			return wrapper.getFromObject(o);
		}

		public object __process(object o)
		{
			return wrapper.getFromObject(o);
		}

		public override void __setProperty(string property, string value, params object[] args)
		{
			this.typeD.setStaticProperty(null, property, value, args);
		}

		public virtual wrapper getWrapper(object o)
		{
			return new wrapper(o, this.typeD);
		}

		public static wrapperStatic loadFromType(Type t)
		{
			wrapperStatic value;
			if (!wrapperStatic.wrappersStatic.TryGetValue(t, out value))
			{
				value = jxshell.dotnet4.typeDescriptor.loadFromType(t).compile();
				wrapperStatic.wrappersStatic[t] = value;
			}
			return value;
		}
	}
}