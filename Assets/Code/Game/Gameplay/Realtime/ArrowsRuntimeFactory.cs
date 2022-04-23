using Game.Gameplay.Realtime.GameplayComponents.GameCamera;
using Game.Gameplay.Realtime.GameplayComponents.Projectiles;
using Game.Gameplay.Realtime.OperationSequence;
using Game.Gameplay.Realtime.OperationSequence.Operation;
using Game.Gameplay.Realtime.PlayfieldComponents;
using Game.Gameplay.Realtime.PlayfieldComponents.Target;
using Game.Gameplay.Realtime.PlayfieldComponents.Track;
using SplineMesh;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using Zenject;

using Vector3 = UnityEngine.Vector3;

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
        
        Playfield _playfield;
                
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
            if(projectileGenerator == null)
                throw new System.Exception("IProjectileProvider not provided to RuntimeFactory");
            if(runContextProvider == null)
                throw new System.Exception("IContextProvider isn't provided to RuntimeFactory");
             if(sequenceManager == null)
                throw new System.Exception("ISequenceManager isn't provided to RuntimeFactory");
               
            _splineMeshGenerator = splineMeshGenerator;
            _trackPopulator = trackPopulator;
            _targetGenerator = targetGenerator; 
            _projectileGenerator = projectileGenerator;         
            _runContextProvider = runContextProvider;
            _sequenceManager = sequenceManager;                        
        }
        
        public async Task<RunthroughContext> GetRunthroughContext()
        {
            var sequenceContext = _runContextProvider.GetContext();
            var runPlayfield = await GetPlayfield(sequenceContext);
            var runFollower = GetTrackFollower(runPlayfield.TrackSpline);
            var runProjectile = _projectileGenerator.CreateArrows(sequenceContext.InitialValue, runPlayfield.trackWidth);            
            runProjectile.transform.SetParent(runFollower.Transform);   
            AttachCameraToFollower(runFollower);
            var runContext = new RunthroughContext(runPlayfield, runFollower, runProjectile, sequenceContext);
            return runContext;
        }
        
        async Task<Playfield> GetPlayfield(SequenceContext sequenceContext)
        {                               
            var targetScore = _sequenceManager.GetNextTargetScore();
            var spread = 15;
            var sequence = _sequenceManager.GenerateSequence(targetScore, spread);
                        
            var splineTrack = await _splineMeshGenerator.GetRandomizedTrackAsync(sequenceContext.Length, _trackSplineMesh); 
            var gates = await _trackPopulator.PlaceGatesAsync(_gatePrefab, splineTrack, sequence);            
            var maxNumberOfTargets = (targetScore > 20) ? 20 : (int)targetScore - 1;
            (int Min, int Max) numberOfTargetsRange = (1, maxNumberOfTargets);  
            var targets = await _targetGenerator.GetSuitableTargetAsync(_targetPrefabs, targetScore, numberOfTargetsRange);   
                        
            await PlaceAtTrackEnd(targets, splineTrack, new Vector3(0, -105, 105));            
            await AssemblePlayfield(splineTrack, gates, targets);
            return _playfield;
        }
        
        async Task PlaceAtTrackEnd(GameObject entity, Spline spline, Vector3 offset)
        {            
            var PlaceAtTrackEndSemaphore = new SemaphoreSlim(0, 1);
            var endPoint = spline.nodes.Last().Position;
            UnityMainThreadDispatcher.Instance().Enqueue(() => {StartCoroutine(PlaceAtEndCoroutine(entity, endPoint + offset, PlaceAtTrackEndSemaphore));});
            await PlaceAtTrackEndSemaphore.WaitAsync();  
        }
        
        async Task AssemblePlayfield(Spline track, GameObject gates, GameObject targets)
        {            
            var PlayfieldAsemblySemaphore = new SemaphoreSlim(0, 1);
            UnityMainThreadDispatcher.Instance().Enqueue(() => {
                StartCoroutine(PlayfieldAssemblyCoroutine(track, gates, targets,
                PlayfieldAsemblySemaphore));});
            await PlayfieldAsemblySemaphore.WaitAsync();  
        }
        
        IEnumerator PlaceAtEndCoroutine(GameObject entity, Vector3 position, SemaphoreSlim semaphore)
        {   
            entity.transform.position = position;
            semaphore.Release();
            yield return null;
        }
        
        IEnumerator PlayfieldAssemblyCoroutine(Spline track, GameObject gates, GameObject targets, SemaphoreSlim semaphore)
        {   
            var playfieldObject = new GameObject("playfield");   
            _playfield = new Playfield(track.gameObject, gates, targets, playfieldObject);
            semaphore.Release();
            yield return null;
        }  
           
        ITrackFollower GetTrackFollower(SplineMesh.Spline splineToAttachFollowerTo = null)
        {
            var follower = new GameObject("Spline Follower").AddComponent<SplineFollower>();
            
            if(splineToAttachFollowerTo == null)
                follower.SetSplineToFollow(splineToAttachFollowerTo, 0);
            
            return follower;          
        }     
        
        void AttachCameraToFollower(ITrackFollower follower)
        {
            var smoothCamera = Camera.main.GetComponent<SmoothFollow>();
            smoothCamera.target = follower.Transform;
        }
    }
}