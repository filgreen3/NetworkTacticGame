using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuCode : MonoBehaviour {

	public UnityEngine.UI.InputField IPAdressContainer, NewNameContainer;

	public LoadingScript Loading;
	bool startServet;

	public Font font;

	void Start () {
		font.material.mainTexture.filterMode = FilterMode.Point;
	}

	public void MakeNewName () {
		FileInfo loginName = new FileInfo ("loginName.txt");
		if (!loginName.Exists)
			loginName.Create ();

		StreamWriter logStream = new StreamWriter ("loginName.txt");
		logStream.WriteLine (NewNameContainer.text);
		logStream.Close ();
	}

	public void StartServer () {
		if (!startServet) {
			try {
				startServet = true;

				Process.Start ("ServerForGame.exe");
				ClientReciverMK1.server = "127.0.0.1";
				Loading.StartLoading (1, 1);
				ClientReciverMK1.Connect ();

			} catch {
				startServet = false;
				UnityEngine.Debug.LogError ("ServerCreateFail!");
			}

		}
	}
	public void ConnectToServer () {
		ClientReciverMK1.server = IPAdressContainer.text;
		string userName = "New Player";
		FileInfo loginName = new FileInfo ("loginName.txt");
		if (!loginName.Exists) {
			StreamReader logStream = new StreamReader ("loginName.txt");
			userName = logStream.ReadToEnd ();
		}

		try {
			ClientReciverMK1.Connect (userName);
			Loading.StartLoading (1, 0.5f);
		} catch {
			UnityEngine.Debug.LogError ("ConnectFail!");
			IPAdressContainer.text = "Fail Connect";
		}
	}

}