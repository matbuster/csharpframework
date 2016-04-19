using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.Globalization;

namespace csharpFramework.FileIO
{
    /*
     * Acces aux fichiers ini
     */
    public class cIniFile
    {
        // Déclarations DLL
        [DllImport("kernel32.dll", EntryPoint = "GetPrivateProfileIntA")]
        private static extern int GetPrivateProfileInt(string sApplicationName, string sKeyName, int iDefault, string sFileName);
        [DllImport("kernel32.dll", EntryPoint = "GetPrivateProfileStringA")]
        private static extern int GetPrivateProfileString(string sApplicationName, string sKeyName, string sDefault, StringBuilder psReturnedString, int iSize, string sFileName);
        [DllImport("kernel32.dll", EntryPoint = "WritePrivateProfileStringA")]
        private static extern int WritePrivateProfileString(string sApplicationName, string sKeyName, string sString, string sFileName);

        // Membres
        private string m_sFile;

        // Constructeur
        public cIniFile(string sFile)
        {
            m_sFile = sFile;
        }

        // Fonctions de lecture classique, pas de paramètres par défaut
        public int ReadInt(string sSection, string sKey)
        {
            return ReadInt(sSection, sKey, 0);
        }
        public float ReadFloat(string sSection, string sKey)
        {
            return ReadFloat(sSection, sKey, 0.0f);
        }
        public string ReadString(string sSection, string sKey)
        {
            return ReadString(sSection, sKey, "");
        }
        public bool ReadBool(string sSection, string sKey)
        {
            return ReadBool(sSection, sKey, false);
        }

        // surcharge de la fonction read bool
        public bool ReadBool(string sSection, string sKey, bool bDefaultValue)
        {
            // initialisation de la valeur par défaut
            int iDefaultValue = 0;

            // A envoyer true ou false
            if (bDefaultValue) iDefaultValue = 1;
            else iDefaultValue = 0;

            // valeur lue dans le fichier
            int ReadValue = ReadInt(sSection, sKey, iDefaultValue);

            // on renvoie la valeur
            if (ReadValue == 0)
            {
                return false;
            }
            else
            {
                return true;
            }
        }
        // surcharge de la fonction readint avec défault valeu
        public int ReadInt(string sSection, string sKey, int iDefaultValue)
        {
            return int.Parse(ReadString(sSection, sKey, iDefaultValue.ToString()));
        }
        // surcharge de readfloat
        public float ReadFloat(string sSection, string sKey, float fDefaultValue)
        {
            // init du paramètre de valeur
            float fValue = 0.0f;

            // on récupère la string
            string sFloat = ReadString(sSection, sKey, fDefaultValue.ToString());

            //// on teste le séparateur
            //if (Application.CurrentCulture.NumberFormat.NumberDecimalSeparator == ",")
            //{
            //    // on remplacez la virgule si elle y est par un point
            //    sFloat = sFloat.Replace(".", ",");
            //}

            // on va lire la valeur dans le fichier
            fValue = float.Parse(sFloat);

            // on retourne la valeur
            return fValue;
        }
        // surcharge des fonctions de lecture avec un paramètre par défaut
        public string ReadString(string sSection, string sKey, string sDefaultString)
        {
            // on récupère le
            StringBuilder s = new StringBuilder(1024);

            // on récupère la string correspondant au sectio/key
            GetPrivateProfileString(sSection, sKey, "", s, s.Capacity, m_sFile);

            // si la string est inexistante, on va écrire la valeur par défaut
            if (s.ToString() == "")
            {
                // on va écrire dans la section/ keys la valeur par défaut
                Write(sSection, sKey, sDefaultString);

                // s est donc égal à default strinf
                return sDefaultString;
            }
            else
            {
                // on retourne la string
                return s.ToString();
            }
        }
        // Fonctions d'écriture
        public int Write(string sSection, string sKey, int iValue)
        {
            return WritePrivateProfileString(sSection, sKey, iValue.ToString(), m_sFile);
        }
        public int Write(string sSection, string sKey, float fValue)
        {
            return WritePrivateProfileString(sSection, sKey, fValue.ToString(), m_sFile);
        }
        public int Write(string sSection, string sKey, string sValue)
        {
            return WritePrivateProfileString(sSection, sKey, sValue, m_sFile);
        }
        public int Write(string sSection, string sKey, bool bValue)
        {
            return WritePrivateProfileString(sSection, sKey, bValue.ToString(), m_sFile);
        }
    }
}
