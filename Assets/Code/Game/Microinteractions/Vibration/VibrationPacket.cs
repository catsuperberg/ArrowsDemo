namespace Game.Microinteracions
{
    public class VibrationPacket : IMicrointerationPacket
    {            
        public readonly VibrationEffect EffectToTrigger;
        
        public VibrationPacket(VibrationEffect effectToTrigger)
        {
            EffectToTrigger = effectToTrigger;
        }
    }
}