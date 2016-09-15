using System.Collections.Specialized;

namespace yellowx.Framework.IO
{
    public static class Directory
    {
        /// <summary>
        /// Ensures that we have a specified directory. Otherwise, creating new directory.
        /// </summary>
        public static bool Ensure(string directory)
        {
            try
            {
                if (!System.IO.Directory.Exists(directory))
                    System.IO.Directory.CreateDirectory(directory);
                return true;
            }
            catch (System.Exception ex)
            {
                var details = new StringDictionary();
                details.Add("Method", "Ensure");
                details.Add("Directory", directory);
                Configuration.LogWriter.Log("Unable to check existing or creating directory " + directory, ex, details);
                return false;
            }
        }
    }
}
