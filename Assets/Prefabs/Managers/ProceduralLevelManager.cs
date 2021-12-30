using Level.Track;
using Sequence;
using SplineMesh;
using System.Collections.Generic;
using System.Numerics;
using System.Linq;
using UnityEngine;
using Zenject;
using Vector3 = UnityEngine.Vector3;

namespace Level
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
        ITargerProvider _targetGenerator;
        
        public GameObject SplineTrack {get; private set;} = null;
        public GameObject Gates {get; private set;}  = null;
        public GameObject Targets {get; private set;}  = null;
        public GameObject Level {get; private set;}  = null;
                
        [Inject]
        public void Construct(ISplineTrackProvider splineMeshGenerator, ITrackPopulator trackPopulator, ITargerProvider targetGenerator)
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
        
        public GameObject InitializeLevel(SequenceContext context, OperationPairsSequence sequence, BigInteger targetResult)
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
            
            var track = new GameObject("Track");       
            
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
            Targets = _targetGenerator.GetSuitableTarget(_targetPrefabs, targetResult, numberOfTargetsRange);   
            stopwatch.Stop();
            Debug.Log("Generating target took: " + stopwatch.ElapsedMilliseconds + " ms"); 
            
            
            PlaceAtEnd(Targets, SplineTrack.GetComponent<Spline>(), new Vector3(0, -105, 105));
            
            SplineTrack.transform.SetParent(track.transform);
            Targets.transform.SetParent(track.transform);
            Gates.transform.SetParent(track.transform);
            Level = track;
            return track;
        }
        
        void PlaceAtEnd(GameObject entity, Spline spline, Vector3 offset)
        {
            var endPoint = spline.nodes.Last().Position;
            entity.transform.position = endPoint + offset;
        }
    }    
}