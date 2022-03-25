using Game.Gameplay.Realtime.GameplayComponents.GameCamera;
using Game.Gameplay.Realtime.GameplayComponents.Projectiles;
using Game.Gameplay.Realtime.OperationSequence;
using Game.Gameplay.Realtime.PlayfieldComponents;
using Game.Gameplay.Realtime.PlayfieldComponents.Target;
using Game.Gameplay.Realtime.PlayfieldComponents.Track;
using SplineMesh;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Zenject;

namespace Game.Gameplay.Realtime
{
    public class ArrowsRuntimeFactory : MonoBehaviour, IRuntimeFactory
    {                
        [SerializeField]
        private GameObject _trackSplineMesh;
        [SerializeField]
        private GameObject _gatePrefab;
        [SerializeField]
        private List<GameObject> _targetPrefabs;
        
        ISplineTrackProvider _splineMeshGenerator;
        ITrackPopulator _trackPopulator;        
        ITargetProvider _targetGenerator;
                
        IProjectileProvider _projectileGenerator;  
        
        IContextProvider _runContextProvider;
        ISequenceManager _sequenceManager;
                
        [Inject]
        public void Construct(ISplineTrackProvider splineMeshGenerator, ITrackPopulator trackPopulator, 
            ITargetProvider targetGenerator, IProjectileProvider projectileGenerator,
            IContextProvider runContextProvider, ISequenceManager sequenceManager)
        {
            if(splineMeshGenerator == null)
                throw new System.Exception("ISplineTrackProvider isn't provided to RuntimeFactory");
            if(trackPopulator == null)
                throw new System.Exception("ITrackPopulator isn't provided to RuntimeFactory");
            if(targetGenerator == null)
                throw new System.Exception("ITargerProvider isn't provided to RuntimeFactory");
               
            _splineMeshGenerator = splineMeshGenerator;
            _trackPopulator = trackPopulator;
            _targetGenerator = targetGenerator; 
                
            // if(follower == null)
            //     throw new System.Exception("ITrackFollower not provided to GameManager");
            if(projectileGenerator == null)
                throw new System.Exception("IProjectileProvider not provided to RuntimeFactory");
                
            // _follower = new GameObject("Spline Follower").AddComponent<SplineFollower>();
            _projectileGenerator = projectileGenerator;            
            
            if(runContextProvider == null)
                throw new System.Exception("IContextProvider isn't provided to RuntimeFactory");
             if(sequenceManager == null)
                throw new System.Exception("ISequenceManager isn't provided to RuntimeFactory");
                
            _runContextProvider = runContextProvider;
            _sequenceManager = sequenceManager;                        
        }
        
        public RunthroughContext GetRunthroughContext()
        {
            var generationContext = _runContextProvider.getContext();
            var runPlayfield = GetPlayfield();
            var runFollower = GetTrackFollower(runPlayfield.TrackSpline);
            var runProjectile = _projectileGenerator.CreateArrows(generationContext.InitialValue, runPlayfield.trackWidth);            
            runProjectile.transform.SetParent(runFollower.Transform);   
            AttachCameraToFollower(runFollower);
            var runContext = new RunthroughContext(runPlayfield, runFollower, runProjectile, generationContext);
            return runContext;
        }
        
        void AttachCameraToFollower(ITrackFollower follower)
        {
            var smoothCamera = Camera.main.GetComponent<SmoothFollow>();
            smoothCamera.target = follower.Transform;
        }
        
        Playfield GetPlayfield()
        {                    
            var context = _runContextProvider.getContext();
            
            var targetScore = _sequenceManager.GetNextTargetScore();
            var spread = 15;
            var sequence = _sequenceManager.GenerateSequence(targetScore, spread);
            
            var stopwatch = System.Diagnostics.Stopwatch.StartNew();
            
            var splineTrack = _splineMeshGenerator.GetRandomizedTrack(context.Length, _trackSplineMesh);
            stopwatch.Restart();
            
            
            var gates = _trackPopulator.PlaceGates(_gatePrefab, splineTrack.GetComponent<Spline>(), sequence);
            
            
            (int Min, int Max) numberOfTargetsRange = (1, 20);  
            var targets = _targetGenerator.GetSuitableTarget(_targetPrefabs, targetScore, numberOfTargetsRange);   
            
            
            PlaceAtTrackEnd(targets, splineTrack.GetComponent<Spline>(), new UnityEngine.Vector3(0, -105, 105));
                        
            var playfieldObject = new GameObject("playfield");   
            var playfield = new Playfield(splineTrack, gates, targets, playfieldObject);
            return playfield;
        }
        
        void PlaceAtTrackEnd(GameObject entity, Spline spline, UnityEngine.Vector3 offset)
        {
            var endPoint = spline.nodes.Last().Position;
            entity.transform.position = endPoint + offset;
        }
        
        // public Runthrough GetRunthrough(Playfield playfield)
        // {     
        //     var follower = new GameObject("Spline Follower").AddComponent<SplineFollower>();
        //     var context = _runContextProvider.getContext();
        //     var run = new Runthrough(follower, _projectileGenerator, playfield, context);      
        //     return run;
        // }
        
        ITrackFollower GetTrackFollower(SplineMesh.Spline splineToAttachFollowerTo = null)
        {
            var follower = new GameObject("Spline Follower").AddComponent<SplineFollower>();
            
            if(splineToAttachFollowerTo == null)
                follower.SetSplineToFollow(splineToAttachFollowerTo, 0);
            
            return follower;          
        }
    }
}