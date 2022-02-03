using System;
using System.Collections.Generic;
using System.Reflection;

namespace jxshell
{
	public class language
	{
		public static Dictionary<string, languageEngine> languages;

		public static languageEngine defaultLanguage;

		public virtual string languageName
		{
			get
			{
				return "c#";
			}
		}

		static language()
		{
			language.languages = new Dictionary<string, languageEngine>();
			language.defaultLanguage = new csharplanguageEngine();
			language.languages["c#"] = language.defaultLanguage;
			language.languages["csharp"] = language.defaultLanguage;
		}

		public language()
		{
		}

		public virtual Assembly getCompiledAssembly()
		{
			return null;
		}

		public virtual void loadClass(string file)
		{
		}

		public virtual void runFile(string file)
		{
		}

		public virtual void runScript(string script)
		{
		}
		
		public virtual void runScriptWithId(string script, string id)
		{
		}
	}
}