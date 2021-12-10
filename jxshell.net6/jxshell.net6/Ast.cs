using System;
using System.Runtime.InteropServices;

namespace jxshell.net6
{
	[ComVisible(true)]
	public class Ast
	{
		public string code;

		public string staticName;

		public string instanceName;

		public Ast()
		{
		}
	}
}