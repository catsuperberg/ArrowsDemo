using Level.Track;
using Sequence;
using SplineMesh;
using System.Collections.Generic;
using System.Numerics;
using System.Linq;
using UnityEngine;
using Vector3 = UnityEngine.Vector3;
using Zenject;

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
                Destroy(Gates);
            if(Gates != null)
                Destroy(Targets);
                
            Debug.Log("initializing track");
            var track = new GameObject("Track");       
            
            SplineTrack = _splineMeshGenerator.GetRandomizedTrack(context.Length, _trackSplineMesh);
            Gates = _trackPopulator.PlaceGates(_gatePrefab, SplineTrack.GetComponent<Spline>(), sequence);
            (int Min, int Max) numberOfTargetsRange = (1, 20);
            Targets = _targetGenerator.GetSuitableTarget(_targetPrefabs, targetResult, numberOfTargetsRange);
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