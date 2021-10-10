using UnityEngine;

[System.Serializable]
public abstract class SceneInformation : ScriptableObject
{
	[Header("Scene Data")]
	public string SceneName;
	public bool AlwaysLoadScene;

	public GameObject PrefabToLoad;

	public ProgressionFlag ProgressionFlagToSet;
}
