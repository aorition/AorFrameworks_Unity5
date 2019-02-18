
namespace Framework.AnimLinkage
{
    /// <summary>
    /// 定义此对象可解算
    /// </summary>
    public interface ISimulateAble
    {
        /// <summary>
        /// 可解算接口方法
        /// </summary>
        /// <param name="time">时间(秒)</param>
        void Process(float time);
    }
}
