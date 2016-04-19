using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace csharpFramework.Debug
{
    public class Output
    {
        /** intenral instance definition */
        private static Output instance = null;

        /** use console in output debug */
        private bool m_bUseConsole;

        /** @brief constructor of out 
         */ 
        private Output()
        {
            m_bUseConsole = true;
        }

        /**@brief singleton on on output instance
         * @return debug output instance
         */ 
        public static Output getInstance()
        {
            if (null == instance)
            {
                instance = new Output();
            }
            return instance;
        }

        /** @brief write line in debug output
         * @param [in] line to print in debug output
         */
        public void WriteLine(String line)
        {
            if (m_bUseConsole)
            {
                Console.WriteLine(line);
            }
        }

        /** @brief sort message presse any key and wait an input key
         */ 
        public void PressAnyKey()
        {
            // Keep the console window open in debug mode.
            if (m_bUseConsole)
            {
                Console.WriteLine("Press any key to exit.");
                Console.ReadKey();
            }
        }
    }
}
