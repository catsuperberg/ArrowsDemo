using Game.Gameplay.Realtime.PlayfieldComponents.Target;
using Game.Gameplay.Realtime.PlayfieldComponents.Track;
using Game.Gameplay.Realtime.OperationSequence.Operation;
using SplineMesh;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using UnityEngine;
using Zenject;

using Vector3 = UnityEngine.Vector3;

namespace Game.Gameplay.Realtime.PlayfieldComponents
{
    public class ProceduralLevelManager : MonoBehaviour, ILevelManager
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
        
        public GameObject SplineTrack {get; private set;} = null;
        public GameObject Gates {get; private set;}  = null;
        public GameObject Targets {get; private set;}  = null;
        public GameObject Level {get; private set;}  = null;
                
        [Inject]
        public void Construct(ISplineTrackProvider splineMeshGenerator, ITrackPopulator trackPopulator, ITargetProvider targetGenerator)
        {
             if(splineMeshGenerator == null)
                throw new System.Exception("ISplineTrackProvider isn't provided to ProceduralLevelManager");
             if(trackPopulator == null)
                throw new System.Exception("ITrackPopulator isn't provided to ProceduralLevelManager");
             if(targetGenerator == null)
                throw new System.Exception("ITargerProvider isn't provided to ProceduralLevelManager");
                
            _splineMeshGenerator = splineMeshGenerator;
            _trackPopulator = trackPopulator;
            _targetGenerator = targetGenerator;
        }
        
        public GameObject InitializeLevel(SequenceContext context, OperationPairsSequence sequence, BigInteger targetScore)
        {            
            if(SplineTrack != null)
                Destroy(SplineTrack);
            if(Gates != null)
            {
                foreach(Transform child in Gates.transform)
                    Destroy(child.gameObject); 
                Destroy(Gates);                
            }
            if(Targets != null)
            {
                foreach(Transform child in Targets.transform)
                    Destroy(child.gameObject); 
                Destroy(Targets);                
            }     
            
            var level = new GameObject("Level");       
            
            var stopwatch = System.Diagnostics.Stopwatch.StartNew();
            
            SplineTrack = _splineMeshGenerator.GetRandomizedTrack(context.Length, _trackSplineMesh);
            stopwatch.Stop();
            Debug.Log("Generating track took: " + stopwatch.ElapsedMilliseconds + " ms");            
            stopwatch.Restart();
            
            
            Gates = _trackPopulator.PlaceGates(_gatePrefab, SplineTrack.GetComponent<Spline>(), sequence);
            stopwatch.Stop();
            Debug.Log("Placing gates took: " + stopwatch.ElapsedMilliseconds + " ms");      
            stopwatch.Restart();
            
            
            (int Min, int Max) numberOfTargetsRange = (1, 20);  
            Targets = _targetGenerator.GetSuitableTarget(_targetPrefabs, targetScore, numberOfTargetsRange);   
            stopwatch.Stop();
            Debug.Log("Generating target took: " + stopwatch.ElapsedMilliseconds + " ms"); 
            
            
            PlaceAtEnd(Targets, SplineTrack.GetComponent<Spline>(), new Vector3(0, -105, 105));
            
            SplineTrack.transform.SetParent(level.transform);
            Targets.transform.SetParent(level.transform);
            Gates.transform.SetParent(level.transform);
            Level = level;
            return level;
        }
        
        void PlaceAtEnd(GameObject entity, Spline spline, Vector3 offset)
        {
            var endPoint = spline.nodes.Last().Position;
            entity.transform.position = endPoint + offset;
        }
    }    
}