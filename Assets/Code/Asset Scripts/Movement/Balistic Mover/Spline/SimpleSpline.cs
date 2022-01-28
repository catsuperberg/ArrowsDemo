using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AssetScripts.Movement
{
    public class SimpleSpline : MonoBehaviour
    {
        public float SplineLength {get; private set;}
        public Vector3 _startPosition {get; private set;}
        public Vector3 _endPosition {get; private set;}
        public Vector3 _enterHandlePosition {get; private set;}
        public Vector3 _exitHandlePosition {get; private set;}
        
        public void initialize(Transform startTransform, Transform endTransform, float enterSmoothMagnitude, float exitSmoothMagnitude)
        {
            _startPosition = startTransform.position;
            _endPosition = endTransform.position;
            var startForwardVector = startTransform.rotation * Vector3.forward;
            var endForwardVector = endTransform.rotation * Vector3.forward;
            _enterHandlePosition = _startPosition + startForwardVector * enterSmoothMagnitude;
            _exitHandlePosition = _endPosition - endForwardVector * exitSmoothMagnitude;
            SplineLength = CalculateSplineLength();        
        }
            
        public Vector3 PositionAt(float length)
        {
            if(length > SplineLength)
                length = SplineLength;
            var t = length/SplineLength;
            return InterpolatedPosition(t);
        }
        
        public Vector3 ForwardAt(float length)
        {
            float firstPoint;
            float secondPoint;
            if(length >= 0.01f)
            {
                if(length <= (SplineLength - 0.01f))
                {
                    firstPoint = length;
                    secondPoint = length + 0.01f;
                }
                else
                {
                    firstPoint = SplineLength - 0.01f;
                    secondPoint = SplineLength;                
                }
            }
            else
            {
                firstPoint = 0.00f;
                secondPoint = 0.01f;
            }
            return (PositionAt(secondPoint) - PositionAt(firstPoint)).normalized;
        }
        
        public Vector3 InterpolatedPosition(float t)
        {
            if (t < 1)
                return transform.position + CubicLerp(_startPosition, _enterHandlePosition, _exitHandlePosition, _endPosition, t);
            else
                return transform.position + _endPosition;
        }
        
        public float CalculateSplineLength(float stepSize = .01f)
        {
            float length = 0f;
            Vector3 lastPosition = InterpolatedPosition(0f);
            for (float t = 0; t < 1f; t += stepSize)        
            {
                length += Vector3.Distance(lastPosition, InterpolatedPosition(t));
                lastPosition = InterpolatedPosition(t);
            }
            length += Vector3.Distance(lastPosition, InterpolatedPosition(1f));
            return length;
        }
            
        
        private Vector3 QuadraticLerp(Vector3 startPosition, Vector3 handlePosition, Vector3 endPosition, float t)
        {
            Vector3 pointOnFirstPath = Vector3.Lerp(startPosition, handlePosition, t);
            Vector3 pointOnSecondPath = Vector3.Lerp(handlePosition, endPosition, t);

            return Vector3.Lerp(pointOnFirstPath, pointOnSecondPath, t);
        }

        private Vector3 CubicLerp(Vector3 startPosition, Vector3 firstHandle, Vector3 secondHandle, Vector3 endPosition, float t)
        {
            Vector3 pointOnFirstPath = QuadraticLerp(startPosition, firstHandle, secondHandle, t);
            Vector3 pointOnSecondPath = QuadraticLerp(firstHandle, secondHandle, endPosition, t);

            return Vector3.Lerp(pointOnFirstPath, pointOnSecondPath, t);
        }
    }
}