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
        
        GameObject _splineTrack = null;
        GameObject _gates = null;
        GameObject _targets = null;
                
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
            if(_splineTrack != null)
                Destroy(_splineTrack);
            if(_gates != null)
                Destroy(_gates);
            if(_gates != null)
                Destroy(_targets);
            _splineTrack = _splineMeshGenerator.GetRandomizedTrack(context.Length, _trackSplineMesh);
            _gates = _trackPopulator.PlaceGates(_gatePrefab, _splineTrack.GetComponent<Spline>(), sequence);
            (int Min, int Max) numberOfTargetsRange = (1, 20);
            _targets = _targetGenerator.GetSuitableTarget(_targetPrefabs, targetResult, numberOfTargetsRange);
            PlaceAtEnd(_targets, _splineTrack.GetComponent<Spline>(), new Vector3(0, -120, 85));
            
            _splineTrack.transform.SetParent(gameObject.transform);
            return gameObject;
        }
        
        void PlaceAtEnd(GameObject entity, Spline spline, Vector3 offset)
        {
            var endPoint = spline.nodes.Last().Position;
            entity.transform.position = endPoint + offset;
        }
    }    
}