using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Shelter : MonoBehaviour
{
    
    public bool wall;
    public int ShelterLevl;
    public GameObject FX;
    public List<Transform> ListofPivots = new List<Transform>();
    public GameObject image;
    public Text InfoText;



    private void Start()
    {
        
        GridABS.ShelterList.Add(this);
    }


    private void OnMouseEnter()
    {
        InfoText.gameObject.SetActive(true);
        image.SetActive(true);
        InfoText.text = ShelterLevl+"%";
    }

    private void OnMouseExit()
    {
        InfoText.gameObject.SetActive(false);
        image.SetActive(false);
    }


    public void SetSelterBlock()
    {
        Debug.Log("Shelter is set");
        int xC;
        int yC;


        foreach (Transform n in ListofPivots)
        {
            GridABS.NodeCoordinat(n.position, out xC, out yC);
            GridABS.GridOfArena[xC, yC].TypeBloc = (!wall)? NodeA.NodeType.Shelter: NodeA.NodeType.Wall;
        }
    }
    private void OnDestroy()
    {
        try
        {
            int xC;
            int yC;

            foreach (Transform n in ListofPivots)
            {
                GridABS.NodeCoordinat(n.position, out xC, out yC);
                GridABS.GridOfArena[xC, yC].TypeBloc = NodeA.NodeType.Walkable;
            }
        }
        catch
        { }
    }
    
}
