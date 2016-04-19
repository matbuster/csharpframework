/*
 * $Id: cLog.cs 7347 2008-07-16 17:40:38Z y-randle $
 */

using System;
using System.IO;

namespace csharpFramework.FileIO
{
    public static class cLog
    {
        // Enum
        public enum Orig
        {
            Measure,
            General,
            Hardware
        }
        public enum Prio
        {
            Error,
            Warning,
            Exception,
            Info,
            Debug,
            Result
        }

        // Membres
        private static String m_sLogFile = "";
        private static bool m_sLogFileDefined = false;
        private static bool m_bDisplayMessage = true;
        private static StreamWriter m_sw;

        // Propriétés
        public static String LogFile
        {
            get { return m_sLogFile; }
        }

        // Fonctions
        public static void SetErrorMessageVisible(bool _bVisible)
        {
            m_bDisplayMessage = _bVisible;
        }
        public static void SetLogFile(String value)
        {
            if (value == "") return;
            m_sLogFile = value;
            m_sLogFileDefined = true;
            if (!Directory.Exists(Path.GetDirectoryName(m_sLogFile)))
            {
                Directory.CreateDirectory(Path.GetDirectoryName(m_sLogFile));
            }
            m_sw = File.AppendText(m_sLogFile);
            LogRotate();
        }
        public static void CloseLogFile()
        {
            m_sLogFile = "";
            m_sLogFileDefined = false;
            m_sw.Close();
            m_sw.Dispose();
        }
        private static void LogRotate()
        {
            // Rotation du fichier de log si supérieur à 4Mo.
            if (m_sw.BaseStream.Length > 4*1024*1024)
            {
                // Fermeture du fichier de log.
                m_sw.Close();

                // Lecture du fichier de log vers un buffer.
                FileStream fs = File.OpenRead(m_sLogFile);
                byte[] buffer = new byte[fs.Length];
                fs.Read(buffer, 0, buffer.Length);
                fs.Close();

                // Recherche du prochain nom de fichier compressé disponible.
                String newFile;
                DateTime date = DateTime.Now.ToLocalTime();
                String tag = date.Year.ToString() + "_" + date.Month.ToString() + "_" + date.Day.ToString() + "_" + date.Hour.ToString() + "_" + date.Minute.ToString() + "_" + date.Second.ToString() + "_";
                //int i = 0;
   			    do
			    {
				    newFile = System.IO.Path.GetDirectoryName(m_sLogFile) + "\\" + tag + System.IO.Path.GetFileNameWithoutExtension(m_sLogFile) + ".log.gz";
			    } while (File.Exists(newFile));

                // Compression du buffer vers le nouveau fichier.
                StreamWriter sw = new StreamWriter(newFile);
                System.IO.Compression.GZipStream gz = new System.IO.Compression.GZipStream(sw.BaseStream, System.IO.Compression.CompressionMode.Compress);
                gz.Write(buffer, 0, buffer.Length);
                gz.Close();

                // Efface le fichier de log puis le rouvre
                fs = File.Create(m_sLogFile);
                fs.Close();
                m_sw = File.AppendText(m_sLogFile);
            }
        }
        public static void Log(String sMsg, Prio ePriorite)
        {
            Log(sMsg, ePriorite, Orig.General, false);
        }
        public static void Log(String sMsg, Prio ePriorite, Orig eOrigine)
        {
            Log(sMsg, ePriorite, eOrigine, false);
        }
        public static void Log(String sMsg, Prio ePriorite, Orig eOrigine, bool bMsgBox)
        {
            DateTime dt = DateTime.Now;
            String sLine = dt.Year.ToString() + "/" + dt.Month.ToString().PadLeft(2, '0') + "/" + dt.Day.ToString().PadLeft(2, '0')
                           + "\t" + dt.Hour.ToString().PadLeft(2, '0') + ":" + dt.Minute.ToString().PadLeft(2, '0') + ":" + dt.Second.ToString().PadLeft(2, '0')
                           + "\t" + eOrigine.ToString()
                           + "\t" + ePriorite.ToString()
                           + "\t" + sMsg;
            if (m_sLogFileDefined)
            {
                m_sw.WriteLine(sLine);
                m_sw.Flush();
            }
            // TODO : vérifier que le destructeur de StreamWriter ferme le fichier
        }
    }
}