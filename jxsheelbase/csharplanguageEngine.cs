using System;

namespace jxshell
{
	public class csharplanguageEngine : languageEngine
	{
		public csharplanguageEngine()
		{
		}

		public override language create()
		{
			return new csharplanguage();
		}
	}
}