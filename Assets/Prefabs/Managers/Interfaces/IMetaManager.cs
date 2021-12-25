using State;
using Sequence;
using System.Numerics;

namespace GameMeta
{
    public interface IMetaManager : IStateReportableProcess
    {
        public SequenceContext GetContext();
        public BigInteger GetNextTargetScore();
        public OperationPairsSequence GenerateSequence(BigInteger targetScore, int spread);
    }    
}