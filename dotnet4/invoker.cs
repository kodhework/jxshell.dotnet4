using System;
using System.Runtime.InteropServices;

namespace jxshell.dotnet4
{
	[ComVisible(true)]
	public class invoker
	{
		public object executer;

		protected invokerparam0[] invokerparams0;

		protected invokerparam1[] invokerparams1;

		protected invokerparam2[] invokerparams2;

		protected invokerparam3[] invokerparams3;

		protected invokerparam4[] invokerparams4;

		protected invokerparam5[] invokerparams5;

		protected invokerparam6[] invokerparams6;

		protected invokerparam7[] invokerparams7;

		protected invokerparam8[] invokerparams8;

		protected invokerparam9[] invokerparams9;

		public invoker()
		{
		}

		public virtual invoker __construct(object o)
		{
			return null;
		}

		public invokerparam0 __getParam0(int index)
		{
			return this.__getParam0(index, false);
		}

		public invokerparam0 __getParam0(int index, bool p)
		{
			if (this.invokerparams0 == null)
			{
				this.invokerparams0 = new invokerparam0[(int)this.IMethods().Length];
			}
			invokerparam0 invokerparam = this.invokerparams0[index];
			if (invokerparam == null)
			{
				invokerparam = new invokerparam0(this.IMethods()[index], p);
				this.invokerparams0[index] = invokerparam;
			}
			return invokerparam;
		}

		public invokerparam1 __getParam1(int index)
		{
			return this.__getParam1(index, false);
		}

		public invokerparam1 __getParam1(int index, bool p)
		{
			if (this.invokerparams1 == null)
			{
				this.invokerparams1 = new invokerparam1[(int)this.IMethods().Length];
			}
			invokerparam1 invokerparam = this.invokerparams1[index];
			if (invokerparam == null)
			{
				invokerparam = new invokerparam1(this.IMethods()[index], p);
				this.invokerparams1[index] = invokerparam;
			}
			return invokerparam;
		}

		public invokerparam2 __getParam2(int index)
		{
			return this.__getParam2(index, false);
		}

		public invokerparam2 __getParam2(int index, bool p)
		{
			if (this.invokerparams2 == null)
			{
				this.invokerparams2 = new invokerparam2[(int)this.IMethods().Length];
			}
			invokerparam2 invokerparam = this.invokerparams2[index];
			if (invokerparam == null)
			{
				invokerparam = new invokerparam2(this.IMethods()[index], p);
				this.invokerparams2[index] = invokerparam;
			}
			return invokerparam;
		}

		public invokerparam3 __getParam3(int index)
		{
			return this.__getParam3(index, false);
		}

		public invokerparam3 __getParam3(int index, bool p)
		{
			if (this.invokerparams3 == null)
			{
				this.invokerparams3 = new invokerparam3[(int)this.IMethods().Length];
			}
			invokerparam3 invokerparam = this.invokerparams3[index];
			if (invokerparam == null)
			{
				invokerparam = new invokerparam3(this.IMethods()[index], p);
				this.invokerparams3[index] = invokerparam;
			}
			return invokerparam;
		}

		public invokerparam4 __getParam4(int index)
		{
			return this.__getParam4(index, false);
		}

		public invokerparam4 __getParam4(int index, bool p)
		{
			if (this.invokerparams4 == null)
			{
				this.invokerparams4 = new invokerparam4[(int)this.IMethods().Length];
			}
			invokerparam4 invokerparam = this.invokerparams4[index];
			if (invokerparam == null)
			{
				invokerparam = new invokerparam4(this.IMethods()[index], p);
				this.invokerparams4[index] = invokerparam;
			}
			return invokerparam;
		}

		public invokerparam5 __getParam5(int index)
		{
			return this.__getParam5(index, false);
		}

		public invokerparam5 __getParam5(int index, bool p)
		{
			if (this.invokerparams5 == null)
			{
				this.invokerparams5 = new invokerparam5[(int)this.IMethods().Length];
			}
			invokerparam5 invokerparam = this.invokerparams5[index];
			if (invokerparam == null)
			{
				invokerparam = new invokerparam5(this.IMethods()[index], p);
				this.invokerparams5[index] = invokerparam;
			}
			return invokerparam;
		}

		public invokerparam6 __getParam6(int index)
		{
			return this.__getParam6(index, false);
		}

		public invokerparam6 __getParam6(int index, bool p)
		{
			if (this.invokerparams6 == null)
			{
				this.invokerparams6 = new invokerparam6[(int)this.IMethods().Length];
			}
			invokerparam6 invokerparam = this.invokerparams6[index];
			if (invokerparam == null)
			{
				invokerparam = new invokerparam6(this.IMethods()[index], p);
				this.invokerparams6[index] = invokerparam;
			}
			return invokerparam;
		}

		public invokerparam7 __getParam7(int index)
		{
			return this.__getParam7(index, false);
		}

		public invokerparam7 __getParam7(int index, bool p)
		{
			if (this.invokerparams7 == null)
			{
				this.invokerparams7 = new invokerparam7[(int)this.IMethods().Length];
			}
			invokerparam7 invokerparam = this.invokerparams7[index];
			if (invokerparam == null)
			{
				invokerparam = new invokerparam7(this.IMethods()[index], p);
				this.invokerparams7[index] = invokerparam;
			}
			return invokerparam;
		}

		public invokerparam8 __getParam8(int index)
		{
			return this.__getParam8(index, false);
		}

		public invokerparam8 __getParam8(int index, bool p)
		{
			if (this.invokerparams8 == null)
			{
				this.invokerparams8 = new invokerparam8[(int)this.IMethods().Length];
			}
			invokerparam8 invokerparam = this.invokerparams8[index];
			if (invokerparam == null)
			{
				invokerparam = new invokerparam8(this.IMethods()[index], p);
				this.invokerparams8[index] = invokerparam;
			}
			return invokerparam;
		}

		public invokerparam9 __getParam9(int index)
		{
			return this.__getParam9(index, false);
		}

		public invokerparam9 __getParam9(int index, bool p)
		{
			if (this.invokerparams9 == null)
			{
				this.invokerparams9 = new invokerparam9[(int)this.IMethods().Length];
			}
			invokerparam9 invokerparam = this.invokerparams9[index];
			if (invokerparam == null)
			{
				invokerparam = new invokerparam9(this.IMethods()[index], p);
				this.invokerparams9[index] = invokerparam;
			}
			return invokerparam;
		}

		public virtual string[] IMethods()
		{
			return null;
		}
	}
}