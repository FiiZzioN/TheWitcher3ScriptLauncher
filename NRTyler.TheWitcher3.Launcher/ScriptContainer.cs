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
    [Serializable]
    [DataContract(Name = "ScriptContainer")]
    public class ScriptContainer
    {
        public ScriptContainer()
        {
            ScriptsToLoad = String.Empty;
        }

        [DataMember(Order = 0)]
        public string ScriptsToLoad { get; private set; } 
    }
}