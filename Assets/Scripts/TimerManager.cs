using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x0200054A RID: 1354
public class TimerManager : MonoBehaviour
{
	// Token: 0x06002C0F RID: 11279 RVA: 0x0001E08E File Offset: 0x0001C28E
	private void Awake()
	{
		if (TimerManager.instance == null)
		{
			TimerManager.instance = this;
			UnityEngine.Object.DontDestroyOnLoad(base.gameObject);
		}
	}

	// Token: 0x06002C10 RID: 11280 RVA: 0x000DD444 File Offset: 0x000DB644
	private static void Init()
	{
		if (TimerManager.instance == null)
		{
			GameObject gameObject = new GameObject("TimerManager");
			gameObject.AddComponent<TimerManager>();
		}
	}

	// Token: 0x06002C11 RID: 11281 RVA: 0x000DD470 File Offset: 0x000DB670
	private void Update()
	{
		this.EventBatch = 0;
		while (this.Active.Count > 0 && this.EventBatch < this.MaxEventsPerFrame)
		{
			if (this.EventIterator < 0)
			{
				this.EventIterator = this.Active.Count - 1;
				break;
			}
			if (this.EventIterator > this.Active.Count - 1)
			{
				this.EventIterator = this.Active.Count - 1;
			}
			if (Time.time >= this.Active[this.EventIterator].DueTime || this.Active[this.EventIterator].ID == 0)
			{
				this.Active[this.EventIterator].Invoke();
			}
			this.EventIterator--;
			this.EventBatch++;
		}
	}

	// Token: 0x06002C12 RID: 11282 RVA: 0x0001E0AE File Offset: 0x0001C2AE
	public static int Start()
	{
		return TimerManager.Start(true);
	}

	// Token: 0x06002C13 RID: 11283 RVA: 0x0001E0B6 File Offset: 0x0001C2B6
	public static int Start(bool cancelOnLoad)
	{
		return TimerManager.In(3.1536E+08f, cancelOnLoad, null);
	}

	// Token: 0x06002C14 RID: 11284 RVA: 0x000DD55C File Offset: 0x000DB75C
	public static float GetDuration(int id)
	{
		for (int i = 0; i < TimerManager.instance.Active.Count; i++)
		{
			if (TimerManager.instance.Active[i].ID == id)
			{
				return Time.time - TimerManager.instance.Active[i].StartTime;
			}
		}
		return 0f;
	}

	// Token: 0x06002C15 RID: 11285 RVA: 0x0001E0C4 File Offset: 0x0001C2C4
	public static int In(float delay, TimerManager.Callback callback)
	{
		return TimerManager.In(delay, true, 1, 0f, callback);
	}

	// Token: 0x06002C16 RID: 11286 RVA: 0x0001E0D4 File Offset: 0x0001C2D4
	public static int In(float delay, bool cancelOnLoad, TimerManager.Callback callback)
	{
		return TimerManager.In(delay, cancelOnLoad, 1, 0f, callback);
	}

	// Token: 0x06002C17 RID: 11287 RVA: 0x0001E0E4 File Offset: 0x0001C2E4
	public static int In(float delay, int iterations, float interval, TimerManager.Callback callback)
	{
		return TimerManager.In(delay, true, iterations, interval, callback);
	}

	// Token: 0x06002C18 RID: 11288 RVA: 0x000DD5C0 File Offset: 0x000DB7C0
	public static int In(float delay, bool cancelOnLoad, int iterations, float interval, TimerManager.Callback callback)
	{
		TimerManager.Init();
		delay = Mathf.Max(0f, delay);
		iterations = Mathf.Max(0, iterations);
		interval = Mathf.Max(0f, interval);
		TimerManager.Event @event;
		if (TimerManager.instance.Pool.Count > 0)
		{
			@event = TimerManager.instance.Pool[0];
			TimerManager.instance.Pool.Remove(@event);
		}
		else
		{
			@event = new TimerManager.Event();
		}
		TimerManager.instance.EventCount++;
		@event.ID = TimerManager.instance.EventCount;
		@event.Function = callback;
		@event.Iterations = iterations;
		@event.Interval = interval;
		@event.StartTime = Time.time;
		@event.DueTime = Time.time + delay;
		@event.CancelOnLoad = cancelOnLoad;
		TimerManager.instance.Active.Add(@event);
		return @event.ID;
	}

	// Token: 0x06002C19 RID: 11289 RVA: 0x000DD6A0 File Offset: 0x000DB8A0
	public static void Cancel(int id)
	{
		if (0 >= id)
		{
			return;
		}
		for (int i = TimerManager.instance.Active.Count - 1; i > -1; i--)
		{
			if (TimerManager.instance.Active[i].ID == id)
			{
				TimerManager.instance.Active[i].ID = 0;
				return;
			}
		}
	}

	// Token: 0x06002C1A RID: 11290 RVA: 0x000DD700 File Offset: 0x000DB900
	public static void Cancel(params int[] ids)
	{
		if (ids != null && ids.Length > 0)
		{
			for (int i = TimerManager.instance.Active.Count - 1; i > -1; i--)
			{
				if (ids.Contains(TimerManager.instance.Active[i].ID))
				{
					TimerManager.instance.Active[i].ID = 0;
				}
			}
		}
	}

	// Token: 0x06002C1B RID: 11291 RVA: 0x000DD768 File Offset: 0x000DB968
	public static void CancelAll()
	{
		for (int i = TimerManager.instance.Active.Count - 1; i > -1; i--)
		{
			TimerManager.instance.Active[i].ID = 0;
		}
	}

	// Token: 0x06002C1C RID: 11292 RVA: 0x000DD7A8 File Offset: 0x000DB9A8
	private void OnLevelWasLoaded(int level)
	{
		for (int i = this.Active.Count - 1; i > -1; i--)
		{
			if (this.Active[i].CancelOnLoad)
			{
				this.Active[i].ID = 0;
			}
		}
	}

	// Token: 0x06002C1D RID: 11293 RVA: 0x000DD7F4 File Offset: 0x000DB9F4
	public static bool IsActive(int id)
	{
		for (int i = TimerManager.instance.Active.Count - 1; i > -1; i--)
		{
			if (TimerManager.instance.Active[i].ID == id)
			{
				return true;
			}
		}
		return false;
	}

	// Token: 0x04001C22 RID: 7202
	private List<TimerManager.Event> Active = new List<TimerManager.Event>();

	// Token: 0x04001C23 RID: 7203
	private List<TimerManager.Event> Pool = new List<TimerManager.Event>();

	// Token: 0x04001C24 RID: 7204
	public int EventCount;

	// Token: 0x04001C25 RID: 7205
	private int EventIterator;

	// Token: 0x04001C26 RID: 7206
	private int EventBatch;

	// Token: 0x04001C27 RID: 7207
	private int MaxEventsPerFrame = 500;

	// Token: 0x04001C28 RID: 7208
	private static TimerManager instance;

	// Token: 0x0200054B RID: 1355
	private class Event
	{
		// Token: 0x06002C1F RID: 11295 RVA: 0x000DD83C File Offset: 0x000DBA3C
		public void Invoke()
		{
			if (this.ID != 0)
			{
				if (this.DueTime != 0f)
				{
					if (this.Function != null)
					{
						try
						{
							this.Function();
						}
						catch
						{
						}
						if (this.Iterations > 0)
						{
							this.Iterations--;
							if (this.Iterations < 1)
							{
								this.Recycle();
								return;
							}
						}
						this.DueTime = Time.time + this.Interval;
						return;
					}
					this.Recycle();
					return;
				}
			}
			this.Recycle();
		}

		// Token: 0x06002C20 RID: 11296 RVA: 0x000DD8D0 File Offset: 0x000DBAD0
		public void Recycle()
		{
			this.ID = 0;
			this.DueTime = 0f;
			this.StartTime = 0f;
			this.CancelOnLoad = true;
			this.Function = null;
			if (TimerManager.instance.Active.Remove(this))
			{
				TimerManager.instance.Pool.Add(this);
			}
		}

		// Token: 0x04001C29 RID: 7209
		public int ID;

		// Token: 0x04001C2A RID: 7210
		public TimerManager.Callback Function;

		// Token: 0x04001C2B RID: 7211
		public int Iterations = 1;

		// Token: 0x04001C2C RID: 7212
		public float Interval = -1f;

		// Token: 0x04001C2D RID: 7213
		public float StartTime;

		// Token: 0x04001C2E RID: 7214
		public float DueTime;

		// Token: 0x04001C2F RID: 7215
		public bool CancelOnLoad = true;
	}

	// Token: 0x0200054C RID: 1356
	// (Invoke) Token: 0x06002C22 RID: 11298
	public delegate void Callback();
}
