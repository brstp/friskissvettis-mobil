using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.IO;

namespace FriskisSvettisLib.Helpers
{
    public class OldLogger
    {
        public static void LogCall(string page, string service, string function)
        {
            string logText = "Page: " + page + ", Service: " + service + ", Function: " + function;
            // WriteFile(logText);
        }

        private static void WriteFile(string text)
        {
                // create a writer and open the file
            string path = HttpContext.Current.Server.MapPath("~/Log.txt");

            // This text is added only once to the file.
            if (!File.Exists(path))
            {
                // Create a file to write to.
                using (StreamWriter sw = File.CreateText(path))
                {
                    sw.WriteLine("Log created " + DateTime.Now.ToString());
                }
            }

            // This text is always added, making the file longer over time
            // if it is not deleted.
            using (StreamWriter sw = File.AppendText(path))
            {
                sw.WriteLine(DateTime.Now.ToString() + " " + text);
            }    
        }
    }
}
