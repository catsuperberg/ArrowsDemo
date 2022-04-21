using System.Threading.Tasks;

namespace Game.Gameplay.Realtime
{
    public interface IRuntimeFactory
    {
        public Task<RunthroughContext> GetRunthroughContext();
    }
}