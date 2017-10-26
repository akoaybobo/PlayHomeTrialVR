using H;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Valve.VR;
using VRGIN.Controls.Tools;
using VRGIN.Core;
using VRGIN.Helpers;

namespace PlayHomeVR
{
    class PlayHomeTool : Tool
    {
        public override Texture2D Image
        {
            get
            {
                return UnityHelper.LoadImage("icon_play.png");
            }
        }
        private bool m_bForceLook = false;

        protected override void OnDestroy()
        {
            
        }

        protected override void OnUpdate()
        {
            base.OnUpdate();
            H_Scene scene = ((PlayHomeInterpreter)VR.Interpreter).Scene;
            if (scene != null)
            {
                var device = this.Controller;
                var tPadPos = device.GetAxis();
                var tPadClick = device.GetPressUp(EVRButtonId.k_EButton_SteamVR_Touchpad);
                var tPadTouch = device.GetTouch(EVRButtonId.k_EButton_SteamVR_Touchpad);
                var tPadPress = device.GetPress(EVRButtonId.k_EButton_SteamVR_Touchpad);
                var tGrip = device.GetPressUp(EVRButtonId.k_EButton_Grip);
                var tTriggerClicked = device.GetPressUp(EVRButtonId.k_EButton_SteamVR_Trigger);

                bool bCanLook = true;
                if (scene.mainMembers.StateMgr.NowStateID == H_STATE.LOOP)
                {
                    if (tPadPress)
                    {
                        if (Math.Abs(scene.MixCtrl.Speed - tPadPos.y) > 0.08f)
                            scene.MixCtrl.Speed += scene.MixCtrl.Pose < tPadPos.y ? 0.08f : -0.08f;
                        else
                            scene.MixCtrl.Speed = tPadPos.y;
                    }
                    else if (tPadTouch)
                    {
                        // avoid "jumping" from one position into the next by limiting the max movement
                        if (Math.Abs(scene.MixCtrl.Pose - tPadPos.x) > 0.03f)
                            scene.MixCtrl.Pose += scene.MixCtrl.Pose < tPadPos.x ? 0.03f : -0.03f;
                        else
                            scene.MixCtrl.Pose = tPadPos.x;

                        if (Math.Abs(scene.MixCtrl.Stroke - tPadPos.y) > 0.03f)
                            scene.MixCtrl.Stroke += scene.MixCtrl.Stroke < tPadPos.y ? 0.03f : -0.03f;
                        else
                            scene.MixCtrl.Stroke = tPadPos.y;
                    }

                    if (tGrip)
                    {
                        if (scene.buttonInEja.IsActive())
                        {
                            scene.Button_EjaIn();
                            VRGIN.Core.Logger.Debug("Button_EjaIn click");
                        }
                        else if (scene.buttonOutEja.IsActive())
                        {
                            scene.Button_EjaOut();
                            VRGIN.Core.Logger.Debug("Button_EjaOut click");
                        }
                        else
                            VRGIN.Core.Logger.Debug("Grip - No Button enbaled");
                    }
                }
                else if (scene.mainMembers.StateMgr.NowStateID == H_STATE.SHOW_MOUTH_LIQUID)
                {
                    if (tGrip && scene.buttonDrink.IsActive())
                    {
                        scene.Button_Drink();
                        VRGIN.Core.Logger.Debug("Button_Drink click");
                    }
                    else if (tTriggerClicked && scene.buttonVomit.IsActive())
                    {
                        scene.Button_Vomit();
                        VRGIN.Core.Logger.Debug("Button_Vomit click");
                    }
                    bCanLook = false;
                }
                else
                {
                    if (tGrip)
                    {
                        if (scene.buttonExtract.IsActive())
                        {
                            scene.Button_Extract();
                            VRGIN.Core.Logger.Debug("Button_Extract click");
                        }
                        else
                            VRGIN.Core.Logger.Debug("Grip - No Button enbaled");
                    }
                }

                if (tPadClick)
                {
                    scene.MixCtrl.OnPointerClick(null);
                }

                if (tTriggerClicked)
                {
                    if (bCanLook && !m_bForceLook)
                    {
                        m_bForceLook = true;
                        foreach (var female in ((PlayHomeInterpreter)VR.Interpreter).FemaleMainActors)
                        {
                            female.Actor.ChangeNeckLook(LookAtRotator.TYPE.TARGET, VRCamera.Instance.SteamCam.head, false);
                            female.Actor.ChangeEyeLook(LookAtRotator.TYPE.TARGET, VRCamera.Instance.SteamCam.head, false);
                            VRGIN.Core.Logger.Debug("Forcing " + (female.Actor as Female).name + " to look");
                        }
                    }
                    else if (m_bForceLook)
                    {
                        m_bForceLook = false;
                        foreach (var female in ((PlayHomeInterpreter)VR.Interpreter).FemaleMainActors)
                        {
                            female.Actor.ChangeNeckLook(LookAtRotator.TYPE.NO, VRCamera.Instance.SteamCam.head, false);
                            female.Actor.ChangeEyeLook(LookAtRotator.TYPE.NO, VRCamera.Instance.SteamCam.head, false);
                        }
                        VRGIN.Core.Logger.Debug("Remove Forced look");
                    }
                }
            }
        }
    }
}
