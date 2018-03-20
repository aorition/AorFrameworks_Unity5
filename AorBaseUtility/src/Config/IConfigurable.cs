namespace AorBaseUtility.Config
{
    public interface IConfigurable<T> where T : Config
    {
        T Config { get; }

        void SetConfig(T config);
    }
}
