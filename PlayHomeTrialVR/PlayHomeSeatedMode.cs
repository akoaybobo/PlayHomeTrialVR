using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using VRGIN.Controls;
using VRGIN.Core;
using VRGIN.Helpers;
using VRGIN.Modes;


namespace PlayHomeTrialVR
{
    class PlayHomeSeatedMode : SeatedMode
    {
        bool seated = Environment.CommandLine.Contains("--seated");
        bool standing = Environment.CommandLine.Contains("--standing");
        protected override IEnumerable<IShortcut> CreateShortcuts()
        {
            return base.CreateShortcuts().Concat(new IShortcut[] {
                new MultiKeyboardShortcut(new KeyStroke("Ctrl+C"), new KeyStroke("Ctrl+C"), () => { VR.Manager.SetMode<PlayHomeStandingMode>(); })
            });
        }

        /// <summary>
        /// Disables controllers for seated mode.
        /// </summary>
        protected override void CreateControllers()
        {
        }

        /// <summary>
        /// Uncomment to automatically switch into Standing Mode when controllers have been detected.
        /// </summary>
        // DEBUG: If it's not directly set for seated mode it will open when there are controllers shown
        protected override void ChangeModeOnControllersDetected()
        {
            if (seated)
            {
                VRLog.Warn("DEBUG: Controllers detected but game is flagged for seated mode");
                VR.Manager.SetMode<PlayHomeSeatedMode>();
            }
            else
            {
                VRLog.Warn("DEBUG: Controllers detected switching to standing mode");
                VR.Manager.SetMode<PlayHomeStandingMode>();

            }
        }
    }
}
