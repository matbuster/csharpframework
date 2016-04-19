using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using csharpFramework.Debug;
using System.Diagnostics;

namespace csharpFrameworkTester
{
    class Program
    {
        static void Main(string[] args)
        {
            csharpFramework.Debug.Output.getInstance().WriteLine("Starting a new cpp framework test");
            csharpFramework.Debug.Output.getInstance().WriteLine("Tested version : " + csharpFramework.Version.getFrameworkVersion());

            // Keep the console window open in debug mode.
            Console.WriteLine("Press any key to exit.");
            Console.ReadKey();
        }
    }
}
