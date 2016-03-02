using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Win01
{
    class Debugger
    {
        public enum ENVIORMENT { DEVELOPMENT, PRODUCTION };
        private static ENVIORMENT value;
        public Debugger(ENVIORMENT v)
        {
            value = v;
            Debug.Listeners.Add(new TextWriterTraceListener(File.Create("./net.log")));
            Debug.AutoFlush = true;
        }
        static int i = 1;
        public static void WriteException(Exception ex, object sender)
        {
            if (Debugger.value == ENVIORMENT.PRODUCTION)
            {
                Debug.WriteLine(String.Format((i++) + ")Error at: {}", sender.GetType()));
                Debug.Indent();
                Debug.WriteLine(String.Format("Type: {0}", ex.GetType()));
                Debug.WriteLine(String.Format("Message: {0}", ex.Message));
                Debug.WriteLine(String.Format("Source: {0}", ex.Source));
                Debug.WriteLine(String.Format("Metod: {0}", ex.TargetSite));
                Debug.WriteLine(String.Format("Help: {0}", ex.HelpLink));
                Debug.Unindent();
            }
            else
            {
                MessageBox.Show(String.Format("{0} at: {1} in {2}",ex.Message,ex.TargetSite,ex.Source),"¡¡ERROR!!",MessageBoxButton.OK);
            }
        }

        public static void Write(String s)
        {
            if (Debugger.value == ENVIORMENT.PRODUCTION)
            {
                Debug.WriteLine((i++)+")LOG: " + s);
            }
            else
            {
                Console.WriteLine("LOG: " + s);
            }
        }
    }
}
