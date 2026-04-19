namespace Takeover
{
    public interface IDataSaveLoad
    {
        void OnSave(bool isQuit);
        void OnLoad();
    }
}