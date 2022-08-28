namespace Game.Gameplay.Meta
{
    public interface IUpdatableUserContext
    {
        public UserContext Context {get;}
        public void UpdateValue(string fieldName, string value);
    }
}