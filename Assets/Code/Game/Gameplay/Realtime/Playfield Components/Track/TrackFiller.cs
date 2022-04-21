using Game.Gameplay.Realtime.OperationSequence.Operation;
using Game.Gameplay.Realtime.PlayfieldComponents.Track.TrackItems;
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
        private Vector3 _gateOffset = new Vector3(4.5f, 4f, 0f);
        private OperationExecutor _exec;
        private float _runUpLength = 40;
        private int _nextPairID = 0;
        
        private GameObject _allGates;
        private GameObject _gatePrefab = null;
        private Spline _track = null;
        private OperationPairsSequence _sequence = null;
        private float _positionIndent = 0;
        private float _offsetOnTrack = 0;
        private List<CurveSample> _pointsOnTrack;
        private List<GameObject> _gatePairs;
        
        [Inject]
        public void Construct(OperationExecutor exec)
        {
            if(exec == null)
                throw new System.Exception("OperationExecutor isn't provided to TrackFiller"); 
                               
            _exec = exec;
        }
                    
        public async Task<GameObject> PlaceGatesAsync(GameObject gatePrefab, Spline track, OperationPairsSequence sequence)
        {
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
        
        // List<GameObject> CreateGates()
        // {
        //     _allGates = Instantiate(gameObject, _track.gameObject.transform.position, Quaternion.identity);
        //     _allGates.name = "Gates";
        //     var gates = new List<(GameObject left, GameObject right)>();
        //     var gatePairs = new List<GameObject>();
        //     foreach(var operationPair in _sequence.Sequence)
        //         gates.Add(InstatiateGatePair(operationPair));
        //     foreach(var gatePair in gates)
        //         gatePairs.Add(EncapsulateGatePair(gatePair, _allGates));
            
        //     return gatePairs;
        // }
        
        // void MoveGates(List<GameObject> gates, List<CurveSample> points)
        // {
        //     var gatesAndPoints = gates.Zip(points, (g, p) => new { Gate = g, Point = p });
        //     foreach(var pair in gatesAndPoints)
        //     {
        //         pair.Gate.transform.position = pair.Point.location;
        //         pair.Gate.transform.rotation = pair.Point.Rotation;
        //     }
        // }
        
        (GameObject left, GameObject right) InstatiateGatePair(OperationPair pair)
        {
            var leftGate = CreateGate(_gatePrefab, pair.LeftOperation, true);
            var rightGate = CreateGate(_gatePrefab, pair.RightOperation, false);
            return (leftGate, rightGate);
        }        
        
        GameObject CreateGate(GameObject gatePrefab, OperationInstance operation, bool isLeft)
        {                
            if(operation.Type != Operation.Blank)
            {
                var position = new Vector3(
                        (isLeft) ? _gateOffset.x*(-1) : _gateOffset.x,
                        _gateOffset.y,
                        _gateOffset.z);
                var ring = Instantiate(gatePrefab, position, Quaternion.identity);                    
                ring.name = (isLeft) ? "Left ring" : "Right ring";
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
            AddGateToPair(pairGO, pair.left);
            AddGateToPair(pairGO, pair.right);
            return pairGO;
        }
        
        void AddGateToPair(GameObject pair, GameObject gate)
        {
            if(pair == null || gate == null)
                return;
                        
            gate.transform.SetParent(pair.transform, false);  
        }
        
        
        
        
        
        
        public GameObject PlaceGates(GameObject gatePrefab, Spline track, OperationPairsSequence sequence)
        {
            var positionIndent = (track.Length - _runUpLength) / (sequence.Sequence.Count+1);
            var offsetOnTrack = positionIndent + _runUpLength;    
            var pointsOnTrack = CalculatePositionsForGates(track, sequence.Sequence.Count, positionIndent, offsetOnTrack);            
            var gatesArray = PlaceGatesAtPointsOnMainThread(gatePrefab, sequence, pointsOnTrack); 
            var gates = Instantiate(gameObject, track.gameObject.transform.position, Quaternion.identity);
            gates.name = "Gates";                                 
            foreach(var gatePair in gatesArray)
                gatePair.transform.SetParent(gates.transform);            
            return gates;
        }
                        
        public GameObject SpreadObjects(List<GameObject> prefabsToSpread, int dencityCoefficient)
        {
            return gameObject;                
        }
        
        List<GameObject> PlaceGatesAtPointsOnMainThread(GameObject gatePrefab, OperationPairsSequence sequence, List<SplineMesh.CurveSample> points)
        {
            var gates = new List<GameObject>();
            var index = 0;            
            foreach(var operationPair in sequence.Sequence)
            {               
                gates.Add(CreateGatePairOnMainThread(gatePrefab, operationPair, points[index].location, points[index].Rotation));
                index++;
            }
            return gates;
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
        
        GameObject CreateGatePairOnMainThread(GameObject gatePrefab, OperationPair pair, Vector3 position, Quaternion rotation)
        {
            var leftGate = CreateGate(gatePrefab, pair.LeftOperation, true);
            var rightGate = CreateGate(gatePrefab, pair.RightOperation, false);
            
            var pairInstance = Instantiate(gameObject, position, rotation);
            pairInstance.name = "Gate Pair";
            UnityMainThreadDispatcher.Instance().Enqueue(() => InitializeGate(leftGate, pairInstance, _nextPairID)); // OPTIMIZATION_POINT register to be done later so to optimise creation loop for cache (first everyghing is generated, and then set to update in scene)
            UnityMainThreadDispatcher.Instance().Enqueue(() => InitializeGate(rightGate, pairInstance, _nextPairID));
            
            _nextPairID++;
            return pairInstance;        
        }
                
        void InitializeGate(GameObject gate, GameObject pairInstance, int id)
        {
            if(gate == null)
                return;
            
            gate.transform.SetParent(pairInstance.transform, false);  
            gate.name = "Gate - " + id.ToString();                 
        }
    }
}