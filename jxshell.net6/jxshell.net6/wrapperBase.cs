using System;
using System.Runtime.InteropServices;

namespace jxshell.net6
{
	[ComVisible(true)]
	public class wrapperBase
	{
		public Type wrappedType;

		public object wrappedObject;

		public typeDescriptor typeD;

		public wrapperBase()
		{
		}

		public virtual object __getProperty(string property, params object[] args)
		{
			return null;
		}

		public virtual object __invokeMethod(string method, params object[] args)
		{
			return null;
		}

		public virtual void __setProperty(string property, string value, params object[] args)
		{
		}
	}
}