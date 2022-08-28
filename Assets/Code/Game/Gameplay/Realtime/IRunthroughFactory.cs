using System.Threading.Tasks;

namespace Game.Gameplay.Realtime
{
    public interface IRunthroughFactory
    {
        public Task<RunthroughContext> GetRunthroughContextHiden();
    }
}