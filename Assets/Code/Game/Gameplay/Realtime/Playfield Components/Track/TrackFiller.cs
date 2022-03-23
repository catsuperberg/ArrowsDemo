using Game.Gameplay.Realtime.PlayfieldComponents.Track.TrackItems;
using Game.Gameplay.Realtime.OperationSequence.Operation;
using SplineMesh;
using System.Collections.Generic;
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
        
        [Inject]
        public void Construct(OperationExecutor exec)
        {
            if(exec == null)
                throw new System.Exception("OperationExecutor isn't provided to TrackFiller"); 
                               
            _exec = exec;
        }
            
        public GameObject PlaceGates(GameObject gatePrefab, Spline track, OperationPairsSequence sequence)
        {
            var positionIndent = (track.Length - _runUpLength) / (sequence.Sequence.Count+1);
            var offsetOnTrack = positionIndent + _runUpLength;    
            var pointsOnTrack = CalculatePositionsForGates(track, sequence.Sequence.Count, positionIndent, offsetOnTrack);            
            var gatesArray = PlaceGatesAtPoints(gatePrefab, sequence, pointsOnTrack); 
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
        
        List<GameObject> PlaceGatesAtPoints(GameObject gatePrefab, OperationPairsSequence sequence, List<SplineMesh.CurveSample> points)
        {
            var gates = new List<GameObject>();
            var index = 0;            
            foreach(var operationPair in sequence.Sequence)
            {               
                gates.Add(CreateGatePair(gatePrefab, operationPair, points[index].location, points[index].Rotation));
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
        
        GameObject CreateGatePair(GameObject gatePrefab, OperationPair pair, Vector3 position, Quaternion rotation)
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
        
        void InitializeGate(GameObject gate, GameObject pairInstance, int id)
        {
            if(gate == null)
                return;
            
            gate.transform.SetParent(pairInstance.transform, false);  
            gate.name = "Gate - " + id.ToString();                 
        }
    }
}