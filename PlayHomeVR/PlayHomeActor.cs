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
        private Transform _EyeTransform;

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

        private void SetHeadVisible(bool bVisible)
        {
            Actor.head.ChangeShow(bVisible);
            Actor.hairs.ChangeShow(bVisible);
        }
    }
}
