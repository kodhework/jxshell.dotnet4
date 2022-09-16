using System;
using System.Reflection;

namespace jxshell.net6
{
	public class memberDescriptor
	{
		public memberDescriptor()
		{
		}

		public static void convertParameters(ref object[] pars)
		{
			int num = -1;
			object[] objArray = pars;
			for (int i = 0; i < (int)objArray.Length; i++)
			{
				object obj = objArray[i];
				if (!(obj is Missing))
				{
					num++;
					if (obj is wrapperBase)
					{
						pars[num] = ((wrapperBase)obj).wrappedObject;
					}
					else if (obj is DBNull)
					{
						pars[num] = null;
					}
				}
			}
			if (num + 1 != (int)pars.Length)
			{
				Array.Resize<object>(ref pars, num + 1);
			}
		}
	}
}