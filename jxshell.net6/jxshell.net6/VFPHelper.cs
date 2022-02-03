using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace jxshell.net6
{
    [ComVisible(true)]
    public class VFPHelper
    {
        public VFPHelper(typeDescriptor typeD)
        {
            this.typeD = typeD;
        }

        public typeDescriptor typeD;
        public Type[] genericTypes;


        public Ast Compile()
        {
            StringBuilder sb = new StringBuilder();
            StringBuilder sbi = new StringBuilder();

            StringBuilder props = new StringBuilder();
            StringBuilder iprops = new StringBuilder();
            if (!typeD.type.IsEnum)
            {
                foreach (KeyValuePair<string, fieldDescriptor> staticField in typeD.staticFields)
                {
                    string str1 = CompileField(staticField.Value.fieldOrder, staticField.Value.name);
                    sb.AppendLine(str1);
                    props.AppendLine(staticField.Value.name).Append("=null");
                }
            }
            foreach (KeyValuePair<string, jxshell.net6.methodDescriptor> staticMethod in typeD.staticMethods)
            {

                if (staticMethod.Value.isGenericMethod)
                {
                    
                }
                string str1 = CompileMethod(staticMethod.Value.methodOrder, staticMethod.Value.name, staticMethod.Value.isGenericMethod, "method");
                sb.AppendLine(str1);
                
            }
            foreach (KeyValuePair<string, propertyDescriptor> staticProperty in typeD.staticProperties)
            {
                string str1 = CompileMethod(staticProperty.Value.propertyOrder, staticProperty.Value.name + "_access", false, "property.get");
                string str2 = CompileMethod(staticProperty.Value.propertyOrder, staticProperty.Value.name + "_assign", false, "property.set");
                sb.AppendLine(str1);
                sb.AppendLine(str2);
                props.Append(staticProperty.Value.name).AppendLine("=null");
            }

            if (typeD.constructor != null)
            {
                string str1 = CompileMethod(typeD.constructor.methodOrder, "construct", typeD.constructor.isGenericMethod, "constructor");
                sb.AppendLine(str1);
            }

            if (typeD.type.IsEnum)
            {
                string[] names = Enum.GetNames(typeD.type);
                StringBuilder stringBuilder3 = new StringBuilder();
                Array values = System.Enum.GetValues(typeD.type);


                int num = 0;
                string[] strArrays = names;
                StringBuilder sb2 = new StringBuilder();
                sb2.AppendLine("function init_class()");
                for (int l = 0; l < (int)strArrays.Length; l++)
                {
                    string value = strArrays[l];

                    props.Append(value).AppendLine("=null");
                    sb2.Append("this.").Append(value).Append("= this.___create(").Append((int)values.GetValue(l)).AppendLine(")");

                    num++;
                }
                sb2.AppendLine("endfunc");
                sb.Append(sb2);
            }

            foreach (KeyValuePair<string, fieldDescriptor> staticField in typeD.instanceFields)
            {
                string str1 = CompileField(staticField.Value.fieldOrder, staticField.Value.name);
                sbi.AppendLine(str1);
                iprops.Append(staticField.Value.name).AppendLine("=null");
            }

            foreach (KeyValuePair<string, jxshell.net6.methodDescriptor> staticMethod in typeD.instanceMethods)
            {

                if (staticMethod.Value.isGenericMethod)
                {

                }
                string str1 = CompileMethod(staticMethod.Value.methodOrder, staticMethod.Value.name, staticMethod.Value.isGenericMethod, "method");
                sbi.AppendLine(str1);

            }

            foreach (KeyValuePair<string, propertyDescriptor> staticProperty in typeD.instanceProperties)
            {
                string str1 = CompileMethod(staticProperty.Value.propertyOrder, staticProperty.Value.name + "_access", false, "property.get");
                string str2 = CompileMethod(staticProperty.Value.propertyOrder, staticProperty.Value.name + "_assign", false, "property.set");
                sbi.AppendLine(str1);
                sbi.AppendLine(str2);
                iprops.Append(staticProperty.Value.name).AppendLine("=null");
            }

            string text = string.Concat("_", environment.uniqueId());
            string text2 = string.Concat("_", environment.uniqueId());

            StringBuilder classDef = new StringBuilder();
            classDef.Append("define class ").Append(text).AppendLine(" as kodnetWrapper ");
            classDef.Append("  __instance_name=\"").Append(text2).AppendLine("\"");
            classDef.Append(props);
            classDef.AppendLine("function ___create(p1)");
            classDef.AppendLine("return createobject(this.__instance_name, this.__helper, m.p1)");
            classDef.AppendLine("endfunc");
            classDef.Append(sb);
            classDef.AppendLine("enddefine");

            classDef.Append("define class ").Append(text2).AppendLine(" as kodnetWrapper ");
            classDef.Append("  __static_name=\"").Append(text).AppendLine("\"");
            classDef.Append(iprops);
            classDef.Append(sbi);
            classDef.AppendLine("enddefine");


            var ast = new Ast();
            ast.code = classDef.ToString();
            ast.staticName = text;
            ast.instanceName = text2;
            return ast;


            
        }

        public string CompileField(int index, string name)
        {
            var str = @"function $name_access()
            local index, value
            $dynamic
            value= this.__helper.FieldGetValue(index, this.___obj)
            return _screen.kodnetManager.getWrapped(m.value)
            endfunc";


            var str1 = @"function $name_assign(value)
            local index, value
            $dynamic
            value= _screen.kodnetManager.getUnwrapped()
            this.__helper.FieldSetValue(index, this.___obj, m.value)
            endfunc";

            StringBuilder sb = new StringBuilder();
            sb.Append("index=").Append(index).AppendLine();
            str = str.Replace("$dynamic", sb.ToString()).Replace("$name", name);
            str1 = str1.Replace("$dynamic", sb.ToString()).Replace("$name", name);
            sb.Clear();
            sb.AppendLine(str );
            sb.AppendLine(str1);
            return sb.ToString();




        }

        public string CompileMethod(int index, string name, bool generic, string methodtype)
        {
            var str = @"FUNCTION $name($value arg1, arg2, arg3,arg4,arg5,arg6,arg7,arg8,arg9,arg10)
		LOCAL pcount1, value, index, target, generic
		$dynamic
        pcount1= PCOUNT()	
        if m.pcount1 > 0
            m.arg1= _screen.kodnetManager.getUnwrapped(m.arg1)
        endif
        if m.pcount1 > 1
            m.arg2= _screen.kodnetManager.getUnwrapped(m.arg2)
        endif
        if m.pcount1 > 2
            m.arg3= _screen.kodnetManager.getUnwrapped(m.arg3)
        endif
        if m.pcount1 > 3
            m.arg4= _screen.kodnetManager.getUnwrapped(m.arg4)
        endif
        if m.pcount1 > 4
            m.arg5= _screen.kodnetManager.getUnwrapped(m.arg5)
        endif
        if m.pcount1 > 5
            m.arg6= _screen.kodnetManager.getUnwrapped(m.arg6)
        endif
if m.pcount1 > 6
            m.arg7= _screen.kodnetManager.getUnwrapped(m.arg7)
        endif   
if m.pcount1 > 7
            m.arg8= _screen.kodnetManager.getUnwrapped(m.arg8)
        endif
        if m.pcount1 > 8
            m.arg9= _screen.kodnetManager.getUnwrapped(m.arg9)
        endif
if m.pcount1 > 9
            m.arg10= _screen.kodnetManager.getUnwrapped(m.arg10)
        endif
		
			
		IF m.pcount1 > 9
			value= this.__helper.$method m.arg1;
				,arg2,arg3,arg4,arg5,arg6,arg7,arg8,arg9,arg10 $valu0)
		ELSE 
			IF m.pcount1 > 8
				value= this.__helper..$method m.arg1;
					,arg2,arg3,arg4,arg5,arg6,arg7,arg8,arg9 $valu0)
			ELSE 
				IF m.pcount1 > 7
					value= this.__helper.$method m.arg1;
						,arg2,arg3,arg4,arg5,arg6,arg7,arg8 $valu0)
				ELSE 
					IF m.pcount1 > 6
						value= this.__helper.$method m.arg1;
							,arg2,arg3,arg4,arg5,arg6,arg7 $valu0)
					ELSE 
						IF m.pcount1 > 5
							value= this.__helper.$method m.arg1;
								,arg2,arg3,arg4,arg5,arg6 $valu0)
						ELSE 
							IF m.pcount1 > 4
								value= this.__helper.$method m.arg1;
									,arg2, arg3, arg4, arg5 $valu0)
							ELSE 
								IF m.pcount1 > 3
									value= this.__helper.$method m.arg1;
										,arg2, arg3, arg4 $valu0)
								ELSE 
									IF m.pcount1 > 2
										value= this.__helper.$method m.arg1;
											,arg2, arg3 $valu0)
									ELSE 
										IF m.pcount1 > 1
											value= this.__helper.$method m.arg1, m.arg2 $valu0)
										ELSE 
											IF m.pcount1 > 0
												value= this.__helper.$method m.arg1 $valu0)
											ELSE 
												value= this.__helper.$methox $valu0)
											ENDIF 
										ENDIF 
									ENDIF 
								ENDIF 
							ENDIF 
						ENDIF 
					ENDIF 
				ENDIF 
			ENDIF 
		ENDIF 
		
        if !ISNULL(m.value) and vartype(m.value)=='O' and m.value.isthis
            return m.target
        endif 
        value= _screen.kodnetManager.getWrapped(m.value)
        return m.value

	ENDFUNC";

            string k = "", l="", m="";
            if(methodtype == "method" || methodtype == "constructor")
            {
                k = "InvokeMetavalueForParameters(index,target,generic,";
                l = "InvokeMetavalueForParameters(index,target,generic";
            }
            else if(methodtype == "property.set")
            {
                k = "PropertyMetavalueForParameters(m.index, m.target, .T., ";
                l= "PropertyMetavalueForParameters(m.index, m.target, .T.,";
                m = "m.value1";
            }
            else if(methodtype == "property.get")
            {
                k = "PropertyMetavalueForParameters(m.index, m.target, .F., ";
                l = "PropertyMetavalueForParameters(m.index, m.target, .F.";
            }
            

            StringBuilder sb = new StringBuilder();
            sb.Append("index=").Append(index).AppendLine();
            sb.AppendLine("target=this.___obj");
            sb.Append("generic=").AppendLine(generic ? ".T" : ".F.");
            str= str.Replace("$dynamic", sb.ToString()).Replace("$name", name)
                .Replace("$value", methodtype == "property.set" ? "value1" : "")
                .Replace("$method", k)
                .Replace("$valu0", m)
                .Replace("$methox", l);
            return str;

        }






        public void SetGenericParameters([Optional]object arg1, [Optional] object arg2, [Optional] object arg3, [Optional] object arg4,
            [Optional] object arg5, [Optional] object arg6, [Optional] object arg7, [Optional] object arg8, [Optional] object arg9, [Optional] object arg10)
        {

            Type[] args = new Type[10];
            int count = 0;
            if (!(arg1 is System.Reflection.Missing))
            {
                args[count] = (Type)arg1;
                count++;
                if (!(arg2 is System.Reflection.Missing))
                {
                    args[count] = (Type)arg2;
                    count++;
                    if (!(arg3 is System.Reflection.Missing))
                    {
                        args[count] = (Type)arg3;
                        count++;
                        if (!(arg4 is System.Reflection.Missing))
                        {
                            args[count] = (Type)arg4;
                            count++;
                            if (!(arg5 is System.Reflection.Missing))
                            {
                                args[count] = (Type)arg5;
                                count++;
                                if (!(arg1 is System.Reflection.Missing))
                                {
                                    args[count] = (Type)arg6;
                                    count++;
                                    if (!(arg7 is System.Reflection.Missing))
                                    {
                                        args[count] = (Type)arg7;
                                        count++;
                                        if (!(arg8 is System.Reflection.Missing))
                                        {
                                            args[count] = (Type)arg8;
                                            count++;
                                            if (!(arg9 is System.Reflection.Missing))
                                            {
                                                args[count] = (Type)arg9;
                                                count++;
                                                if (!(arg10 is System.Reflection.Missing))
                                                {
                                                    args[count] = (Type)arg10;
                                                    count++;
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }


            if (count != 10)
            {
                Array.Resize<Type>(ref args, count);
            }
            this.genericTypes = args;


        }

        public object FieldGetValue(int index, object target)
        {
            var value = typeD.fields[index].fieldInfo.GetValue(target);
            return metaObject.getFromObject(value);
        }

        public void FieldSetValue(int index, object target, object value)
        {
            typeD.fields[index].fieldInfo.SetValue(target, value);
            
        }


        public object PropertyMetavalueForParameters(int index, object target, bool set, [Optional] object value,[Optional]object arg1, [Optional] object arg2, [Optional] object arg3, [Optional] object arg4,
            [Optional] object arg5, [Optional] object arg6, [Optional] object arg7, [Optional] object arg8, [Optional] object arg9, [Optional] object arg10)
        {
            

            object[] args = new object[10];
            int count = 0;
            if (!(arg1 is System.Reflection.Missing))
            {
                args[count] = arg1;
                count++;
                if (!(arg2 is System.Reflection.Missing))
                {
                    args[count] = arg2;
                    count++;
                    if (!(arg3 is System.Reflection.Missing))
                    {
                        args[count] = arg3;
                        count++;
                        if (!(arg4 is System.Reflection.Missing))
                        {
                            args[count] = arg4;
                            count++;
                            if (!(arg5 is System.Reflection.Missing))
                            {
                                args[count] = arg5;
                                count++;
                                if (!(arg1 is System.Reflection.Missing))
                                {
                                    args[count] = arg6;
                                    count++;
                                    if (!(arg7 is System.Reflection.Missing))
                                    {
                                        args[count] = arg7;
                                        count++;
                                        if (!(arg8 is System.Reflection.Missing))
                                        {
                                            args[count] = arg8;
                                            count++;
                                            if (!(arg9 is System.Reflection.Missing))
                                            {
                                                args[count] = arg9;
                                                count++;
                                                if (!(arg10 is System.Reflection.Missing))
                                                {
                                                    args[count] = arg10;
                                                    count++;
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }


            if (count != 10)
            {
                Array.Resize<object>(ref args, count);
            }

            if (!set)
            {
                var value1= typeD.properties[index].getPropertyMetavalueForParameters(args, target);
                return metaObject.getFromObject(value1);
            }
            else
            {
                typeD.properties[index].setPropertyMetavalueForParameters(args, target, value);
            }
            return null;
        }

        public object Construct([Optional]object arg1, [Optional] object arg2, [Optional] object arg3, [Optional] object arg4,
            [Optional] object arg5, [Optional] object arg6, [Optional] object arg7, [Optional] object arg8, [Optional] object arg9, [Optional] object arg10)
        {

            try
            {
                object[] args = new object[10];
                int count = 0;
                if (!(arg1 is System.Reflection.Missing))
                {
                    args[count] = arg1;
                    count++;
                    if (!(arg2 is System.Reflection.Missing))
                    {
                        args[count] = arg2;
                        count++;
                        if (!(arg3 is System.Reflection.Missing))
                        {
                            args[count] = arg3;
                            count++;
                            if (!(arg4 is System.Reflection.Missing))
                            {
                                args[count] = arg4;
                                count++;
                                if (!(arg5 is System.Reflection.Missing))
                                {
                                    args[count] = arg5;
                                    count++;
                                    if (!(arg1 is System.Reflection.Missing))
                                    {
                                        args[count] = arg6;
                                        count++;
                                        if (!(arg7 is System.Reflection.Missing))
                                        {
                                            args[count] = arg7;
                                            count++;
                                            if (!(arg8 is System.Reflection.Missing))
                                            {
                                                args[count] = arg8;
                                                count++;
                                                if (!(arg9 is System.Reflection.Missing))
                                                {
                                                    args[count] = arg9;
                                                    count++;
                                                    if (!(arg10 is System.Reflection.Missing))
                                                    {
                                                        args[count] = arg10;
                                                        count++;
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }


                if (count != 10)
                {
                    Array.Resize<object>(ref args, count);
                }

                var value= typeD.constructor.invokeMetavalueForParameters(args, null);
                return metaObject.getFromObject(value);

            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public object InvokeMetavalueForParameters(int index, object target, bool generic, [Optional]object arg1, [Optional] object arg2, [Optional] object arg3, [Optional] object arg4,
            [Optional] object arg5, [Optional] object arg6, [Optional] object arg7, [Optional] object arg8, [Optional] object arg9, [Optional] object arg10)
        {

            try
            {
                object[] args = new object[10];
                int count = 0;
                if (!(arg1 is System.Reflection.Missing))
                {
                    args[count] = arg1;
                    count++;
                    if (!(arg2 is System.Reflection.Missing))
                    {
                        args[count] = arg2;
                        count++;
                        if (!(arg3 is System.Reflection.Missing))
                        {
                            args[count] = arg3;
                            count++;
                            if (!(arg4 is System.Reflection.Missing))
                            {
                                args[count] = arg4;
                                count++;
                                if (!(arg5 is System.Reflection.Missing))
                                {
                                    args[count] = arg5;
                                    count++;
                                    if (!(arg1 is System.Reflection.Missing))
                                    {
                                        args[count] = arg6;
                                        count++;
                                        if (!(arg7 is System.Reflection.Missing))
                                        {
                                            args[count] = arg7;
                                            count++;
                                            if (!(arg8 is System.Reflection.Missing))
                                            {
                                                args[count] = arg8;
                                                count++;
                                                if (!(arg9 is System.Reflection.Missing))
                                                {
                                                    args[count] = arg9;
                                                    count++;
                                                    if (!(arg10 is System.Reflection.Missing))
                                                    {
                                                        args[count] = arg10;
                                                        count++;
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }


                if (count != 10)
                {
                    Array.Resize<object>(ref args, count);
                }
                if (generic)
                {
                    return typeD.methods[index].invokeMetavalueGenericForParameters(genericTypes, args, target);
                }
                else
                {
                    return typeD.methods[index].invokeMetavalueForParameters(args, target);
                }

            }
            catch (Exception e)
            {
                throw e;
            }
            finally
            {
                genericTypes = null;
            }
        }
    }
}
