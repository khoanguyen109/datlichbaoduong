using System;
using System.Collections;
using System.Diagnostics;

namespace Vendare.Utils.Performance
{
	/// <summary>
	/// Maintain an average response time in a performance counter computing the average from samples over a specified
	/// sampling interval. Counter instance name and sampling interval are input for creation of this class
	/// </summary>
	public class AverageResponseTimeCounter : CounterBase
	{
		#region Data Members
		#endregion Data Members

		#region Public API
		/// <summary>
		/// Instantiate PerformanceCounter wrapper instance for average response time
		/// </summary>
		/// <param name="instanceName">The RTS object for which the average response time will be maintained</param>
		/// <param name="samplingInterval">The sampling interval used for computing the average</param>
		/// <param name="estimatedSampleSize">Estimated sample size for initial allocation of sample list</param>
		public AverageResponseTimeCounter(string instanceName) : base()
		{
            theCounter = new PerformanceCounter(categoryName, "AverageResponse", instanceName, false);
			theCounter.RawValue = 0;

            theBaseCounter = new PerformanceCounter(categoryName, "AverageTimerBase", instanceName, false);
			theBaseCounter.RawValue = 0;
		}

		/// <summary>
		/// Add milliseconds for a single response time sample that will be used for computing
		/// the average responsetime over the sampling interval.
		/// </summary>
		/// <param name="milliseconds">Milliseconds for a single sample of response time being averaged</param>
		public void AddSampleValue(long milliseconds)
		{
			theCounter.IncrementBy(milliseconds);
		}

		/// <summary>
		/// Increment the number of samples
		/// </summary>
		public void IncrementSampleCount()
		{
			theBaseCounter.Increment();
		}
		#endregion Public API

	}
}
