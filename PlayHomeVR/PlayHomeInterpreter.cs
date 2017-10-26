using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Character;
using VRGIN.Core;

namespace PlayHomeVR
{

    public class PlayHomeInterpreter : GameInterpreter
    {
        public H_Scene Scene { get; private set; }

        private List<PlayHomeActor> _Actors = new List<PlayHomeActor>();
        public override IEnumerable<IActor> Actors
        {
            get { return _Actors.Cast<IActor>(); }
        }

        public List<PlayHomeActor> FemaleMainActors { get; } = new List<PlayHomeActor>();

        protected override void OnUpdate()
        {
            base.OnUpdate();

            if (Scene)
            {
                RefreshActors();
            }
            else
            {
                _Actors.Clear();
            }

        }

        protected override void OnLevel(int level)
        {
            base.OnLevel(level);

            Scene = GameObject.FindObjectOfType<H_Scene>();
        }

        private void RefreshActors()
        {
            List<PlayHomeActor> newActors = new List<PlayHomeActor>();
            foreach (var memberList in Scene.members)
            {
                foreach (var member in memberList.GetHumans())
                {
                    if (member.isActiveAndEnabled)
                    {
                        newActors.Add(_Actors.Find(x => x.Actor == member) ?? new PlayHomeActor(member));
                    }
                }
            }
            _Actors = newActors;

            FemaleMainActors.Clear();
            foreach (var member in Scene.mainMembers.GetFemales())
            {
                if (member.isActiveAndEnabled)
                {
                    FemaleMainActors.Add(_Actors.Find(x => x.Actor == member) ?? new PlayHomeActor(member));
                }
            }
        }

        public override IActor FindImpersonatedActor()
        {
            return _Actors.FirstOrDefault(a => a.IsImpersonated);
        }

        public override IActor FindNextActorToImpersonate()
        {
            var actors = _Actors.Where(x => x.IsMale).ToList();
            var res = actors.OrderByDescending(actor => Vector3.Dot((actor.Eyes.position - VR.Camera.transform.position).normalized, VR.Camera.SteamCam.head.forward)).FirstOrDefault();
            VRGIN.Core.Logger.Debug("Impersonated Actor: " + res != null ? res.Actor.name : "None");
            return res;
        }

    }
}
