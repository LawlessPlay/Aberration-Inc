using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class MapManager : MonoBehaviour
{
    public static MapManager _instance;
    public static MapManager Instance { get { return _instance; } }
    public Tilemap tileMap;
    public OverlayTile overTilePrefab;
    public GameObject overlayContainer;

    public Dictionary<Vector2Int, OverlayTile> map;

    private void Awake()
    {
        if(_instance != null && _instance != this)
        {
            Destroy(gameObject);
        } else
        {
            _instance = this;
        }
    }


    // Start is called before the first frame update
    void Start()
    {
        SetMap();
    }

    public void SetMap()
    {
        map = new Dictionary<Vector2Int, OverlayTile>();
        BoundsInt bounds = tileMap.cellBounds;
        
        for (int x = bounds.xMin; x <= bounds.xMax; x++)
        {
            for (int y = bounds.yMin; y <= bounds.yMax; y++)
            {
                var tileLocation = new Vector3Int(x, y, 0);
                var gridLocation = new Vector2Int(x, y);
                if (tileMap.HasTile(tileLocation))
                {
                    var overlayTile = Instantiate(overTilePrefab, overlayContainer.transform);
                    var cellLocation = tileMap.GetCellCenterWorld(tileLocation);
                    overlayTile.transform.localPosition = cellLocation;
                    overlayTile.gridLocation = gridLocation;

                    map.Add(gridLocation, overlayTile);
                }
            }
        }
    }
}
