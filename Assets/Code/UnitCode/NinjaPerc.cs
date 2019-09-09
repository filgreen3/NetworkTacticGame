using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NinjaPerc : MonoBehaviour, IPercActivity {

    [Header ("Настройки энергетика")]

    private ParticleSystem _lineEffect, _granadeFx, _granadelineFx;
    public int PowerPerc;
    public int Reuse;
    public int Dist;
    public int EnergyCost;
    public bool isCreateList;

    private Vector3 _point = Vector3.zero;
    BasicUnit ThisUnit;

    private List<NetworkElement> targetList = new List<NetworkElement> ();

    public void PercAction () {
        if (ThisUnit == null) ThisUnit = gameObject.GetComponent<BasicUnit> ();
        if (!isCreateList) {
            int xSelf, ySelf;
            xSelf = GridABS.NodeFromWorldPoint (transform.position).x;
            ySelf = GridABS.NodeFromWorldPoint (transform.position).y;
            for (int x = -Dist; x <= Dist; x++) {
                int local = Mathf.Abs (x);
                int func = Dist - local;
                for (int y = -func; y <= func; y++) {
                    if(x==0&&y==0) continue;
                    try {
                        var nodes = GridABS.GridOfArena[x + xSelf, y + ySelf];
                        if (nodes.TypeBloc == NodeA.NodeType.Wall) continue;
                        SpriteRenderer localmarcer = ThisUnit.Manger.PoolElement;
                        if (nodes.UnitOnNode != null) {
                            targetList.Add (nodes.UnitOnNode);
                            localmarcer.color = Color.yellow;
                        } else localmarcer.color = Color.magenta;

                        localmarcer.transform.position = nodes.Position;
                        ThisUnit.Manger.TrashNode.Add (localmarcer);
                    } catch { };
                }

            }
            isCreateList = true;
        }

        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);
        Physics.Raycast (ray, out hit, Mathf.Infinity);

        if (Input.GetKeyDown (KeyCode.Mouse0) && hit.rigidbody != null) {
            BasicUnit target = hit.rigidbody.GetComponent<BasicUnit> ();
            if (ThisUnit == target) {
                GetFinalAction ();
            }
        }

    }
    public void Reset () {
        isCreateList = false;
    }
    void GetFinalAction () {

        foreach (NetworkElement item in targetList) {
            item.unit.GetDamage (0, PowerPerc, (int) BasicUnit.DamageType.Stan);
            ClientReciverMK1.WriteMsg ("/Damage" + ThisUnit.Manger.GetUnitId (item) + ":" + 0 + ":" + PowerPerc + ":" + (int) BasicUnit.DamageType.Stan);
        }

        ThisUnit.Manger.ClearGrid ();

        ThisUnit.Manger.EndTurn ();
        ClientReciverMK1.WriteMsg ("/EndTurn");
    }

    public void Clean () {

    }

}