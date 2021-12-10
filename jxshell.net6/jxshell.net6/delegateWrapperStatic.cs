using System;
using System.Runtime.InteropServices;

namespace jxshell.net6
{
	[ComVisible(true)]
	public class delegateWrapperStatic : wrapperStatic
	{
		public delegateWrapperStatic(Type t, typeDescriptor td) : base(t, td)
		{
		}

		public object construct(object target, string method)
		{
			delegateWrapper thisWrapper = this.getThisWrapper();
			Delegate @delegate = Delegate.CreateDelegate(this.wrappedType, thisWrapper, "__internalInvoke");
			object obj = @delegate;
			thisWrapper.wrappedObject = @delegate;
			//Delegate delegate = (Delegate)obj;
			thisWrapper.wrappedType = this.wrappedType;
			thisWrapper.typeD = this.typeD;
			thisWrapper.__internalMethod = method;
			thisWrapper.__internalTarget = target;
			return thisWrapper;
		}

		public virtual delegateWrapper getThisWrapper()
		{
			return null;
		}
	}
}