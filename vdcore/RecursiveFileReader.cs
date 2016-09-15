using System;
using System.IO;
using System.Text.RegularExpressions;
using Vendare.Error;
using Vendare.Utils;
using System.Threading;

/// <summary>
/// utility for reading a file.  includes ability to queue up to a line, trim a read line, 
/// and if error while trying to open a file, attempt to read again for maxAttempts times.
/// </summary>
/// 
namespace Vendare.Utils
{
	public class RecursiveFileReader
	{
		// the name of the file being read
		private String filename = null;
		private StreamReader reader = null;
		// current line number being read
		private int lineNo = 0;
		// total number of lines in this file
		private int linesInFile = 0;

		// if error while opening file, this is the number of times tried to open the file
		private int attempts = 0;
		// the default max number of time to try to read the file until throwing an exception
		private int maxAttempts = 5;

		// flag to see if this object has been initialized or not
		private bool isInitialized = false;
		// flag for whether to trim the read line or not
		private bool isTrim = false;
		// used to trimming, sometimes the delimiter is a tab or space like char which should not be trimmed
		private String delimiter = null;

		// initializes the FileReader with the name of the file (contained within the unprocessed dir)
		// as in Env.cs and the max number of times to try to open the file.
		public RecursiveFileReader(String filename, int maxAttempts)
		{
			this.maxAttempts = maxAttempts;

			// opens the file
			ResetMark(filename);
			linesInFile = 0;
		}

		// initializes the FileReader with the name of the file (contained within the unprocessed dir)
		// using default maxAttempts
		public RecursiveFileReader(String filename)
		{
			ResetMark(filename);
			linesInFile = 0;
		}

		// initializes the FileReader with the name of the file (contained within the unprocessed dir)
		// using default maxAttempts
		private void ResetMark(String filename)
		{
			isInitialized = false;

			this.filename = filename;
			Close();

			try
			{
				reader = new StreamReader(filename, System.Text.Encoding.ASCII, false, 256);
				attempts = 0;
			}
			catch(Exception e)
			{
				if(attempts < maxAttempts)
				{					
					attempts++;
					Thread.Sleep(60000);
					ResetMark(filename);
				}
				else
				{
					attempts = 0;	
					throw new ApplicationException("Error Reading File:"+filename, e);
				}
			}

			isInitialized = true;
			lineNo = 0;
		}

		// true will trim when returning the string from ReadLine()
		public void SetTrim(bool isTrim)
		{
			this.isTrim = isTrim;
		}

		// says what the delimiter to be sure it's not trimmed when ReadLine()
		public void SetDelimiter(String delimiter)
		{
			this.delimiter = delimiter;
		}

		// read a line from the file.  when returns null, the line has been read
		public String ReadLine()
		{
			if(!isInitialized)
				throw new Exception("Cannot read before initialized");

			// peekaboo to make sure the file isn't already done
			if(reader.Peek() > -1)
			{
				++lineNo;
				// if isTrim is true, this will return a trimmed line
				return Trim(reader.ReadLine());
			}
			else
				return null;
		}

		public int Peek()
		{
			return reader.Peek();
		}

		// read from a line number
		public String ReadLine(int newLineNo)
		{
			QueueToLine(newLineNo);
			return ReadLine();
		}

		// queue this reader to start reading from this line.
		public void QueueToLine(int line)
		{
			if(!isInitialized)
				throw new Exception("Cannot read before initialized", null);

			if(lineNo >= line)
				ResetMark(filename);

			while((reader.Peek() > -1) && ((lineNo + 1) < line))
			{
				reader.ReadLine();
				++lineNo;
			}
		}

		public void Close()
		{
			if(reader != null)
				reader.Close();
		}

		public String GetRelativeFileName()
		{
			return (new FileInfo(filename)).Name;
		}

		public String GetAbsoluteFileName()
		{
			return filename;
		}

		public int GetLineNo()
		{
			return lineNo;
		}

		public int GetLinesInFile()
		{
			return linesInFile;
		}

		public void SetLinesInFile(int linesInFile)
		{
			this.linesInFile = linesInFile;
		}

		private String Trim(String text)
		{
			if(!isTrim)
				return text;

			else if((delimiter == null) || !text.StartsWith(delimiter))
				return text.Trim();
			
			else	
				return text.TrimEnd();
		}
	}
}
