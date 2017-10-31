using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using VRGIN.Controls;
using VRGIN.Controls.Tools;
using VRGIN.Core;
using VRGIN.Helpers;
using VRGIN.Modes;


namespace PlayHomeVR
{
    class PlayHomeStandingMode : StandingMode
    {
        protected override IEnumerable<IShortcut> CreateShortcuts()
        {
            return base.CreateShortcuts().Concat(new IShortcut[] {
                new MultiKeyboardShortcut(new KeyStroke("Ctrl+C"), new KeyStroke("Ctrl+C"), () => { VR.Manager.SetMode<PlayHomeSeatedMode>(); })
            });
        }

        public override IEnumerable<Type> Tools
        {
            get
            {
                return base.Tools.Concat(new Type[] { typeof(PlayHomePlayTool), typeof(PlayHomeObeyTool) });
            }
        }
    }
}
