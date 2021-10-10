using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SceneSetupManager : MonoBehaviour
{
	public static SceneSetupManager Instance;

	public delegate void SceneSetupCompletedEventHandler();
	public event SceneSetupCompletedEventHandler SceneSetupCompleted;

	public delegate void GameStartedEventHandler();
	public event GameStartedEventHandler GameStarted;

#if UNITY_EDITOR
	[Header("Debug")]
	public bool SimulateLongLoadingTime = true;
	public float AddedLoadingTime = 2.5f;
#endif

	[Header("Loading Bar")]
	[SerializeField]
	private float currentValue = 0.0f;
	[SerializeField]
	private float targetValue = 0.0f;

	[Header("Flow Control")]
	public bool WaitForKeyPress = true;
	[Range(0.0f, 2.5f)]
	public float MinLoadTimeAtEnd = 1.5f;
	public string canvasHide = "Canvas_Hide";
	public string canvasShow = "Canvas_Show";
	public string loadingIndicatorHide = "LoadingIndicator_Hide";

	[Header("Objects")]
	public GameObject UIRootObject;
	private Animator uiAnimator;
	public GameObject ContinueObject;
	public Slider loadingBar;
	public GameObject loadingIndicatorObject;
	private Animator loadingIndicatorAnimator;

	private bool loadingBarComplete => loadingBar.value == loadingBar.maxValue;
	private bool longSetupComplete = false;
	private bool gameStarted = false;

	private int monoEntitiesCount = 0;
	private int loadedMonoEntitiesCount = 0;

	private void Awake()
	{
		if (Instance != null)
			Destroy(gameObject);
		else
			Instance = this;

		Time.timeScale = 0.0f;

		loadingBar.value = 0.0f;
		loadingBar.maxValue = 0.0f;

		ContinueObject.SetActive(false);
		UIRootObject.SetActive(true);
		loadingBar.gameObject.SetActive(true);
		loadingIndicatorObject.SetActive(false);

		uiAnimator = UIRootObject.GetComponentInChildren<Animator>();
		loadingIndicatorAnimator = loadingIndicatorObject.GetComponent<Animator>();
	}

	private void Start()
	{
		MonoEntity[] monoEntities = FindObjectsOfType<MonoEntity>();
		monoEntitiesCount = monoEntities.Length;
		loadingBar.maxValue = monoEntitiesCount;

		if (monoEntitiesCount == 0)
			SceneSetupComplete();

		for (int i = 0; i < monoEntitiesCount; i++)
			monoEntities[i].MonoEntityLoaded += HandleMonoEntityLoaded;
	}

	private void HandleMonoEntityLoaded(MonoEntity monoEntity)
	{
		loadedMonoEntitiesCount++;
		monoEntity.MonoEntityLoaded -= HandleMonoEntityLoaded;
	}

	private void Update()
	{
#if UNITY_EDITOR
		if (!longSetupComplete)
			Debug.Log("Entities: " + loadedMonoEntitiesCount + " / " + monoEntitiesCount);
#endif

		if (!longSetupComplete)
			if (monoEntitiesCount == loadedMonoEntitiesCount)
				SceneSetupComplete();

		if (loadingBar.value != loadingBar.maxValue)
			if (targetValue != loadingBar.maxValue)
				SetSmoothLoadingBar();
			else
				SetSmoothLoadingBar(speedMultiplier: 10.0f);

		if (WaitForKeyPress && longSetupComplete && !gameStarted)
			if (Input.anyKeyDown)
				StartCoroutine(StartGame());

		if (!WaitForKeyPress && longSetupComplete && !gameStarted && loadingBarComplete)
			StartCoroutine(StartGame());
	}

	private void SetSmoothLoadingBar(float speedMultiplier = 1.0f)
	{
		targetValue = (loadedMonoEntitiesCount == 0 ? (monoEntitiesCount / 100.0f) : loadedMonoEntitiesCount);
		// Try to get a faster progress change by using the difference between current & target as a multiplier
		int difference = (int)targetValue - (int)currentValue;
		difference = (difference == 0 ? 1 : difference);
		currentValue = Mathf.MoveTowards(currentValue, targetValue, difference * speedMultiplier * Time.unscaledDeltaTime);
		loadingBar.value = currentValue;
	}

	private IEnumerator StartGame()
	{
		uiAnimator.SetTrigger(canvasHide);
		GameStarted?.Invoke();
		gameStarted = true;
		yield return null;

		Time.timeScale = 1.0f;
		yield return new WaitForSecondsRealtime(uiAnimator.GetAnimationLength(canvasHide));

		ContinueObject.SetActive(false);
		loadingBar.gameObject.SetActive(false);
		yield return null;
	}

	private void SceneSetupComplete()
	{
		SceneSetupCompleted?.Invoke();
		longSetupComplete = true;

		if (WaitForKeyPress)
			ContinueObject.SetActive(true);
	}

	public void LoadScene(string nextSceneName)
	{
		StartCoroutine(LoadSceneAsync(nextSceneName));
	}

	private IEnumerator LoadSceneAsync(string nextSceneName)
	{
		ContinueObject.SetActive(false);
		loadingIndicatorObject.SetActive(true);
		uiAnimator.SetTrigger(canvasShow);
		yield return new WaitForSecondsRealtime(uiAnimator.GetAnimationLength(canvasShow));

		Time.timeScale = 0.0f;
		yield return null;

		var async = SceneManager.LoadSceneAsync(nextSceneName);
		async.allowSceneActivation = false;

#if UNITY_EDITOR
		if (SimulateLongLoadingTime)
		{
			float startAddedTime = Time.realtimeSinceStartup;
			WaitForSecondsRealtime delay = new WaitForSecondsRealtime(0.1f);
			while (Time.realtimeSinceStartup - startAddedTime < AddedLoadingTime)
				yield return delay;
		}
#endif

		float startTime = Time.realtimeSinceStartup;
		while (!async.isDone)
		{
			if (async.progress >= 0.9f && !async.allowSceneActivation)
			{
				if (Time.realtimeSinceStartup - startTime >= MinLoadTimeAtEnd)
				{
					loadingIndicatorAnimator.SetTrigger(loadingIndicatorHide);
					yield return new WaitForSecondsRealtime(loadingIndicatorAnimator.GetAnimationLength(loadingIndicatorHide));
					async.allowSceneActivation = true;
				}
			}
			yield return null;
		}
	}
}
