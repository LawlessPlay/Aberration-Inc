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
    public MouseController mouseController;

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
                    mouseController.AddObserver(overlayTile);
                    overlayTile._tileSubject = mouseController;
                    map.Add(gridLocation, overlayTile);
                }
            }
        }
    }

    public List<OverlayTile> GetNeighbourTiles(OverlayTile currentOverlayTile, List<OverlayTile> searchableTiles)
    {
        Dictionary<Vector2Int, OverlayTile> tileToSearch = new Dictionary<Vector2Int, OverlayTile>();

        if (searchableTiles.Count > 0)
        {
            foreach (var item in searchableTiles)
            {
                tileToSearch.Add(item.gridLocation, item);
            }
        }
        else
        {
            tileToSearch = map;
        }

        List<OverlayTile> neighbours = new List<OverlayTile>();
        if (currentOverlayTile != null)
        {
            foreach (var direction in GetDirections())
            {
                Vector2Int locationToCheck = currentOverlayTile.gridLocation + direction;
                ValidateNeighbour(tileToSearch, neighbours, locationToCheck);
            }
        }

        return neighbours;
    }

    //Check the neighbouring tile is valid.
    private static void ValidateNeighbour(Dictionary<Vector2Int, OverlayTile> tilesToSearch, List<OverlayTile> neighbours, Vector2Int locationToCheck)
    {
        bool canAccessLocation = false;

        if (tilesToSearch.ContainsKey(locationToCheck))
        {
            OverlayTile tile = tilesToSearch[locationToCheck];
            bool isBlocked = tile.IsOccupied;

            if (!isBlocked)
            {
                canAccessLocation = true;
            }

            if (canAccessLocation)
            {
                //artificial jump height. 
                    neighbours.Add(tilesToSearch[locationToCheck]);
            }
        }
    }

    public OverlayTile GetClosestUnoccupiedTile(Vector3 position)
    {
        var gridPosition = tileMap.WorldToCell(position);
        return map[new Vector2Int(gridPosition.x, gridPosition.y)];
    }

    private IEnumerable<Vector2Int> GetDirections()
    {
        yield return Vector2Int.up;
        yield return Vector2Int.down;
        yield return Vector2Int.right;
        yield return Vector2Int.left;
        yield return new Vector2Int(1, 1);
        yield return new Vector2Int(-1, 1);
        yield return new Vector2Int(1, -1);
        yield return new Vector2Int(-1, -1);
    }
}
