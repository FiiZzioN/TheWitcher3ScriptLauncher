// ***********************************************************************
// Assembly         : NRTyler.TheWitcher3.Launcher
//
// Author           : Nicholas Tyler
// Created          : 05-02-2018
//
// Last Modified By : Nicholas Tyler
// Last Modified On : 05-08-2018
//
// License          : MIT License
// ***********************************************************************

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Threading;

namespace NRTyler.TheWitcher3.Launcher
{
    public class Program
    {
        private static void Main()
        {
            Console.Title = Assembly.GetExecutingAssembly().GetName().Name;

            var scriptsToLoad = GrabScriptsToLoad();

            var scriptIDs = OpenScripts(scriptsToLoad);
            var process = LaunchGame();

            Write("This application, if left open, will end all scripts that were started once you close the game.");
            Write();

            process.WaitForExit();

            CloseScripts(scriptIDs);

            CloseAfterElapsedTime(10);
            CloseApplication(true);
        }

        private static ScriptContainer GrabScriptsToLoad()
        {
            var currentDirectory = Environment.CurrentDirectory;
            var containerName    = ApplicationSettings.ScriptContainerName;

            var scriptContainerRepo = new ScriptContainerRepo();


            if (!File.Exists($"{currentDirectory}/{containerName}.xml"))
            {
                var container = new ScriptContainer();
                scriptContainerRepo.Create(container);

                return container;
            }

            return scriptContainerRepo.Retrieve();
        }

        /// <summary>
        /// Opens all scripts that are specified in the script container.
        /// </summary>
        /// <param name="scriptContainer">The script container.</param>
        private static IEnumerable<int> OpenScripts(ScriptContainer scriptContainer)
        {
            var currentDirectory = Environment.CurrentDirectory;
            var scriptFolderName = ApplicationSettings.ScriptsFolderName;
            var scriptNames      = scriptContainer.ScriptsToLoad.Split(',');
            var scriptIDs        = new List<int>();

            Write("======================================");
            Write();

            foreach (var scriptName in scriptNames)
            {
                Write($"Starting script: {scriptName.Trim()}");

                var process = new Process()
                {
                    StartInfo =
                    {
                        FileName       = $"{currentDirectory}/{scriptFolderName}/{scriptName.Trim()}",
                        CreateNoWindow = true
                    }
                };

                process.Start();
                scriptIDs.Add(process.Id);
            }

            Write();        
            Write("======================================");
            Write();
            Write(scriptNames.Length == 1
                ? $"Started {scriptNames.Length} script."
                : $"Started {scriptNames.Length} scripts.");
            Write();

            return scriptIDs;
        }

        /// <summary>
        /// Launches The Witcher 3: Wild Hunt.
        /// </summary>
        private static Process LaunchGame()
        {
            var currentDirectory = Environment.CurrentDirectory;
            var gameDirectory = $"{currentDirectory}/Witcher3Shortcut";

            Write($"Starting \"The Witcher 3: Wild Hunt\" with high priority.");

            var process = new Process()
            {
                StartInfo =
                {
                    FileName       = $"{gameDirectory}",
                    CreateNoWindow = true
                }
            };

            process.Start();
            process.PriorityClass = ProcessPriorityClass.High;

            Write();

            return process;
        }

        /// <summary>
        /// Closes all scripts that are specified in the script container.
        /// </summary>
        /// <param name="scriptIDs">The script container.</param>
        private static void CloseScripts(IEnumerable<int> scriptIDs)
        {
            var processesEnded = 0;

            Write("======================================");

            foreach (var ID in scriptIDs)
            {
                var process = Process.GetProcessById(ID);

                // You have to kill a process that has no UI.
                using (process)
                {
                    process.Kill();
                }

                processesEnded++;
            }

            Write();
            Write(processesEnded == 1
                ? $"Ended {processesEnded} script."
                : $"Ended {processesEnded} scripts.");
            Write();
        }

        /// <summary>
        /// Closes the current application after the specified amount of time has passed.
        /// </summary>
        /// <param name="seconds">
        /// The amount of seconds that should pass before the current application is automatically closed.
        /// </param>
        private static void CloseAfterElapsedTime(int seconds)
        {
            var autoCloseThread = new Thread(AutoClose);   
            autoCloseThread.Start();

            // The logic that closes the application after the specified amount of time has passed.
            void AutoClose()
            {
                var stopwatch = new Stopwatch();

                stopwatch.Start();

                while (stopwatch.IsRunning)
                {
                    if (stopwatch.Elapsed.Seconds >= seconds)
                    {
                        CloseApplication(false);
                    }
                }
            }
        }

        /// <summary>
        /// Closes the current application.
        /// </summary>
        /// <param name="waitForKeyPress">
        /// If set to <see langword="true"/>, the classic "Press any key to continue..." dialog will appear before the console is closed.
        /// </param>
        private static void CloseApplication(bool waitForKeyPress)
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

        private static void Write(object obj = null)
        {
            Console.WriteLine(obj);
        }
    }
}
