using UnityEngine;

public class FakeGameStartSubscriber : MonoBehaviour
{
	private SceneSetupManager manager;

	private void Awake()
	{
		manager = FindObjectOfType<SceneSetupManager>();
	}

	private void OnEnable()
	{
		manager.GameStarted += DebugMessage;
	}

	private void OnDisable()
	{
		manager.GameStarted -= DebugMessage;
	}

	private void DebugMessage()
	{
		Debug.Log("GameStarted after " + Time.realtimeSinceStartup + " seconds!");
	}
}
