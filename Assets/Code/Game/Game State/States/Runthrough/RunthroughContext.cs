using AssetScripts.Instantiation;
using Game.Gameplay.Realtime.OperationSequence.Operation;
using Game.Gameplay.Realtime.PlayfieldComponents;
using Game.Gameplay.Realtime.PlayfieldComponents.Track;
using UnityEngine;

namespace Game.Gameplay.Realtime
{
    public class RunthroughContext
    {
        public Playfield PlayfieldForRun;
        public ITrackFollower Follower; 
        public GameObject Projectile;
        public IInstatiator Instatiator;
        public SequenceContext GenerationContext;

        public RunthroughContext(Playfield playfieldForRun, ITrackFollower follower, 
            GameObject projectile, IInstatiator instatiator, SequenceContext generationContext)
        {
            PlayfieldForRun = playfieldForRun;
            Follower = follower;
            Projectile = projectile;
            Instatiator = instatiator;
            GenerationContext = generationContext;               
        }
    }
}