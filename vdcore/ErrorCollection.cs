using System;
using System.Text;
using System.Collections;

namespace Vendare.Error
{
	/// <summary>
	/// Provides a Collection of Errors
	/// </summary>
	
	[Serializable]
	public class ErrorCollection : CollectionBase
	{
		/// <summary> Prefix which appears before each message upon calling getList()*</summary>
		private String prefix = "<li class=error>";
		/// <summary> Suffix which appears after each message upon calling getList()</summary>
		private String suffix = "</li>";
		/// <summary> Prefix which appears before entire list upon calling getList()*</summary>
		private String listPrefix = "";
		/// <summary> Suffix which appears after entire list upon calling getList()</summary>
		private String listSuffix = "";

		public ErrorCollection()
		{
		}

		public void Add(SingleError error) 
		{
			if(!List.Contains(error))
				List.Add(error);
		}

		public SingleError Item(int index) 
		{
			return (SingleError)List[index];
		}

		public bool IsEmpty
		{
			get{
				return List.Count == 0;
				}
		}

		/// <summary> Prefix which appears before each message upon calling ToString()*</summary>
		public String Prefix 
		{
			get 
			{
				return prefix;
			}
			set 
			{
				prefix = value;
			}
		}

		/// <summary> Suffix which appears after each message upon calling ToString()</summary>
		public String Suffix 
		{
			get 
			{
				return suffix;
			}
			set 
			{
				suffix = value;
			}
		}

		/// <summary> Prefix which appears before entire List upon calling ToString()*</summary>
		public String ListPrefix 
		{
			get 
			{
				return listPrefix;
			}
			set 
			{
				listPrefix = value;
			}
		}

		/// <summary> Suffix which appears after entire List upon calling ToString()</summary>
		public String ListSuffix 
		{
			get 
			{
				return listSuffix;
			}
			set 
			{
				listSuffix = value;
			}
		}

		public override String ToString() 
		{
			StringBuilder list = new StringBuilder(Count * 100);
			list.Append(listPrefix);
			foreach(SingleError error in List) 
			{
				list.Append(prefix);
				list.Append(error.Message);
				list.Append(suffix);
			}
			list.Append(listSuffix);
			return list.ToString();
		}
	}
}
