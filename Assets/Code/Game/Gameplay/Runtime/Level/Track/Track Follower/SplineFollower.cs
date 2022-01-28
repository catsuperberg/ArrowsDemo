using UnityEngine;
using SplineMesh;
using System;

namespace Game.Gameplay.Runtime.Level.Track
{
    public class SplineFollower : MonoBehaviour, ITrackFollower
    {       
        public Transform Transform {get {return gameObject.transform;} private set{;}}
        public event EventHandler OnFinished;
         
        public float Speed {get; private set;} = 0;
        public float Position {get; private set;} = 0;
        public bool Moving {get; private set;} = false;
        public bool Finished {get; private set;} = false;
        
        private Spline _spline;
        private CurveSample _sample = new CurveSample();
        
        public void SetSplineToFollow(Spline spline, float startingPoint)
        {
            Finished = false;
            _spline = spline;
            Position = startingPoint;
            _sample = _spline.GetSampleAtDistance(Position);
            UpdateTransform();
        }
        
        void Update()
        {
            if(Moving)
            {                
                goToNextPointOnSpline();
                _sample = _spline.GetSampleAtDistance(Position);
                UpdateTransform();
            }
        }
        
        public void MoveToLength(float newPosition)
        {            
            Finished = false;
            Position = newPosition;
            _sample = _spline.GetSampleAtDistance(Position);
            UpdateTransform();
        }
        
        public void SetSpeed(float speed)
        {
            Finished = false;
            Speed = speed;
        }
        
        public void StartMovement()
        {
            Moving = true;
        }
        
        public void ToggleMovement()
        {
            Moving = !Moving;            
        }
        
        public void PauseMovement()
        {
            Moving = false;
        }
        
        
        
        void goToNextPointOnSpline()
        {
            Position = getNextFramePosition();
            _sample = _spline.GetSampleAtDistance(Position);
            UpdateTransform();         
        }
        
        float getNextFramePosition()
        {
            var newPosition = Position + Speed * Time.deltaTime;
            if(newPosition >= _spline.Length)
            {
                Moving = false; 
                Finished = true;
                OnFinished?.Invoke(this, EventArgs.Empty);
                return Position;
            } 
            return newPosition;
        }
        
        void UpdateTransform()
        {            
            gameObject.transform.position = _sample.location;
            gameObject.transform.rotation = _sample.Rotation;
        }        
    }   
}