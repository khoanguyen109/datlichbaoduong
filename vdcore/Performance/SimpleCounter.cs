using System;
using System.Collections;
using System.Diagnostics;
using System.Threading;

namespace Vendare.Utils.Performance
{
	/// <summary>
	/// Provide SimpleCounter counter functionality
	/// </summary>
	public class SimpleCounter : CounterBase
	{
		#region Data Members
		protected internal long nextSampleEmitValue = 0;
		#endregion Data Members

		#region Public API
		/// <summary>
		/// Instantiate and instance of a counter and assign it name and instance name
		/// </summary>
		/// <param name="instanceName">The instance name of this counter. If null is passed
		/// it will be set to the empty string</param>
		/// <param name="samplingInterval">Whether the counter will be reset at hourly interval, daily interval, or not at all</param>
		/// <remarks>Instance names are not necessary but we will use them when it comes to 
		/// validators since the same types of info are needed for each validator instance</remarks>
		public SimpleCounter(string instanceName) : base()
		{
			theCounter = new PerformanceCounter(categoryName, "SimpleCounter", instanceName, false);
			theCounter.RawValue = 0;

		}

		/// <summary>
		/// Increments the counter
		/// </summary>
		/// <param name="SimpleCounter">The simple counter being incremented</param>
		/// <returns>the performance counter that was incremented</returns>
		public static SimpleCounter operator ++(SimpleCounter simple)
		{
			simple.theCounter.Increment();
			return simple;
		}
		/// <summary>
		/// Increments the counter
		/// </summary>
		/// <param name="simple">The simple counter being incremented</param>
		/// <returns>the performance counter that was incremented</returns>
		public static SimpleCounter operator --(SimpleCounter simple)
		{
			if (simple.theCounter.RawValue > 0)
				simple.theCounter.Decrement();
			return simple;
		}
		/// <summary>
		/// Subtract amount from counter
		/// </summary>
		/// <param name="simple">SimpleCounter instance</param>
		/// <param name="amount">the amount being subtracted from the counter value</param>
		/// <returns>the performance counter from which the value was subtracted</returns>
		public static SimpleCounter operator -(SimpleCounter simple, long amount)
		{
			simple.theCounter.IncrementBy(- amount);
			return simple;
		}
		/// <summary>
		/// Add amount to counter
		/// </summary>
		/// <param name="simple">SimpleCounter instance</param>
		/// <param name="amount">the amount being added to the counter value</param>
		/// <returns>the performance counter to which the value was added</returns>
		public static SimpleCounter operator +(SimpleCounter simple, long amount)
		{
			simple.theCounter.IncrementBy(amount);
			return simple;
		}
		/// <summary>
		/// Resets the counter
		/// </summary>
		/// <returns></returns>
		public long Reset(SimpleCounter simple)
		{
			return simple.theCounter.IncrementBy(- simple.theCounter.RawValue);
		}
		#endregion Public API

	}
}
