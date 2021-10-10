using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SceneStack
{
	private Stack<SceneInformation> sceneInformationStack = new Stack<SceneInformation>();

	public void Push(List<SceneInformation> sceneInformationList, bool reverseListBeforePush = true)
	{
		if (reverseListBeforePush)
			foreach (SceneInformation sceneInformation in sceneInformationList.Reverse<SceneInformation>())
				Push(sceneInformation);
		else
			foreach (SceneInformation sceneInformation in sceneInformationList)
				Push(sceneInformation);
	}

	public void Push(SceneInformation sceneInformation)
	{
#if UNITY_EDITOR
		Debug.Log("Push: " + sceneInformation.SceneName);
#endif
		sceneInformationStack.Push(sceneInformation);
	}

	public SceneInformation Pop()
	{
#if UNITY_EDITOR
		Debug.Log("Pop: " + (sceneInformationStack.Count != 0 ? sceneInformationStack.Peek().SceneName : "NULL"));
#endif
		if (sceneInformationStack.Count != 0)
			return sceneInformationStack.Pop();

		return null;
	}

	public void Clear()
	{
		sceneInformationStack.Clear();
	}
}
