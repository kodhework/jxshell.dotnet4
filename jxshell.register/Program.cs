using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace jxshell.register
{
    public class Program
    {
        static void Main(string[] args)
        {
            jxshell.net6.SelfRegister.Register(typeof(jxshell.net6.Manager));
        }
    }
}
