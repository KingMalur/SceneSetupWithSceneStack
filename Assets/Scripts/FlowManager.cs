using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(DDOL))]
public class FlowManager : MonoBehaviour
{
	public static FlowManager Instance;

	[Header("Fallback Scene Information")]
	public SceneInformation FallbackSceneInformation;
	public ProgressionFlag FallbackSceneFlag = ProgressionFlag.None;

	private Dictionary<ProgressionFlag, bool> ProgressionFlags;
	private SceneStack sceneStack = new SceneStack();
	private SceneInformation currentSceneInformation;

	public bool CurrentSceneIsFallbackScene => GetCurrentScene() == GetFallbackScene();

	private void Awake()
	{
		if (Instance != null)
			Destroy(gameObject);
		else
			Instance = this;

		FakeLoadProgressionFlags();
		SetCurrentScene(GetFallbackScene());
	}

	public void AddSceneFlow(SceneFlow sceneFlow)
	{
		AbortSceneFlow();
		sceneStack.Push(sceneFlow.SceneInformation);
	}

	public void AddSceneToFlow(SceneInformation sceneInformation)
	{
		sceneStack.Push(sceneInformation);
	}

	public SceneInformation GetSceneFromFlow()
	{
		// I know I could use the variables but it looks cleaner & is nice to read
		SetCurrentScene(sceneStack.Pop() ?? GetFallbackScene());

		if (CurrentSceneIsFallbackScene)
			return GetCurrentScene();

		if (GetCurrentScene().AlwaysLoadScene)
			return GetCurrentScene();

		bool flagValue = false;
		if (ProgressionFlags.TryGetValue(currentSceneInformation.ProgressionFlagToSet, out flagValue) && flagValue)
			return GetSceneFromFlow();

		return GetCurrentScene();
	}

	public void AbortSceneFlow()
	{
		sceneStack.Clear();
		SetCurrentScene(GetFallbackScene());
	}

	public SceneInformation GetCurrentScene()
	{
		return currentSceneInformation;
	}

	private void SetCurrentScene(SceneInformation sceneInformation)
	{
		currentSceneInformation = sceneInformation;
	}

	private SceneInformation GetFallbackScene()
	{
		return FallbackSceneInformation;
	}

	// TODO: Make private for real application
	public void FakeLoadProgressionFlags()
	{
		ProgressionFlags = Enum.GetValues(typeof(ProgressionFlag))
			.Cast<ProgressionFlag>()
			.ToDictionary(k => k, v => false);
	}

	public void SetProgressionFlag()
	{
		SetProgressionFlag(currentSceneInformation.ProgressionFlagToSet);
	}

	private void SetProgressionFlag(ProgressionFlag progressionFlag)
	{
		if (ProgressionFlags.ContainsKey(progressionFlag))
			ProgressionFlags[progressionFlag] = true;
	}
}
