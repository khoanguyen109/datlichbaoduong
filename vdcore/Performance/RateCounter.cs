using System;
using System.Collections;
using System.Diagnostics;

namespace Vendare.Utils.Performance
{
	/// <summary>
	/// Summary description for RateCounter.
	/// </summary>
	public class RateCounter : CounterBase
	{
		#region Data Members
		#endregion Data Members

		#region Public API
		/// <summary>
		/// Instantiate a Performance counter wrapper that can be used for reporting rates line leads/sec.
		/// </summary>
		/// <param name="instanceName">The name of the performance counter instance of the performance counter type being instantiated</param>
		/// <param name="samplingInterval">The interval during which samples are collected for use in rate computation</param>
		/// <param name="estimatedSampleSize">The estimated number of samples that will be collected during a sampling interval. This is used for initial allocation
		/// of the sample list after which the size is self-adjusting at the high water mark.</param>
		public RateCounter(string instanceName) : base()
		{
			theCounter = new PerformanceCounter( categoryName, "Rate", instanceName, false);
			theCounter.RawValue = 0;
		}

		/// <summary>
		/// Add the sample value that will be averaged over the sampling interval
		/// </summary>
		/// <param name="milliseconds">Milliseconds for a single sample of response time being averaged</param>
		public void AddSampleValue(long val)
		{
			theCounter.IncrementBy(val);
		}
		#endregion Public API

	}
}
