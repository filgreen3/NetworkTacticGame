using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class ChoosePackUI : MonoBehaviour {

	private List<string> listunit = new List<string>();
	public AudioSource audioSource;
	public AudioClip AcsessAudio, BlockAudio;

	private int chooseTurn;
	private bool isLockChoose;

	public ClientReciverMK1 client;
	public GameObject InGameUI;

	public SpriteRenderer[] unitsBlocks = new SpriteRenderer[6];

	[Header("Unit Text")]
	public Text NameUnitText;
	public Text HPText, EPText, MovementText, classText;
	public Image UnitImage;
	int iUnitID, UnitCount;
	int UnitID {
		get { return iUnitID; }
		set {
			iUnitID = value;
			if (value >= UnitCount) iUnitID = 0;
			if (value < 0) iUnitID = UnitCount - 1;
		}
	}
	BasicUnit[] Units;

	[Header("Weapon Text")]
	public Text NameWeaponText;
	public Text DamageText, DistantText, InfoWeaponText;
	public Image WeaponImage;
	int iWeaponID, WeaponCount;
	int WeaponID {
		get { return iWeaponID; }
		set {
			iWeaponID = value;
			if (value >= WeaponCount) iWeaponID = 0;
			if (value < 0) iWeaponID = WeaponCount - 1;
		}
	}
	BasicWeapon[] Weapons;

	void Start() {
		IdentificationParams.GetUnitFromId(1);
		Units = IdentificationParams.unitsArray;
		UnitCount = Units.Length;
		GetUnitInfo();

		IdentificationParams.GetWeaponFromId(1);
		Weapons = IdentificationParams.weaponsArray;
		WeaponCount = Weapons.Length;
		GetWeaponInfo();
	}

	public void UpdateUnitID(int i) {
		UnitID += i;
		GetUnitInfo();

	}
	public void GetUnitInfo() {
		BasicUnit localUnit = Units[UnitID];
		NameUnitText.text = localUnit.CorrectName;
		HPText.text = localUnit.MaxHP.ToString();
		EPText.text = localUnit.MaxEP.ToString();
		MovementText.text = localUnit.MaxMovement.ToString();
		UnitImage.sprite = localUnit.GetComponent<SpriteRenderer>().sprite;
	}

	public void UpdateWeaponID(int i) {
		WeaponID += i;
		GetWeaponInfo();

	}
	public void GetWeaponInfo() {
		BasicWeapon localWeapon = Weapons[WeaponID];
		NameWeaponText.text = localWeapon.CorrectName;
		DamageText.text = localWeapon.Damage + " | " + localWeapon.BulletPerShoot;
		DistantText.text = localWeapon.NormalDist + " | " + localWeapon.MaxDist;
		WeaponImage.sprite = localWeapon.GetComponent<SpriteRenderer>().sprite;
	}

	public void AddToList() {
		if (listunit.Count < 3 && !isLockChoose) {
			listunit.Add("#" + (UnitID + 1) + ":" + (WeaponID + 1) + ":1:1");
			PlaySpecific(AcsessAudio);
			isLockChoose = true;
		} else PlaySpecific(BlockAudio);
	}

	public void SignNewUnit(int unitID) {
		Sprite unitsprite = IdentificationParams.GetUnitFromId(unitID).GetComponent<SpriteRenderer>().sprite;
		unitsBlocks[chooseTurn + 3].sprite = unitsprite;
		chooseTurn++;
		isLockChoose = false;
	}

	public void StartBattle() {
		if (listunit.Count > 2) return;
		string result = "/SetUnits";
		foreach (string n in listunit) {
			result += n;
		}
		ClientReciverMK1.WriteMsg(result);
		client.StartReading();
		InGameUI.SetActive(true);
		gameObject.SetActive(false);

	}
	void PlaySpecific(AudioClip audioClip) {
		audioSource.clip = audioClip;
		audioSource.Play();
	}
}
