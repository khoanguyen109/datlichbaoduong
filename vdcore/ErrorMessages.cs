using System;
using System.Text;

namespace Vendare.Error
{
	/// <summary>
	///This class provides a means of building collections of custom errors.
	///The class is instantiated with the max number of errors in the collection
	///Call setMesage for each possible error with its message
	///call setError to indicate that an error has occured
	/// </summary>
	public class ErrorMessages
	{
		/// <summary> number of messages class can hold</summary>
		private int capacity = 0;
		/// <summary> holds readable messages</summary>
		private String[] messages = null;
		/// <summary> toggle switches for individual messages</summary>
		private bool[] switches;
		/// <summary> Prefix which appears before each message upon calling getList()*</summary>
		private String errorPrefix = "<li class=error>";
		/// <summary> Suffix which appears after each message upon calling getList()</summary>
		private String errorSuffix = "</li>";
		/// <summary> Prefix which appears before list of messages upon calling getList()</summary>
		private String errorsPrefix = "<ul class=errorList>";
		/// <summary> Suffix which appears after list of messages upon calling getList()</summary>
		private String errorsSuffix = "</ul>";
		/// <summary> indicates whether or not there are any errors</summary>
		private bool empty = true;

		/// <summary>
		/// Creates an error collection with the specified capacity
		/// </summary>
		/// <param name="capacity">Total number of possible errors</param>
		public ErrorMessages(int capacity) 
		{
			this.capacity = capacity;
			messages = new String[capacity];
			switches = new bool[capacity];
		}

		/// <summary>
		/// Sets the text message for an error
		/// </summary>
		/// <param name="index">specifies which error to set</param>
		/// <param name="message">the textual message to set</param>
		public void SetMessage(System.ValueType index, String message) 
		{
			if((int)index >= capacity)
			{
				capacity += 5;
				String[] newMessages = new String[capacity];
				bool[] newSwitches = new bool[capacity];
				messages.CopyTo(newMessages,0);
				switches.CopyTo(newSwitches,0);
				messages = newMessages;
				switches = newSwitches;
			}
			messages[(int)index] = message;
		}

		/// <summary>
		/// flag that an error has occured
		/// </summary>
		/// <param name="index">specifies which error has occured</param>
		public void SetError(System.ValueType index) 
		{
			if((int)index < capacity) 
			{
				switches[(int)index] = true;
				empty = false;
			}        
		}

		/// <summary>
		/// clear all errors in the collection
		/// </summary>
		public void ClearErrors() 
		{
			for(int i=0; i < capacity; i++)
				switches[i] = false;
			empty = true;
		}
    
		/// <summary>
		/// provide formatting for the prefix of the message. 
		/// This string will appear before each message upon calling GetList()
		/// </summary>
		/// <param name="prefix">String to assign to prefix</param>
		public void SetPrefix(String prefix) 
		{
			errorPrefix = prefix;
		}

		/// <summary>
		/// provide formatting for the suffix of the message. 
		/// This string will appear after each message upon calling GetList()
		/// </summary>
		/// <param name="prefix">String to assign to suffix</param>
		public void SetSuffix(String suffix) 
		{
			errorSuffix = suffix;
		}

		/// <summary>
		/// provide formatting for the prefix of the entire list of errors. 
		/// This string will appear before the entire list upon calling GetList()
		/// </summary>
		/// <param name="prefix">String to assign to prefix</param>
		public void SetListPrefix(String prefix) 
		{
			errorsPrefix = prefix;
		}

		/// <summary>
		/// provide formatting for the suffix of the entire list of massages. 
		/// This string will appear after the entire list upon calling GetList()
		/// </summary>
		/// <param name="prefix">String to assign to suffix</param>
		public void SetListSuffix(String suffix) 
		{
			errorsSuffix = suffix;
		}

		/// <summary>
		/// Returns true if any errors have occured
		/// </summary>
		/// <returns>Returns true if any errors have occured</returns>
		public bool IsEmpty() 
		{
			return empty;
		}
    
		/// <summary>
		/// return a list of all errors preceeded by prefix and proceeded by suffix
		/// </summary>
		/// <returns>return a list of all errors</returns>
		public String GetList() 
		{
			StringBuilder list = new StringBuilder(200);
			list.Append(errorsPrefix);
			for(int i=0;i<capacity;i++) 
			{
				if(switches[i]) 
				{
					list.Append(errorPrefix);
					list.Append(messages[i]);
					list.Append(errorSuffix);
				}
			}
        
			list.Append(errorsSuffix);
			return list.ToString();
		}

	}
}
