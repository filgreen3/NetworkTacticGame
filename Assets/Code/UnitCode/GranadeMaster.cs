using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GranadeMaster : MonoBehaviour, IPercActivity {

    
    public enum GranadeType {
        Frag,
        Stun,
        Fire,
        Kick
    }

    [System.Serializable]
    public class GranadeInPoket {
        public int PowerPerc;
        public GranadeType granade;
        public int Range;
        public int icount;
        public int count {
            get { return icount; }
            set {
                icount = value;
                InfoText.text = "x" + value + " " +
                    granade.ToString ();
            }
        }
        public UnityEngine.UI.Text InfoText;

    }
    private ParticleSystem _lineEffect, _granadelineFx;
[Header ("Настройки гранат")]
    public int MaxDistantion;

    private Vector3 _point = Vector3.zero;
    Vector2[] vectorsOfGrande;

    public GameObject GranadeUI;
    public GranadeInPoket[] Granades;
    private int currentGranade = -1;
    BasicUnit ThisUnit;

    public void Awake () {
        SetEffects ();
    }

    public void SetEffects () {
        if (_lineEffect == null) _lineEffect = Instantiate (Resources.Load ("FX/PercGranade/LineSystemFX1") as GameObject, transform.position, Quaternion.identity, transform).GetComponent<ParticleSystem> ();
        if (_granadelineFx == null) _granadelineFx = (Resources.Load ("FX/PercGranade/GranadeLineFX") as GameObject).GetComponent<ParticleSystem> ();
        if (ThisUnit == null) ThisUnit = GetComponent<BasicUnit> ();
    }

    public void Reset () {
        GranadeUI.SetActive (false);
        currentGranade = -1;
    }
    public void SetGranade (int i) {
        if (Granades[i].count > 0) {
            currentGranade = Mathf.Clamp (i, 0, Granades.Length - 1);
        } else {
            ThisUnit.Manger.ShowInfoText ("No this granad", Color.white, transform.position);
            return;
        }
    }

    public void PercAction () {

        if (!GranadeUI.activeInHierarchy) GranadeUI.SetActive (true);
        if (currentGranade == -1) return;

        GranadeUI.SetActive (false);

        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);
        Physics.Raycast (ray, out hit, Mathf.Infinity, ThisUnit.Manger.Mask);

        Vector3 localpoint = hit.point;
        if (hit.collider != null) {
            ThisUnit.Manger.ClearGrid ();

            if (!_lineEffect.gameObject.activeInHierarchy)
                _lineEffect.gameObject.SetActive (true);

            _point = GridABS.NodeFromWorldPoint (localpoint).Position;
            vectorsOfGrande = GetMatrix ();
            _lineEffect.Stop (true, ParticleSystemStopBehavior.StopEmittingAndClear);

            var vel = _lineEffect.velocityOverLifetime;
            Vector3 dir = (_point - transform.position).normalized;

            float dist = Vector3.Distance (_point, transform.position);
            if (dist > MaxDistantion)
                dist = MaxDistantion;
            vel.x = new ParticleSystem.MinMaxCurve (dir.x * dist);
            vel.z = new ParticleSystem.MinMaxCurve (dir.z * dist);

            _lineEffect.Play ();

            Vector3 vector = GridABS.NodeFromWorldPoint (transform.position + dist * dir).Position;

            for (int i = 0; i < vectorsOfGrande.Length; i++) {

                vectorsOfGrande[i] += new Vector2 (vector.x, vector.z);
                SpriteRenderer localmarcer = ThisUnit.Manger.PoolElement;
                localmarcer.transform.position = new Vector3 (vectorsOfGrande[i].x, 0, vectorsOfGrande[i].y);
                localmarcer.color = Color.cyan;
                ThisUnit.Manger.TrashNode.Add (localmarcer);
            }

            if (Input.GetKeyDown (KeyCode.Mouse0) && !UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject ()) {
                ThisUnit.Manger.ClearGrid ();

                var node = GridABS.NodeFromWorldPoint (_point);
                ClientReciverMK1.WriteMsg ("/VSBoom" + node.x + ":" + node.y + ":" + (int) Granades[currentGranade].granade);
                var granadeLineFx = Instantiate (_granadelineFx, transform.position, Quaternion.identity);
                granadeLineFx.Stop ();
                var fxvel = granadeLineFx.velocityOverLifetime;

                fxvel.x = new ParticleSystem.MinMaxCurve (dir.x * dist);
                fxvel.z = new ParticleSystem.MinMaxCurve (dir.z * dist);

                granadeLineFx.gameObject.SetActive (true);
                granadeLineFx.Play ();

                _point = vector;
                ThisUnit.Manger.IsLock = true;

                Invoke ("GetBoom", 1);

            }
        }
    }

    void GetBoom () {
        Instantiate (ImportVisualEffect ((int) Granades[currentGranade].granade), _point, Quaternion.identity);
        for (int i = 0; i < vectorsOfGrande.Length; i++) {
            Vector2 n = vectorsOfGrande[i];
            NetworkElement unitTarget = GridABS.NodeFromWorldPoint (new Vector3 (n.x, 0, n.y)).UnitOnNode;
            if (unitTarget != null) {
                int PowerPerc = Granades[currentGranade].PowerPerc;
                int Range = Granades[currentGranade].Range;
                switch ((int) Granades[currentGranade].granade) {
                    case 0:
                        unitTarget.unit.GetDamage (PowerPerc);
                        ClientReciverMK1.WriteMsg ("/Damage" + ThisUnit.Manger.GetUnitId (unitTarget) + ":" + PowerPerc + ":" + 0 + ":" + 0);
                        break;
                    case 1:
                        unitTarget.unit.GetDamage (0, PowerPerc, (int) BasicUnit.DamageType.Stan);
                        ClientReciverMK1.WriteMsg ("/Damage" + ThisUnit.Manger.GetUnitId (unitTarget) + ":" + 0 + ":" + PowerPerc + ":" + (int) BasicUnit.DamageType.Stan);
                        break;
                    case 2:
                        unitTarget.unit.GetDamage (PowerPerc, Range, (int) BasicUnit.DamageType.Fire);
                        ClientReciverMK1.WriteMsg ("/Damage" + ThisUnit.Manger.GetUnitId (unitTarget) + ":" + PowerPerc + ":" + Range + ":" + (int) BasicUnit.DamageType.Fire);
                        break;
                    case 3:
                        Vector3 poitionToChek = unitTarget.transform.position + (unitTarget.transform.position - _point).normalized * PowerPerc;
                        var node = GridABS.NodeFromWorldPoint (poitionToChek);

                        if (node.TypeBloc == NodeA.NodeType.Walkable && node.UnitOnNode == null) {
                            ClientReciverMK1.WriteMsg ("/Move" + ThisUnit.Manger.GetUnitId (unitTarget) + ":" + node.x + ":" + node.y);
                            unitTarget.MoveToCord (node.x, node.y);

                        }

                        break;
                    case 4:

                        break;
                }
            }

        }
        Granades[currentGranade].count -= 1;
        currentGranade = -1;
        ThisUnit.Manger.EndTurn ();
        ClientReciverMK1.WriteMsg ("/EndTurn");

    }

    public void Clean () {
        if (_lineEffect != null)
            _lineEffect.gameObject.SetActive (false);

    }

    Vector2[] GetMatrix () {
        int Range = Granades[currentGranade].Range;
        Vector2[] result = new Vector2[(Range * 2 - 1) * (Range * 2 - 1)];
        int k = 0;
        for (int i = -(Range - 1); i <= Range - 1; i++) {
            for (int j = -(Range - 1); j <= Range - 1; j++) {
                result[k] = new Vector2 (i, j);
                k++;
            }
        }

        return result;

    }
    public static GameObject ImportVisualEffect (int typeGranade) {
        string pathGanadeFx = "FX/PercGranade/";
        switch (typeGranade) {
            case 0:
                pathGanadeFx += "FX_Explosion_Rubble";
                break;
            case 1:
                pathGanadeFx += "StanGrandeFx";
                break;
            case 2:
                pathGanadeFx += "FireGrandeFX";
                break;
            case 3:
                pathGanadeFx += "KickFX";
                break;
        }
        return Resources.Load (pathGanadeFx) as GameObject;

    }
}