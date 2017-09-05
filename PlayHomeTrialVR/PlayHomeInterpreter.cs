using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace VRGIN.Core
{

    public class PlayHomeInterpreter : GameInterpreter
    {

        protected override CameraJudgement JudgeCameraInternal(Camera camera)
        {
            bool guiInterested = VR.GUI.IsInterested(camera);
            if (camera.targetTexture == null)
            {
                if (guiInterested)
                {
                    return CameraJudgement.GUIAndCamera;
                }
                else if (camera.CompareTag("MainCamera"))
                {
                    return CameraJudgement.MainCamera;
                }
                else
                {
                    return CameraJudgement.SubCamera;
                }
            }
            return guiInterested ? CameraJudgement.GUI : CameraJudgement.Ignore;
        }
    }
}
