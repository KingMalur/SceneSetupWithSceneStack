using System.Collections;
using UnityEngine;

public class FakeMonoEntity : MonoEntity
{
	public bool UseMultiplier = true;
	private WaitForSecondsRealtime delay = new WaitForSecondsRealtime(0.1f);

	public override IEnumerator LoadMonoEntity()
	{
		int random = Random.Range(5, 10);
		int multiplier = Random.Range(2, 5);
		int max = random * (UseMultiplier ? multiplier : 1);

		for (int i = 0; i < max; i++)
			yield return delay;

		yield return base.LoadMonoEntity();
	}
}
