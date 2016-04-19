/**
 * \file 		version.cs
 * \date 		04/11/2015 
 * \author 		NSJ
 */

using System;
namespace csharpFramework
{
    public class Version
    {
        /**
         * \def FRAMEWORK_MAJOR_VERSION
         * \brief Edit to each code restructuring 
         */
        private static int FRAMEWORK_MAJOR_VERSION = 1;

        /**
         * \def FRAMEWORK_MINOR_VERSION
         * \brief Edit to each add function
         */
        private static int FRAMEWORK_MINOR_VERSION = 0;

        /**
         * \def FRAMEWORK_RELEASE_VERSION
         * \brief Edit to each bug correction 
         */
        private static int FRAMEWORK_RELEASE_VERSION = 0;

        /** \brief separator definition
         */ 
        private static String SEP = ".";

        /** \brief getter on framework version
         * \return formatted string version
         */ 
        public static String getFrameworkVersion()
        {
            return FRAMEWORK_MAJOR_VERSION.ToString() + SEP + FRAMEWORK_MINOR_VERSION.ToString() + SEP + FRAMEWORK_RELEASE_VERSION.ToString();
        }
    }
}