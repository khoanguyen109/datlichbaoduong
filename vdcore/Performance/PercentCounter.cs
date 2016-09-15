using System;
using System.Collections;
using System.Diagnostics;

namespace Vendare.Utils.Performance
{
	/// <summary>
	/// Maintain a percentage in a performance counter computing the average from samples over a specified
	/// sampling interval. Counter instance name and sampling interval are input for creation of this class
	/// </summary>
	public class PercentCounter : CounterBase
	{
		#region Data Members
		#endregion Data Members

		#region Public API
		/// <summary>
		/// Instantiate a performance counter wrapper can be used for maintaining percentages 
		/// </summary>
		/// <param name="instanceName">Name given to this instance of the performance counter of the specific type being instantiated</param>
		/// <param name="samplingInterval">Interval over which samples used to compute percentages are collected</param>
		/// <param name="estimatedSampleSize">Estimated size of sample collected of sampling interval for initial allocation of sample list</param>
		public PercentCounter(string instanceName) : base()
		{
			theCounter = new PerformanceCounter(categoryName, "Percent", instanceName, false);
			theCounter.RawValue = 0;

            theBaseCounter = new PerformanceCounter(categoryName, "PercentBase", instanceName, false);
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
		}
		#endregion Public API

	}
}
