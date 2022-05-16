using AssetScripts.Instantiation;
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
using IInstantiator = AssetScripts.Instantiation.IInstatiator;

namespace Game.Gameplay.Realtime
{
    public class ArrowsRunthroughFactory : MonoBehaviour, IRunthroughFactory
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
        ITrackFollower _follower;
        GameObject _projectile;
                
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
        
        public async Task<RunthroughContext> GetRunthroughContextHiden()
        {
            var instantiator = new InvisibleInstantiator();
            var sequenceContext = _runContextProvider.GetContext();
            
            var runPlayfield = await GetPlayfield(sequenceContext, instantiator);
            var runFollower = await GetTrackFollower(runPlayfield.TrackSpline);
            var runProjectile = await GetProjectile(sequenceContext.InitialValue, runPlayfield.trackWidth, instantiator);         
            var runContext = new RunthroughContext(runPlayfield, runFollower, runProjectile, instantiator, sequenceContext);
            
            return runContext;
        }
        
        async Task<Playfield> GetPlayfield(SequenceContext sequenceContext, IInstantiator instantiator)
        {                               
            var targetScore = _sequenceManager.GetNextTargetScore();
            var sequence = _sequenceManager.GenerateSequence(targetScore, spread: 15);   
            (int Min, int Max) targetCountRange = (1, MaxTargerCount(targetScore)); 
                        
            var track = await _splineMeshGenerator.GetRandomizedTrackAsync(sequenceContext.Length, _trackSplineMesh, instantiator); 
            var gates = await _trackPopulator.PlaceGatesAsync(_gatePrefab, track, sequence, instantiator);   
            var targets = await _targetGenerator.GetTargetAsync(_targetPrefabs, targetScore, targetCountRange, instantiator);   
                        
            await PlaceAtTrackEnd(targets, track, new Vector3(0, -105, 105));            
            await AssemblePlayfield(track, gates, targets);            
            
            return _playfield;
        }
        
        int MaxTargerCount(System.Numerics.BigInteger score)
        {
            var value = (score > 20) ? 20 : (int)score - 1;        
            return value;
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
           
        async Task<ITrackFollower> GetTrackFollower(SplineMesh.Spline splineToAttachFollowerTo = null)
        {            
            var followerCreationSemaphore = new SemaphoreSlim(0, 1);
            UnityMainThreadDispatcher.Instance().Enqueue(() => {StartCoroutine(FollowerCreationCoroutine(splineToAttachFollowerTo, followerCreationSemaphore));});
            
            await followerCreationSemaphore.WaitAsync();
            return _follower;          
        }     
        
        IEnumerator FollowerCreationCoroutine(SplineMesh.Spline splineToAttachFollowerTo, SemaphoreSlim semaphore)
        {   
            _follower = new GameObject("Spline Follower").AddComponent<SplineFollower>();
            
            if(splineToAttachFollowerTo != null)
                _follower.SetSplineToFollow(splineToAttachFollowerTo, 0);
                
            semaphore.Release();
            yield return null;
        }  
        
        async Task<GameObject> GetProjectile(int initialCount, float movementWidth, IInstatiator assetInstatiator)
        {            
            var projectileCreationSemaphore = new SemaphoreSlim(0, 1);
            UnityMainThreadDispatcher.Instance().Enqueue(() => {StartCoroutine(ProjectileCreationCoroutine(initialCount, movementWidth, assetInstatiator, projectileCreationSemaphore));});
            await projectileCreationSemaphore.WaitAsync();
            return _projectile;
        }
        
        IEnumerator ProjectileCreationCoroutine(int initialCount, float movementWidth, IInstatiator assetInstatiator, SemaphoreSlim semaphore)
        {   
            _projectile = _projectileGenerator.CreateArrows(initialCount, movementWidth, assetInstatiator);     
                       
            _projectile.transform.SetParent(_follower.Transform);   
            AttachCameraToFollower(_follower);
                         
            semaphore.Release();
            yield return null;
        }  
        
        void AttachCameraToFollower(ITrackFollower follower)
        {
            var smoothCamera = Camera.main.GetComponent<SmoothFollow>();
            smoothCamera.target = follower.Transform;
        }
    }
}