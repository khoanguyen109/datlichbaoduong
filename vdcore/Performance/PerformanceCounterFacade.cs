using System;
using System.Collections;
using System.Collections.Specialized;
using Vendare.Error;

namespace Vendare.Utils.Performance
{
	/// <summary>
	/// Summary description for PerformanceCounterFacade.
	/// </summary>
	public class PerformanceCounterFacade
	{
		#region Data Members
		public static Hashtable Counters = null;
		#endregion Data Members
		#region Constructors
		static PerformanceCounterFacade()
		{	
			// some dynamically defined counters need a threadsafe container
			Counters = Hashtable.Synchronized(new Hashtable(20));
		}
		private PerformanceCounterFacade() {}
		#endregion Constructors

		/// <summary>
		/// Synchonize creation and destruction of performance counter references
		/// </summary>
		/// <param name="pc">reference to perfcounter variable from caller</param>
		/// <param name="counterType">underlying perf counter type for which new instance might be created</param>
		/// <param name="instancename">instance name to be assigned for new counter instance</param>
		/// <param name="si">sampling interval for new instance</param>
		/// <param name="estsamplesize">estimated initial sample size</param>
		/// <returns>true if perf counter ref is non null and monitoring should be done</returns>
		/// <remarks>This was done in order to have monitoring respond to config setting and dynamically allocate
		/// and deallocate performance counters according to the setting. It also keeps a lot of 
		/// potentially messy code in one place</remarks>
		public static CounterBase SetCounter(Type senderType, ref CounterBase pc, string counterType, string instancename, bool isMonitoringOn)
		{
			// if they should continue not monitoring then exit
			if (! isMonitoringOn && pc == null)
				return null;
			// if they should continue monitoring then exit
			if (isMonitoringOn && pc != null)
				return pc;

			// either they are monitoring and should stop or they aren't and they should start

			CounterBase newpc = null;
			

			if (isMonitoringOn)
			{
				if (pc == null)
				{
					switch(counterType)
					{
						case "SimpleCounter" :
							newpc = new SimpleCounter(instancename); 
							break;
						case "Average" :
							newpc = new AverageCounter(instancename);
							break;
						case "Percent":
							newpc = new PercentCounter(instancename);
							break;
						case "AverageResponse" :
							newpc = new AverageResponseTimeCounter(instancename);
							break;
						case "Rate" :
							newpc = new RateCounter(instancename);
							break;
						default :
							break;
					}
				}
			}

			// since perf counters are static shared objects we must lock
			lock(senderType)
			{
				// change their monitoring state from on to off or from off to on
				if (newpc == null)
					pc.theCounter.RemoveInstance();
				pc = newpc;
			}

			return newpc;
		}
		/// <summary>
		/// Monitor an average value with a counter having the specified instance name
		/// </summary>
		/// <param name="counterInstanceName">Performance counter instance name</param>
		/// <param name="count">the value for a single sample being averaged</param>
		/// <param name="lockType">some type used internally for locking</param>
		/// <param name="isMonitoringActive">caller can forward external monitoring control</param>
		public static void MonitorCurrentCount(string counterInstanceName, long count, System.Type lockType, bool isMonitoringActive)
		{
			try
			{
				// access counter in hashlist if it is there, else null
				CounterBase counter = (CounterBase) PerformanceCounterFacade.Counters[counterInstanceName];
				// allocate counter if needed
				SimpleCounter countCounter = (SimpleCounter) PerformanceCounterFacade.SetCounter(lockType, ref counter, "SimpleCounter", counterInstanceName, isMonitoringActive);

				if (countCounter != null)
				{
					countCounter.theCounter.RawValue = count;
				}
				// persist static, threadsafe counter (null if perf monitoring off) for reuse by validator instances
				PerformanceCounterFacade.Counters[counterInstanceName] = countCounter;
			}
			catch(Exception ex)
			{
				NameValueCollection nvc = new NameValueCollection(2);
				nvc.Add("Method", "MonitorCurrentCount");
				nvc.Add("Probable cause", "PerfCounters not registered or wrong category in config file");
				new LoggableException(ex, nvc);
			}
		}
		/// <summary>
		/// Monitor an average value with a counter having the specified instance name
		/// </summary>
		/// <param name="counterInstanceName">Performance counter instance name</param>
		/// <param name="count">the value for a single sample being averaged</param>
		/// <param name="lockType">some type used internally for locking</param>
		/// <param name="isMonitoringActive">caller can forward external monitoring control</param>
		public static void MonitorAverageCount(string counterInstanceName, long count, System.Type lockType, bool isMonitoringActive)
		{
			try
			{
				// access counter in hashlist if it is there, else null
				CounterBase counter = (CounterBase) PerformanceCounterFacade.Counters[counterInstanceName];
				// allocate counter if needed
				AverageCounter avgCounter = (AverageCounter) PerformanceCounterFacade.SetCounter(lockType, ref counter, "Average", counterInstanceName, isMonitoringActive);

				if (avgCounter != null)
				{
					// adjust counters
					avgCounter.AddSampleValue(count);
					avgCounter.IncrementSampleCount();;
				}
				// persist static, threadsafe counter (null if perf monitoring off) for reuse by validator instances
				PerformanceCounterFacade.Counters[counterInstanceName] = avgCounter;
			}
			catch(Exception ex)
			{
				NameValueCollection nvc = new NameValueCollection(2);
				nvc.Add("Method", "MonitorAverageCount");
				nvc.Add("Probable cause", "PerfCounters not registered or wrong category in config file");
				new LoggableException(ex, nvc);
			}
        }

        /// <summary>
        /// Return a SimpleCounter for use by the caller 
        /// </summary>
        /// <param name="counterInstanceName">Performance counter instance name</param>
        /// <param name="count">the value for a single sample being averaged</param>
        /// <param name="lockType">some type used internally for locking</param>
        /// <param name="isMonitoringActive">caller can forward external monitoring control</param>
        public static SimpleCounter GetSimpleCounter(string counterInstanceName, System.Type lockType, bool isMonitoringActive)
        {
            SimpleCounter simpleCounter = null; 

            try
            {
                // access counter in hashlist if it is there, else null
                CounterBase counter = (CounterBase)PerformanceCounterFacade.Counters[counterInstanceName];
                // allocate counter if needed
                simpleCounter = (SimpleCounter)PerformanceCounterFacade.SetCounter(lockType, ref counter, "SimpleCounter", counterInstanceName, isMonitoringActive);
                // persist static, threadsafe counter (null if perf monitoring off) for reuse by validator instances
                PerformanceCounterFacade.Counters[counterInstanceName] = simpleCounter;
            }
            catch (Exception ex)
            {
                NameValueCollection nvc = new NameValueCollection(2);
                nvc.Add("Method", "MonitorAverageCount");
                nvc.Add("Probable cause", "PerfCounters not registered or wrong category in config file");
                new LoggableException(ex, nvc);
            }
            return simpleCounter;
        }
		/// <summary>
		/// Monitor average response time using a counter having the specified instance name
		/// </summary>
		/// <param name="counterInstanceName">Performance counter instance name</param>
		/// <param name="starttime">the start of the interval for a single sample or response time being averaged</param>
		/// <param name="lockType">some type used internally for locking</param>
		/// <param name="isMonitoringActive">caller can forward external monitoring control</param>
		/// <returns>Current time used as end time for previous sample. Can be used as starttime for next sample</returns>
		public static DateTime MonitorAverageResponse(string counterInstanceName, DateTime starttime, System.Type lockType, bool isMonitoringActive)
		{
			try
			{
				// access counter in hashlist if it is there, else null
				CounterBase counter = (CounterBase) PerformanceCounterFacade.Counters[counterInstanceName];
				// allocate counter if needed
				AverageCounter rtCounter = (AverageCounter) PerformanceCounterFacade.SetCounter(lockType, ref counter, "Average", counterInstanceName, isMonitoringActive);

				if (rtCounter != null)
				{
					// calculate milliseconds
					TimeSpan ts = DateTime.Now - starttime;
					long ms = (long) ts.TotalMilliseconds;
					// adjust counters
					rtCounter.AddSampleValue(ms);
					rtCounter.IncrementSampleCount();;
				}
				// persist static, threadsafe counter (null if perf monitoring off) for reuse by validator instances
				PerformanceCounterFacade.Counters[counterInstanceName] = rtCounter;
			}
			catch(InvalidOperationException ex)
			{
			}
			return DateTime.Now;
		}


        public static void MonitorRate(string counterInstanceName, long samplevalue, System.Type lockType, bool isMonitoringActive)
        {
            try
            {
                // access counter in hashlist if it is there, else null
                CounterBase counter = (CounterBase)PerformanceCounterFacade.Counters[counterInstanceName];
                // allocate counter if needed
                RateCounter rateCounter = (RateCounter)PerformanceCounterFacade.SetCounter(lockType, ref counter, "Rate", counterInstanceName, isMonitoringActive);

                if (rateCounter != null)
                {
                    rateCounter.AddSampleValue(samplevalue);
                }
                // persist static, threadsafe counter (null if perf monitoring off) for reuse by validator instances
                PerformanceCounterFacade.Counters[counterInstanceName] = rateCounter;
            }
            catch (Exception ex)
            {
                NameValueCollection nvc = new NameValueCollection(2);
                nvc.Add("Method", "MonitorRate");
                nvc.Add("Probable cause", "PerfCounters not registered or wrong category in config file");
                new LoggableException(ex, nvc);
            }
        }
	}
}
