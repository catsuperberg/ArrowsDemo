using UnityEngine;

namespace GamePlay
{
    public class BallisticMover : MonoBehaviour
    {
        SimpleSpline _spline;
        float _curentPositionOnSpline = 0;
        float _speed = 0;
        
        public void initialize(Transform startTransform, Transform endTransform, float entryCurvePrioritization)
        {
            var splineTrajectory = new GameObject("Ballistic spline");
            splineTrajectory.transform.SetParent(null);
            
            var tempObj = new GameObject();
            var spread = Random.insideUnitSphere*5;
            tempObj.transform.position = endTransform.position + spread;
            tempObj.transform.rotation = endTransform.rotation;
            var endTransformWithSpread = tempObj.transform;
            
            var preCurveLength = Vector3.Distance(startTransform.position, endTransformWithSpread.position);
            var enterMagnitude = (preCurveLength*entryCurvePrioritization)*0.65f;
            var exitMagnitude = (preCurveLength*(1-entryCurvePrioritization))*0.65f;
                                    
            _spline = splineTrajectory.AddComponent<SimpleSpline>();
            _spline.initialize(startTransform, endTransformWithSpread, enterMagnitude, exitMagnitude);
        }
        
        public void StartMover(float speed)
        {
            _speed = speed;
        }
        
        void Update()
        {
            if(_speed == 0)
                return;
            
            transform.position = _spline.PositionAt(_curentPositionOnSpline);
            transform.rotation = Quaternion.LookRotation(_spline.ForwardAt(_curentPositionOnSpline));
            if(_curentPositionOnSpline < _spline.SplineLength)
                _curentPositionOnSpline += _speed * Time.deltaTime;
            else
            {
                Destroy(_spline.gameObject);
                foreach(Transform child in transform) 
                    Destroy(child.gameObject);
                Destroy(gameObject);
            }
        }
    }    
}