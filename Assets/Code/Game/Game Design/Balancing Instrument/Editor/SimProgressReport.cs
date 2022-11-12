using System.Threading;

namespace Game.GameDesign
{
    public class SimProgressReport
    {
        public int FinishedCount => _simulationsDone;
        int _simulationsDone;
        public readonly int CountToFinish;

        public void IncrementDone()
        {
            Interlocked.Increment(ref _simulationsDone);
        }
        
        public SimProgressReport(int finalSimultationCount)
        {
            CountToFinish = finalSimultationCount;
        }
        
        public float Part()
            => (float)FinishedCount/(float)CountToFinish;
    }
}