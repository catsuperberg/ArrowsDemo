using System.Numerics;

namespace Game.Gameplay.Realtime.OperationSequence.Operation
{
    public interface IMathContainer
    {
        public BigInteger ApplyOperation(BigInteger initialValue);
    }
}
