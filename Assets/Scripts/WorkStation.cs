using Assets.Scripts;
using System.Collections.Generic;
using UnityEngine;

public class WorkStation : MonoBehaviour
{
    public List<OverlayTile> activeTiles;
    public Vector2 size;

    public GameObject location;
    public Vector2 direction;
    public JobSO _job;

    // Start is called before the first frame update
    void Start()
    {
        SetupWorkStation();
    }

    private void SetupWorkStation()
    {
        activeTiles = new List<OverlayTile>();
        var closestTile = MapManager.Instance.GetClosestUnoccupiedTile(transform.position);
        PositionObjectOnTiles(closestTile, transform);

        var locationTile = MapManager.Instance.GetClosestUnoccupiedTile(location.transform.position);
        PositionLocationOnTile(locationTile, location.transform);

        var newJob = new Job(locationTile.gridLocation, direction, false, false, _job);
        ResourcesController.Instance.AddJob(newJob);
    }
    public void PositionLocationOnTile(OverlayTile originTile, Transform _transform)
    {
        if (originTile != null)
        {
            _transform.position = new Vector3(originTile.transform.position.x, originTile.transform.position.y + 0.0001f, originTile.transform.position.z);
            var map = MapManager.Instance.map;
            var gridLocation = new Vector2Int(originTile.gridLocation.x, originTile.gridLocation.y);
            if (map.ContainsKey(gridLocation))
            {
                var otherTile = MapManager.Instance.map[new Vector2Int(originTile.gridLocation.x, originTile.gridLocation.y)];
                activeTiles.Add(otherTile);
            }
        }
    }

    public void PositionObjectOnTiles(OverlayTile originTile, Transform _transform)
    {
        if (originTile != null)
        {
            _transform.position = new Vector3(originTile.transform.position.x, originTile.transform.position.y + 0.0001f, originTile.transform.position.z);


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
