using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace VRGIN.Core
{
    //<summary>
    //The GameInterpreter is for discovering information about the running scene and make decisions
    //</summary>
    public class PlayHomeInterpreter : GameInterpreter
    {    private SteamVR_Camera steamcam;
         protected override CameraJudgement JudgeCameraInternal(Camera camera)
        {
            //DEBUG: Understanding how Cameras work
            VRLog.Info("DEBUG: Interpreter: CameraJudgement - Identifying Camera {0}", camera);

            // The code needs to be added that finds the girl so she can be moved to you, or vice versa
            //var actors = Actors.ToList();
            //VRLog.Info("DEBUG: There are {0} actors", actors.Count);

            bool guiInterested = VR.GUI.IsInterested(camera);
            if (camera.targetTexture == null)
            {
                if (guiInterested)
                {
                    VRLog.Info("DEBUG: Interpreter: CameraJudgement - Camera {0} is GUIAndCamera", camera);
                    return CameraJudgement.GUIAndCamera;
                }
                else if (camera.CompareTag("MainCamera"))
                {
                    VRLog.Info("DEBUG: Interpreter: CameraJudgement - Camera {0} is MainCamera", camera);
                    return CameraJudgement.MainCamera;
                }
                else
                {
                    VRLog.Info("DEBUG: Interpreter: CameraJudgement - Camera {0} is SubCamera", camera);
                    return CameraJudgement.SubCamera;
                }
            }
            VRLog.Info("DEBUG: Interpreter: CameraJudgement - Camera {0} is GuI ({1}) or Ignore", camera, guiInterested);
            return guiInterested ? CameraJudgement.GUI : CameraJudgement.Ignore;
        }
    }
}
