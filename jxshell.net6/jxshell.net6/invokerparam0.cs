using Microsoft.CSharp.RuntimeBinder;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace jxshell.net6
{
	public class invokerparam0 : invokerparam
	{
		private string method = "";

		protected CallSite<Func<CallSite, object, object>> invoker;

		protected CallSite<Func<CallSite, object, object, object>> invoker_p;

		protected CallSite<Func<CallSite, object, object>> invoker_v;

		private bool isProperty;

		public invokerparam0(string met)
		{
			this.method = met;
		}

		public invokerparam0(string met, bool isProperty)
		{
			this.method = met;
			this.isProperty = isProperty;
		}

		private void ensureInvoker()
		{
			if (this.invoker == null)
			{
				if (!this.isProperty)
				{
					this.invoker = CallSite<Func<CallSite, object, object>>.Create(Binder.InvokeMember(CSharpBinderFlags.None, this.method, null, typeof(jxshell.net6.invoker), (IEnumerable<CSharpArgumentInfo>)(new CSharpArgumentInfo[] { CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null) })));
					return;
				}
				this.invoker = CallSite<Func<CallSite, object, object>>.Create(Binder.GetMember(CSharpBinderFlags.None, this.method, typeof(jxshell.net6.invoker), (IEnumerable<CSharpArgumentInfo>)(new CSharpArgumentInfo[] { CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null) })));
			}
		}

		private void ensureInvokerP()
		{
			if (this.invoker_p == null)
			{
				this.invoker_p = CallSite<Func<CallSite, object, object, object>>.Create(Binder.SetMember(CSharpBinderFlags.None, this.method, typeof(jxshell.net6.invoker), (IEnumerable<CSharpArgumentInfo>)(new CSharpArgumentInfo[] { CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null), CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null), CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null) })));
			}
		}

		private void ensureInvokerVoid()
		{
			if (this.invoker_v == null)
			{
				this.invoker_v = CallSite<Func<CallSite, object, object>>.Create(Binder.InvokeMember(CSharpBinderFlags.ResultDiscarded, this.method, null, typeof(jxshell.net6.invoker), (IEnumerable<CSharpArgumentInfo>)(new CSharpArgumentInfo[] { CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null) })));
			}
		}

		public object invoke(object obj)
		{
			this.ensureInvoker();
			object obj2 = this.invoker.Target(this.invoker, obj);
			if (obj2 == null)
			{
				return null;
			}
			return obj2;
		}

		public void invokeasVoid(object obj)
		{
			this.ensureInvokerVoid();
			this.invoker_v.Target(this.invoker_v, obj);
		}

		public object setProperty(object obj, object value)
		{
			this.ensureInvokerP();
			return this.invoker_p.Target(this.invoker_p, obj, value);
		}
	}
}