using SplineMesh;
using UnityEngine;

namespace Game.Gameplay.Realtime.PlayfieldComponents
{
    public class Playfield
    {
        public readonly Spline TrackSpline;
        public readonly GameObject Track;
        public readonly GameObject Crossbow;
        public readonly GameObject Gates;
        public readonly GameObject Targets;
        public readonly GameObject GameObject;
        public float trackWidth {get; private set;} = 12;
        
        public Playfield(GameObject track, GameObject crossbow, GameObject gates, GameObject targets, GameObject trackObject)
        {
            Track = track ?? throw new System.ArgumentNullException(nameof(track));
            Crossbow = crossbow ?? throw new System.ArgumentNullException(nameof(crossbow));
            Gates = gates ?? throw new System.ArgumentNullException(nameof(gates));
            Targets = targets ?? throw new System.ArgumentNullException(nameof(targets));  
            GameObject = trackObject ?? throw new System.ArgumentNullException(nameof(trackObject));
            
            var spline = track.GetComponentInChildren<Spline>();
            if(spline == null)
                throw new System.Exception("track in Level constructor doensn't contain spline");  
                
            TrackSpline = spline;  
                
            Crossbow.transform.SetParent(GameObject.transform);
            Track.transform.SetParent(GameObject.transform);
            Gates.transform.SetParent(GameObject.transform);
            Targets.transform.SetParent(GameObject.transform);
        }
    }
}