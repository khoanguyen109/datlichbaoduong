using System;
using System.Collections;
using System.Diagnostics;

namespace Vendare.Utils.Performance
{
	/// <summary>
	/// Maintain an average in a performance counter computing the average from samples over a specified
	/// sampling interval. Counter instance name and sampling interval are input for creation of this class
	/// </summary>
	public class AverageCounter : CounterBase
	{
		#region Data Members
		#endregion Data Members

		#region Public API
		public AverageCounter(string instanceName) : base()
		{
            theCounter = new PerformanceCounter(categoryName, "Average", instanceName, false);
            theBaseCounter = new PerformanceCounter(categoryName, "AverageCountBase", instanceName, false);

			theCounter.RawValue = 0;
			theBaseCounter.RawValue = 0;
		}

		/// <summary>
		/// Add the sample value that will be averaged over the sampling interval
		/// </summary>
		/// <param name="milliseconds">Milliseconds for a single sample of response time being averaged</param>
		public void AddSampleValue(long val)
		{
			theCounter.IncrementBy(val);
		}

		/// <summary>
		/// Increment the number of samples
		/// </summary>
		public void IncrementSampleCount()
		{
			theBaseCounter.Increment();
			theCounter.NextSample();
		}

		/// <summary>
		/// Increment the number of samples
		/// </summary>
		public void IncrementSampleCount(long amount)
		{
			theBaseCounter.IncrementBy(amount);
			theCounter.NextSample();
		}

		#endregion Public API

	}
}
