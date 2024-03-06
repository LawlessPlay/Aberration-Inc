using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class AddToGrid : MonoBehaviour
{
    public List<OverlayTile> activeTiles;
    public Vector2 size;

    // Start is called before the first frame update
    void Start()
    {
        activeTiles = new List<OverlayTile>();
        var closestTile = MapManager.Instance.GetClosestUnoccupiedTile(transform.position);
        PositionObjectOnTiles(closestTile);
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void PositionObjectOnTiles(OverlayTile originTile)
    {
        if (originTile != null)
        {
            transform.position = new Vector3(originTile.transform.position.x, originTile.transform.position.y + 0.0001f, originTile.transform.position.z);


            for (int i = 0; i < size.x; i++)
            {
                for (int j = 0; j < size.y; j++)
                {
                    var map = MapManager.Instance.map;
                    var gridLocation = new Vector2Int(originTile.gridLocation.x + i, originTile.gridLocation.y + j);
                    if (map.ContainsKey(gridLocation))
                    {
                        var otherTile = MapManager.Instance.map[new Vector2Int(originTile.gridLocation.x + i, originTile.gridLocation.y + j)];
                        otherTile.IsOccupied = true;
                        activeTiles.Add(otherTile);
                    }
                }
            }
        }
    }
}
