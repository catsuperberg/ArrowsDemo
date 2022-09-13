using AssetScripts.Instantiation;
using Game.Gameplay.Realtime.OperationSequence.Operation;
using Game.Gameplay.Realtime.PlayfieldComponents.Track.TrackItems;
using GameMath;
using SplineMesh;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using Zenject;

namespace Game.Gameplay.Realtime.PlayfieldComponents.Track
{
    public class TrackFiller : MonoBehaviour, ITrackPopulator
    {           
        Vector3 _gateOffset = new Vector3(4.5f, 4f, 0f);
        OperationExecutor _exec;
        float _runUpLength = 40;
        int _nextPairID = 0;
        
        
        GameObject _allGates;
        GameObject _gatePrefab = null;
        Spline _track = null;
        OperationPairsSequence _sequence = null;
        float _positionIndent = 0;
        float _offsetOnTrack = 0;
        List<CurveSample> _pointsOnTrack;
        List<GameObject> _gatePairs;
        IInstatiator _assetInstatiator;
        
        [Inject]
        public void Construct(OperationExecutor exec)
        {
            if(exec == null)
                throw new System.Exception("OperationExecutor isn't provided to TrackFiller"); 
                               
            _exec = exec;
        }
                    
        public async Task<GameObject> PlaceGatesAsync(GameObject gatePrefab, Spline track, OperationPairsSequence sequence, IInstatiator assetInstatiator)
        {
            if(assetInstatiator == null)
                throw new System.ArgumentNullException("IInstatiator isn't provided for: " + this.GetType().Name);
            
            _assetInstatiator = assetInstatiator;
            
            _allGates = null;
            _gatePrefab = gatePrefab;
            _track = track;
            _sequence = sequence;
            _positionIndent = (_track.Length - _runUpLength) / (_sequence.Sequence.Count+1);
            _offsetOnTrack = _positionIndent + _runUpLength;  
            _pointsOnTrack = CalculatePositionsForGates(_track, _sequence.Sequence.Count, _positionIndent, _offsetOnTrack);   
            
            var gateCreationSemaphore = new SemaphoreSlim(0, 1);
            UnityMainThreadDispatcher.Instance().Enqueue(() => {StartCoroutine(CreationCoroutine(gateCreationSemaphore));});
            await gateCreationSemaphore.WaitAsync();    
            var gatePlacementSemaphore = new SemaphoreSlim(0, 1);
            UnityMainThreadDispatcher.Instance().Enqueue(() => {StartCoroutine(PlacementCoroutine(gatePlacementSemaphore));});
            await gatePlacementSemaphore.WaitAsync();          
            return _allGates;
        }
        
        IEnumerator CreationCoroutine(SemaphoreSlim semaphore)
        {                  
            _allGates = Instantiate(gameObject, _track.gameObject.transform.position, Quaternion.identity);
            _allGates.name = "Gates";
            var gates = new List<(GameObject left, GameObject right)>();
            var gatePairs = new List<GameObject>();
            foreach(var operationPair in _sequence.Sequence)
                gates.Add(InstatiateGatePair(operationPair));
                if(Time.deltaTime >= 0.01)
                   yield return null;
            foreach(var gatePair in gates)
                gatePairs.Add(EncapsulateGatePair(gatePair, _allGates));                
                if(Time.deltaTime >= 0.01)
                   yield return null;
                
            _gatePairs = gatePairs;
            
            semaphore.Release();
        }
        
        IEnumerator PlacementCoroutine(SemaphoreSlim semaphore)
        {                  
            var gatesAndPoints = _gatePairs.Zip(_pointsOnTrack, (g, p) => new { Gate = g, Point = p });
            foreach(var pair in gatesAndPoints)
            {
                pair.Gate.transform.position = pair.Point.location;
                pair.Gate.transform.rotation = pair.Point.Rotation;                
                if(Time.deltaTime >= 0.01)
                   yield return null;
            }
            
            semaphore.Release();
        }
        
        (GameObject left, GameObject right) InstatiateGatePair(OperationPair pair)
        {
            var leftGate = CreateGateHiden(_gatePrefab, pair.LeftOperation, true);
            var rightGate = CreateGateHiden(_gatePrefab, pair.RightOperation, false);
            return (leftGate, rightGate);
        }        
        
        GameObject CreateGateHiden(GameObject gatePrefab, OperationInstance operation, bool isLeft)
        {                
            if(operation.Type != Operation.Blank)
            {
                var gatePosition = new Vector3(
                        (isLeft) ? _gateOffset.x*(-1) : _gateOffset.x,
                        _gateOffset.y,
                        _gateOffset.z);
                var ring = _assetInstatiator.Instantiate(gatePrefab, name: (isLeft) ? "Left ring" : "Right ring", position: gatePosition);     
                var ringLogic = ring.GetComponent<Ring>();
                ringLogic.Initialize(operation, _exec);
                return ring;
            }                 
            else
                return null;
        }
        
        GameObject EncapsulateGatePair((GameObject left, GameObject right) pair, GameObject allGates)
        {
            var pairGO = Instantiate(gameObject, _track.gameObject.transform.position, Quaternion.identity);
            pairGO.name = "Gate pair";
            pairGO.transform.SetParent(allGates.transform);
            AddPairContainer(pairGO, pair);
            AddGateToPair(pairGO, pair.left);
            AddGateToPair(pairGO, pair.right);
            return pairGO;
        }
        
        void AddPairContainer(GameObject pair, (GameObject left, GameObject right) gates)
        {
            var left = (gates.left != null) ? gates.left.GetComponent<Ring>().Operation : new OperationInstance(Operation.Blank, 0);
            var right = (gates.right != null) ? gates.right.GetComponent<Ring>().Operation : new OperationInstance(Operation.Blank, 0);
            var pairContainer =  pair.AddComponent<OperationPairComponent>();
            pairContainer.Initialize(new OperationPair(left, right));
        }
        
        void AddGateToPair(GameObject pair, GameObject gate)
        {
            if(pair == null || gate == null)
                return;
                        
            gate.transform.SetParent(pair.transform, false);  
        }
        
        List<SplineMesh.CurveSample> CalculatePositionsForGates(Spline track, int pointCount, float positionIndent, float offsetOnTrack)
        {
            var points = new List<SplineMesh.CurveSample>();            
            for(int point = 0; point < pointCount; point++)
            {                    
                points.Add(track.GetSampleAtDistance(offsetOnTrack));
                offsetOnTrack += positionIndent;    
            }
            return points;
        }
        
                
        List<GameObject> _scattered;
        GameObject _scatterContainer;
        
        public async Task<GameObject> SpreadBackgroundScatterAsync(
            IEnumerable<IEnumerable<GameObject>> GroupsOfPrefabsToSpread, Spline track, 
            (float dencityCoefficient, float width) parameters, IInstatiator assetInstatiator)
        {                        
            
            var scatterSemaphore = new SemaphoreSlim(0, 1);
            UnityMainThreadDispatcher.Instance().Enqueue(() => {
                    var scatterPlacer = new TrackBackgroundScatterPlacer(GroupsOfPrefabsToSpread, track, parameters);
                    var toInstantiate = scatterPlacer.SpreadAssets();                       
                    StartCoroutine(ScatterCoroutine(toInstantiate, assetInstatiator, scatterSemaphore));});
            await scatterSemaphore.WaitAsync();  
            
            return _scatterContainer;
        }
        
        IEnumerator ScatterCoroutine(
            List<(GameObject prefab, Vector3 position, Quaternion rotation)> toInstantiate, 
            IInstatiator assetInstatiator, SemaphoreSlim semaphore)
        {
            _scattered = new List<GameObject>(); 
            
            foreach(var asset in toInstantiate)
            {
                _scattered.Add(assetInstatiator.Instantiate(asset.prefab, asset.position, asset.rotation));
                if(Time.deltaTime >= 0.01)
                    yield return null;
            }
            _scatterContainer = Instantiate(gameObject, _track.gameObject.transform.position, Quaternion.identity);
            _scatterContainer.name = "Background Scatter";  
            _scattered.ForEach(entry => entry.transform.SetParent(_scatterContainer.transform));   
                    
            semaphore.Release();
        }
    }
}