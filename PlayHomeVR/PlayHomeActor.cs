using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using VRGIN.Core;

namespace PlayHomeVR
{
    public class PlayHomeActor : DefaultActor<Human>
    {
        //private Transform _EyeTransform;
        private Quaternion m_lastHeadRotation;

        public PlayHomeActor(Human nativeActor) : base(nativeActor)
        {
        }

        public override Transform Eyes
        {
            get
            {
                return Actor.HeadPosTrans;
            }
        }

        public override bool HasHead
        {
            get
            {
                return Actor.head.Rend_skin.enabled;
            }
            set
            {
                SetHeadVisible(value);
            }
        }

        public bool IsImpersonated => !HasHead;
        public bool IsMale => Actor is Male;
        public bool IsFemale => Actor is Female;

        protected override void Initialize(Human actor)
        {
            base.Initialize(actor);

            //Fixme Get Eyes
        }

        public void OnUpdate()
        {
            CheckLookAt();
        }

        private void SetHeadVisible(bool bVisible)
        {
            Actor.head.ChangeShow(bVisible);
            Actor.hairs.ChangeShow(bVisible);
        }

        public void SetLookAt(Transform target)
        {
            if (Actor.NeckLook.CalcType == LookAtRotator.TYPE.NO)
            {
                Actor.NeckLook.Init();
            }
            Actor.ChangeNeckLook(LookAtRotator.TYPE.TARGET, target, false);
            Actor.ChangeEyeLook(LookAtRotator.TYPE.TARGET, target, false);
            VRGIN.Core.Logger.Debug("Forcing " + Actor.name + " to look");
        }

        public void ClearLookAt()
        {
            m_lastHeadRotation.Set(0, 0, 0, 0);
            Actor.ChangeNeckLook(LookAtRotator.TYPE.FORWARD, null, false);
            VRGIN.Core.Logger.Debug("Start ClearLookAt");
            Actor.ChangeEyeLook(LookAtRotator.TYPE.NO, null, false);
        }

        private void CheckLookAt()
        {
            if (Actor.NeckLook.CalcType == LookAtRotator.TYPE.FORWARD)
            {
                if (Quaternion.Angle(m_lastHeadRotation, Actor.HeadPosTrans.rotation) < 0.0001f)
                {
                    Actor.ChangeNeckLook(LookAtRotator.TYPE.NO, null, false);
                    VRGIN.Core.Logger.Debug("ClearLookAt finished");
                }
                else
                    m_lastHeadRotation = Actor.HeadPosTrans.rotation;
            }
        }
    }
}
