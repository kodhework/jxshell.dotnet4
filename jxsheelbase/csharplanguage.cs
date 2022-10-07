using System.Security.Cryptography;
using Microsoft.CSharp;
using System;
using System.CodeDom.Compiler;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Reflection;
using System.Text;

namespace jxshell
{
	public class csharplanguage : language
	{
		private CSharpCodeProvider cp;

		private CompilerParameters p = new CompilerParameters();

		public Assembly compiled = null;

		private string sourceDefault = "";

		private static Dictionary<string, int> compilations;

		private static Dictionary<string, Assembly> compileds;

		public override string languageName
		{
			get
			{
				return "c#";
			}
		}

		static csharplanguage()
		{
			csharplanguage.compilations = new Dictionary<string, int>(0);
			csharplanguage.compileds = new Dictionary<string, Assembly>(0);
		}

		public csharplanguage()
		{
			this.cp = new CSharpCodeProvider();
			this.p.GenerateInMemory = false;
			this.p.GenerateExecutable = false;
		}

		public void compileString(string script, string file)
		{
			string[] location = new string[environment.assemblies.Count];
			for (int i = 0; i < environment.assemblies.Count; i++)
			{
				if (environment.assemblies[i] == null)
				{
					throw new Exception("No se pudo cargar uno o más ensamblados.");
				}
				location[i] = environment.assemblies[i].Location;
			}
			this.p.TreatWarningsAsErrors = false;
			if (file != "")
			{
				this.p.OutputAssembly = file;
			}
			this.p.ReferencedAssemblies.AddRange(location);
			string str = string.Concat(Path.GetTempPath(), environment.uniqueId(), ".cs");
			FileStream fileStream = new FileStream(str, FileMode.OpenOrCreate, FileAccess.Write);
			StreamWriter streamWriter = new StreamWriter(fileStream);
			streamWriter.Write(script);
			streamWriter.Close();
			fileStream.Close();
			string str1 = string.Concat(Path.GetTempPath(), environment.uniqueId(), ".cs");
			fileStream = new FileStream(str1, FileMode.OpenOrCreate, FileAccess.Write);
			streamWriter = new StreamWriter(fileStream);
			streamWriter.Write(this.sourceDefault);
			streamWriter.Close();
			fileStream.Close();
			lock (this.cp)
			{
				CompilerResults compilerResult = this.cp.CompileAssemblyFromFile(this.p, new string[] { str, str1 });
				if (compilerResult.Errors.Count > 0)
				{
					StringBuilder stringBuilder = new StringBuilder();
					if (!compilerResult.Errors[0].IsWarning)
					{
						stringBuilder.Append(compilerResult.Errors[0].ErrorText);
					}
					for (int j = 1; j < compilerResult.Errors.Count; j++)
					{
						if (!compilerResult.Errors[j].IsWarning)
						{
							stringBuilder.AppendLine();
							stringBuilder.Append(compilerResult.Errors[j].ErrorText);
						}
					}
					if (stringBuilder.Length > 0)
					{
						throw new Exception(stringBuilder.ToString());
					}
				}
				this.compiled = compilerResult.CompiledAssembly;
			}
		}

		public override Assembly getCompiledAssembly()
		{
			return this.compiled;
		}

		public CompilerParameters getCompilerParameters()
		{
			return this.p;
		}

		public override void loadClass(string file)
		{
			int item = -1;
			try
			{
				item = csharplanguage.compilations[file];
			}
			catch (Exception exception)
			{
			}
			if (item == 0)
			{
				DateTime now = DateTime.Now;
				while (true)
				{
					if (((DateTime.Now - now).TotalMilliseconds >= 4000 ? true : item != 0))
					{
						break;
					}
					item = csharplanguage.compilations[file];
				}
				if (item == 0)
				{
					throw new TimeoutException(string.Concat("No se pudo compilar correctamente el archivo ", file, ". Se agoto el tiempo de espera para la sincronizacion de compilacion entre diferentes hilos."));
				}
			}
			if (item == 1)
			{
				this.compiled = csharplanguage.compileds[file];
			}
			else
			{
				csharplanguage.compilations[file] = 0;
				bool flag = true;
				string str = environment.uniqueId();
				string fileName = Path.GetFileName(file);
				string str1 = string.Concat(Path.GetDirectoryName(file), "/__jxshell__cache");
				string str2 = string.Concat(str1, "/", fileName, ".cache");
				string end = "";
				environment.mkDir(str1);
				if (File.Exists(str2))
				{
					DateTime lastWriteTime = File.GetLastWriteTime(str2);
					if (File.GetLastWriteTime(file) <= lastWriteTime)
					{
						FileStream fileStream = new FileStream(str2, FileMode.Open, FileAccess.Read);
						StreamReader streamReader = new StreamReader(fileStream);
						end = streamReader.ReadToEnd();
						streamReader.Close();
						fileStream.Close();
						if (end != "")
						{
							flag = false;
						}
					}
				}
				if (!flag)
				{
					try
					{
						Assembly assembly = Assembly.LoadFile(end);
						this.compiled = assembly;
						csharplanguage.compilations[file] = 1;
						csharplanguage.compileds[file] = assembly;
					}
					catch (Exception exception1)
					{
						flag = true;
					}
				}
				if (flag)
				{
					end = environment.getCompilationFile(str);
					FileStream fileStream1 = new FileStream(file, FileMode.Open, FileAccess.Read);
					StreamReader streamReader1 = new StreamReader(fileStream1);
					string end1 = streamReader1.ReadToEnd();
					streamReader1.Close();
					fileStream1.Close();
					try
					{
						this.compileString(end1, end);
					}
					catch (Exception exception3)
					{
						Exception exception2 = exception3;
						csharplanguage.compilations[file] = -1;
						throw new Exception(string.Concat("No se pudo realizar la compilacion de ", file, ". ", exception2.ToString()), exception2);
					}
					csharplanguage.compilations[file] = 1;
					csharplanguage.compileds[file] = this.compiled;
					fileStream1 = new FileStream(str2, FileMode.OpenOrCreate, FileAccess.Write);
					StreamWriter streamWriter = new StreamWriter(fileStream1);
					fileStream1.SetLength((long)0);
					streamWriter.Write(end);
					streamWriter.Close();
					fileStream1.Close();
				}
				try
				{
				}
				catch (Exception exception4)
				{
				}
				try
				{
					MethodInfo method = this.compiled.GetType("program").GetMethod("mainLibrary");
					method.Invoke(null, new object[0]);
				}
				catch (Exception exception5)
				{
				}
			}
		}

		public override void runFile(string file)
		{
			this.loadClass(file);
			Type type = this.compiled.GetType("program");
			MethodInfo method = type.GetMethod("main", new Type[0]);
			method.Invoke(null, new object[0]);
		}
	
		
		public static string GetSHA1(String texto)
		{
			SHA1 sha1 = SHA1CryptoServiceProvider.Create();
			Byte[] textOriginal = Encoding.UTF8.GetBytes(texto);
			Byte[] hash = sha1.ComputeHash(textOriginal);
			StringBuilder cadena = new StringBuilder();
			foreach (byte i in hash)
			{
			  cadena.AppendFormat("{0:x2}", i);
			}
			return cadena.ToString();
		}
		
		
		public override void runScript(string script)
		{
			
			//string file = environment.getCompilationFile(GetSHA1(script));
			runScriptWithId(script, "JIT-" + GetSHA1(script));
		}
		
		public override void runScriptWithId(string script, string id)
		{
			Type type = null;
			string file  = environment.getCompilationFile(id);
			var f = new FileInfo(file);
			bool compile= true;
			if(f.Exists){
				
				try{
					this.compiled = Assembly.LoadFile(file);
					type = this.compiled.GetType("program");	
					compile = false;
				}
				catch(Exception){}
				
			}
			if(compile){
				this.p.GenerateInMemory = false;
				this.compileString(script, file);
				type = this.compiled.GetType("program");
			}
			if(type != null){
				MethodInfo method = type.GetMethod("main", new Type[0]);
				method.Invoke(null, new object[0]);	
			}
		}

		public void runScript(string script, bool inMemory)
		{
			
			this.p.GenerateInMemory = inMemory;
			this.compileString(script, (inMemory ? "" : environment.getCompilationFile()));
			Type type = this.compiled.GetType("program");
			if(type != null){
				MethodInfo method = type.GetMethod("main", new Type[0]);
				method.Invoke(null, new object[0]);	
			}
		}
	}
}