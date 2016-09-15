namespace yellowx.Framework.UnitWork
{
    public interface IQuery<out TResponse>
    {
        TResponse Invoke();
    }

    public interface IQuery<in TRequest, out TResponse>
    {
        TResponse Invoke(TRequest request);
    }
}
