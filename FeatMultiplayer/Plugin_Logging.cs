// Copyright (c) David Karnok, 2023
// Licensed under the Apache License, Version 2.0

using BepInEx;
using BepInEx.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace FeatMultiplayer
{
    public partial class Plugin : BaseUnityPlugin
    {
        static ManualLogSource globalLogger;

        static object logExclusion = new object();

        // This is a ugly workaround for the current homebrew-logsystem
        private static Dictionary<string, StreamWriter> logWriters = new();

        void InitLogging()
        {
            globalLogger = Logger;
        }

        static void Log(int level, object message)
        {
            try
            {
                var md = multiplayerMode;
                if (md == MultiplayerMode.HostLoading || md == MultiplayerMode.Host)
                {
                    if (hostLogLevel.Value <= level)
                    {
                        AppendLog("Player_Host.log", level, message);
                    }
                }
                else if (md == MultiplayerMode.ClientJoin || md == MultiplayerMode.Client)
                {
                    if (clientLogLevel.Value <= level)
                    {
                        AppendLog("Player_Client_" + clientName + ".log", level, message);
                    }
                }
                else
                {
                    if (level == 0)
                    {
                        globalLogger.LogDebug(message);
                    }
                    else if (level == 1)
                    {
                        globalLogger.LogInfo(message);
                    }
                    else if (level == 2)
                    {
                        globalLogger.LogWarning(message);
                    }
                    else if (level == 3)
                    {
                        globalLogger.LogError(message);
                    }
                    else if (level == 4)
                    {
                        globalLogger.LogFatal(message);
                    }
                }
            }
            catch (Exception ex)
            {
                globalLogger.LogError(ex);
            }
        }

        static string GetLogLevelName(int level)
            => level switch
            {
                0 => "DEBUG  ",
                1 => "INFO   ",
                2 => "WARNING",
                3 => "ERROR  ",
                4 => "FATAL  ",
                _ => "????????"
            };

        static void AppendLog(string logFile, int level, object message)
        {
            lock (logExclusion)
            {
                var path = Path.Combine(Application.persistentDataPath, logFile);
                if (!logWriters.TryGetValue(path, out var logger))
                {
                    logger = new StreamWriter(path)
                    {
                        AutoFlush = true
                    };

                    logWriters.Add(path, logger);
                }

                logger.WriteLine("{0} | {1} | {2}", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"), GetLogLevelName(level), message);
            }
        }

        /// <summary>
        /// Log a debug message to the appropriate log file.
        /// </summary>
        /// <param name="message"></param>
        public static void LogDebug(object message)
        {
            Log(0, message);
        }

        /// <summary>
        /// Log an info message to the appropriate log file.
        /// </summary>
        /// <param name="message"></param>
        public static void LogInfo(object message)
        {
            Log(1, message);
        }

        /// <summary>
        /// Log a warning message to the appropriate log file.
        /// </summary>
        /// <param name="message"></param>
        public static void LogWarning(object message)
        {
            Log(2, message);
        }

        /// <summary>
        /// Log an error message to the appropriate log file.
        /// </summary>
        /// <param name="message"></param>
        public static void LogError(object message)
        {
            Log(3, message);
        }

        /// <summary>
        /// Log a fatal message to the appropriate log file.
        /// </summary>
        /// <param name="message"></param>
        public static void LogFatal(object message)
        {
            Log(4, message);
        }

    }
}
