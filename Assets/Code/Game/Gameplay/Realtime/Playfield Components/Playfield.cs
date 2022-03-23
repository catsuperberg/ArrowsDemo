using SplineMesh;
using UnityEngine;

namespace Game.Gameplay.Realtime.PlayfieldComponents
{
    public class Playfield
    {
        public Spline TrackSpline  {get; private set;}
        public GameObject Track {get; private set;}
        public GameObject Gates {get; private set;}
        public GameObject Targets {get; private set;}
        public GameObject GameObject {get; private set;}
        public float trackWidth {get; private set;} = 12;
        
        public Playfield(GameObject track, GameObject gates, GameObject targets, GameObject trackObject)
        {
            if(track == null)
                throw new System.Exception("track isn't provided to Level constructor");
            if(gates == null)
                throw new System.Exception("gates isn't provided to Level constructor");
            if(targets == null)
                throw new System.Exception("targets isn't provided to Level constructor");          
            if(trackObject == null)
                throw new System.Exception("trackObject isn't provided to Level constructor");           
            var spline = track.GetComponentInChildren<Spline>();
            if(spline == null)
                throw new System.Exception("track in Level constructor doensn't contain spline");      
            
            Track = track;
            Gates = gates;
            Targets = targets;  
            TrackSpline = spline;  
            GameObject = trackObject;
            
            Track.transform.SetParent(GameObject.transform);
            Gates.transform.SetParent(GameObject.transform);
            Targets.transform.SetParent(GameObject.transform);
        }
    }
}