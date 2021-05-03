using System;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace ControlDrones.Logger
{
    public enum LogType { INDETERMINATE = 1, DEBUG = 2, INFO = 3, WARNING = 4, ERROR = 5, FATAL = 6, OFF = 7 }

    public class Log
    {
        private const String LOG_PATH = "LOG_PATH";
        private const String LOG_LEVEL = "LOG_LEVEL";


        public static void Write(String log, String nombreClase = null, String nombreMetodo = null)
        {
            try
            {
                WriteLog(log, LogType.INDETERMINATE, nombreClase, nombreMetodo);
            }
            catch
            {
                //Nothing
            }
        }


        public static void WriteDebug(String log, String nombreClase = null, String nombreMetodo = null)
        {
            try
            {
                WriteLog(log, LogType.DEBUG, nombreClase, nombreMetodo);
            }
            catch
            {
                //Nothing
            }
        }

        public static void WriteInfo(String log, String nombreClase = null, String nombreMetodo = null)
        {
            try
            {
                WriteLog(log, LogType.INFO, nombreClase, nombreMetodo);
            }
            catch
            {
                //Nothing
            }
        }

        public static void WriteWarning(String log, String nombreClase = null, String nombreMetodo = null)
        {
            try
            {
                WriteLog(log, LogType.WARNING, nombreClase, nombreMetodo);
            }
            catch
            {
                //Nothing
            }
        }

        public static void WriteError(String log, String nombreClase = null, String nombreMetodo = null)
        {
            try
            {
                WriteLog(log, LogType.ERROR, nombreClase, nombreMetodo);
            }
            catch
            {
                //Nothing
            }
        }



        public static void WriteCritical(String log, String nombreClase = null, String nombreMetodo = null)
        {
            try
            {
                WriteLog(log, LogType.FATAL, nombreClase, nombreMetodo);
            }
            catch
            {
                //Nothing
            }
        }

        




        private static void WriteLog(String log, LogType tipoLog, String nombreClase, String nombreMetodo)
        {
            try
            {
                String logPath = ConfigurationManager.AppSettings[LOG_PATH];
                if (logPath != null && logPath != "")
                {
                    LogType? logLevelDefinido = GetDefinedLogLevel();

                    if (logLevelDefinido == null || logLevelDefinido <= tipoLog)
                    {
                        EspecificacionLog especificacionLog = new EspecificacionLog(nombreClase, nombreMetodo);

                        logPath = logPath.EndsWith("\\") ? logPath : logPath + "\\"; 
                        String directoryPath = logPath + String.Format("{0:yyyyMMdd}", DateTime.Now);
                        if (!Directory.Exists(directoryPath))
                        {
                            Directory.CreateDirectory(directoryPath);
                        }

                        String fileName = directoryPath + "\\" + String.Format("{0:yyyyMMdd}", DateTime.Now) + "_" + especificacionLog.NombreNamespace + ".log";
                        StreamWriter sw = new StreamWriter(fileName, true);
                        sw.WriteLine(String.Format("{0:HH:mm:ss.fff}", DateTime.Now) + ": " + tipoLog + " " + especificacionLog.NombreClase + "(" + especificacionLog.NombreMetodo + ") - " + log);
                        sw.Close();
                    }
                }
            }
            catch
            {
                //Nothing
            }
        }

        private static LogType? GetDefinedLogLevel()
        {
            LogType? resultado = null;

            String logLevelStr = ConfigurationManager.AppSettings[LOG_LEVEL];
            try
            {
                if (logLevelStr != null)
                {
                    resultado = (LogType)Enum.Parse(typeof(LogType), logLevelStr, true);
                }
            }
            catch
            {
                //Nothing
            }

            return resultado;
        }
    }
}
