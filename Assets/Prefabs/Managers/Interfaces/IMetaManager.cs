using Sequence;
using System.Numerics;

namespace GameMeta
{
    public interface IMetaManager
    {
        public SequenceContext GetContext();
        public BigInteger GetNextTargetScore();
        public OperationPairsSequence GenerateSequence();
    }    
}