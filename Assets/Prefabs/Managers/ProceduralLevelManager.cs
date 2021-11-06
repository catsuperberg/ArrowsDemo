using Level.Track;
using Sequence;
using SplineMesh;
using UnityEngine;
using Zenject;

namespace Level
{
    public class ProceduralLevelManager : MonoBehaviour, ILevelManager
    {        
        [SerializeField]
        private GameObject _trackSplineMesh;
        [SerializeField]
        private GameObject _gatePrefab;
        
        ISplineTrackProvider _splineMeshGenerator;
        ITrackPopulator _trackPopulator;
        
        // ITargerProvider _targetGenerator;
        // primaryContext _context;
        
        // public void 
                
        [Inject]
        public void Construct(ISplineTrackProvider splineMeshGenerator, ITrackPopulator trackPopulator)
        {
             if(splineMeshGenerator == null)
                throw new System.Exception("ISplineTrackProvider isn't provided to ProceduralLevelManager");
             if(trackPopulator == null)
                throw new System.Exception("ITrackPopulator isn't provided to ProceduralLevelManager");
                
            _splineMeshGenerator = splineMeshGenerator;
            _trackPopulator = trackPopulator;
        }
        
        public void InitializeLevel(SequenceContext context, OperationPairsSequence sequence)
        {
            var track = _splineMeshGenerator.GetRandomizedTrack(context.Length, _trackSplineMesh);
            var gates = _trackPopulator.PlaceGates(_gatePrefab, track.GetComponent<Spline>(), sequence);
            
            track.transform.SetParent(gameObject.transform);
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