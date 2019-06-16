using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChange : MonoBehaviour
{
	[SerializeField] private Trigger trigger;
	[SerializeField] private string sceneName = "Quit";

	private enum Trigger
	{
		None,
		OnTriggerEnter,
		OnCollisionEnter
	}

	private void OnTriggerEnter(Collider other)
	{
		if (trigger != Trigger.OnTriggerEnter) return;
		trigger = Trigger.None;

		LoadScene();
	}

	private void OnCollisionEnter(Collision collision)
	{
		if (trigger != Trigger.OnCollisionEnter) return;
		trigger = Trigger.None;

		LoadScene();
	}

	private void LoadScene()
	{
		if (sceneName.ToLower() == "quit" || sceneName.ToLower() == "applicationquit")
		{
			StartCoroutine(Quit());
		}
		else
		{
			StartCoroutine(LoadSceneAsync());
		}
	}

	IEnumerator LoadSceneAsync()
	{
		AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName);
		asyncLoad.allowSceneActivation = false;

		yield return new WaitForSecondsRealtime(1f);

		asyncLoad.allowSceneActivation = true;

		// Wait until the asynchronous scene fully loads
		while (!asyncLoad.isDone)
		{
			yield return null;
		}
	}

	IEnumerator Quit()
	{
		yield return new WaitForSecondsRealtime(1f);

		#if UNITY_EDITOR
			UnityEditor.EditorApplication.isPlaying = false;
		#else
			Application.Quit();
		#endif
	}
}
