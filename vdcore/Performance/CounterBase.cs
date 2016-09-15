using System;
using System.Collections;
using System.Configuration;
using System.Diagnostics;
using System.Threading;

namespace Vendare.Utils.Performance
{
	/// <summary>
	/// Base class for counters in the RealTimeScrubbing Category
	/// </summary>
	/// <remarks>Note that in Perfmon the category name appears in the Category
	/// dropdown and typically relates to a performance area, sometimes
	/// called a "managed object". We treat RTS as a single managed object since
	/// it is a major EMM component</remarks>
	public abstract class CounterBase
	{
		#region Data Members
		/// <summary>
		/// Internal Performance Counter Instance
		/// </summary>
		protected internal PerformanceCounter theBaseCounter;
		/// <summary>
		/// Internal Base Performance Counter Instance
		/// </summary>
		protected internal PerformanceCounter theCounter;
		public string InstanceName
		{
			get
			{
				return theCounter.InstanceName;
			}
		}
		protected string categoryName = ConfigurationSettings.AppSettings.Get("PerformanceCounterCategory");
		protected string categoryHelp = ConfigurationSettings.AppSettings.Get("PerformanceCounterCategoryHelp");
		/// <summary>
		/// ArrayList of CounterSample objects
		/// </summary>
		protected object sampleList;
		protected ArrayList oldSampleList;
		#endregion Data Members

		#region Constructor(s)
		/// <summary>
		/// Partially initialize counter
		/// </summary>
		/// <param name="estimatedSampleSize">The estimated number of samples per sampling interval used for initial allocation of sample list.</param>
		/// <param name="samplingInterval">Frequency at which asynchronous result calculation will be driven. If null then it will not be driven</param>
		public CounterBase()
		{
			// defaults for missing config file settings
			if (categoryName == null)
				categoryName = "FirstLook";
			if (categoryHelp == null)
				categoryHelp = "There is no help";

			PerformanceCounterCategory pcc = null;
			//PerformanceCounterCategory.Delete(categoryName);
			if (! PerformanceCounterCategory.Exists(categoryName))
			{
				CounterCreationDataCollection CCDC = new CounterCreationDataCollection(); 

				// Simple Counter (for things like numer cached offers, cache memory used)
				CounterCreationData simple = new CounterCreationData();
				simple.CounterType = PerformanceCounterType.NumberOfItems32;
				simple.CounterHelp = "Scalar or quantity of something";
				simple.CounterName = "SimpleCounter";
				CCDC.Add(simple);

				// AverageCount64 Counter (for things like average number available threads)
				CounterCreationData averageCount64 = new CounterCreationData();
				averageCount64.CounterType = PerformanceCounterType.AverageCount64;
				averageCount64.CounterName = "Average";
				averageCount64.CounterHelp = "Average";
				CCDC.Add(averageCount64);
            
				// AverageBase Counter.
				CounterCreationData averageCount64Base = new CounterCreationData();
				averageCount64Base.CounterType = PerformanceCounterType.AverageBase;
				averageCount64Base.CounterName = "AverageCountBase";
				CCDC.Add(averageCount64Base);            
			
				// Raw Fraction Counter (for things line lead % success, reject, exception, timeout)
				CounterCreationData rf = new CounterCreationData();
				rf.CounterType = PerformanceCounterType.RawFraction;
				rf.CounterName = "Percent";
				CCDC.Add(rf);
        
				// Raw Base Counter
				CounterCreationData rfBase = new CounterCreationData();
				rfBase.CounterType = PerformanceCounterType.RawBase;
				rfBase.CounterName = "PercentBase";
				CCDC.Add(rfBase);

				// Average Timer32 Counter (for things like average validator or validation response)
				CounterCreationData averageTimer32 = new CounterCreationData();
				averageTimer32.CounterType = PerformanceCounterType.AverageTimer32;
				averageTimer32.CounterName = "AverageResponse";
				CCDC.Add(averageTimer32);
            
				// AverageBase Counter.
				averageCount64Base = new CounterCreationData();
				averageCount64Base.CounterType = PerformanceCounterType.AverageBase;
				averageCount64Base.CounterName = "AverageTimerBase";
				CCDC.Add(averageCount64Base);      

				// Rate of Counts/Second32 Counter. (for things like leads/sec)
				CounterCreationData rateOfCounts32 = new CounterCreationData();
				rateOfCounts32.CounterType = PerformanceCounterType.RateOfCountsPerSecond32;
				rateOfCounts32.CounterName = "Rate";
				CCDC.Add(rateOfCounts32);
            
				// Create the category.
				pcc = PerformanceCounterCategory.Create(categoryName, /*categoryHelp*/ "Generic Performance Counters", CCDC);
				PerformanceCounter.CloseSharedResources();
			}
		}
		
		#endregion Constructor(s)

	}
}

