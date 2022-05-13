using AssetScripts.Instantiation;
using SplineMesh;
using System.Collections;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace Game.Gameplay.Realtime.PlayfieldComponents.Track
{
    public class RandomizedSmoothTrackGenerator : MonoBehaviour, ISplineTrackProvider 
    {
        (float Min, float Max) _posRgt = (-20f, 20f);
        (float Min, float Max) _posUp  = (-2f, 5f);
        (float Min, float Max) _posFwd = (80f, 40f);
        
        (float Min, float Max) _dirRgt = (-2f, 2f);
        (float Min, float Max) _dirUp  = (-2f, 4f);
        (float Min, float Max) _dirFwd = (8f, 20f);
        
        Spline _splineMesh;
        GameObject _splineMeshPrefab;
        IInstatiator _assetInstatiator;
        float _trackLength;
        
        public async Task<Spline> GetRandomizedTrackAsync(float length, GameObject splineMeshPrefab, IInstatiator assetInstatiator)
        {
            if(assetInstatiator == null)
                throw new System.ArgumentNullException("IInstatiator isn't provided for: " + this.GetType().Name);
            
            _assetInstatiator = assetInstatiator;
                
            _splineMesh = null;
            _splineMeshPrefab = splineMeshPrefab;
            _trackLength = length;
            var trackGenerationSemaphore = new SemaphoreSlim(0, 1);
            UnityMainThreadDispatcher.Instance().Enqueue(() => {StartCoroutine(TrackGenerationCoroutine(trackGenerationSemaphore));});    
            await trackGenerationSemaphore.WaitAsync();                        
            return _splineMesh;
        }
        
        IEnumerator TrackGenerationCoroutine(SemaphoreSlim semaphore)
        {            
            var spline = _assetInstatiator.Instantiate(_splineMeshPrefab);
            var splineComponent = spline.GetComponent<Spline>();            
            randomizeSplineToLength(splineComponent, _trackLength); 
            splineComponent.RefreshCurves();
            _splineMesh = splineComponent;
            
            semaphore.Release();
            yield return null;
        }
        
        public void randomizeSplineToLength(Spline spline, float length)
        {
            if(spline.nodes.Any())
                spline.nodes.Clear();
            spline.AddNode(new SplineNode(Vector3.zero,Vector3.forward*_dirFwd.Max));  
            
            while(spline.Length < length)
                spline.AddNode(GenerateNextNodeSmoothly(spline.nodes.Last()));
            TrimSplineToLength(spline, length); 
        }
        
        public SplineNode GenerateNextNodeSmoothly(SplineNode previousNode)
        {                              
            var node = new SplineNode(Vector3.zero, Vector3.zero);
            var randomPosition = new Vector3(Random.Range(_posRgt.Min, _posRgt.Max),
                Random.Range(_posUp.Min, _posUp.Max),
                Random.Range(_posFwd.Min, _posFwd.Max));
            var randomDirection = new Vector3(Random.Range(_dirRgt.Min, _dirRgt.Max),
                Random.Range(_dirUp.Min, _dirUp.Max),
                Random.Range(_dirFwd.Min, _dirFwd.Max));
            node.Position = previousNode.Position + randomPosition;
            node.Direction = node.Position + randomDirection;
            return node;                         
        }
        
        public void TrimSplineToLength(Spline spline, float length)
        {              
            var endSample = spline.GetSampleAtDistance(length);
            var offset = _posFwd.Min*0.5f;
            var previousNodeSample = spline.GetSampleAtDistance(length-offset);
            
            var lastNode = spline.nodes.Last();                
            lastNode.Position = endSample.location;
            lastNode.Direction = lastNode.Position + (Vector3.forward*_dirFwd.Max);
            
            var previousNode = spline.nodes[spline.nodes.Count-2];
            var previousNodeDirection = previousNode.Direction - previousNode.Position;
            previousNode.Position = previousNodeSample.location; 
            previousNode.Direction = previousNode.Position + previousNodeDirection/2; 
        }
        
    }
}

