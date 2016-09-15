using System;

namespace Vendare.Utils.Performance
{
	/// <summary>
	/// Summary description for enumResetInterval.
	/// </summary>
	public enum SamplingInterval : int
	{
		None = 0,
		Every5Seconds = 5 * 1000,
		Every10Seconds = 10 * 1000,
		Every30Seconds = 30 * 1000,
		EveryMinute = 60 * 1000,
		Every2Minutes = 2 * 60 * 1000,
		Every5Minutes = 5 * 60 * 1000,
		Every10Minutes = 10 * 60 * 1000,
		QuarterHourly = 15 * 60 * 1000,
		HalfHourly = 30 * 60 * 1000,
		Hourly = 60 * 60 * 1000,
		Daily = 24 * 60 * 60 * 1000
	}
}
