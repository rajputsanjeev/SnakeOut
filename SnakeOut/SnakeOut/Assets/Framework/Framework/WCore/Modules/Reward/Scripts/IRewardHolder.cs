namespace Framework.Core
{
    public interface IRewardHolder
    {
        bool IsDirty { get; }
        void MarkAsDirty();
    }
}