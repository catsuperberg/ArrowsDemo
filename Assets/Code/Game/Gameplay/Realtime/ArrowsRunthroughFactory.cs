using AssetScripts.Instantiation;
using Game.Gameplay.Realtime.GameplayComponents.GameCamera;
using Game.Gameplay.Realtime.GameplayComponents.Projectiles;
using Game.Gameplay.Realtime.OperationSequence;
using Game.Gameplay.Realtime.OperationSequence.Operation;
using Game.Gameplay.Realtime.PlayfieldComponents;
using Game.Gameplay.Realtime.PlayfieldComponents.Crossbow;
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
        ICrossbowProvider _crossbowGenerator; 
        ScatterModels _scatterModels; 
                
        IProjectileProvider _projectileGenerator;  
        
        IContextProvider _runContextProvider;
        ISequenceManager _sequenceManager;
        
        Playfield _playfield;
        ITrackFollower _follower;
        GameObject _projectile;
        GameObject _crossbow;
                
        [Inject]
        public void Construct(
            ISplineTrackProvider splineMeshGenerator, ITrackPopulator trackPopulator, 
            ITargetProvider targetGenerator, ICrossbowProvider crossbowGenerator,
            IProjectileProvider projectileGenerator, IContextProvider runContextProvider,
            ISequenceManager sequenceManager, ScatterModels scatterModels)
        {               
            _splineMeshGenerator = splineMeshGenerator ?? throw new System.ArgumentNullException(nameof(splineMeshGenerator));
            _trackPopulator = trackPopulator ?? throw new System.ArgumentNullException(nameof(trackPopulator));            
            _targetGenerator = targetGenerator ?? throw new System.ArgumentNullException(nameof(targetGenerator)); 
            _crossbowGenerator = crossbowGenerator ?? throw new System.ArgumentNullException(nameof(crossbowGenerator));
            _projectileGenerator = projectileGenerator ?? throw new System.ArgumentNullException(nameof(projectileGenerator));         
            _runContextProvider = runContextProvider ?? throw new System.ArgumentNullException(nameof(runContextProvider));
            _sequenceManager = sequenceManager ?? throw new System.ArgumentNullException(nameof(sequenceManager));
            _scatterModels = scatterModels ?? throw new System.ArgumentNullException(nameof(scatterModels));                  
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
            var crossbow = await GetCrossbow();
            var gates = await _trackPopulator.PlaceGatesAsync(_gatePrefab, track, sequence, instantiator);   
            var backgroundScatter = await _trackPopulator.SpreadBackgroundScatterAsync(
                _scatterModels.AllGroups, track, 
                (dencityCoefficient: 1, width: 180), instantiator);   
            var targets = await _targetGenerator.GetTargetAsync(_targetPrefabs, targetScore, targetCountRange, instantiator);   
                        
            await PlaceAtTrackEnd(targets, track, new Vector3(0, -105, 105));            
            await AssemblePlayfield(track, gates, targets, crossbow);            
            
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
        
        async Task AssemblePlayfield(Spline track, GameObject gates, GameObject targets, GameObject crossbow)
        {            
            var playfieldAsemblySemaphore = new SemaphoreSlim(0, 1);
            UnityMainThreadDispatcher.Instance().Enqueue(() => {
                StartCoroutine(PlayfieldAssemblyCoroutine(track, gates, targets, crossbow,
                playfieldAsemblySemaphore));});
            await playfieldAsemblySemaphore.WaitAsync();  
        }
        
        IEnumerator PlaceAtEndCoroutine(GameObject entity, Vector3 position, SemaphoreSlim semaphore)
        {   
            entity.transform.position = position;
            semaphore.Release();
            yield return null;
        }
        
        IEnumerator PlayfieldAssemblyCoroutine(Spline track, GameObject gates, GameObject targets, GameObject crossbow, SemaphoreSlim semaphore)
        {   
            var playfieldObject = new GameObject("playfield");   
            _playfield = new Playfield(track.gameObject, crossbow, gates, targets, playfieldObject);
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
            _projectile = _projectileGenerator.CreateSelected(initialCount, movementWidth, assetInstatiator);     
                       
            _projectile.transform.SetParent(_follower.Transform);   
            AttachCameraToFollower(_follower);
                         
            semaphore.Release();
            yield return null;
        }  
                 
        
        async Task<GameObject> GetCrossbow()
        {            
            var crossbowCreationSemaphore = new SemaphoreSlim(0, 1);
            UnityMainThreadDispatcher.Instance().Enqueue(() => {StartCoroutine(CrossbowCreationCoroutine(crossbowCreationSemaphore));});
            await crossbowCreationSemaphore.WaitAsync();
            return _crossbow;
        }
        
        IEnumerator CrossbowCreationCoroutine(SemaphoreSlim semaphore)
        {   
            _crossbow = _crossbowGenerator.CreateSelected();   
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