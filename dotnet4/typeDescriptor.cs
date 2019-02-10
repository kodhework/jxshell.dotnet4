using jxshell;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;

namespace jxshell.dotnet4
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
			jxshell.dotnet4.methodDescriptor methodDescriptor;
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
				if (!methodInfo.Name.StartsWith("get_") && !methodInfo.Name.StartsWith("set_"))
				{
					jxshell.dotnet4.methodDescriptor value = null;
					if (methodInfo.IsGenericMethod)
					{
						string text = string.Concat("generic_", methodInfo.Name);
						if (!this.instanceMethods.TryGetValue(text, out value))
						{
							value = new jxshell.dotnet4.methodDescriptor()
							{
								isGenericMethod = true
							};
							this.instanceMethods[text] = value;
							this.methods.Add(value);
							value.methodOrder = this.methods.Count - 1;
							value.name = text;
						}
					}
					else if (!this.instanceMethods.TryGetValue(methodInfo.Name, out value))
					{
						value = new jxshell.dotnet4.methodDescriptor();
						this.instanceMethods[methodInfo.Name] = value;
						this.methods.Add(value);
						value.methodOrder = this.methods.Count - 1;
						value.name = methodInfo.Name;
					}
					value.baseMethods.Add(methodInfo);
					value.maxParameterCount = Math.Max(value.maxParameterCount, (int)methodInfo.GetParameters().Length);
					if (methodInfo.IsGenericMethod)
					{
						value.genericParameterCount = Math.Max(value.genericParameterCount, (int)methodInfo.GetGenericArguments().Length);
					}
				}
			}
			PropertyInfo[] properties = t.GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.FlattenHierarchy);
			for (int j = 0; j < (int)properties.Length; j++)
			{
				PropertyInfo propertyInfo = properties[j];
				propertyDescriptor value2 = null;
				if (!this.instanceProperties.TryGetValue(propertyInfo.Name, out value2))
				{
					value2 = new propertyDescriptor();
					this.instanceProperties[propertyInfo.Name] = value2;
					this.properties.Add(value2);
					value2.propertyOrder = this.properties.Count - 1;
					value2.name = propertyInfo.Name;
				}
				value2.properties.Add(propertyInfo);
				value2.maxParameterCount = Math.Max(value2.maxParameterCount, (int)propertyInfo.GetIndexParameters().Length);
			}
			FieldInfo[] fields = t.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.FlattenHierarchy | BindingFlags.GetField | BindingFlags.SetField);
			for (int k = 0; k < (int)fields.Length; k++)
			{
				FieldInfo fieldInfo = fields[k];
				fieldDescriptor value3 = null;
				if (!this.instanceFields.TryGetValue(fieldInfo.Name, out value3))
				{
					value3 = new fieldDescriptor();
					this.instanceFields[fieldInfo.Name] = value3;
					this.fields.Add(value3);
					value3.fieldOrder = this.fields.Count - 1;
					value3.name = fieldInfo.Name;
				}
				value3.fieldInfo = fieldInfo;
			}
			ConstructorInfo[] constructors = t.GetConstructors();
			for (int l = 0; l < (int)constructors.Length; l++)
			{
				ConstructorInfo constructorInfo = constructors[l];
				if (this.constructor != null)
				{
					methodDescriptor = this.constructor;
				}
				else
				{
					jxshell.dotnet4.methodDescriptor _methodDescriptor = new jxshell.dotnet4.methodDescriptor();
					jxshell.dotnet4.methodDescriptor _methodDescriptor1 = _methodDescriptor;
					this.constructor = _methodDescriptor;
					methodDescriptor = _methodDescriptor1;
					this.methods.Add(methodDescriptor);
					methodDescriptor.name = "construct";
					methodDescriptor.methodOrder = this.methods.Count - 1;
				}
				methodDescriptor.baseMethods.Add(constructorInfo);
				methodDescriptor.maxParameterCount = Math.Max(methodDescriptor.maxParameterCount, (int)constructorInfo.GetParameters().Length);
			}
			MethodInfo[] methodInfoArray = t.GetMethods(BindingFlags.Static | BindingFlags.Public | BindingFlags.FlattenHierarchy | BindingFlags.InvokeMethod);
			for (int m = 0; m < (int)methodInfoArray.Length; m++)
			{
				MethodInfo methodInfo2 = methodInfoArray[m];
				if (!methodInfo2.Name.StartsWith("get_") && !methodInfo2.Name.StartsWith("set_"))
				{
					jxshell.dotnet4.methodDescriptor value4 = null;
					if (methodInfo2.IsGenericMethod)
					{
						string text2 = string.Concat("generic_", methodInfo2.Name);
						if (!this.instanceMethods.TryGetValue(text2, out value4))
						{
							value4 = new jxshell.dotnet4.methodDescriptor();
							this.staticMethods[text2] = value4;
							value4.isGenericMethod = true;
							this.methods.Add(value4);
							value4.methodOrder = this.methods.Count - 1;
							value4.name = text2;
						}
					}
					else if (!this.staticMethods.TryGetValue(methodInfo2.Name, out value4))
					{
						value4 = new jxshell.dotnet4.methodDescriptor();
						this.staticMethods[methodInfo2.Name] = value4;
						this.methods.Add(value4);
						value4.name = methodInfo2.Name;
						value4.methodOrder = this.methods.Count - 1;
					}
					value4.baseMethods.Add(methodInfo2);
					value4.maxParameterCount = Math.Max(value4.maxParameterCount, (int)methodInfo2.GetParameters().Length);
					if (methodInfo2.IsGenericMethod)
					{
						value4.genericParameterCount = Math.Max(value4.genericParameterCount, (int)methodInfo2.GetGenericArguments().Length);
					}
				}
			}
			PropertyInfo[] propertyInfoArray = t.GetProperties(BindingFlags.Static | BindingFlags.Public | BindingFlags.FlattenHierarchy);
			for (int n = 0; n < (int)propertyInfoArray.Length; n++)
			{
				PropertyInfo propertyInfo2 = propertyInfoArray[n];
				propertyDescriptor value5 = null;
				if (!this.staticProperties.TryGetValue(propertyInfo2.Name, out value5))
				{
					value5 = new propertyDescriptor();
					this.staticProperties[propertyInfo2.Name] = value5;
					this.properties.Add(value5);
					value5.propertyOrder = this.properties.Count - 1;
					value5.name = propertyInfo2.Name;
				}
				value5.properties.Add(propertyInfo2);
				value5.maxParameterCount = Math.Max(value5.maxParameterCount, (int)propertyInfo2.GetIndexParameters().Length);
			}
			FieldInfo[] fieldInfoArray = t.GetFields(BindingFlags.Static | BindingFlags.Public | BindingFlags.FlattenHierarchy | BindingFlags.GetField | BindingFlags.SetField);
			for (int o = 0; o < (int)fieldInfoArray.Length; o++)
			{
				FieldInfo fieldInfo2 = fieldInfoArray[o];
				fieldDescriptor value6 = null;
				if (!this.instanceFields.TryGetValue(fieldInfo2.Name, out value6))
				{
					value6 = new fieldDescriptor();
					this.staticFields[fieldInfo2.Name] = value6;
					this.fields.Add(value6);
					value6.fieldOrder = this.fields.Count - 1;
					value6.name = fieldInfo2.Name;
				}
				value6.fieldInfo = fieldInfo2;
			}
			this.type = t;
			typeDescriptor.loadedTypes[t] = this;
			if (compile && typeDescriptor.gencompile)
			{
				this.compile();
			}
		}

		public static void addUsingsStatements(StringBuilder sb)
		{
			sb.AppendLine("using System;");
			sb.AppendLine("using System.Runtime.InteropServices;");
			sb.AppendLine("using System.Reflection;");
			sb.AppendLine("using jxshell.dotnet4;");
		}

		public wrapperStatic compile()
		{
			if (this.compiled)
			{
				return this.compiledWrapper;
			}
			StringBuilder stringBuilder = new StringBuilder();
			string staticClass = "";
			string instanceClass = "";
			typeDescriptor.addUsingsStatements(stringBuilder);
			this.precompile(stringBuilder, ref staticClass, ref instanceClass);
			stringBuilder.AppendLine("class program{public static void main(){}}");
			try
			{
				jxshell.csharplanguage csharplanguage = typeDescriptor.language;
				csharplanguage.runScript(stringBuilder.ToString(), typeDescriptor.generateInMemory);
				Type type = csharplanguage.getCompiledAssembly().GetType(string.Concat("jxshell.dotnet4.", staticClass));
				ConstructorInfo constructorInfo = type.GetConstructor(new Type[] { typeof(Type), typeof(typeDescriptor) });
				this.compiledWrapper = (wrapperStatic)constructorInfo.Invoke(new object[] { this.type, this });
				this.compiled = true;
			}
			catch (Exception exception)
			{
				Exception ex = exception;
				throw new Exception(string.Concat("No se puede obtener un wrapper para el tipo ", this.type.ToString(), ". ", ex.Message), ex);
			}
			return this.compiledWrapper;
		}

		public MethodBase getMethodForParameters(string method, ref object[] parameters)
		{
			return this.instanceMethods[method].getMethodForParameters(ref parameters, null);
		}

		public static string getNameForType(Type t)
		{
			StringBuilder stringBuilder = new StringBuilder();
			if (!t.IsGenericType)
			{
				return t.ToString().Replace("+", ".").Replace("&", "");
			}
			string text = t.ToString();
			text = text.Substring(0, text.IndexOf("`"));
			stringBuilder.Append(text);
			stringBuilder.Append("<");
			Type[] genericArguments = t.GetGenericArguments();
			bool flag = false;
			Type[] typeArray = genericArguments;
			for (int i = 0; i < (int)typeArray.Length; i++)
			{
				Type t2 = typeArray[i];
				if (flag)
				{
					stringBuilder.Append(",");
				}
				else
				{
					flag = true;
				}
				stringBuilder.Append(typeDescriptor.getNameForType(t2));
			}
			stringBuilder.Append(">");
			return stringBuilder.ToString();
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

		internal static void loadEvaluator()
		{
		}

		public static typeDescriptor loadFromType(Type t)
		{
			typeDescriptor value;
			if (typeDescriptor.loadedTypes.TryGetValue(t, out value))
			{
				return value;
			}
			return new typeDescriptor(t);
		}

		public static typeDescriptor loadFromType(Type t, string typeName, bool compile = true)
		{
			typeDescriptor value;
			if (typeDescriptor.loadedTypes.TryGetValue(t, out value))
			{
				return value;
			}
			return new typeDescriptor(t, typeName, compile);
		}

		public void precompile(StringBuilder sb, ref string staticClass, ref string instanceClass)
		{
			sb.AppendLine("namespace jxshell.dotnet4{");
			string text = string.Concat("_", environment.uniqueId());
			string text2 = string.Concat("_", environment.uniqueId());
			sb.AppendLine();
			sb.AppendLine("[ComVisible(true)]");
			sb.Append("public class ").Append(text2).Append(" : ");
			if (!typeof(MulticastDelegate).IsAssignableFrom(this.type.BaseType))
			{
				sb.Append("wrapperStatic{");
			}
			else
			{
				sb.Append("delegateWrapperStatic{");
			}
			sb.AppendLine();
			sb.Append("public ").Append(text2).Append("(Type t, typeDescriptor td):base(t,td){");
			if (this.type.IsEnum)
			{
				sb.Append("__initEnum();");
			}
			sb.Append("}");
			sb.AppendLine();
			sb.Append("public override ").Append("wrapper").Append(" getWrapper(object o){return new ").Append(text).Append("(o,typeD);}");
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
			foreach (KeyValuePair<string, jxshell.dotnet4.methodDescriptor> staticMethod in this.staticMethods)
			{
				sb.Append("public object ").Append(staticMethod.Value.name).Append("(");
				StringBuilder stringBuilder = new StringBuilder();
				bool isGenericMethod = staticMethod.Value.isGenericMethod;
				if (isGenericMethod)
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
					if (j > 0 | isGenericMethod)
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
				if (!isGenericMethod)
				{
					sb.Append("var method = m.getMethodForParameters(ref args);");
				}
				else
				{
					sb.Append("var method = m.getGenericMethodForParameters(genericArguments.ToArray(), ref args);");
				}
				sb.AppendLine();
				sb.AppendLine("try{");
				sb.Append("return __process(method.Invoke(null,args));");
				sb.AppendLine("}catch(Exception e){if(e.InnerException!=null){throw e.InnerException;}throw e;}");
				sb.AppendLine();
				sb.AppendLine("}");
			}
			sb.AppendLine("/* PROPIEDADES  */");
			foreach (KeyValuePair<string, propertyDescriptor> staticProperty in this.staticProperties)
			{
				sb.Append("public object ");
				if (staticProperty.Value.maxParameterCount <= 0)
				{
					sb.Append(staticProperty.Value.name);
				}
				else
				{
					sb.Append("this[");
				}
				StringBuilder stringBuilder2 = new StringBuilder();
				if (staticProperty.Value.maxParameterCount <= 0)
				{
					stringBuilder2.Append("object[] args = {};");
				}
				else
				{
					stringBuilder2.Append("object[] args = {");
					for (int k = 0; k < staticProperty.Value.maxParameterCount; k++)
					{
						if (k > 0)
						{
							stringBuilder2.Append(",");
							sb.Append(",");
						}
						stringBuilder2.Append("a").Append(k);
						sb.Append("[Optional] object ").Append("a").Append(k);
					}
					sb.Append("]");
					stringBuilder2.Append("};");
				}
				sb.Append("{");
				sb.AppendLine();
				sb.AppendLine("get{");
				sb.Append(stringBuilder2);
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
				sb.Append(stringBuilder2);
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
				StringBuilder stringBuilder3 = new StringBuilder();
				stringBuilder3.Append("Type ty = typeof(").Append(typeDescriptor.getNameForType(this.type)).Append(");");
				stringBuilder3.Append("Array values = global::System.Enum.GetValues(ty);");
				stringBuilder3.AppendLine();
				int num = 0;
				string[] strArrays = names;
				for (int l = 0; l < (int)strArrays.Length; l++)
				{
					string value = strArrays[l];
					sb.Append("public ").Append(text).Append(" ").Append(value).Append("= new ").Append(text).Append("();").AppendLine();
					stringBuilder3.Append(value).Append(".wrappedObject = values.GetValue(").Append(num).Append(");");
					stringBuilder3.AppendLine();
					stringBuilder3.Append(value).Append(".wrappedType = ty;");
					stringBuilder3.AppendLine();
					stringBuilder3.Append(value).Append(".typeD = typeD;");
					stringBuilder3.AppendLine();
					num++;
				}
				sb.Append("public void __initEnum(){").AppendLine().Append(stringBuilder3.ToString()).AppendLine().Append("}");
			}
			jxshell.dotnet4.methodDescriptor methodDescriptor = this.constructor;
			if (methodDescriptor != null)
			{
				if (!typeof(MulticastDelegate).IsAssignableFrom(this.type.BaseType))
				{
					sb.Append("public object ").Append(methodDescriptor.name).Append("(");
					StringBuilder stringBuilder4 = new StringBuilder();
					stringBuilder4.Append("object[] args = {");
					for (int m = 0; m < methodDescriptor.maxParameterCount; m++)
					{
						if (m > 0)
						{
							stringBuilder4.Append(",");
							sb.Append(",");
						}
						stringBuilder4.Append("a").Append(m);
						sb.Append("[Optional] object ").Append("a").Append(m);
					}
					sb.Append("){");
					stringBuilder4.Append("}");
					sb.AppendLine();
					sb.Append(stringBuilder4).Append(";");
					sb.AppendLine();
					sb.Append("var m = typeD.methods[").Append(methodDescriptor.methodOrder).Append("];");
					sb.AppendLine();
					sb.Append("var method = m.getConstructorForParameters(ref args);");
					sb.AppendLine();
					sb.AppendLine("try{");
					sb.Append("return getWrapper(method.Invoke(args));");
					sb.AppendLine("}catch(Exception e){if(e.InnerException!=null){throw e.InnerException;}throw e;}");
					sb.AppendLine();
					sb.AppendLine("}");
				}
				else
				{
					sb.Append("public override delegateWrapper getThisWrapper(){");
					sb.Append("var o = new ").Append(text).Append("();return o;");
					sb.Append("}");
				}
			}
			sb.AppendLine("}");
			sb.AppendLine();
			sb.AppendLine("[ComVisible(true)]");
			sb.Append("public class ").Append(text).Append(" : ");
			if (this.type.IsEnum)
			{
				sb.Append("enumW");
			}
			else if (!typeof(MulticastDelegate).IsAssignableFrom(this.type.BaseType))
			{
				sb.Append("w");
			}
			else
			{
				sb.Append("delegateW");
			}
			sb.Append("rapper{").AppendLine();
			sb.Append("public ").Append(text).Append("(object o, typeDescriptor td):base(o,td){}");
			sb.Append("public ").Append(text).Append("():base(){}");
			sb.AppendLine();
			typeof(MulticastDelegate).IsAssignableFrom(this.type.BaseType);
			sb.AppendLine();
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
			foreach (KeyValuePair<string, jxshell.dotnet4.methodDescriptor> instanceMethod in this.instanceMethods)
			{
				if (!typeof(MulticastDelegate).IsAssignableFrom(this.type.BaseType) || !(instanceMethod.Value.name == "Invoke"))
				{
					sb.Append("public object ").Append(instanceMethod.Value.name).Append("(");
					StringBuilder stringBuilder7 = new StringBuilder();
					bool isGenericMethod2 = instanceMethod.Value.isGenericMethod;
					if (isGenericMethod2)
					{
						stringBuilder7.Append("System.Collections.Generic.List<System.Type> genericArguments=new System.Collections.Generic.List<System.Type>(1);");
						for (int num3 = 0; num3 < instanceMethod.Value.genericParameterCount; num3++)
						{
							if (num3 > 0)
							{
								sb.Append(",");
							}
							stringBuilder7.Append("if(type").Append(num3).Append("!=null){if(type").Append(num3);
							stringBuilder7.Append(" is wrapper){genericArguments.Add((System.Type)(((wrapper)type").Append(num3).Append(").wrappedObject));}else{");
							stringBuilder7.Append("genericArguments.Add(Manager.lastManager.getTypeOrGenericType(type").Append(num3).Append(".ToString()));}").Append("}");
							sb.Append("[Optional] object ").Append("type").Append(num3);
							stringBuilder7.AppendLine();
						}
					}
					stringBuilder7.Append("object[] args = {");
					for (int num4 = 0; num4 < instanceMethod.Value.maxParameterCount; num4++)
					{
						if (num4 > 0)
						{
							stringBuilder7.Append(",");
						}
						if (num4 > 0 | isGenericMethod2)
						{
							sb.Append(",");
						}
						stringBuilder7.Append("a").Append(num4);
						sb.Append("[Optional] object ").Append("a").Append(num4);
					}
					sb.Append("){");
					stringBuilder7.Append("}");
					sb.AppendLine();
					sb.Append(stringBuilder7).Append(";");
					sb.AppendLine();
					sb.Append("var m = typeD.methods[").Append(instanceMethod.Value.methodOrder).Append("];");
					sb.AppendLine();
					sb.AppendLine("try{");
					sb.Append("var method = m.getMethodForParameters(ref args);");
					sb.AppendLine();
					sb.Append("return __process(method.Invoke(wrappedObject,args));");
					sb.AppendLine("}catch(Exception e){if(e.InnerException!=null){throw e.InnerException;}throw e;}");
					sb.AppendLine();
					sb.AppendLine("}");
				}
				else
				{
					sb.Append("public object call(");
					StringBuilder stringBuilder5 = new StringBuilder();
					StringBuilder stringBuilder6 = new StringBuilder();
					stringBuilder5.Append("");
					for (int n = 0; n < instanceMethod.Value.maxParameterCount; n++)
					{
						if (n > 0)
						{
							stringBuilder5.Append(",");
							stringBuilder6.Append(",");
							sb.Append(",");
						}
						stringBuilder6.Append("wrapper.getFromObject(a").Append(n).Append(")");
						stringBuilder5.Append("a").Append(n);
						sb.Append("[Optional] object ").Append("a").Append(n);
					}
					sb.Append("){");
					sb.AppendLine();
					int maxParameterCount = instanceMethod.Value.maxParameterCount;
					sb.Append("invokerparam").Append(maxParameterCount).Append(" invoker = new invokerparam").Append(maxParameterCount).Append("(__internalMethod);");
					sb.Append("object o =null;");
					MethodInfo methodInfo = (MethodInfo)instanceMethod.Value.baseMethods[0];
					if (methodInfo.ReturnType != typeof(void))
					{
						sb.Append("o=invoker.invoke");
					}
					else
					{
						sb.Append("invoker.invokeasVoid");
					}
					sb.Append("(__internalTarget");
					if (instanceMethod.Value.maxParameterCount > 0)
					{
						sb.Append(",");
					}
					sb.Append(stringBuilder5.ToString()).Append(");");
					sb.AppendLine();
					sb.AppendLine("return o;}");
					sb.Append("public ");
					if (methodInfo.ReturnType != typeof(void))
					{
						sb.Append(typeDescriptor.getNameForType(methodInfo.ReturnType));
					}
					else
					{
						sb.Append("void ");
					}
					sb.Append(" __internalInvoke(");
					ParameterInfo[] parameters = methodInfo.GetParameters();
					for (int num2 = 0; num2 < (int)parameters.Length; num2++)
					{
						ParameterInfo parameterInfo = parameters[num2];
						if (num2 > 0)
						{
							sb.Append(",");
						}
						string value2 = (!parameterInfo.ParameterType.IsPointer ? typeDescriptor.getNameForType(parameterInfo.ParameterType) : "object");
						sb.Append(value2).Append(" a").Append(num2);
					}
					sb.Append("){");
					sb.AppendLine();
					if (methodInfo.ReturnType != typeof(void))
					{
						sb.Append("object o= ");
						sb.Append("call(").Append(stringBuilder6.ToString()).Append(");");
						sb.AppendLine();
						sb.Append("if(o is wrapper){o = ((wrapper)o).wrappedObject;}");
						sb.AppendLine();
						sb.Append("return (").Append(typeDescriptor.getNameForType(methodInfo.ReturnType)).Append(")o;");
						sb.AppendLine();
					}
					else
					{
						sb.Append("call(").Append(stringBuilder6.ToString()).Append(");");
					}
					sb.Append("}\n");
				}
			}
			sb.AppendLine("/* PROPIEDADES  */");
			foreach (KeyValuePair<string, propertyDescriptor> instanceProperty in this.instanceProperties)
			{
				sb.Append("public object ");
				if (instanceProperty.Value.maxParameterCount <= 0)
				{
					sb.Append(instanceProperty.Value.name);
				}
				else
				{
					sb.Append("this[");
				}
				StringBuilder stringBuilder8 = new StringBuilder();
				if (instanceProperty.Value.maxParameterCount <= 0)
				{
					stringBuilder8.Append("object[] args = {};");
				}
				else
				{
					stringBuilder8.Append("object[] args = {");
					for (int num5 = 0; num5 < instanceProperty.Value.maxParameterCount; num5++)
					{
						if (num5 > 0)
						{
							stringBuilder8.Append(",");
							sb.Append(",");
						}
						stringBuilder8.Append("a").Append(num5);
						sb.Append("[Optional] object ").Append("a").Append(num5);
					}
					sb.Append("]");
					stringBuilder8.Append("};");
				}
				sb.Append("{");
				sb.AppendLine();
				sb.AppendLine("get{");
				sb.Append(stringBuilder8);
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
				sb.Append(stringBuilder8);
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
			staticClass = text2;
			instanceClass = text;
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