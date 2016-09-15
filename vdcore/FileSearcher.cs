using System;
using System.IO;
using System.Text.RegularExpressions;
using System.Configuration;

/// <summary>
/// searches a directory for the oldest file.  the file is specified by directory.
/// </summary>
/// 
namespace Vendare.Utils
{
	public class FileSearcher
	{
		private static FileSearcher instance = null;

		// the directory to search
		private String directory = null;

		public static FileSearcher GetInstance()
		{
			lock(typeof(FileSearcher)) 
			{
				if(instance == null)
					instance = new FileSearcher();

				return instance;
			}
		}

		// sets the directory to search
		public void Init(String directory)
		{
			this.directory = directory;
		}
		
		private FileSearcher()
		{			
		}

		// returns the oldest file in the directory or null if there is no file
		public String Search()
		{
			FileInfo file = null;
			try
			{
				// util to get the oldest dir
				file = GetOldestFile(directory);
				if(file != null)
				{
					return file.FullName;
				}
			}
			catch(Exception e)
			{
				new ApplicationException("Error searching for file in directory:"+directory, e);
			}

			return null;
		}

		// util to get the oldest dir
		private FileInfo GetOldestFile(String path)  
		{
			DirectoryInfo directoryInfo = new DirectoryInfo(path);

			if(!directoryInfo.Exists)
				return null;

			FileInfo oldestFile = null;

			FileInfo[] fileInfo = directoryInfo.GetFiles();
			foreach(FileInfo fi in fileInfo) 
			{
				if(oldestFile == null)
					oldestFile = fi;

				if(fi.CreationTime < oldestFile.CreationTime)   
					oldestFile = fi;
			}
			return oldestFile;
		}		
	}
}
