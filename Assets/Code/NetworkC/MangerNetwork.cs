using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;
public class MangerNetwork : MonoBehaviour {
	[Header ("Список юнитов")]
	public NetworkElement[] ElementsInGame;
	public NetworkElement[] TeamA, TeamB;
	private int teamASize, teamBSize;

	[Header ("Пулл ячеек")]
	private SpriteRenderer[] _marcerPool = new SpriteRenderer[40];
	private int _imarcerIndex;
	private int _marcerIndex {
		get {
			_imarcerIndex++;
			if (_imarcerIndex >= 40) {
				_imarcerIndex = 0;
				return 0;
			} else
				return _imarcerIndex;

		}

	}
	public SpriteRenderer PoolElement {
		get {
			var result = _marcerPool[_marcerIndex];
			result.gameObject.SetActive (true);
			return result;
		}
	}
	public SpriteRenderer marcerElement;
	public List<SpriteRenderer> TrashNode = new List<SpriteRenderer> ();

	[Header ("Камеры")]
	public GameObject CameraFollow;
	public Camera TerrainCam;

	[Header ("Другое")]
	public AudioSource audioSource;

	public NetworcVisualisation VisualManger;

	public bool IsLock;

	public UnityEngine.UI.Text AmmoInfo;

	public NetworkElement DeflatElement;
	public NetworkElement CorrectUnit;

	public enum EnumTurnMode {
		_moveMode,
		_attackMode,
		_percMode,
		_noneMode
	}
	public EnumTurnMode TurnMode;

	TextMeshPro InfoText;
	List<GameObject> Junk = new List<GameObject> ();

	public List<NodeA> ZoneMarced = new List<NodeA> ();
	public LayerMask Mask;
	public SpriteRenderer MarcerCorrectUnit;
	int TurnNum;

	[Header ("Сетевые настройки")]
	public TeamType TeamName;
	public enum TeamType {
		TeamA,
		TeamB
	}

	void CreateLists () {
		int allcount = teamASize + teamBSize;
		ElementsInGame = new NetworkElement[allcount];
		TeamA = new NetworkElement[teamASize];
		TeamB = new NetworkElement[teamBSize];
		for (int i = 0; i < allcount; i++)
			ElementsInGame[i] = Instantiate (DeflatElement, transform);
		for (int i = 0; i < teamASize; i++)
			TeamA[i] = ElementsInGame[i];
		for (int i = 0; i < teamBSize; i++)
			TeamB[i] = ElementsInGame[i + teamASize];
	}
	public void GetUnitArray (string msg) {
		InfoText = (Resources.Load ("InfoMarcer/HitInfo") as GameObject).GetComponent<TextMeshPro> ();
		marcerElement.gameObject.SetActive (false);
		for (int i = 0; i < 40; i++) {
			_marcerPool[i] = Instantiate (marcerElement);

		}
		marcerElement.gameObject.SetActive (true);

		string[] lines = msg.Remove (0, 5).Split ('#');
		string[] allValue = lines[0].Split (':');
		teamASize = Convert.ToInt32 (allValue[0]);
		teamBSize = Convert.ToInt32 (allValue[1]);
		CreateLists ();
		GridABS.CreateGrid ();
		for (int i = 1; i < lines.Length; i++) {
			string[] line = lines[i].Split (':');
			Instantiate (IdentificationParams.GetUnitFromId (Convert.ToInt32 (line[0])), ElementsInGame[i - 1].transform);
			Instantiate (IdentificationParams.GetWeaponFromId (Convert.ToInt32 (line[1])), ElementsInGame[i - 1].transform);
		}
		InstantiateUnitTeams ();

	}
	public void InstantiateUnitTeams () {
		for (int i = 0; i < teamASize; i++) {
			NetworkElement unit = TeamA[i];
			if (TeamName == TeamType.TeamA)
				unit.Controlled = true;
			unit.transform.position = Vector3.right * 4 + Vector3.forward * i;
			unit.SetElements ();
			unit.unit.Manger = this;
		}
		for (int i = 0; i < teamBSize; i++) {
			NetworkElement unit = TeamB[i];
			if (TeamName == TeamType.TeamB)
				unit.Controlled = true;
			unit.transform.position = -Vector3.right * 5 + Vector3.back + Vector3.forward * i;
			unit.SetElements ();
			unit.unit.Manger = this;
		}
		CorrectUnit = correctUnitturn ();

		EndTurn ();

	}

	NetworkElement correctUnitturn () {

		if (TurnNum % 2 == 0)
			return TeamA[TurnNum / 2 % teamASize];
		else
			return TeamB[TurnNum / 2 % teamBSize];

	}
	public int GetUnitId (NetworkElement element) {
		for (int i = 0; i < teamASize + teamBSize; i++) {
			if (ElementsInGame[i] == element)
				return i;

		}
		throw new Exception ("No in GameIndex");
	}

	void FixedUpdate () {

		Vector3 NeedMoveVectorCam = Vector3.zero;
		if (Input.GetKey (KeyCode.W))
			CameraFollow.transform.Translate (-0.1f, 0, 0.1f);
		if (Input.GetKey (KeyCode.D))
			CameraFollow.transform.Translate (0.1f, 0, 0.1f);
		if (Input.GetKey (KeyCode.A))
			CameraFollow.transform.Translate (-0.1f, 0, -0.1f);
		if (Input.GetKey (KeyCode.S))
			CameraFollow.transform.Translate (0.1f, 0, -0.1f);

		if (!Input.GetKey (KeyCode.LeftAlt)) {
			if (Input.GetAxis ("Mouse ScrollWheel") != 0 && (TerrainCam.orthographicSize > 1 || Input.GetAxis ("Mouse ScrollWheel") < 0)) {
				Camera.main.orthographicSize -= (Input.GetAxis ("Mouse ScrollWheel") * (Camera.main.transform.position.y * 0.3f));
				TerrainCam.orthographicSize -= (Input.GetAxis ("Mouse ScrollWheel") * (Camera.main.transform.position.y * 0.3f));
			}
		} else {
			if (Input.GetAxis ("Mouse ScrollWheel") != 0 && (Camera.main.transform.eulerAngles.x > 25 || Input.GetAxis ("Mouse ScrollWheel") < 0) && (Camera.main.transform.eulerAngles.x < 90 || Input.GetAxis ("Mouse ScrollWheel") > 0)) {
				Camera.main.transform.eulerAngles -= new Vector3 (Input.GetAxis ("Mouse ScrollWheel") * 10, 0, 0);
			}

		}

		CameraFollow.transform.position = CameraFollow.transform.position + NeedMoveVectorCam;

	}
	void Update () {
		if (ElementsInGame.Length == 0 || CorrectUnit == null || IsLock) return;
		if (CorrectUnit.Controlled) {
			switch (TurnMode) {
				case EnumTurnMode._moveMode:
					GetMoving ();
					break;
				case EnumTurnMode._attackMode:
					GetAttack ();
					break;
				case EnumTurnMode._percMode:
					GetPerc ();
					break;
			}
		}
	}
	void LateUpdate () {

		if (CorrectUnit != null && CorrectUnit.Controlled) {
			if (Input.GetKeyDown (KeyCode.Alpha1))
				SetTurnMode (0);
			if (Input.GetKeyDown (KeyCode.Alpha2))
				SetTurnMode (1);
			if (Input.GetKeyDown (KeyCode.Alpha3))
				SetTurnMode (2);
			if (Input.GetKeyDown (KeyCode.R))
				GetReload ();

		}
	}

	public void SetTurnMode (int num) {
		ClearGrid ();
		CorrectUnit.unit.PercSystem.Reset ();
		switch (num) {
			case 0:
				if (TurnMode == EnumTurnMode._moveMode) {
					TurnMode = EnumTurnMode._noneMode;
					break;
				}
				TurnMode = EnumTurnMode._moveMode;
				break;
			case 1:
				if (TurnMode == EnumTurnMode._attackMode) {
					TurnMode = EnumTurnMode._noneMode;
					break;
				}

				if (CorrectUnit.weapon.Ammo > 0)
					TurnMode = EnumTurnMode._attackMode;
				else {
					ShowInfoText ("No ammo", Color.white, CorrectUnit.transform.position);
					TurnMode = EnumTurnMode._noneMode;
				}
				break;
			case 2:
				if (TurnMode == EnumTurnMode._percMode) {
					TurnMode = EnumTurnMode._noneMode;
					break;
				}
				TurnMode = EnumTurnMode._percMode;
				break;
			default:
				TurnMode = EnumTurnMode._noneMode;
				break;
		}

	}

	void GetMoving () {
		if (ZoneMarced.Count == 0) {
			ZoneMarced = CorrectUnit.unit.MovmentArea ();
			foreach (var item in ZoneMarced) {
				SpriteRenderer localmarcer = PoolElement;
				localmarcer.transform.position = item.Position;
				localmarcer.color = Color.green;
				TrashNode.Add (localmarcer);
			}
		}
		Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);
		RaycastHit hit;
		Vector3 PoitionToChek;
		if (Input.GetMouseButton (0) && Physics.Raycast (ray, out hit, 5000, Mask)) {

			PoitionToChek = hit.point;
			var node = GridABS.NodeFromWorldPoint (PoitionToChek);
			if (ZoneMarced.Contains (node) && node.UnitOnNode == null) {
				ClientReciverMK1.WriteMsg ("/Move" + GetUnitId (CorrectUnit) + ":" + node.x + ":" + node.y);
				CorrectUnit.MoveToCord (node.x, node.y);
				EndTurn ();
			}

		}
	}
	void GetAttack () {
		if (ZoneMarced.Count == 0) {
			ZoneMarced = CorrectUnit.weapon.AttackArea ();
			foreach (var item in ZoneMarced) {
				SpriteRenderer localmarcer = PoolElement;
				localmarcer.transform.position = item.Position;
				localmarcer.color = Color.red;
				TrashNode.Add (localmarcer);

				if (item.UnitOnNode != null) {
					ShowInfoText (Mathf.Clamp01 (CorrectUnit.weapon.NormalDist / (Vector3.Distance (item.UnitOnNode.transform.position, CorrectUnit.transform.position))), item.UnitOnNode.transform.position);
					SpriteRenderer localmarcerSecond = PoolElement;
					localmarcerSecond.transform.position = item.Position;
					localmarcerSecond.color = Color.yellow;
					TrashNode.Add (localmarcerSecond);
				}
			}
		}
		Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);
		RaycastHit hit;
		Vector3 PoitionToChek;
		if (Input.GetMouseButton (0) && Physics.Raycast (ray, out hit, 5000, Mask)) {

			PoitionToChek = hit.point;
			var node = GridABS.NodeFromWorldPoint (PoitionToChek);
			if (ZoneMarced.Contains (node) && node.UnitOnNode != null) {
				ClientReciverMK1.WriteMsg ("/Shoot" + GetUnitId (CorrectUnit) + ":" + node.x + ":" + node.y);
				CorrectUnit.ShootToСord (node.x, node.y);
				EndTurn ();
			}

		}

	}
	void GetPerc () {
		if (CorrectUnit.unit.TimeReusePerc < 1) {
			CorrectUnit.unit.PercSystem.PercAction ();
		}
	}
	public void GetReload () {
		if (CorrectUnit.Controlled) {
			CorrectUnit.weapon.Ammo = CorrectUnit.weapon.MaxAmmo;
			ShowInfoText ("Reload", Color.white, CorrectUnit.transform.position);
			EndTurn ();
			ClientReciverMK1.WriteMsg ("/EndTurn");
		}
	}

	public void ClearUnitNode (Vector3 position, NetworkElement networkElement = null) {
		GridABS.NodeFromWorldPoint (position).UnitOnNode = networkElement;
	}

	public void ShowInfoText (string info, Color color, Vector3 positionSelf) {
		TextMeshPro meshPro = Instantiate (InfoText, positionSelf + 2.4f * Vector3.up - Vector3.right * .25f * 1 /* (UnityEngine.Random.Range (-11, 11) / 5f)*/ + Vector3.forward * .1f, Quaternion.identity);
		meshPro.transform.eulerAngles = CorrectUnit.unit.transform.eulerAngles;
		meshPro.GetComponent<UpInfo> ().HitInfoMetod ();
		meshPro.color = color;
		meshPro.text = info;
	}
	public void ShowInfoText (float procency, Vector3 positionSelf) {
		TextMeshPro meshPro = Instantiate (InfoText, positionSelf + 2 * Vector3.up - Vector3.right * .25f + Vector3.forward * .1f, Quaternion.identity);
		meshPro.transform.eulerAngles = CorrectUnit.unit.transform.eulerAngles;
		meshPro.text = ((int) (procency * 100)).ToString () + "%";
		meshPro.color = new Color (1, 0.43f, 0);
		Junk.Add (meshPro.gameObject);
	}

	public void ClearGrid () {
		if (CorrectUnit.unit.PercSystem != null)
			CorrectUnit.unit.PercSystem.Clean ();
		ZoneMarced.Clear ();
		foreach (var item in TrashNode) {
			item.color = Color.white;
			item.transform.transform.position = Vector3.one * 100f;
		}
		foreach (var item in Junk) {
			Destroy (item);
		}
	}

	private void GetUnitInfo () {
		AmmoInfo.text = CorrectUnit.weapon.Ammo + " | " + CorrectUnit.weapon.MaxAmmo;
	}

	[ContextMenu ("EndTurn")]
	public void EndTurn () {

		if (CorrectUnit != null) {
			CorrectUnit.unit.EndingTurn ();
			CorrectUnit.unit.TimeReusePerc--;
		}
		TurnNum++;
		ClearGrid ();
		if (!correctUnitturn ().unit.Alive) {
			EndTurn ();
		}
		IsLock = false;
		CorrectUnit = correctUnitturn ();
		GetUnitInfo ();
		if (CorrectUnit.Controlled)
			CameraFollow.transform.position = new Vector3 (CorrectUnit.transform.position.x + Camera.main.transform.position.y / 1.5f, 0, CorrectUnit.transform.position.z - Camera.main.transform.position.y / 1.5f) + new Vector3 (0, Camera.main.transform.position.y, 0);
		MarcerCorrectUnit.transform.position = CorrectUnit.transform.position - Vector3.right * .25f + Vector3.forward * .1f;
		MarcerCorrectUnit.transform.parent = CorrectUnit.transform;
		CorrectUnit.unit.DamageSystem.CheckEffect ();
		TurnMode = EnumTurnMode._noneMode;
	}
	public void PlaySpecific (AudioClip audioClip) {
		audioSource.clip = audioClip;
		audioSource.Play ();
	}
}