namespace Game.Microinteracions
{
    public class SFXPacket : IMicrointerationPacket
    {            
        public readonly SoundEffect EffectToTrigger;
        
        public SFXPacket(SoundEffect effectToTrigger)
        {
            EffectToTrigger = effectToTrigger;
        }
    }
}