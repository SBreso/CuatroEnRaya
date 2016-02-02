using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Win01
{
    static class Debugger
    {
        public static void Print(Exception ex, object sender)
        {
            Console.WriteLine(sender.ToString()+"\n"+String.Format("{0}--{1}", ex.Message, ex.GetType()));
        }

        public static void Print(String s)
        {
            Console.WriteLine(s);
        }
    }
}
