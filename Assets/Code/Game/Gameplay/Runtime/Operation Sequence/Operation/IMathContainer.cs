using System.Numerics;

namespace Game.Gameplay.Runtime.OperationSequence.Operation
{
    public interface IMathContainer
    {
        public BigInteger ApplyOperation(BigInteger initialValue);
    }
}
