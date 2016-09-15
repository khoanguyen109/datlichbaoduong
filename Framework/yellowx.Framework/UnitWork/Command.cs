namespace yellowx.Framework.UnitWork
{
    public interface ICommand<in TRequest, out TResponse>
        where TResponse : CommandResponse
    {
        TResponse Invoke(TRequest request);
    }

    public abstract class Command<TRequest, TResponse> : Object, ICommand<TRequest, TResponse>
        where TResponse : CommandResponse
    {
        public virtual TResponse Invoke(TRequest request)
        {
            return Default<TResponse>();
        }
    }

    public abstract class CommandResponse
    {
        public System.Exception Exception { get; set; }
    }

    public abstract class CommandResponse<TResponseStatus> : CommandResponse
    {
        public TResponseStatus Status { get; set; }
    }
}
