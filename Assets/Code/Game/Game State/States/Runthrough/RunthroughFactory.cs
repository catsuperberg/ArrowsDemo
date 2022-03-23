using DataManagement;
using Game.Gameplay.Realtime;
using Game.Gameplay.Realtime.GameplayComponents;
using UnityEngine;

namespace Game.GameState
{
    public class RunthroughFactory
    {
        IRuntimeFactory _runtimeFactory;  
        IUpdatedNotification _userContextNotifier; 
        IRegistryAccessor _userContextAccessor;
        
        public RunthroughFactory()
        {
                        
        }
        
        public Runthrough GetRunthrough(GameObject RunthroughPrefab, RunthroughContext context)
        {
            var runthroughGO = GameObject.Instantiate(RunthroughPrefab);
            prepareContext(context, runthroughGO);
            var rewardCalculator = new RewardCalculator(context.PlayfieldForRun.Targets.GetComponent<IMultiplierEventNotifier>());
            
            var runthrough = runthroughGO.GetComponent<Runthrough>();
            runthrough.Initialize(context, rewardCalculator);
            return runthrough;
        }
        
        void prepareContext(RunthroughContext context, GameObject baseObject)
        {
            InitFollower(context, baseObject);
            context.Projectile.transform.SetParent(context.Follower.Transform);     
        }
        
        void InitFollower(RunthroughContext context, GameObject baseObject)
        {
            context.Follower.Transform.SetParent(baseObject.transform);
            context.Follower.SetSplineToFollow(context.PlayfieldForRun.TrackSpline, 0);
            context.Follower.SetSpeed(context.GenerationContext.ProjectileSpeed);            
        }
    }
}