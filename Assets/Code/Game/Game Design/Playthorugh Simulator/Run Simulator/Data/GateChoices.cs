namespace Game.GameDesign
{
    public class GateChoices
    {
        public readonly uint RightCount;
        public readonly uint WrongCount;

        public GateChoices(uint rightCount, uint wrongCount)
        {
            RightCount = rightCount;
            WrongCount = wrongCount;
        }
    }
}