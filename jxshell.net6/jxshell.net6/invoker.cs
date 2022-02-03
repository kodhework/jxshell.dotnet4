using jxshell;
using jxshell.net6;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace jxshell.net6
{
    public class invoker
    {
        private Manager manager;

        public Dictionary<string, object> values = new Dictionary<string, object>();

        public Dictionary<string, object> scopeExceptions = new Dictionary<string, object>();

        public static Dictionary<string, invokerparam0> invokerparam0_pdict;

        public static Dictionary<string, invokerparam1> invokerparam1_pdict;

        public static Dictionary<string, invokerparam2> invokerparam2_pdict;

        public static Dictionary<string, invokerparam0> invokerparam0_dict;

        public static Dictionary<string, invokerparam1> invokerparam1_dict;

        public static Dictionary<string, invokerparam2> invokerparam2_dict;

        public static Dictionary<string, invokerparam3> invokerparam3_dict;

        public static Dictionary<string, invokerparam4> invokerparam4_dict;

        public static Dictionary<string, invokerparam5> invokerparam5_dict;

        public static Dictionary<string, invokerparam6> invokerparam6_dict;

        public static Dictionary<string, invokerparam7> invokerparam7_dict;

        public static Dictionary<string, invokerparam8> invokerparam8_dict;

        public static Dictionary<string, invokerparam9> invokerparam9_dict;

        public Action<string> continue1;

       
        static invoker()
        {
            invokerparam0_pdict = new Dictionary<string, invokerparam0>();
            invokerparam1_pdict = new Dictionary<string, invokerparam1>();
            invokerparam2_pdict = new Dictionary<string, invokerparam2>();
            invokerparam0_dict = new Dictionary<string, invokerparam0>();
            invokerparam1_dict = new Dictionary<string, invokerparam1>();
            invokerparam2_dict = new Dictionary<string, invokerparam2>();
            invokerparam3_dict = new Dictionary<string, invokerparam3>();
            invokerparam4_dict = new Dictionary<string, invokerparam4>();
            invokerparam5_dict = new Dictionary<string, invokerparam5>();
            invokerparam6_dict = new Dictionary<string, invokerparam6>();
            invokerparam7_dict = new Dictionary<string, invokerparam7>();
            invokerparam8_dict = new Dictionary<string, invokerparam8>();
            invokerparam9_dict = new Dictionary<string, invokerparam9>();
        }

        public invoker()
        {
            //this.values["@command"] = this;
            if (this.manager == null)
            {
                this.manager = Manager.lastManager;             
            }            
        }

       

        public invokerparam0 GetInvokerparam0(string method)
        {
            invokerparam0 inv = null;
            if (!invoker.invokerparam0_dict.TryGetValue(method, out inv))
            {
                inv = new invokerparam0(method);
                invoker.invokerparam0_dict[method] = inv;
            }
            return inv;
        }

        public invokerparam0 GetInvokerparam0P(string method)
        {
            invokerparam0 inv = null;
            if (!invoker.invokerparam0_pdict.TryGetValue(method, out inv))
            {
                inv = new invokerparam0(method, true);
                invoker.invokerparam0_pdict[method] = inv;
            }
            return inv;
        }

        public invokerparam1 GetInvokerparam1(string method)
        {
            invokerparam1 inv = null;
            if (!invoker.invokerparam1_dict.TryGetValue(method, out inv))
            {
                inv = new invokerparam1(method);
                invoker.invokerparam1_dict[method] = inv;
            }
            return inv;
        }

        public invokerparam1 GetInvokerparam1P(string method)
        {
            invokerparam1 inv = null;
            if (!invoker.invokerparam1_pdict.TryGetValue(method, out inv))
            {
                inv = new invokerparam1(method, true);
                invoker.invokerparam1_pdict[method] = inv;
            }
            return inv;
        }

        public invokerparam2 GetInvokerparam2(string method)
        {
            invokerparam2 inv = null;
            if (!invoker.invokerparam2_dict.TryGetValue(method, out inv))
            {
                inv = new invokerparam2(method);
                invoker.invokerparam2_dict[method] = inv;
            }
            return inv;
        }

        public invokerparam2 GetInvokerparam2P(string method)
        {
            invokerparam2 inv = null;
            if (!invoker.invokerparam2_pdict.TryGetValue(method, out inv))
            {
                inv = new invokerparam2(method, true);
                invoker.invokerparam2_pdict[method] = inv;
            }
            return inv;
        }

        public invokerparam3 GetInvokerparam3(string method)
        {
            invokerparam3 inv = null;
            if (!invoker.invokerparam3_dict.TryGetValue(method, out inv))
            {
                inv = new invokerparam3(method);
                invoker.invokerparam3_dict[method] = inv;
            }
            return inv;
        }

        public invokerparam4 GetInvokerparam4(string method)
        {
            invokerparam4 inv = null;
            if (!invoker.invokerparam4_dict.TryGetValue(method, out inv))
            {
                inv = new invokerparam4(method);
                invoker.invokerparam4_dict[method] = inv;
            }
            return inv;
        }

        public invokerparam5 GetInvokerparam5(string method)
        {
            invokerparam5 inv = null;
            if (!invoker.invokerparam5_dict.TryGetValue(method, out inv))
            {
                inv = new invokerparam5(method);
                invoker.invokerparam5_dict[method] = inv;
            }
            return inv;
        }

        public invokerparam6 GetInvokerparam6(string method)
        {
            invokerparam6 inv = null;
            if (!invoker.invokerparam6_dict.TryGetValue(method, out inv))
            {
                inv = new invokerparam6(method);
                invoker.invokerparam6_dict[method] = inv;
            }
            return inv;
        }

        public invokerparam7 GetInvokerparam7(string method)
        {
            invokerparam7 inv = null;
            if (!invoker.invokerparam7_dict.TryGetValue(method, out inv))
            {
                inv = new invokerparam7(method);
                invoker.invokerparam7_dict[method] = inv;
            }
            return inv;
        }

        public invokerparam8 GetInvokerparam8(string method)
        {
            invokerparam8 inv = null;
            if (!invoker.invokerparam8_dict.TryGetValue(method, out inv))
            {
                inv = new invokerparam8(method);
                invoker.invokerparam8_dict[method] = inv;
            }
            return inv;
        }

        public invokerparam9 GetInvokerparam9(string method)
        {
            invokerparam9 inv = null;
            if (!invoker.invokerparam9_dict.TryGetValue(method, out inv))
            {
                inv = new invokerparam9(method);
                invoker.invokerparam9_dict[method] = inv;
            }
            return inv;
        }

        public object getResultFromTask(Task t)
        {
            if (t.GetType().GetGenericArguments().Length == 0)
            {
                return null;
            }
            return this.GetInvokerparam0P("Result").invoke(t);
        }

        public object invokeMethod(object o, string method, object[] arguments)
        {
            if (arguments.Length == 0)
            {
                return this.GetInvokerparam0(method).invoke(o);
            }
            if ((int)arguments.Length == 1)
            {
                return this.GetInvokerparam1(method).invoke(o, arguments[0]);
            }
            if ((int)arguments.Length == 2)
            {
                return this.GetInvokerparam2(method).invoke(o, arguments[0], arguments[1]);
            }
            if ((int)arguments.Length == 3)
            {
                return this.GetInvokerparam3(method).invoke(o, arguments[0], arguments[1], arguments[2]);
            }
            if ((int)arguments.Length == 4)
            {
                return this.GetInvokerparam4(method).invoke(o, arguments[0], arguments[1], arguments[2], arguments[3]);
            }
            if ((int)arguments.Length == 5)
            {
                return this.GetInvokerparam5(method).invoke(o, arguments[0], arguments[1], arguments[2], arguments[3], arguments[4]);
            }
            if ((int)arguments.Length == 6)
            {
                return this.GetInvokerparam6(method).invoke(o, arguments[0], arguments[1], arguments[2], arguments[3], arguments[4], arguments[5]);
            }
            if ((int)arguments.Length == 7)
            {
                return this.GetInvokerparam7(method).invoke(o, arguments[0], arguments[1], arguments[2], arguments[3], arguments[4], arguments[5], arguments[6]);
            }
            if ((int)arguments.Length == 8)
            {
                return this.GetInvokerparam8(method).invoke(o, arguments[0], arguments[1], arguments[2], arguments[3], arguments[4], arguments[5], arguments[6], arguments[7]);
            }
            if ((int)arguments.Length != 9)
            {
                return null;
            }
            return this.GetInvokerparam9(method).invoke(o, arguments[0], arguments[1], arguments[2], arguments[3], arguments[4], arguments[5], arguments[6], arguments[7], arguments[8]);
        }

        public object invokeMethodVoid(object o, string method, object[] arguments)
        {
            if (arguments.Length == 0)
            {
                this.GetInvokerparam0(method).invokeasVoid(o);
            }
            else if ((int)arguments.Length == 1)
            {
                this.GetInvokerparam1(method).invokeasVoid(o, arguments[0]);
            }
            else if ((int)arguments.Length == 2)
            {
                this.GetInvokerparam2(method).invokeasVoid(o, arguments[0], arguments[1]);
            }
            else if ((int)arguments.Length == 3)
            {
                this.GetInvokerparam3(method).invokeasVoid(o, arguments[0], arguments[1], arguments[2]);
            }
            else if ((int)arguments.Length == 4)
            {
                this.GetInvokerparam4(method).invokeasVoid(o, arguments[0], arguments[1], arguments[2], arguments[3]);
            }
            else if ((int)arguments.Length == 5)
            {
                this.GetInvokerparam5(method).invokeasVoid(o, arguments[0], arguments[1], arguments[2], arguments[3], arguments[4]);
            }
            else if ((int)arguments.Length == 6)
            {
                this.GetInvokerparam6(method).invokeasVoid(o, arguments[0], arguments[1], arguments[2], arguments[3], arguments[4], arguments[5]);
            }
            else if ((int)arguments.Length == 7)
            {
                this.GetInvokerparam7(method).invokeasVoid(o, arguments[0], arguments[1], arguments[2], arguments[3], arguments[4], arguments[5], arguments[6]);
            }
            else if ((int)arguments.Length == 8)
            {
                this.GetInvokerparam8(method).invokeasVoid(o, arguments[0], arguments[1], arguments[2], arguments[3], arguments[4], arguments[5], arguments[6], arguments[7]);
            }
            else if ((int)arguments.Length == 9)
            {
                this.GetInvokerparam9(method).invokeasVoid(o, arguments[0], arguments[1], arguments[2], arguments[3], arguments[4], arguments[5], arguments[6], arguments[7], arguments[8]);
            }
            return null;
        }

        public object invokeProperty(object o, string method, object[] arguments)
        {
            if (arguments.Length == 0)
            {
                return this.GetInvokerparam0P(method).invoke(o);
            }
            if ((int)arguments.Length == 1)
            {
                return this.GetInvokerparam1P(method).invoke(o, arguments[0]);
            }
            if ((int)arguments.Length != 2)
            {
                return null;
            }
            return this.GetInvokerparam2P(method).invoke(o, arguments[0], arguments[1]);
        }

        public object invokePropertySet(object o, string method, object value, object[] arguments)
        {
            if (arguments.Length == 0)
            {
                return this.GetInvokerparam0P(method).setProperty(o, value);
            }
            if ((int)arguments.Length == 1)
            {
                return this.GetInvokerparam1P(method).setProperty(o, arguments[0], value);
            }
            if ((int)arguments.Length != 2)
            {
                return null;
            }
            return this.GetInvokerparam2P(method).setProperty(o, arguments[0], arguments[1], value);
        }
    }
}