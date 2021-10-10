using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
[CreateAssetMenu(fileName = "SceneFlow", menuName = "Scene Information/Scene Flow", order = 0)]
public class SceneFlow : ScriptableObject
{
	public List<SceneInformation> SceneInformation;
}
