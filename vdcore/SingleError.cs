using System;

namespace Vendare.Error
{
	/// <summary>
	/// Holds Basic Error Info for a single error.
	/// </summary>
	public class SingleError
	{
		private long code;
		private String shortName;
		private String message;

		public SingleError(long code, String shortName, String message)
		{
			this.code = code;
			this.shortName = shortName;
			this.message = message;
		}

		public long Code 
		{
			get 
			{
				return code;
			}
		}

		public String ShortName 
		{
			get 
			{
				return shortName;
			}
		}

		public String Message 
		{
			get 
			{
				return message;
			}
			set 
			{
				message = value;
			}
		}
	}
}
