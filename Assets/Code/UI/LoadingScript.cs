using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadingScript : MonoBehaviour {

	public UnityEngine.UI.Slider SliderOfLoad;

	private int _sceneID;
	public void StartLoading (int sceneID, float waittime) {
		_sceneID = sceneID;
		StartCoroutine (LoadAsyn (waittime));
	}

	IEnumerator LoadAsyn (float wait) {
		float loadingProgress;
		yield return new WaitForSeconds (wait);
		GetComponent<Camera> ().enabled = true;
		AsyncOperation operation = SceneManager.LoadSceneAsync (_sceneID);
		while (!operation.isDone) {
			loadingProgress = operation.progress / 0.9f;

			SliderOfLoad.value = loadingProgress;
			yield return null;
		}
	}

}