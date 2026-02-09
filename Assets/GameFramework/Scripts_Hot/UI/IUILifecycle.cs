namespace GameFramework.Hot
{
    public interface IUILifecycle
    {
        void OnInit(object userData);

        void OnRecycle();

        void OnUIDestroy();
    }
}