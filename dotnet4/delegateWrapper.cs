using System;
using System.Runtime.InteropServices;

namespace jxshell.dotnet4
{
	[ComVisible(true)]
	public class delegateWrapper : wrapper
	{
		public object __internalTarget;

		public string __internalMethod = "";

		public delegateWrapper()
		{
		}

		public delegateWrapper(object o) : base(o)
		{
		}

		public delegateWrapper(object o, typeDescriptor td) : base(o, td)
		{
		}
	}
}