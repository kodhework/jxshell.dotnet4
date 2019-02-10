using jxshell;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;

namespace jxshell.dotnet4
{
	[ComVisible(true)]
	[Guid("9173A427-2F3B-405D-9B0F-23C7B7048114")]
	[ProgId("jxshell.dotnet4.Manager")]
	public class Manager : IDisposable
	{
		public static Manager lastManager;

		public ResolveEventHandler assemblyResolve;

		private Dictionary<string, Assembly> fileAssemblies = new Dictionary<string, Assembly>();

		private static bool environmentInit;

		public List<Assembly> assemblies = new List<Assembly>();

		public Dictionary<string, Type> loadedTypes = new Dictionary<string, Type>();

		static Manager()
		{
		}

		public Manager()
		{
		}

		public void @add(Assembly a)
		{
			if (this.assemblies.IndexOf(a) < 0)
			{
				this.assemblies.Add(a);
				environment.loadAssembly(a, true);
				Type[] types = a.GetTypes();
				for (int i = 0; i < (int)types.Length; i++)
				{
					Type type = types[i];
					if (!type.IsGenericType)
					{
						string text3 = type.ToString();
						string text4 = text3.Replace("+", ".");
						this.loadedTypes[text3] = type;
						if (text4 != text3)
						{
							this.loadedTypes[text4] = type;
						}
					}
					else
					{
						string text = type.ToString();
						string text2 = text.Replace("+", ".");
						int length = text.IndexOf("[");
						this.loadedTypes[text.Substring(0, length)] = type;
						if (text2 != text)
						{
							this.loadedTypes[text2.Substring(0, length)] = type;
						}
					}
				}
			}
		}

		public void Dispose()
		{
			this.assemblies.Clear();
			this.assemblies = null;
			this.fileAssemblies.Clear();
			this.loadedTypes.Clear();
		}

		public object getBytesFromString(string s)
		{
			return wrapper.getFromObject(Encoding.ASCII.GetBytes(s));
		}

		private byte[] getBytesFromString(string s, bool xprivate)
		{
			return Encoding.ASCII.GetBytes(s);
		}

		public wrapper getDefaultFor(object type)
		{
			Type type2 = (!(type is wrapper) ? this.getTypeOrGenericType(type.ToString()) : (Type)((wrapper)type).wrappedObject);
			object o = null;
			if (type2.IsValueType)
			{
				o = Activator.CreateInstance(type2);
			}
			return wrapper.createWrapper(o, typeDescriptor.loadFromType(type2));
		}

		public jxshell.dotnet4.wrapper getObjectAsType(object o, object xt)
		{
			Type type;
			type = (!(xt is jxshell.dotnet4.wrapper) ? this.getTypeOrGenericType(xt.ToString()) : (Type)((jxshell.dotnet4.wrapper)xt).wrappedObject);
			object o2 = Convert.ChangeType(o, type);
			return jxshell.dotnet4.wrapper.createWrapper(o2, typeDescriptor.loadFromType(type));
		}

		public wrapperStatic getStaticWrapper(string typeName)
		{
			return wrapperStatic.loadFromType(this.getTypeOrGenericType(typeName));
		}

		public wrapper getTypeFromObject(object o)
		{
			if (o == null)
			{
				return (wrapper)wrapper.getFromObject(typeof(DBNull));
			}
			if (o is wrapper)
			{
				o = ((wrapper)o).wrappedObject;
			}
			return (wrapper)wrapper.getFromObject(o.GetType());
		}

		public object getTypeFromString(string name)
		{
			return wrapper.getFromObject(this.getTypeOrGenericType(name));
		}

		public Type getTypeOrGenericType(string typeName)
		{
			Type value;
			string[] array = typeName.Split(new char[] { '<' });
			if ((int)array.Length <= 1)
			{
				if (typeName.IndexOf("[") <= 0)
				{
					if (!this.loadedTypes.TryGetValue(typeName, out value))
					{
						throw new Exception("El tipo especificado no se encontró. Revise si debe cargar un ensamblado.");
					}
					return value;
				}
				int num = typeName.IndexOf('[');
				string typeName3 = typeName.Substring(0, num);
				string text3 = typeName.Substring(num);
				Type type2 = this.getTypeOrGenericType(typeName3);
				char c = text3[0];
				int num2 = 0;
				int num3 = 1;
				while (num2 < text3.Length)
				{
					while (true)
					{
						if (c == ',')
						{
							num3++;
						}
						else if (c == ']')
						{
							break;
						}
						num2++;
						c = text3[num2];
					}
					num2++;
					type2 = type2.MakeArrayType(num3);
					num3 = 1;
				}
				return type2;
			}
			if (array[1].IndexOf(">") < 0)
			{
				throw new Exception("El nombre del tipo no es válido.");
			}
			int length = array[1].LastIndexOf('>');
			string text = array[1].Substring(0, length);
			string[] array2 = text.Split(new char[] { ',' });
			List<Type> list = new List<Type>();
			string[] strArrays = array2;
			for (int i = 0; i < (int)strArrays.Length; i++)
			{
				string text2 = strArrays[i];
				list.Add(this.getTypeOrGenericType(text2.Trim()));
			}
			Type[] array4 = list.ToArray();
			string str = array[0];
			int num1 = (int)array4.Length;
			Type typeOrGenericType = this.getTypeOrGenericType(string.Concat(str, "`", num1.ToString()));
			return typeOrGenericType.MakeGenericType(array4);
		}

		public string getTypeString(wrapperBase typex)
		{
			return typeDescriptor.getNameForType((Type)typex.wrappedObject);
		}

		public object getUTF8WrappedString(object s)
		{
			byte[] numArray;
			numArray = (!(s is wrapper) ? this.getBytesFromString(s.ToString(), true) : (byte[])((wrapper)s).wrappedObject);
			string string1 = Encoding.UTF8.GetString(numArray);
			return wrapper.createWrapper(string1, typeDescriptor.loadFromType(typeof(string)));
		}

		public void init()
		{
			Manager.lastManager = this;
			if (!Manager.environmentInit)
			{
				ResolveEventHandler value = (object sender, ResolveEventArgs args) => {
					Assembly value2 = null;
					this.fileAssemblies.TryGetValue(args.Name, out value2);
					return value2;
				};
				AppDomain.CurrentDomain.AssemblyResolve += value;
				environment.initEnvironment();
				Manager.environmentInit = true;
			}
			this.loadAssembly(typeof(Console).Assembly);
			this.loadAssembly(typeof(WebClient).Assembly);
			this.loadAssembly(typeof(typeDescriptor).Assembly);
		}

		public void loadAssembly(string name)
		{
			this.@add(Assembly.Load(name));
		}

		public void loadAssembly(Assembly a)
		{
			this.@add(a);
		}

		public void loadAssemblyFile(string file)
		{
			string item = environment.addBs(Path.GetDirectoryName(file));
			if (environment.directories.IndexOf(item) < 0)
			{
				environment.directories.Add(item);
			}
			string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(file);
			try
			{
				Assembly assembly = Assembly.LoadFile(file);
				this.fileAssemblies[assembly.FullName] = assembly;
				this.fileAssemblies[fileNameWithoutExtension] = assembly;
				this.@add(assembly);
			}
			catch (ReflectionTypeLoadException reflectionTypeLoadException)
			{
				ReflectionTypeLoadException ex = reflectionTypeLoadException;
				StringBuilder stringBuilder = new StringBuilder();
				stringBuilder.AppendLine("Se encontró los siguientes errores al cargar el ensamblado:");
				Exception[] loaderExceptions = ex.LoaderExceptions;
				for (int i = 0; i < (int)loaderExceptions.Length; i++)
				{
					stringBuilder.AppendLine(loaderExceptions[i].ToString());
				}
				throw new Exception(stringBuilder.ToString());
			}
		}

		public void loadAssemblyPartialName(string name)
		{
			this.@add(Assembly.LoadWithPartialName(name));
		}

		public void loadManyTypes(string types)
		{
			string[] array = types.Split(new string[] { "-" }, StringSplitOptions.RemoveEmptyEntries);
			StringBuilder stringBuilder = new StringBuilder();
			jxshell.dotnet4.typeDescriptor.addUsingsStatements(stringBuilder);
			Dictionary<Type, type_1> dictionary = new Dictionary<Type, type_1>();
			string[] strArrays = array;
			for (int i = 0; i < (int)strArrays.Length; i++)
			{
				string typeName = strArrays[i];
				Type typeOrGenericType = this.getTypeOrGenericType(typeName);
				jxshell.dotnet4.typeDescriptor typeDescriptor = new jxshell.dotnet4.typeDescriptor(typeOrGenericType, typeName, false);
				if (!typeDescriptor.isCompiled())
				{
					string staticClass = "";
					string instanceClass = "";
					typeDescriptor.precompile(stringBuilder, ref staticClass, ref instanceClass);
					dictionary[typeOrGenericType] = new type_1()
					{
						td = typeDescriptor,
						staticClass = staticClass,
						instanceClass = instanceClass,
						t = typeOrGenericType
					};
				}
			}
			stringBuilder.AppendLine("class program{public static void main(){}}");
			jxshell.csharplanguage csharplanguage = (jxshell.csharplanguage)language.defaultLanguage.create();
			if (dictionary.Count > 0)
			{
				csharplanguage.runScript(stringBuilder.ToString(), jxshell.dotnet4.typeDescriptor.generateInMemory);
			}
			foreach (KeyValuePair<Type, type_1> item in dictionary)
			{
				type_1 value = item.Value;
				Type type = csharplanguage.getCompiledAssembly().GetType(string.Concat("jxshell.dotnet4.", value.staticClass));
				ConstructorInfo constructor = type.GetConstructor(new Type[] { typeof(Type), typeof(jxshell.dotnet4.typeDescriptor) });
				value.td.setCompiledWrapper((wrapperStatic)constructor.Invoke(new object[] { value.t, value.td }));
			}
		}

		public void loadManyTypes(Type[] types)
		{
			StringBuilder stringBuilder = new StringBuilder();
			jxshell.dotnet4.typeDescriptor.addUsingsStatements(stringBuilder);
			Dictionary<Type, type_1> dictionary = new Dictionary<Type, type_1>();
			Type[] typeArray = types;
			for (int i = 0; i < (int)typeArray.Length; i++)
			{
				Type type = typeArray[i];
				if (!type.IsGenericType && type.IsPublic)
				{
					jxshell.dotnet4.typeDescriptor typeDescriptor = new jxshell.dotnet4.typeDescriptor(type, jxshell.dotnet4.typeDescriptor.getNameForType(type), false);
					if (!typeDescriptor.isCompiled())
					{
						string staticClass = "";
						string instanceClass = "";
						typeDescriptor.precompile(stringBuilder, ref staticClass, ref instanceClass);
						dictionary[type] = new type_1()
						{
							td = typeDescriptor,
							staticClass = staticClass,
							instanceClass = instanceClass,
							t = type
						};
					}
				}
			}
			stringBuilder.AppendLine("class program{public static void main(){}}");
			jxshell.csharplanguage csharplanguage = (jxshell.csharplanguage)language.defaultLanguage.create();
			if (dictionary.Count > 0)
			{
				csharplanguage.runScript(stringBuilder.ToString(), jxshell.dotnet4.typeDescriptor.generateInMemory);
			}
			foreach (KeyValuePair<Type, type_1> item in dictionary)
			{
				type_1 value = item.Value;
				Type type2 = csharplanguage.getCompiledAssembly().GetType(string.Concat("jxshell.dotnet4.", value.staticClass));
				ConstructorInfo constructor = type2.GetConstructor(new Type[] { typeof(Type), typeof(jxshell.dotnet4.typeDescriptor) });
				value.td.setCompiledWrapper((wrapperStatic)constructor.Invoke(new object[] { value.t, value.td }));
			}
		}

		public typeDescriptor loadType(string typeName)
		{
			return typeDescriptor.loadFromType(this.getTypeOrGenericType(typeName));
		}

		public void registerVFPClassToDotnet(string code, string vfpclassname, string netclassName)
		{
		}

		public void setThreadedLibraryFile(string file)
		{
		}
	}
}