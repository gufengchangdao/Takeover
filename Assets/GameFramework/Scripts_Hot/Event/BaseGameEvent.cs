namespace GameFramework.Hot
{
    public abstract class GameEvent : IRecyclable
    {
        //中断机制，在回调里修改该字段为true可中断回调执行
        public bool handled = false;

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
