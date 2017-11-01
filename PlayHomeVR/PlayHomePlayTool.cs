using Character;
using H;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using Valve.VR;
using VRGIN.Controls;
using VRGIN.Controls.Tools;
using VRGIN.Core;
using VRGIN.Helpers;
using static PlayHomeVR.PlayHomeActor;

namespace PlayHomeVR
{
    class PlayHomePlayTool : Tool
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
                        float fLimit = 2.0f * Time.deltaTime;
                        if (Math.Abs(scene.MixCtrl.Speed - tPadPos.y) > fLimit)
                            scene.MixCtrl.Speed += scene.MixCtrl.Pose < tPadPos.y ? fLimit : -fLimit;
                        else
                            scene.MixCtrl.Speed = tPadPos.y;
                    }
                    else if (tPadTouch)
                    {
                        float fLimit = 2.0f * Time.deltaTime;
                        // avoid "jumping" from one position into the next by limiting the max movement
                        if (Math.Abs(scene.MixCtrl.Pose - tPadPos.x) > fLimit)
                            scene.MixCtrl.Pose += scene.MixCtrl.Pose < tPadPos.x ? fLimit : -fLimit;
                        else
                            scene.MixCtrl.Pose = tPadPos.x;

                        if (Math.Abs(scene.MixCtrl.Stroke - tPadPos.y) > fLimit)
                            scene.MixCtrl.Stroke += scene.MixCtrl.Stroke < tPadPos.y ? fLimit : -fLimit;
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
                            female.SetLookAt(VRCamera.Instance.SteamCam.head);
                        }
                    }
                    else if (m_bForceLook)
                    {
                        m_bForceLook = false;
                        foreach (var female in ((PlayHomeInterpreter)VR.Interpreter).FemaleMainActors)
                        {
                            female.ClearLookAt();
                        }
                    }
                }
            }
        }

        public override List<HelpText> GetHelpTexts()
        {
            List<HelpText> res = new List<HelpText>();
            H_Scene scene = ((PlayHomeInterpreter)VR.Interpreter).Scene;
            if (scene != null)
            {
                if (scene.mainMembers.StateMgr.NowStateID == H_STATE.LOOP)
                {
                    res.Add(HelpText.Create("Change movement", FindAttachPosition("trackpad"), new Vector3(0, 0.02f, 0.05f)));
                    res.Add(HelpText.Create("Press for speed", FindAttachPosition("trackpad"), new Vector3(0.05f, 0.02f, 0)));
                    if (scene.buttonInEja.IsActive())
                    {
                        res.Add(HelpText.Create("Cum inside", FindAttachPosition("lgrip"), new Vector3(-0.06f, 0.0f, -0.05f)));
                    }
                    else if (scene.buttonOutEja.IsActive())
                    {
                        res.Add(HelpText.Create("Cum outside", FindAttachPosition("lgrip"), new Vector3(-0.06f, 0.0f, -0.05f)));
                    }
                }
                else if (scene.mainMembers.StateMgr.NowStateID == H_STATE.SHOW_MOUTH_LIQUID)
                {
                    if (scene.buttonDrink.IsActive())
                    {
                        res.Add(HelpText.Create("Swallow", FindAttachPosition("lgrip"), new Vector3(-0.06f, 0.0f, -0.05f)));
                    }
                    else if (scene.buttonVomit.IsActive())
                    {
                        res.Add(HelpText.Create("Spit out", FindAttachPosition("trigger"), new Vector3(-0.06f, 0.0f, -0.05f)));
                    }
                }
                else if (scene.buttonExtract.IsActive())
                {
                    res.Add(HelpText.Create("Pull out", FindAttachPosition("lgrip"), new Vector3(-0.06f, 0.0f, -0.05f)));
                }
                else if (scene.mainMembers.StateMgr.NowStateID == H_STATE.PRE_INSERT_WAIT)
                {
                    res.Add(HelpText.Create("Click to push in", FindAttachPosition("trackpad"), new Vector3(0.06f, 0.04f, -0.05f)));
                }
                else if (scene.mainMembers.StateMgr.NowStateID == H_STATE.PRE_TOUCH_WAIT || scene.mainMembers.StateMgr.NowStateID == H_STATE.INSERTED_WAIT)
                {
                    res.Add(HelpText.Create("Click to start", FindAttachPosition("trackpad"), new Vector3(0.06f, 0.04f, -0.05f)));
                }

                if (scene.mainMembers.StateMgr.NowStateID != H_STATE.SHOW_MOUTH_LIQUID)
                {
                    if (m_bForceLook)
                        res.Add(HelpText.Create("Stop looking", FindAttachPosition("trigger"), new Vector3(-0.06f, 0.0f, -0.05f)));
                    else
                        res.Add(HelpText.Create("Girl looks at you", FindAttachPosition("trigger"), new Vector3(-0.06f, 0.0f, -0.05f)));

                }
            }
            return res;
        }
    }
}
