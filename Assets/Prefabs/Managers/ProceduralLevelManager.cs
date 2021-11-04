using Level.Track;
using Sequence;
using UnityEngine;
using Zenject;

namespace Level
{
    public class ProceduralLevelManager : MonoBehaviour, ILevelManager
    {        
        [SerializeField]
        private GameObject _trackSplineMesh;
        ISplineTrackProvider _splineMeshGenerator;
        // ITargerProvider _targetGenerator;
        // primaryContext _context;
        
        // public void 
                
        [Inject]
        public void Construct(ISplineTrackProvider splineMeshGenerator)
        {
             if(splineMeshGenerator == null)
                throw new System.Exception("ISplineTrackProvider isn't provided to ProceduralLevelManager");
                
            _splineMeshGenerator = splineMeshGenerator;
        }
        
        public void InitializeLevel(SequenceContext context, OperationPairsSequence sequence)
        {
            GameObject track = _splineMeshGenerator.GetRandomizedTrack(context.Length, _trackSplineMesh);
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