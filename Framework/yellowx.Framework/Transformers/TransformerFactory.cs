using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using yellowx.Framework.Extensions;

namespace yellowx.Framework.Transformers
{
    public class TransformerFactory
    {
        private static readonly List<ITransformer> transformers;

        static TransformerFactory()
        {
            transformers = new List<ITransformer>();
        }

        public static void Add<TInput, TOutput>(ITransformer<TInput, TOutput> transformer)
        {
            if (!transformers.Contains(transformer))
                transformers.Add(transformer);
        }
        public static ITransformer<TInput, TOutput> GetTransformer<TInput, TOutput>()
        {
            var transformer = transformers.FirstOrDefault(x => x.InputType == typeof(TInput) && x.OutputType == typeof(TOutput));
            return transformer as ITransformer<TInput, TOutput>;
        }

        /// <summary>
        /// Scan all dll that start with array of namespaces to get out all transformer we need.
        /// </summary>
        /// <param name="nsPatterns"></param>
        public static void ScanTransformers(string path, string[] nsPatterns)
        {
            if (Directory.Exists(path))
            {
                foreach (var ns in nsPatterns)
                {
                    var files = Directory.GetFiles(path, ns);
                    files.ForEach(f =>
                    {
                        var assembly = Assembly.LoadFrom(f);
                        var transformerTypes = assembly.GetExportedTypes().Where(t => typeof(ITransformer).IsAssignableFrom(t));
                        transformerTypes.ForEach(t =>
                        {
                            var transformer = Activator.CreateInstance(t) as ITransformer;
                            if (transformer != null)
                                transformers.Add(transformer);
                        });
                    });
                }
            }
        }
    }
}
