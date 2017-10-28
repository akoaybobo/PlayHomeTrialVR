using IllusionPlugin;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Xml.Serialization;
using VRGIN.Core;
using VRGIN.Helpers;

namespace PlayHomeVR
{

    /// <summary>
    /// This is an example for a VR plugin. At the same time, it also functions as a generic one.
    /// </summary>
    public class VRPlugin : IPlugin
    {

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
                return AssemblyName.GetAssemblyName(Assembly.GetExecutingAssembly().Location).Version.ToSt‌​ring();
            }
        }

        /// <summary>
        /// Determines when to boot the VR code. In most cases, it makes sense to do the check as described here.
        /// </summary>
        public void OnApplicationStart()
        {
            bool vrDeactivated = Environment.CommandLine.Contains("--novr");
            bool vrActivated = Environment.CommandLine.Contains("--vr");
            bool seated = Environment.CommandLine.Contains("--seated");
            bool standing = Environment.CommandLine.Contains("--standing");
            
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

        #region Helper code

        private IVRManagerContext CreateContext(string path) {
            var serializer = new XmlSerializer(typeof(ConfigurableContext));

            if(File.Exists(path))
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
            } catch(Exception e)
            {
                VRLog.Error("{0} Failed to write {1}",e, path);
            }

            return context;
        }
        #endregion

        #region Unused
        public void OnApplicationQuit() { }
        public void OnFixedUpdate() { }
        public void OnLevelWasInitialized(int level) { }
        public void OnLevelWasLoaded(int level) { }
        public void OnUpdate() { }
        #endregion
    }
}
