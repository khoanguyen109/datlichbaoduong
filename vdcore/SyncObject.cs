using System;
using System.Threading;

using Vendare.Utils;
using Vendare.Error;



public class SyncObject
{
	public SyncObject()
	{
	}

	public void Awake()
	{
		try
		{					
			Monitor.Enter(this);
			Monitor.PulseAll(this);
		}
		catch(Exception e)
		{
			throw new LoggableException(e, null);
		}
		finally
		{
			Monitor.Exit(this);
		}
	}

	public void Sleep()
	{
		try
		{
			Monitor.Enter(this);
			Monitor.Wait(this);
		}
		catch(Exception e)
		{
			throw new LoggableException(e, null);
		}
		finally
		{
			Monitor.Exit(this);
		}
	}
}
