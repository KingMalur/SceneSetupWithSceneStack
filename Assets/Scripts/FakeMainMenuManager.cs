using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class FakeMainMenuManager : MonoBehaviour
{
	private void Start()
	{
		SceneInformation currentScene = FlowManager.Instance.GetCurrentScene();
		Instantiate(currentScene.PrefabToLoad, transform);
	}

	public void AddSceneFlow(SceneFlow sceneFlow)
	{
		FlowManager.Instance.AddSceneFlow(sceneFlow);
	}

	public void ResetProgressionFlags()
	{
		FlowManager.Instance.FakeLoadProgressionFlags();
	}

	public void FakeGameStartButton()
	{
		string sceneName = FlowManager.Instance.GetSceneFromFlow().SceneName;
		if (FlowManager.Instance.CurrentSceneIsFallbackScene)
			return;
		SceneSetupManager.Instance.LoadScene(sceneName);
	}
}
