using Microsoft.VisualBasic;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;

namespace IWH
{

    /// <summary>
    /// Предоставляет набор функций для получения параметров, переданных через командную строку.
    /// </summary>
    public static class CommandLine
    {

        private const char ParameterSeparator = ':';
        private const string ConfigFileParameterName = "config";
        
        /// <summary>
        /// Возвращает имя конфигурационного ini-файла.
        /// </summary>
        /// <param name="defaultIniFileName">Значение по умолчанию</param>
        public static string GetIniFileName(string defaultIniFileName)
        {
            string resultValue = null;
            resultValue = GetArgumentByName(ConfigFileParameterName);
            if (string.IsNullOrEmpty(resultValue))
            {
                return defaultIniFileName;
            }
            return resultValue;
        }

        private static string GetArgumentByName(string name)
        {
            List<string> paramList = new List<string>(System.Environment.GetCommandLineArgs());
            paramList.RemoveAt(0);
            Dictionary<string, string> paramDict = ParseCommandLine(paramList);
            if (paramDict.ContainsKey(name))
            {
                return paramDict[name];
            }
            return string.Empty;
        }

        private static Dictionary<string, string> ParseCommandLine(IEnumerable commandLineArgs)
        {
            Dictionary<string, string> paramDict = new Dictionary<string, string>();
            foreach (string parameter in commandLineArgs)
            {
                List<string> paramList = new List<string>(parameter.Split(ParameterSeparator));
                if (paramList.Count > 1)
                {
                    paramDict.Add(paramList[0].Trim().ToLower(), paramList[1].Trim());
                }
            }
            return paramDict;
        }
    }

}
