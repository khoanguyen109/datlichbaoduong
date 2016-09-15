using System;

namespace yellowx.Framework.Transformers
{
    public interface ITransformer
    {
        Type InputType { get; }
        Type OutputType { get; }
    }

    public interface ITransformer<in TInput, out TOutput> : ITransformer
    {
        TOutput Transform(TInput input);
    }
    public class Transformer<TInput, TOutput> : ITransformer<TInput, TOutput>
    {
        public Type InputType
        {
            get
            {
                return typeof(TInput);
            }
        }

        public Type OutputType
        {
            get
            {
                return typeof(TOutput);
            }
        }

        public virtual TOutput Transform(TInput input)
        {
            return default(TOutput);
        }

        public override bool Equals(object obj)
        {
            var transformer = (ITransformer)obj;
            return transformer.InputType == InputType && transformer.OutputType == OutputType;
        }

        public override int GetHashCode()
        {
            return (InputType.FullName + "_" + OutputType.FullName).GetHashCode();
        }
    }
}
