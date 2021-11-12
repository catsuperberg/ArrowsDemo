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
        
        public void InitializeLevel(SequenceContext context, OperationPairsSequence sequence, BigInteger targetResult)
        {            
            var track = _splineMeshGenerator.GetRandomizedTrack(context.Length, _trackSplineMesh);
            var gates = _trackPopulator.PlaceGates(_gatePrefab, track.GetComponent<Spline>(), sequence);
            (int Min, int Max) numberOfTargetsRange = (1, 20);
            var targets = _targetGenerator.GetSuitableTarget(_targetPrefabs, targetResult, numberOfTargetsRange);
            PlaceAtEnd(targets, track.GetComponent<Spline>(), new Vector3(0, -120, 85));
            
            track.transform.SetParent(gameObject.transform);
        }
        
        void PlaceAtEnd(GameObject entity, Spline spline, Vector3 offset)
        {
            var endPoint = spline.nodes.Last().Position;
            entity.transform.position = endPoint + offset;
        }
        
        
        void UpdateLevel(SequenceContext context)
        {
            
            // var numberOfPrecalculatedTargets = 5;
            // PopulateTargets(numberOfPrecalculatedTargets);
        }
        
        void PopulateTargets(int listSize)
        {
            //TODO how to get length indent per upgrade
            //TODO target generation goes here (creating target generator, using it's methods)
            // var context = new TrackContext(length, _context.InitialValue, _context.NumberOfGates);
            // var targetScore = _trackGenerator.GetAverageMaxScore(context);
            // _targetGenerator.GetRandomisedTarget(biome, targetScore);
        }
    }    
}