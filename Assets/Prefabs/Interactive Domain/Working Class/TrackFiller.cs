using Sequence;
using SplineMesh;
using System.Collections.Generic;
using UnityEngine;
using Level.Track.Items;
using Zenject;

namespace Level
{
    namespace Track
    {
        public class TrackFiller : MonoBehaviour, ITrackPopulator
        {           
            private Vector3 _gateOffset = new Vector3(4.5f, 4f, 0f);
            private OperationExecutor _exec;
            private float _runUpLength = 40;
            private int _nextPairID = 0;
            
             
            public GameObject PlaceGates(GameObject gatePrefab, Spline track, OperationPairsSequence sequence)
            {
                var positionIndent = (track.Length - _runUpLength) / (sequence.Sequence.Count+1);
                var offsetOnTrack = positionIndent + _runUpLength;              
                var gates = Instantiate(gameObject, track.gameObject.transform.position, Quaternion.identity);
                gates.name = "Gates";
                foreach(OperationPair operationPair in sequence.Sequence)
                {
                    var pointOnTrack = track.GetSampleAtDistance(offsetOnTrack);
                    offsetOnTrack += positionIndent;                    
                    var gatePair = CreateGatePair(gatePrefab, operationPair, pointOnTrack.location, pointOnTrack.Rotation);
                    gatePair.transform.SetParent(gates.transform);
                }
                
                return gates;
            }
            
            [Inject]
            public void Construct(OperationExecutor exec)
            {
                if(exec == null)
                    throw new System.Exception("OperationExecutor isn't provided to TrackFiller");
                    
                _exec = exec;
            }
            
            public GameObject SpreadObjects(List<GameObject> prefabsToSpread, int dencityCoefficient)
            {
                
                return gameObject;                
            }
            
            GameObject CreateGatePair(GameObject gatePrefab, OperationPair pair, Vector3 position, Quaternion rotation)
            {
                var pairInstance = Instantiate(gameObject, position, rotation);
                pairInstance.name = "Gate Pair";
                
                var leftGate = CreateGate(gatePrefab, pair.LeftOperation, true);
                var rightGate = CreateGate(gatePrefab, pair.RightOperation, false);
                if(leftGate != null) 
                {
                    leftGate.transform.SetParent(pairInstance.transform, false);  
                    leftGate.name = "Gate - " + _nextPairID.ToString();  
                }      
                if(rightGate != null) 
                {
                    rightGate.transform.SetParent(pairInstance.transform, false);  
                    rightGate.name = "Gate - " + _nextPairID.ToString(); 
                }
                _nextPairID++;
                return pairInstance;        
            }
            
            GameObject CreateGate(GameObject gatePrefab, OperationInstance operation, bool isLeft)
            {                
                if(operation.operationType != Operations.Blank)
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
        }
    }
}