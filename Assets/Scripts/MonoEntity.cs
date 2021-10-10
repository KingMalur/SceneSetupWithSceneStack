using System.Collections;
using UnityEngine;

public abstract class MonoEntity : MonoBehaviour
{
	public delegate void MonoEntityLoadedEventHandler(MonoEntity monoEntity);
	private event MonoEntityLoadedEventHandler monoEntityLoaded;
	public event MonoEntityLoadedEventHandler MonoEntityLoaded
	{
		add
		{
			lock(this)
			{
				monoEntityLoaded += value;
				StartCoroutine(LoadMonoEntity());
			}
		}
		remove
		{
			lock (this)
			{
				monoEntityLoaded -= value;
			}
		}
	}

	public virtual IEnumerator LoadMonoEntity()
	{
		monoEntityLoaded?.Invoke(this);
		yield return null;
	}
}
