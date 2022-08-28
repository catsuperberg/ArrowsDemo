namespace Game.Microinteracions
{
    public class RingAnimationPacket : IMicrointerationPacket
    {            
        public readonly bool CorrectChoice;
        
        public RingAnimationPacket(bool correctChoice)
        {
            CorrectChoice = correctChoice;
        }
    }
}