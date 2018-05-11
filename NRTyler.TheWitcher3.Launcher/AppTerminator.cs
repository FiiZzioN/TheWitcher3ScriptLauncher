// ************************************************************************
// Assembly         : NRTyler.TheWitcher3.Launcher
// 
// Author           : Nicholas Tyler
// Created          : 05-10-2018
// 
// Last Modified By : Nicholas Tyler
// Last Modified On : 05-10-2018
// 
// License          : MIT License
// ***********************************************************************

using System;
using System.Diagnostics;
using System.Threading;

namespace NRTyler.TheWitcher3.Launcher
{
    /// <summary>
    /// Contain methods that aid in ending this application.
    /// </summary>
    public static class AppTerminator
    {
        /// <summary>
        /// Closes the current application after the specified amount of time has passed.
        /// </summary>
        /// <param name="timeSpan">
        /// The <see cref="TimeSpan"/> object holding the amount of time that should pass before the application is automatically closed.
        /// </param>
        /// <param name="useNewThread">
        /// If set to <see langword="true"/>, this method will use a new <see cref="Thread"/> to keep track of the elapsed time. This 
        /// allows the application to do other things during the waiting period. If set to <see langword="false"/>, the application will stop 
        /// executing any code after this method has been called. The application will then close after the specified amount of time has passed.
        /// </param>
        public static void CloseAfterElapsedTime(TimeSpan timeSpan, bool useNewThread = false)
        {
            if (useNewThread)
            {
                var autoCloseThread = new Thread(AutoClose)
                {
                    Name = "AutoCloseThread"
                };

                autoCloseThread.Start();
            }
            else
            {
                AutoClose();
            }

            // The logic behind this function.
            void AutoClose()
            {
                Thread.Sleep(timeSpan);
                CloseApplication(false);
            }
        }

        /// <summary>
        /// Closes the current application.
        /// </summary>
        /// <param name="waitForKeyPress">
        /// If set to <see langword="true"/>, the classic "Press any key to continue..." dialog will appear before the console is closed.
        /// </param>
        public static void CloseApplication(bool waitForKeyPress)
        {
            var currentProcess = Process.GetCurrentProcess();

            if (waitForKeyPress)
            {
                Console.Write("Press any key to continue . . . ");
                Console.ReadKey(true);
            }

            using (currentProcess)
            {
                currentProcess.CloseMainWindow();
            }
        }
    }
}