using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class FakeSceneManager : MonoBehaviour
{
	public Text DataText;

	private void Start()
	{
		SceneInformation currentScene = FlowManager.Instance.GetCurrentScene();
		DataText.text = currentScene.PrefabToLoad.name;
		Instantiate(currentScene.PrefabToLoad, transform);
	}

	public void FakeContinueButton()
	{
		string sceneName = FlowManager.Instance.GetSceneFromFlow().SceneName;
		SceneSetupManager.Instance.LoadScene(sceneName);
	}

	public void FakeSetProgressionFlagOfCurrentScene()
	{
		FlowManager.Instance.SetProgressionFlag();
	}

	public void FakeGoToMainMenu()
	{
		FlowManager.Instance.AbortSceneFlow();

		string sceneName = FlowManager.Instance.GetSceneFromFlow().SceneName;
		SceneSetupManager.Instance.LoadScene(sceneName);
	}
}
