using IllusionPlugin;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using VRGIN.Core;
using VRGIN.Helpers;
using UnityEngine;

namespace PlayHomeTrialVR
{

    /// <summary>
    /// This is an example for a VR plugin. At the same time, it also functions as a generic one.
    /// </summary>
    public class VRPlugin : IPlugin
    {
        public bool vrDeactivated = Environment.CommandLine.Contains("--novr");
        public bool vrActivated = Environment.CommandLine.Contains("--vr");
        public bool seated = Environment.CommandLine.Contains("--seated");
        public bool standing = Environment.CommandLine.Contains("--standing");
        public bool noaa = Environment.CommandLine.Contains("--no-aa");

        /// <summary>
        /// Put the name of your plugin here.
        /// </summary>
        public string Name
        {
            get
            {
                return "PlayHome VR Experimental";
            }
        }

        public string Version
        {
            get
            {
                return "0.2.1";
            }
        }

        /// <summary>
        /// Determines when to boot the VR code. In most cases, it makes sense to do the check as described here.
        /// </summary>
        public void OnApplicationStart()
        {
            VRLog.Info("== Application Start ==");

            if (noaa)
            {
                QualitySettings.antiAliasing = 0;
            }
            else
            {
                QualitySettings.antiAliasing = 1;
            }

            if (vrActivated || (!vrDeactivated && SteamVRDetector.IsRunning))
            {
                // Boot VRManager!
                // Note: Use your own implementation of GameInterpreter to gain access to a few useful operatoins
                // (e.g. characters, camera judging, colliders, etc.)
                VRManager.Create<PlayHomeInterpreter>(CreateContext("VRContext.xml"));
                if (standing)
                {
                    VR.Manager.SetMode<PlayHomeStandingMode>();
                }
                else
                {
                    VR.Manager.SetMode<PlayHomeSeatedMode>();
                }
            }
        }

        public void OnLevelWasLoaded(int level)
        {
        //    VRLog.Info("DEBUG: On Level {0}", level);
                        
        //    if (noaa)
        //    {
        //        QualitySettings.antiAliasing = 0;
        //    }
        //    else
        //    {
        //        QualitySettings.antiAliasing = 1;
        //    }
        //    if ((level == 0) && (vrActivated || (!vrDeactivated && SteamVRDetector.IsRunning)))
        //    {
        //        // Boot VRManager!
        //        // Note: Use your own implementation of GameInterpreter to gain access to a few useful operatoins
        //        // (e.g. characters, camera judging, colliders, etc.)
        //        VRLog.Info("DEBUG: On Level {0} - Booting VR", level);
        //        VRManager.Create<PlayHomeInterpreter>(CreateContext("VRContext.xml"));
        //        if (standing)
        //        {
        //            VR.Manager.SetMode<PlayHomeStandingMode>();
        //        }
        //        else
        //        {
        //            VR.Manager.SetMode<PlayHomeSeatedMode>();
        //        }
        //}
    }

        #region Helper code

        private IVRManagerContext CreateContext(string path)
        {
            var serializer = new XmlSerializer(typeof(ConfigurableContext));

            if (File.Exists(path))
            {
                // Attempt to load XML
                using (var file = File.OpenRead(path))
                {
                    try
                    {
                        return serializer.Deserialize(file) as ConfigurableContext;
                    }
                    catch (Exception e)
                    {
                        VRLog.Error("{0} Failed to deserialize {1} -- using default", e, path);
                    }
                }
            }

            // Create and save file
            var context = new ConfigurableContext();
            try
            {
                using (var file = new StreamWriter(path))
                {
                    file.BaseStream.SetLength(0);
                    serializer.Serialize(file, context);
                }
            }
            catch (Exception e)
            {
                VRLog.Error("{0} Failed to write {1}", e, path);
            }

            return context;
        }
        #endregion

        #region Unused
        public void OnApplicationQuit() { }
        public void OnFixedUpdate() { }
        public void OnLevelWasInitialized(int level) { }
        //public void OnLevelWasLoaded(int level) { }
        public void OnUpdate() { }



        #endregion
    }
}
