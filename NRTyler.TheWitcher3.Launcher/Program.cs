// ***********************************************************************
// Assembly         : NRTyler.TheWitcher3.Launcher
//
// Author           : Nicholas Tyler
// Created          : 05-02-2018
//
// Last Modified By : Nicholas Tyler
// Last Modified On : 05-10-2018
//
// License          : MIT License
// ***********************************************************************

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using NRTyler.CodeLibrary.Extensions;
using NRTyler.CodeLibrary.Utilities;

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

            if (!process.HasExited)
            {
                process.WaitForExit();
            }

            CloseScripts(scriptIDs);
            
            LaunchBackupCreator();

            AppTerminator.CloseAfterElapsedTime(TimeSpan.FromSeconds(10), true);
            AppTerminator.CloseApplication(true);
        }

        /// <summary>
        /// Grabs the scripts to load from the ScriptsToLoad.xml file.
        /// </summary>
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
                var trimedScriptName = scriptName.Trim();

                Write($"Starting script: {trimedScriptName}");

                var process = new Process()
                {
                    StartInfo =
                    {
                        FileName = $"{currentDirectory}/{scriptFolderName}/{trimedScriptName}",
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

        private static void LaunchBackupCreator()
        {
            var currentDirectory = DirectoryEx.GetExecutingDirectory().FullName;
            var creatorDirectory = $"{currentDirectory}/BackupCreator/The Witcher 3 Backup Creator.exe";

            var process = new Process()
            {
                StartInfo =
                {
                    FileName       = $"{creatorDirectory}",
                    CreateNoWindow = false
                }
            };

            process.Start();

            // Removes the possibility of stopping the backup prematurely.
            if (!process.HasExited)
            {
                process.WaitForExit();
            }
        }

        #region Shorthand Aliases for Other Methods

        /// <summary>
        /// A shorthand alias for the "<see cref="Console.WriteLine()"/>" method.
        /// </summary>
        private static void Write(object obj = null)
        {
            Console.WriteLine(obj);
        } 

        #endregion
    }
}
