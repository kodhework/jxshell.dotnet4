using System.IO;
using System.Security.Cryptography;
using jxshell;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;

namespace jxshell.net6
{
	[ComVisible(true)]
	public class typeDescriptor
	{
		public Dictionary<string, methodDescriptor> instanceMethods = new Dictionary<string, methodDescriptor>();

		public Dictionary<string, methodDescriptor> staticMethods = new Dictionary<string, methodDescriptor>();

		public Dictionary<string, propertyDescriptor> instanceProperties = new Dictionary<string, propertyDescriptor>();

		public Dictionary<string, propertyDescriptor> staticProperties = new Dictionary<string, propertyDescriptor>();

		public Dictionary<string, fieldDescriptor> instanceFields = new Dictionary<string, fieldDescriptor>();

		public Dictionary<string, fieldDescriptor> staticFields = new Dictionary<string, fieldDescriptor>();

		public static bool gencompile;

		public List<methodDescriptor> methods = new List<methodDescriptor>();

		public methodDescriptor constructor;

		public List<propertyDescriptor> properties = new List<propertyDescriptor>();

		public List<fieldDescriptor> fields = new List<fieldDescriptor>();

		internal static csharplanguage language;

		internal wrapperStatic compiledWrapper;

		internal static bool generateInMemory;

		internal Type type;

		internal string typeString = "";

		private bool compiled;

		private static Dictionary<Type, typeDescriptor> loadedTypes;

		static typeDescriptor()
		{
			typeDescriptor.gencompile = true;
			typeDescriptor.generateInMemory = true;
			typeDescriptor.loadedTypes = new Dictionary<Type, typeDescriptor>();
		}

		public typeDescriptor(Type t) : this(t, "", true)
		{
			this.typeString = typeDescriptor.getNameForType(t);
		}

		public typeDescriptor(Type t, string typeName, bool compile = true)
		{
			methodDescriptor count;
			typeDescriptor.loadEvaluator();
			if (typeDescriptor.language == null)
			{
				typeDescriptor.language = (csharplanguage)jxshell.language.defaultLanguage.create();
			}
			this.typeString = typeName;
			MethodInfo[] methods = t.GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.FlattenHierarchy | BindingFlags.InvokeMethod);
			for (int i = 0; i < (int)methods.Length; i++)
			{
				MethodInfo methodInfo = methods[i];
				if ((methodInfo.Name.StartsWith("get_") ? false : !methodInfo.Name.StartsWith("set_")))
				{
					methodDescriptor _methodDescriptor = null;
					if (methodInfo.IsGenericMethod)
					{
						string str = string.Concat("generic_", methodInfo.Name);
						if (!this.instanceMethods.TryGetValue(str, out _methodDescriptor))
						{
							_methodDescriptor = new methodDescriptor()
							{
								isGenericMethod = true
							};
							this.instanceMethods[str] = _methodDescriptor;
							this.methods.Add(_methodDescriptor);
							_methodDescriptor.methodOrder = this.methods.Count - 1;
							_methodDescriptor.name = str;
						}
					}
					else if (!this.instanceMethods.TryGetValue(methodInfo.Name, out _methodDescriptor))
					{
						_methodDescriptor = new methodDescriptor();
						this.instanceMethods[methodInfo.Name] = _methodDescriptor;
						this.methods.Add(_methodDescriptor);
						_methodDescriptor.methodOrder = this.methods.Count - 1;
						_methodDescriptor.name = methodInfo.Name;
					}
					_methodDescriptor.baseMethods.Add(methodInfo);
					_methodDescriptor.maxParameterCount = Math.Max(_methodDescriptor.maxParameterCount, (int)methodInfo.GetParameters().Length);
					if (methodInfo.IsGenericMethod)
					{
						_methodDescriptor.genericParameterCount = Math.Max(_methodDescriptor.genericParameterCount, (int)methodInfo.GetGenericArguments().Length);
					}
				}
			}
			PropertyInfo[] properties = t.GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.FlattenHierarchy);
			for (int j = 0; j < (int)properties.Length; j++)
			{
				PropertyInfo propertyInfo = properties[j];
				propertyDescriptor _propertyDescriptor = null;
				if (!this.instanceProperties.TryGetValue(propertyInfo.Name, out _propertyDescriptor))
				{
					_propertyDescriptor = new propertyDescriptor();
					this.instanceProperties[propertyInfo.Name] = _propertyDescriptor;
					this.properties.Add(_propertyDescriptor);
					_propertyDescriptor.propertyOrder = this.properties.Count - 1;
					_propertyDescriptor.name = propertyInfo.Name;
				}
				_propertyDescriptor.properties.Add(propertyInfo);
				_propertyDescriptor.maxParameterCount = Math.Max(_propertyDescriptor.maxParameterCount, (int)propertyInfo.GetIndexParameters().Length);
			}
			FieldInfo[] fields = t.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.FlattenHierarchy | BindingFlags.GetField | BindingFlags.SetField);
			for (int k = 0; k < (int)fields.Length; k++)
			{
				FieldInfo fieldInfo = fields[k];
				fieldDescriptor _fieldDescriptor = null;
				if (!this.instanceFields.TryGetValue(fieldInfo.Name, out _fieldDescriptor))
				{
					_fieldDescriptor = new fieldDescriptor();
					this.instanceFields[fieldInfo.Name] = _fieldDescriptor;
					this.fields.Add(_fieldDescriptor);
					_fieldDescriptor.fieldOrder = this.fields.Count - 1;
					_fieldDescriptor.name = fieldInfo.Name;
				}
				_fieldDescriptor.fieldInfo = fieldInfo;
			}
			ConstructorInfo[] constructors = t.GetConstructors();
			for (int l = 0; l < (int)constructors.Length; l++)
			{
				ConstructorInfo constructorInfo = constructors[l];
				if (this.constructor == null)
				{
					methodDescriptor _methodDescriptor1 = new methodDescriptor();
					methodDescriptor _methodDescriptor2 = _methodDescriptor1;
					this.constructor = _methodDescriptor1;
					count = _methodDescriptor2;
					this.methods.Add(count);
					count.name = "construct";
					count.methodOrder = this.methods.Count - 1;
				}
				else
				{
					count = this.constructor;
				}
				count.baseMethods.Add(constructorInfo);
				count.maxParameterCount = Math.Max(count.maxParameterCount, (int)constructorInfo.GetParameters().Length);
			}
			MethodInfo[] methodInfoArray = t.GetMethods(BindingFlags.Static | BindingFlags.Public | BindingFlags.FlattenHierarchy | BindingFlags.InvokeMethod);
			for (int m = 0; m < (int)methodInfoArray.Length; m++)
			{
				MethodInfo methodInfo1 = methodInfoArray[m];
				if ((methodInfo1.Name.StartsWith("get_") ? false : !methodInfo1.Name.StartsWith("set_")))
				{
					methodDescriptor name = null;
					if (methodInfo1.IsGenericMethod)
					{
						string str1 = string.Concat("generic_", methodInfo1.Name);
						if (!this.instanceMethods.TryGetValue(str1, out name))
						{
							name = new methodDescriptor();
							this.staticMethods[str1] = name;
							name.isGenericMethod = true;
							this.methods.Add(name);
							name.methodOrder = this.methods.Count - 1;
							name.name = str1;
						}
					}
					else if (!this.staticMethods.TryGetValue(methodInfo1.Name, out name))
					{
						name = new methodDescriptor();
						this.staticMethods[methodInfo1.Name] = name;
						this.methods.Add(name);
						name.name = methodInfo1.Name;
						name.methodOrder = this.methods.Count - 1;
					}
					name.baseMethods.Add(methodInfo1);
					name.maxParameterCount = Math.Max(name.maxParameterCount, (int)methodInfo1.GetParameters().Length);
					if (methodInfo1.IsGenericMethod)
					{
						name.genericParameterCount = Math.Max(name.genericParameterCount, (int)methodInfo1.GetGenericArguments().Length);
					}
				}
			}
			PropertyInfo[] propertyInfoArray = t.GetProperties(BindingFlags.Static | BindingFlags.Public | BindingFlags.FlattenHierarchy);
			for (int n = 0; n < (int)propertyInfoArray.Length; n++)
			{
				PropertyInfo propertyInfo1 = propertyInfoArray[n];
				propertyDescriptor _propertyDescriptor1 = null;
				if (!this.staticProperties.TryGetValue(propertyInfo1.Name, out _propertyDescriptor1))
				{
					_propertyDescriptor1 = new propertyDescriptor();
					this.staticProperties[propertyInfo1.Name] = _propertyDescriptor1;
					this.properties.Add(_propertyDescriptor1);
					_propertyDescriptor1.propertyOrder = this.properties.Count - 1;
					_propertyDescriptor1.name = propertyInfo1.Name;
				}
				_propertyDescriptor1.properties.Add(propertyInfo1);
				_propertyDescriptor1.maxParameterCount = Math.Max(_propertyDescriptor1.maxParameterCount, (int)propertyInfo1.GetIndexParameters().Length);
			}
			FieldInfo[] fieldInfoArray = t.GetFields(BindingFlags.Static | BindingFlags.Public | BindingFlags.FlattenHierarchy | BindingFlags.GetField | BindingFlags.SetField);
			for (int o = 0; o < (int)fieldInfoArray.Length; o++)
			{
				FieldInfo fieldInfo1 = fieldInfoArray[o];
				fieldDescriptor _fieldDescriptor1 = null;
				if (!this.instanceFields.TryGetValue(fieldInfo1.Name, out _fieldDescriptor1))
				{
					_fieldDescriptor1 = new fieldDescriptor();
					this.staticFields[fieldInfo1.Name] = _fieldDescriptor1;
					this.fields.Add(_fieldDescriptor1);
					_fieldDescriptor1.fieldOrder = this.fields.Count - 1;
					_fieldDescriptor1.name = fieldInfo1.Name;
				}
				_fieldDescriptor1.fieldInfo = fieldInfo1;
			}
			this.type = t;
			typeDescriptor.loadedTypes[t] = this;
			if ((!compile ? false : typeDescriptor.gencompile))
			{
				this.compile();
			}
		}

		public static void addUsingsStatements(StringBuilder sb)
		{
			sb.AppendLine("using System;");
			sb.AppendLine("using System.Runtime.InteropServices;");
			sb.AppendLine("using System.Reflection;");
			sb.AppendLine("using jxshell.net6;");
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
			
		public wrapperStatic compile()
		{
			wrapperStatic _wrapperStatic;
			if (!this.compiled)
			{
				
				// generar un archivo  por cada tipo 
				string name = GetSHA1(this.type.AssemblyQualifiedName).ToUpper();
				string file = environment.getCompilationFile(name);
				FileInfo f = new FileInfo(file);
				
				try
				{
					
					if(f.Exists){
						
						string str = "C" + GetSHA1(type.AssemblyQualifiedName);
						string str1 = "C" + GetSHA1(type.AssemblyQualifiedName) + "_static";
			
						Type _type = Assembly.LoadFile(file).GetType(string.Concat("jxshell.net6.", str1));
						ConstructorInfo _constructor = _type.GetConstructor(new Type[] { typeof(Type), typeof(typeDescriptor) });
						
						this.compiledWrapper = (wrapperStatic)_constructor.Invoke(new object[] { this.type, this });
						this.compiled = true;
					}
					else{
						StringBuilder stringBuilder = new StringBuilder();
						string str = "";
						string str1 = "";
						typeDescriptor.addUsingsStatements(stringBuilder);
						this.precompile(stringBuilder, ref str, ref str1);
						stringBuilder.AppendLine("class program{public static void main(){}}");
					
						csharplanguage _csharplanguage = typeDescriptor.language;
						_csharplanguage.runScriptWithId(stringBuilder.ToString(), name);
						
						Type _type = _csharplanguage.getCompiledAssembly().GetType(string.Concat("jxshell.net6.", str));
						ConstructorInfo _constructor = _type.GetConstructor(new Type[] { typeof(Type), typeof(typeDescriptor) });
						
						this.compiledWrapper = (wrapperStatic)_constructor.Invoke(new object[] { this.type, this });
						this.compiled = true;
						
					}
				}
				catch (Exception exception1)
				{
					Exception exception = exception1;
					throw new Exception(string.Concat("No se puede obtener un wrapper para el tipo ", this.type.ToString(), ". ", exception.Message), exception);
				}
				_wrapperStatic = this.compiledWrapper;
				
				
			}
			else
			{
				_wrapperStatic = this.compiledWrapper;
			}
			return _wrapperStatic;
		}

		public Ast compileForVfp()
		{
			return null;
		}

		public object fieldGetValue(int index, object o)
		{
			return this.fields[index].getMetavalue(o);
		}

		public void fieldSetValue(int index, object o, object value)
		{
			this.fields[index].setValue(value, o);
		}

		public object getMetavalueFromObject(object o)
		{
			metaObject _metaObject = new metaObject()
			{
				@value = o,
				isstatic = false,
				name = this.typeString
			};
			return _metaObject;
		}

		public MethodBase getMethodForParameters(string method, ref object[] parameters)
		{
			return this.instanceMethods[method].getMethodForParameters(ref parameters, null);
		}

		public static string getNameForType(Type t)
		{
			string str;
			StringBuilder stringBuilder = new StringBuilder();
			if (t.IsGenericType)
			{
				string str1 = t.ToString();
				str1 = str1.Substring(0, str1.IndexOf("`"));
				stringBuilder.Append(str1);
				stringBuilder.Append("<");
				Type[] genericArguments = t.GetGenericArguments();
				bool flag = false;
				Type[] typeArray = genericArguments;
				for (int i = 0; i < (int)typeArray.Length; i++)
				{
					Type type = typeArray[i];
					if (!flag)
					{
						flag = true;
					}
					else
					{
						stringBuilder.Append(",");
					}
					stringBuilder.Append(typeDescriptor.getNameForType(type));
				}
				stringBuilder.Append(">");
				str = stringBuilder.ToString();
			}
			else
			{
				str = t.ToString().Replace("+", ".").Replace("&", "");
			}
			return str;
		}

		public object getProperty(object o, string property, params object[] args)
		{
			PropertyInfo propertyForParameters = this.getPropertyForParameters(property, ref args);
			return propertyForParameters.GetValue(o, args);
		}

		public void getProperty(out object o, object ox, string property, params object[] args)
		{
			PropertyInfo propertyForParameters = this.getPropertyForParameters(property, ref args);
			o = propertyForParameters.GetValue(ox, args);
		}

		public PropertyInfo getPropertyForParameters(string property, ref object[] parameters)
		{
			return this.instanceProperties[property].getPropertyForParameters(ref parameters);
		}

		public MethodBase getStaticMethodForParameters(string method, ref object[] parameters)
		{
			return this.staticMethods[method].getMethodForParameters(ref parameters, null);
		}

		public object getStaticProperty(object o, string property, params object[] args)
		{
			PropertyInfo staticPropertyForParameters = this.getStaticPropertyForParameters(property, ref args);
			return staticPropertyForParameters.GetValue(o, args);
		}

		public void getStaticProperty(out object o, object ox, string property, params object[] args)
		{
			PropertyInfo staticPropertyForParameters = this.getStaticPropertyForParameters(property, ref args);
			o = staticPropertyForParameters.GetValue(ox, args);
		}

		public PropertyInfo getStaticPropertyForParameters(string property, ref object[] parameters)
		{
			return this.staticProperties[property].getPropertyForParameters(ref parameters);
		}

		public object invokeMethod(object o, string method, params object[] args)
		{
			MethodBase methodForParameters = this.getMethodForParameters(method, ref args);
			return methodForParameters.Invoke(o, args);
		}

		public void invokeMethod(out object o, object ox, string method, params object[] args)
		{
			MethodBase methodForParameters = this.getMethodForParameters(method, ref args);
			o = methodForParameters.Invoke(ox, args);
		}

		public object invokeStaticMethod(object o, string method, params object[] args)
		{
			MethodBase staticMethodForParameters = this.getStaticMethodForParameters(method, ref args);
			return staticMethodForParameters.Invoke(o, args);
		}

		public void invokeStaticMethod(out object o, object ox, string method, params object[] args)
		{
			MethodBase staticMethodForParameters = this.getStaticMethodForParameters(method, ref args);
			o = staticMethodForParameters.Invoke(ox, args);
		}

		public bool isCompiled()
		{
			return this.compiled;
		}

		public static bool isSpecialMethod(MethodBase method)
		{
			return (method.Name.StartsWith("add_") ? true : method.IsSpecialName);
		}

		internal static void loadEvaluator()
		{
		}

		public static typeDescriptor loadFromType(Type t)
		{
			typeDescriptor _typeDescriptor;
			typeDescriptor _typeDescriptor1;
			_typeDescriptor1 = (!typeDescriptor.loadedTypes.TryGetValue(t, out _typeDescriptor) ? new typeDescriptor(t) : _typeDescriptor);
			return _typeDescriptor1;
		}

		public static typeDescriptor loadFromType(Type t, string typeName, bool compile = true)
		{
			typeDescriptor _typeDescriptor;
			typeDescriptor _typeDescriptor1;
			_typeDescriptor1 = (!typeDescriptor.loadedTypes.TryGetValue(t, out _typeDescriptor) ? new typeDescriptor(t, typeName, compile) : _typeDescriptor);
			return _typeDescriptor1;
		}

		public void precompile(StringBuilder sb, ref string staticClass, ref string instanceClass)
		{
			sb.AppendLine("namespace jxshell.net6{");
			
			
			//string str = string.Concat("_", environment.uniqueId());
			//string str1 = string.Concat("_", environment.uniqueId());
			string str = "C" + GetSHA1(type.AssemblyQualifiedName);
			string str1 = "C" + GetSHA1(type.AssemblyQualifiedName) + "_static";
			
			sb.AppendLine();
			sb.AppendLine("[ComVisible(true)]");
			sb.Append("public class ").Append(str1).Append(" : ");
			if (typeof(MulticastDelegate).IsAssignableFrom(this.type.BaseType))
			{
				sb.Append("delegateWrapperStatic{");
			}
			else
			{
				sb.Append("wrapperStatic{");
			}
			sb.AppendLine();
			sb.Append("public ").Append(str1).Append("(Type t, typeDescriptor td):base(t,td){");
			if (this.type.IsEnum)
			{
				sb.Append("__initEnum();");
			}
			sb.Append("}");
			sb.AppendLine();
			sb.AppendLine("static invoker __invoker= new invoker();");
			sb.Append("public override ").Append("wrapper").Append(" getWrapper(object o){return new ").Append(str).Append("(o,typeD);}");
			sb.AppendLine();
			if (!this.type.IsEnum)
			{
				sb.AppendLine("/* FIELDS */");
				foreach (KeyValuePair<string, fieldDescriptor> staticField in this.staticFields)
				{
					sb.Append("public object ").Append(staticField.Value.name).Append("{");
					sb.Append("get{");
					sb.Append("var fielD = typeD.fields[").Append(staticField.Value.fieldOrder).Append("];");
					sb.Append("return fielD.getValue(null);");
					sb.Append("}");
					sb.AppendLine();
					sb.Append("set{");
					sb.Append("var fielD = typeD.fields[").Append(staticField.Value.fieldOrder).Append("];");
					sb.Append("fielD.setValue(value, null);");
					sb.Append("}");
					sb.AppendLine();
					sb.AppendLine("}");
				}
			}
			sb.AppendLine("/* MÉTODOS */");
			foreach (KeyValuePair<string, methodDescriptor> staticMethod in this.staticMethods)
			{
				sb.Append("public object ").Append(staticMethod.Value.name).Append("(");
				StringBuilder stringBuilder = new StringBuilder();
				bool value = staticMethod.Value.isGenericMethod;
				if (value)
				{
					stringBuilder.Append("System.Collections.Generic.List<System.Type> genericArguments=new System.Collections.Generic.List<System.Type>(1);");
					for (int i = 0; i < staticMethod.Value.genericParameterCount; i++)
					{
						if (i > 0)
						{
							sb.Append(",");
						}
						stringBuilder.Append("if(type").Append(i).Append("!=null){if(type").Append(i);
						stringBuilder.Append(" is wrapper){genericArguments.Add((System.Type)(((wrapper)type").Append(i).Append(").wrappedObject));}else{");
						stringBuilder.Append("genericArguments.Add(Manager.lastManager.getTypeOrGenericType(type").Append(i).Append(".ToString()));}").Append("}");
						sb.Append("[Optional] object ").Append("type").Append(i);
						stringBuilder.AppendLine();
					}
				}
				stringBuilder.Append("object[] args = {");
				for (int j = 0; j < staticMethod.Value.maxParameterCount; j++)
				{
					if (j > 0)
					{
						stringBuilder.Append(",");
					}
					if (j > 0 | value)
					{
						sb.Append(",");
					}
					stringBuilder.Append("a").Append(j);
					sb.Append("[Optional] object ").Append("a").Append(j);
				}
				sb.Append("){");
				stringBuilder.Append("}");
				sb.AppendLine();
				sb.Append(stringBuilder).Append(";");
				sb.AppendLine();
				sb.Append("var m = typeD.methods[").Append(staticMethod.Value.methodOrder).Append("];");
				sb.AppendLine();
				if (value)
				{
					sb.Append("var method = m.getGenericMethodForParameters(genericArguments.ToArray(), ref args);");
				}
				else
				{
					sb.Append("var method = m.getMethodForParameters(ref args);");
				}
				sb.AppendLine();
				sb.AppendLine("try{");
				sb.AppendLine("return __process(method.Invoke(null,args));");
				sb.AppendLine("}catch(Exception e){if(e.InnerException!=null){throw e.InnerException;}throw e;}");
				sb.AppendLine();
				sb.AppendLine("}");
			}
			sb.AppendLine("/* PROPIEDADES  */");
			foreach (KeyValuePair<string, propertyDescriptor> staticProperty in this.staticProperties)
			{
				sb.Append("public object ");
				if (staticProperty.Value.maxParameterCount > 0)
				{
					sb.Append("this[");
				}
				else
				{
					sb.Append(staticProperty.Value.name);
				}
				StringBuilder stringBuilder1 = new StringBuilder();
				if (staticProperty.Value.maxParameterCount > 0)
				{
					stringBuilder1.Append("object[] args = {");
					for (int k = 0; k < staticProperty.Value.maxParameterCount; k++)
					{
						if (k > 0)
						{
							stringBuilder1.Append(",");
							sb.Append(",");
						}
						stringBuilder1.Append("a").Append(k);
						sb.Append("[Optional] object ").Append("a").Append(k);
					}
					sb.Append("]");
					stringBuilder1.Append("};");
				}
				else
				{
					stringBuilder1.Append("object[] args = {};");
				}
				sb.Append("{");
				sb.AppendLine();
				sb.AppendLine("get{");
				sb.Append(stringBuilder1);
				sb.AppendLine();
				sb.Append("var m = typeD.properties[").Append(staticProperty.Value.propertyOrder).Append("];");
				sb.AppendLine();
				sb.Append("var method = m.getPropertyForParameters(ref args);");
				sb.AppendLine();
				sb.AppendLine("try{");
				sb.Append("return __process(method.GetValue(null,args));");
				sb.AppendLine("}catch(Exception e){if(e.InnerException!=null){throw e.InnerException;}throw e;}");
				sb.AppendLine();
				sb.AppendLine("}");
				sb.AppendLine("set{");
				sb.Append(stringBuilder1);
				sb.AppendLine();
				sb.Append("var m = typeD.properties[").Append(staticProperty.Value.propertyOrder).Append("];");
				sb.AppendLine();
				sb.Append("var method = m.getPropertyForParameters(ref args);");
				sb.AppendLine();
				sb.AppendLine("try{");
				sb.AppendLine("if(value is wrapper){value=((wrapper)value).wrappedObject;}");
				sb.Append("method.SetValue(null,value,args);");
				sb.AppendLine("}catch(Exception e){if(e.InnerException!=null){throw e.InnerException;}throw e;}");
				sb.AppendLine();
				sb.AppendLine("}");
				sb.AppendLine("}");
			}
			if (this.type.IsEnum)
			{
				string[] names = Enum.GetNames(this.type);
				StringBuilder stringBuilder2 = new StringBuilder();
				stringBuilder2.Append("Type ty = typeof(").Append(typeDescriptor.getNameForType(this.type)).Append(");");
				stringBuilder2.Append("Array values = global::System.Enum.GetValues(ty);");
				stringBuilder2.AppendLine();
				int num = 0;
				string[] strArrays = names;
				for (int l = 0; l < (int)strArrays.Length; l++)
				{
					string str2 = strArrays[l];
					sb.Append("public ").Append(str).Append(" ").Append(str2).Append("= new ").Append(str).Append("();").AppendLine();
					stringBuilder2.Append(str2).Append(".wrappedObject = values.GetValue(").Append(num).Append(");");
					stringBuilder2.AppendLine();
					stringBuilder2.Append(str2).Append(".wrappedType = ty;");
					stringBuilder2.AppendLine();
					stringBuilder2.Append(str2).Append(".typeD = typeD;");
					stringBuilder2.AppendLine();
					num++;
				}
				sb.Append("public void __initEnum(){").AppendLine().Append(stringBuilder2.ToString()).AppendLine().Append("}");
			}
			methodDescriptor _methodDescriptor = this.constructor;
			if (_methodDescriptor != null)
			{
				if (typeof(MulticastDelegate).IsAssignableFrom(this.type.BaseType))
				{
					sb.Append("public override delegateWrapper getThisWrapper(){");
					sb.Append("var o = new ").Append(str).Append("();return o;");
					sb.Append("}");
				}
				else
				{
					sb.Append("public object ").Append(_methodDescriptor.name).Append("(");
					StringBuilder stringBuilder3 = new StringBuilder();
					stringBuilder3.Append("object[] args = {");
					for (int m = 0; m < _methodDescriptor.maxParameterCount; m++)
					{
						if (m > 0)
						{
							stringBuilder3.Append(",");
							sb.Append(",");
						}
						stringBuilder3.Append("a").Append(m);
						sb.Append("[Optional] object ").Append("a").Append(m);
					}
					sb.Append("){");
					stringBuilder3.Append("}");
					sb.AppendLine();
					sb.Append(stringBuilder3).Append(";");
					sb.AppendLine();
					sb.Append("var m = typeD.methods[").Append(_methodDescriptor.methodOrder).Append("];");
					sb.AppendLine();
					sb.Append("var method = m.getConstructorForParameters(ref args);");
					sb.AppendLine();
					sb.AppendLine("try{");
					sb.Append("return getWrapper(method.Invoke(args));");
					sb.AppendLine("}catch(Exception e){if(e.InnerException!=null){throw e.InnerException;}throw e;}");
					sb.AppendLine();
					sb.AppendLine("}");
				}
			}
			sb.AppendLine("}");
			sb.AppendLine();
			sb.AppendLine("[ComVisible(true)]");
			sb.Append("public class ").Append(str).Append(" : ");
			if (this.type.IsEnum)
			{
				sb.Append("enumW");
			}
			else if (typeof(MulticastDelegate).IsAssignableFrom(this.type.BaseType))
			{
				sb.Append("delegateW");
			}
			else
			{
				sb.Append("w");
			}
			sb.Append("rapper{").AppendLine();
			sb.Append("public ").Append(str).Append("(object o, typeDescriptor td):base(o,td){}");
			sb.Append("public ").Append(str).Append("():base(){}");
			sb.AppendLine();
			typeof(MulticastDelegate).IsAssignableFrom(this.type.BaseType);
			sb.AppendLine("static jxshell.net6.invoker __invoker= new invoker();");
			sb.AppendLine("/* FIELDS */");
			foreach (KeyValuePair<string, fieldDescriptor> instanceField in this.instanceFields)
			{
				sb.Append("public object ").Append(instanceField.Value.name).Append("{");
				sb.Append("get{");
				sb.Append("var fielD = typeD.fields[").Append(instanceField.Value.fieldOrder).Append("];");
				sb.Append("return fielD.getValue(wrappedObject);");
				sb.Append("}");
				sb.AppendLine();
				sb.Append("set{");
				sb.Append("var fielD = typeD.fields[").Append(instanceField.Value.fieldOrder).Append("];");
				sb.Append("fielD.setValue(value, wrappedObject);");
				sb.Append("}");
				sb.AppendLine();
				sb.AppendLine("}");
			}
			sb.AppendLine("/* MÉTODOS */");
			foreach (KeyValuePair<string, methodDescriptor> instanceMethod in this.instanceMethods)
			{
				if ((!typeof(MulticastDelegate).IsAssignableFrom(this.type.BaseType) ? false : instanceMethod.Value.name == "Invoke"))
				{
					sb.Append("public object call(");
					StringBuilder stringBuilder4 = new StringBuilder();
					StringBuilder stringBuilder5 = new StringBuilder();
					stringBuilder4.Append("");
					for (int n = 0; n < instanceMethod.Value.maxParameterCount; n++)
					{
						if (n > 0)
						{
							stringBuilder4.Append(",");
							stringBuilder5.Append(",");
							sb.Append(",");
						}
						stringBuilder5.Append("wrapper.getFromObject(a").Append(n).Append(")");
						stringBuilder4.Append("a").Append(n);
						sb.Append("[Optional] object ").Append("a").Append(n);
					}
					sb.Append("){");
					sb.AppendLine();
					int value1 = instanceMethod.Value.maxParameterCount;
					sb.AppendLine("if(disposed) return null;");
					sb.Append("invokerparam").Append(value1).Append(" invoker = new invokerparam").Append(value1).Append("(__internalMethod);");
					sb.Append("object o =null;");
					MethodInfo item = (MethodInfo)instanceMethod.Value.baseMethods[0];
					if (item.ReturnType == typeof(void))
					{
						sb.Append("invoker.invokeasVoid");
					}
					else
					{
						sb.Append("o=invoker.invoke");
					}
					sb.Append("(__internalTarget");
					if (instanceMethod.Value.maxParameterCount > 0)
					{
						sb.Append(",");
					}
					sb.Append(stringBuilder4.ToString()).Append(");");
					sb.AppendLine();
					sb.AppendLine("return o;}");
					sb.Append("public ");
					if (item.ReturnType == typeof(void))
					{
						sb.Append("void ");
					}
					else
					{
						sb.Append(typeDescriptor.getNameForType(item.ReturnType));
					}
					sb.Append(" __internalInvoke(");
					ParameterInfo[] parameters = item.GetParameters();
					for (int o = 0; o < (int)parameters.Length; o++)
					{
						ParameterInfo parameterInfo = parameters[o];
						if (o > 0)
						{
							sb.Append(",");
						}
						string str3 = (!parameterInfo.ParameterType.IsPointer ? typeDescriptor.getNameForType(parameterInfo.ParameterType) : "object");
						sb.Append(str3).Append(" a").Append(o);
					}
					sb.Append("){");
					sb.AppendLine();
					if (item.ReturnType == typeof(void))
					{
						sb.Append("call(").Append(stringBuilder5.ToString()).Append(");");
					}
					else
					{
						sb.Append("object o= ");
						sb.Append("call(").Append(stringBuilder5.ToString()).Append(");");
						sb.AppendLine();
						sb.Append("if(o is wrapper){o = ((wrapper)o).wrappedObject;}");
						sb.AppendLine();
						sb.Append("return (").Append(typeDescriptor.getNameForType(item.ReturnType)).Append(")o;");
						sb.AppendLine();
					}
					sb.Append("}\n");
				}
				else
				{
					sb.Append("public object ").Append(instanceMethod.Value.name).Append("(");
					StringBuilder stringBuilder6 = new StringBuilder();
					bool flag = instanceMethod.Value.isGenericMethod;
					if (flag)
					{
						stringBuilder6.Append("System.Collections.Generic.List<System.Type> genericArguments=new System.Collections.Generic.List<System.Type>(1);");
						for (int p = 0; p < instanceMethod.Value.genericParameterCount; p++)
						{
							if (p > 0)
							{
								sb.Append(",");
							}
							stringBuilder6.Append("if(type").Append(p).Append("!=null){if(type").Append(p);
							stringBuilder6.Append(" is wrapper){genericArguments.Add((System.Type)(((wrapper)type").Append(p).Append(").wrappedObject));}else{");
							stringBuilder6.Append("genericArguments.Add(Manager.lastManager.getTypeOrGenericType(type").Append(p).Append(".ToString()));}").Append("}");
							sb.Append("[Optional] object ").Append("type").Append(p);
							stringBuilder6.AppendLine();
						}
					}
					stringBuilder6.Append("object[] args = {");
					for (int q = 0; q < instanceMethod.Value.maxParameterCount; q++)
					{
						if (q > 0)
						{
							stringBuilder6.Append(",");
						}
						if (q > 0 | flag)
						{
							sb.Append(",");
						}
						stringBuilder6.Append("a").Append(q);
						sb.Append("[Optional] object ").Append("a").Append(q);
					}
					sb.Append("){");
					stringBuilder6.Append("}");
					sb.AppendLine();
					sb.Append(stringBuilder6).Append(";");
					sb.AppendLine();
					sb.Append("var m = typeD.methods[").Append(instanceMethod.Value.methodOrder).Append("];");
					sb.AppendLine();
					sb.AppendLine("try{");
					sb.Append("var method = m.getMethodForParameters(ref args);");
					sb.AppendLine();
					sb.AppendLine("if(typeDescriptor.isSpecialMethod(method)){");
					sb.AppendLine("return __process(method.Invoke(wrappedObject,args));");
					sb.AppendLine("}else{");
					sb.AppendLine("object ret= null;");
					sb.AppendLine("System.Reflection.MethodInfo mi= (System.Reflection.MethodInfo) method;");
					sb.AppendLine("if(mi.ReturnType == typeof(void))");
					sb.AppendLine("__invoker.invokeMethodVoid(wrappedObject,mi.Name,args);");
					sb.AppendLine("else");
					sb.AppendLine("ret= __invoker.invokeMethod(wrappedObject,mi.Name,args);");
					sb.AppendLine("return __process(ret); }");
					sb.AppendLine("}catch(Exception e){if(e.InnerException!=null){throw e.InnerException;}throw e;}");
					sb.AppendLine();
					sb.AppendLine("}");
				}
			}
			sb.AppendLine("/* PROPIEDADES  */");
			foreach (KeyValuePair<string, propertyDescriptor> instanceProperty in this.instanceProperties)
			{
				sb.Append("public object ");
				if (instanceProperty.Value.maxParameterCount > 0)
				{
					sb.Append("this[");
				}
				else
				{
					sb.Append(instanceProperty.Value.name);
				}
				StringBuilder stringBuilder7 = new StringBuilder();
				if (instanceProperty.Value.maxParameterCount > 0)
				{
					stringBuilder7.Append("object[] args = {");
					for (int r = 0; r < instanceProperty.Value.maxParameterCount; r++)
					{
						if (r > 0)
						{
							stringBuilder7.Append(",");
							sb.Append(",");
						}
						stringBuilder7.Append("a").Append(r);
						sb.Append("[Optional] object ").Append("a").Append(r);
					}
					sb.Append("]");
					stringBuilder7.Append("};");
				}
				else
				{
					stringBuilder7.Append("object[] args = {};");
				}
				sb.Append("{");
				sb.AppendLine();
				sb.AppendLine("get{");
				sb.Append(stringBuilder7);
				sb.AppendLine();
				sb.Append("var m = typeD.properties[").Append(instanceProperty.Value.propertyOrder).Append("];");
				sb.AppendLine();
				sb.AppendLine("try{");
				sb.Append("var method = m.getPropertyForParameters(ref args);");
				sb.AppendLine();
				sb.Append("object o= method.GetValue(wrappedObject,args);");
				sb.AppendLine("return __process(o);");
				sb.AppendLine("}catch(Exception e){if(e.InnerException!=null){throw e.InnerException;}throw e;}");
				sb.AppendLine();
				sb.AppendLine("}");
				sb.AppendLine("set{");
				sb.Append(stringBuilder7);
				sb.AppendLine();
				sb.Append("var m = typeD.properties[").Append(instanceProperty.Value.propertyOrder).Append("];");
				sb.AppendLine();
				sb.Append("var method = m.getPropertyForParameters(ref args);");
				sb.AppendLine();
				sb.AppendLine("try{");
				sb.AppendLine("if(value is wrapper){value=((wrapper)value).wrappedObject;}");
				sb.Append("method.SetValue(wrappedObject,value,args);");
				sb.AppendLine("}catch(Exception e){if(e.InnerException!=null){throw e.InnerException;}throw e;}");
				sb.AppendLine();
				sb.AppendLine("}");
				sb.AppendLine("}");
			}
			sb.AppendLine("}}");
			staticClass = str1;
			instanceClass = str;
		}

		public void setCompiledWrapper(wrapperStatic ww)
		{
			this.compiled = true;
			this.compiledWrapper = ww;
		}

		public void setProperty(object o, string property, object value, params object[] args)
		{
			PropertyInfo propertyForParameters = this.getPropertyForParameters(property, ref args);
			propertyForParameters.SetValue(o, value, args);
		}

		public void setStaticProperty(object o, string property, object value, params object[] args)
		{
			PropertyInfo staticPropertyForParameters = this.getStaticPropertyForParameters(property, ref args);
			staticPropertyForParameters.SetValue(o, value, args);
		}
	}
}