namespace GameFramework.Hot
{
    public abstract class GameEvent : IRecyclable
    {
        public abstract void OnRecycle();
    }

    public abstract class BaseGameEvent<T> : GameEvent where T : class, IRecyclable, new()
    {
        public static T Create()
        {
            return TypeReferencePool.GetOrNew<T>();
        }
    }
}
