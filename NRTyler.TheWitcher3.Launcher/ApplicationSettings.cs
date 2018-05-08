// ************************************************************************
// Assembly         : NRTyler.TheWitcher3.Launcher
// 
// Author           : Nicholas Tyler
// Created          : 05-02-2018
// 
// Last Modified By : Nicholas Tyler
// Last Modified On : 05-02-2018
// 
// License          : MIT License
// ***********************************************************************

using System;
using System.Runtime.Serialization;

namespace NRTyler.TheWitcher3.Launcher
{
    public class ApplicationSettings
    {
        /// <summary>
        /// The name that the script container file will go by.
        /// </summary>
        public const string ScriptContainerName = "ScriptsToLoad";

        /// <summary>
        /// Gets or sets the name of the folder holding the various scripts.
        /// </summary>
        public const string ScriptsFolderName = "UserScripts";
    }
}