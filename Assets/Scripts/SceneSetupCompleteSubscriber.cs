using UnityEngine;

public class SceneSetupCompleteSubscriber : MonoBehaviour
{
	private SceneSetupManager manager;

	private void Awake()
	{
		manager = FindObjectOfType<SceneSetupManager>();
	}

	private void OnEnable()
	{
		manager.SceneSetupCompleted += DebugMessage;
	}

	private void OnDisable()
	{
		manager.SceneSetupCompleted -= DebugMessage;
	}

	private void DebugMessage()
	{
		Debug.Log("SceneSetupCompleted after " + Time.realtimeSinceStartup + " seconds!");
	}
}
