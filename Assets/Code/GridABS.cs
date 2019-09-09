using System.Collections.Generic;
using UnityEngine;

public class GridABS : MonoBehaviour {

    public static int ArenaSizeX = 16, ArenaSizeY = 8;
    public static NodeA[, ] GridOfArena;
    public static NodeA lastNodeA;

    static public List<Shelter> ShelterList = new List<Shelter> ();

    public static void CreateGrid () {
        GridOfArena = new NodeA[ArenaSizeX, ArenaSizeY];
        Texture2D Map = Resources.LoadAll<Texture2D> ("Marcer") [0];
        Transform MiniNodaLevel = Resources.LoadAll<Transform> ("Marcer") [0];

        Vector3 BotomLeft = Vector3.zero - Vector3.right * (ArenaSizeX / 2) - Vector3.forward * (ArenaSizeY / 2);
        for (int x = 0; x < ArenaSizeX; x++)
            for (int y = 0; y < ArenaSizeY; y++) {

                if (Map.GetPixel (x, y) == Color.green || Map.GetPixel (x, y) == Color.blue) {
                    GridOfArena[x, y] = new NodeA (BotomLeft + Vector3.right * x + Vector3.forward * y, x, y, NodeA.NodeType.Walkable);
                    Vector3 positionofnode = (BotomLeft + Vector3.right * x + Vector3.forward * y);
                    Transform node = Instantiate (MiniNodaLevel, positionofnode, Quaternion.identity).transform;
                    node.eulerAngles = new Vector3 (90, 0, 0);
                    node.localScale *= 2f;
                }
                else GridOfArena[x, y] = new NodeA (BotomLeft + Vector3.right * x + Vector3.forward * y, x, y, NodeA.NodeType.Wall);
            }

        foreach (Shelter n in ShelterList) {
            n.SetSelterBlock ();
        }
    }

    public static NodeA NodeFromWorldPoint (Vector3 worldPosition) {
        try {
            float percentX = (worldPosition.x + ArenaSizeX / 2) / ArenaSizeX;
            float percentY = (worldPosition.z + ArenaSizeY / 2) / ArenaSizeY;
            percentX = Mathf.Clamp01 (percentX);
            percentY = Mathf.Clamp01 (percentY);

            int x = Mathf.RoundToInt ((ArenaSizeX) * percentX);
            int y = Mathf.RoundToInt ((ArenaSizeY) * percentY);
            lastNodeA = GridOfArena[x, y];
            return GridOfArena[x, y];
        } catch {
            return lastNodeA;

        }

    }

    public static void NodeCoordinat (Vector3 worldPosition, out int x, out int y) {
        float percentX = (worldPosition.x + ArenaSizeX / 2) / ArenaSizeX;
        float percentY = (worldPosition.z + ArenaSizeY / 2) / ArenaSizeY;
        percentX = Mathf.Clamp01 (percentX);
        percentY = Mathf.Clamp01 (percentY);

        x = Mathf.RoundToInt ((ArenaSizeX) * percentX);
        y = Mathf.RoundToInt ((ArenaSizeY) * percentY);

    }

}
public class NodeA {
    public int x, y;
    public Vector3 Position;
    public Vector3 UnitPosition {
        get { return Position + Vector3.right * .25f + Vector3.back * .1f; }
    }
    public enum NodeType {
        Walkable,
        Wall,
        Shelter
    }
    public NodeType TypeBloc;

    NetworkElement iUnitOnNode;
    public NetworkElement UnitOnNode {
        get {
            return iUnitOnNode;
        }

        set {

            if (value != null) {
                iUnitOnNode = value;
                value.CreateMarker (Position);
            } else
                iUnitOnNode = null;

        }
    }

    public NodeA (Vector3 position, int xCor, int yCor, NodeType type) {
        TypeBloc = type;
        x = xCor;
        y = yCor;
        Position = position;
        iUnitOnNode = null;
    }

}