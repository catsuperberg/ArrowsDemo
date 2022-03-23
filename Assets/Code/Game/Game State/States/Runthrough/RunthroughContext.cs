using Game.Gameplay.Realtime.GameplayComponents;
using Game.Gameplay.Realtime.GeneralUseInterfaces;
using Game.Gameplay.Realtime.OperationSequence.Operation;
using Game.Gameplay.Realtime.PlayfieldComponents;
using Game.Gameplay.Realtime.PlayfieldComponents.Track;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Gameplay.Realtime
{
    public class RunthroughContext
    {
        public Playfield PlayfieldForRun;
        public ITrackFollower Follower; 
        public GameObject Projectile;
        public SequenceContext GenerationContext;

        public RunthroughContext(Playfield playfieldForRun, ITrackFollower follower, GameObject projectile, SequenceContext generationContext)
        {
            PlayfieldForRun = playfieldForRun;
            Follower = follower;
            Projectile = projectile;
            GenerationContext = generationContext;
        }
    }
}