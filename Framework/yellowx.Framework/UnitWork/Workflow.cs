namespace yellowx.Framework.UnitWork
{
    public interface IWorkFlow<in TInput, out TOutput>
    {
        TOutput Flow(TInput output);
    }
    public class Workflow<TInput, TOutput> : Object, IWorkFlow<TInput, TOutput>
    {
        public virtual TOutput Flow(TInput output)
        {
            return Default<TOutput>();
        }
    }
}
