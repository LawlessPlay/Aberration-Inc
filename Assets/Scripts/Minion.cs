using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using static UnityEngine.EventSystems.EventTrigger;

public class Minion : MonoBehaviour
{
    public PathFinder pathFinder;
    public float speed = 3;
    private List<OverlayTile> path = new List<OverlayTile>();
    public Vector2 facingDirection = new Vector2(0, -1);

    public Animator animator;
    public OverlayTile activeTile;
    private LineRenderer lineRenderer;
    // Start is called before the first frame update
    void Start()
    {
        pathFinder = new PathFinder();
        lineRenderer = gameObject.GetComponent<LineRenderer>();
        animator = gameObject.GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButton(0))
        {
            PositionCharacterOnTile(MapManager.Instance.map[new Vector2Int(-10, 10)]);
            path = pathFinder.FindPath(activeTile, MapManager.Instance.map[new Vector2Int(0, -10)], new List<OverlayTile>());
            path[path.Count-1].ShowNode();
        }

        if (path.Count > 0)
        {
            lineRenderer.positionCount = path.Count;
            for (int i = 0; i < path.Count; i++)
            {
                if (i != path.Count - 1)
                {
                    direction = new Vector2(path[i + 1].gridLocation.x - path[i].gridLocation.x, path[i + 1].gridLocation.y - path[i].gridLocation.y);
                }

                if (i != 0 && direction != pastDirection)
                {
                    path[i].ShowNode();
                }

                if (i == 0)
                {
                    lineRenderer.SetPosition(0, transform.position);
                }
                else
                {
                    lineRenderer.SetPosition(i, path[i].transform.position);
                }
                pastDirection = direction;
            }

            MoveAlongPath();
        }
    }

    Vector2 direction = new Vector2(0,0);
    Vector2 pastDirection = new Vector2(0, 0);

    private void MoveAlongPath()
    {
        var step = speed * Time.deltaTime;
        transform.position = Vector3.MoveTowards(transform.position, path[0].transform.position, step);

        if (Vector3.Distance(transform.position, path[0].transform.position) < 0.0001f)
        {
            //second last tile
            if (path.Count >= 2)
            {
                facingDirection = new Vector2(path[1].gridLocation.x - path[0].gridLocation.x, path[1].gridLocation.y - path[0].gridLocation.y);
                animator.SetFloat("xDir", facingDirection.x);
                animator.SetFloat("yDir", facingDirection.y);
            }

            //last tile
            if (path.Count == 1)
                PositionCharacterOnTile(path[0]);

            path[0].HideNode();
            path.RemoveAt(0);
        }
    }

    public void PositionCharacterOnTile(OverlayTile tile)
    {
        if (tile != null)
        {
            transform.position = new Vector3(tile.transform.position.x, tile.transform.position.y + 0.0001f, tile.transform.position.z);
            
            if(activeTile)
                activeTile.IsOccupied = false;
            
            tile.IsOccupied = true;
            activeTile = tile;
        }
    }
}
