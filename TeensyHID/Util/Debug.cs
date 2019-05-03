// Copyright 2018 RED Software, LLC. All Rights Reserved.

using System;

namespace TeensyHID
{
    /// <summary>
    /// Class containing methods to ease debugging while developing a server.
    /// </summary>
    public static class Debug
    {
        /// <summary>
        /// Logs a message to the console window.
        /// </summary>
        /// <param name="message">The message to log.</param>
        public static void Log(object message)
        {
            Print(DebugLevel.DL_LOG, message, null);
        }

        /// <summary>
        /// Logs a message to the console window.
        /// </summary>
        /// <param name="message">The message to log.</param>
        /// <param name="context">The <see cref="Object"/> that the message references.</param>
        public static void Log(object message, Object context)
        {
            Print(DebugLevel.DL_LOG, message, context);
        }

        /// <summary>
        /// Logs an assert message to the console window.
        /// </summary>
        /// <param name="message">The message to log.</param>
        public static void LogAssert(object message)
        {
            Print(DebugLevel.DL_ASSERT, message, null);
        }

        /// <summary>
        /// Logs an assert message to the console window.
        /// </summary>
        /// <param name="message">The message to log.</param>
        /// <param name="context">The <see cref="Object"/> that the message references.</param>
        public static void LogAssert(object message, Object context)
        {
            Print(DebugLevel.DL_ASSERT, message, context);
        }

        /// <summary>
        /// Logs an error message to the console window.
        /// </summary>
        /// <param name="message">The message to log.</param>
        public static void LogError(object message)
        {
            Print(DebugLevel.DL_ERROR, message, null);
        }

        /// <summary>
        /// Logs an error message to the console window.
        /// </summary>
        /// <param name="message">The message to log.</param>
        /// <param name="context">The <see cref="Object"/> that the message references.</param>
        public static void LogError(object message, Object context)
        {
            Print(DebugLevel.DL_ERROR, message, context);
        }

        /// <summary>
        /// Logs an exception message to the console window.
        /// </summary>
        /// <param name="message">The message to log.</param>
        public static void LogException(object message)
        {
            Print(DebugLevel.DL_EXCEPTION, message, null);
        }

        /// <summary>
        /// Logs an exception message to the console window.
        /// </summary>
        /// <param name="message">The message to log.</param>
        /// <param name="context">The <see cref="Object"/> that the message references.</param>
        public static void LogException(object message, Object context)
        {
            Print(DebugLevel.DL_EXCEPTION, message, context);
        }

        /// <summary>
        /// Logs a warning message to the console window.
        /// </summary>
        /// <param name="message">The message to log.</param>
        public static void LogWarning(object message)
        {
            Print(DebugLevel.DL_WARNING, message, null);
        }

        /// <summary>
        /// Logs a warning message to the console window.
        /// </summary>
        /// <param name="message">The message to log.</param>
        /// <param name="context">The <see cref="Object"/> that the message references.</param>
        public static void LogWarning(object message, Object context)
        {
            Print(DebugLevel.DL_WARNING, message, context);
        }

        /// <summary>
        /// Gets the appropriate color for the log message.
        /// </summary>
        /// <param name="type">The type of the log message.</param>
        /// <returns>The color to set the console window to.</returns>
        private static ConsoleColor GetColor(DebugLevel type)
        {
            switch (type)
            {
                case DebugLevel.DL_LOG:
                    return ConsoleColor.Gray;
                case DebugLevel.DL_ERROR:
                case DebugLevel.DL_EXCEPTION:
                    return ConsoleColor.Magenta;
                case DebugLevel.DL_ASSERT:
                case DebugLevel.DL_WARNING:
                    return ConsoleColor.Yellow;
                default:
                    return ConsoleColor.Gray;
            }
        }

        /// <summary>
        /// Prints a message to the console window.
        /// </summary>
        /// <param name="level">The type of the message.</param>
        /// <param name="message">The message to print.</param>
        /// <param name="context">The <see cref="Object"/> object that the message references.</param>
        private static void Print(DebugLevel level, object message, Object context)
        {
            Console.ForegroundColor = GetColor(level);
            Console.WriteLine($"{message}{(context ? $" (Context: {context})" : string.Empty)}");

            Console.ResetColor();
        }
    }
}
