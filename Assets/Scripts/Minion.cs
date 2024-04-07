using Assets.Scripts;
using System.Collections.Generic;
using System.Linq;
using System;
using UnityEngine;
using static UnityEngine.EventSystems.EventTrigger;

public class Minion : MonoBehaviour
{
    public enum MinionStates
    {
        LookingForWork,
        LookingForBed,
        Working,
        HeadingToWork,
        GoingToBed,
        Resting
    }

    public MinionStates state;
    public bool stateComplete;

    public PathFinder pathFinder;
    public float speed = 3;
    private List<OverlayTile> path = new List<OverlayTile>();
    public Vector2 facingDirection = new Vector2(0, -1);

    public Animator animator;
    public OverlayTile activeTile;
    private LineRenderer lineRenderer;

    public int MaxEnergy = 100;
    public int Energy;

    public List<Job> possibleJobs = new List<Job>();
    public ActiveJob activeJob;
    public ActiveJob bed;
    private bool isWalking;

    public List<OverlayTile> nodes = new List<OverlayTile>();

    Vector2 direction = new Vector2(0, 0);
    Vector2 pastDirection = new Vector2(0, 0);

    // Start is called before the first frame update
    void Start()
    {
        Energy = MaxEnergy;
        pathFinder = new PathFinder();
        lineRenderer = gameObject.GetComponent<LineRenderer>();
        animator = gameObject.GetComponent<Animator>();

        var locationTile = MapManager.Instance.GetClosestUnoccupiedTile(transform.position);
        PositionCharacterOnTile(locationTile);
        state = MinionStates.LookingForWork;
    }

    // Update is called once per frame
    void Update()
    {
        if (stateComplete)
        {
            state = SetState();
        }

        UpdateState();
    }

    MinionStates SetState()
    {
        stateComplete = false;
        if (activeJob != null && activeJob.occupiedBy == gameObject)
        {
            return MinionStates.Working;
        }

        if (bed != null && bed.occupiedBy == gameObject)
        {
            return MinionStates.Resting;
        }

        if (activeJob != null)
        {
            return MinionStates.HeadingToWork;
        }

        if (bed != null)
        {
            return MinionStates.GoingToBed;
        }

        if (Energy < 15)
        {
            return MinionStates.LookingForBed;
        }

        return MinionStates.LookingForWork;
    }

    void UpdateState()
    {
        switch (state)
        {
            case MinionStates.LookingForWork:
                LookForWork();
                break;
            case MinionStates.Working:
                Work();
                break;
            case MinionStates.HeadingToWork:
                GoingToWork();
                break;
            case MinionStates.LookingForBed:
                LookForBed();
                break;
            case MinionStates.GoingToBed:
                GoingToBed();
                break;
            case MinionStates.Resting:
                Rest();
                break;
        }
    }

    private void LookForBed()
    {
        var newBed = ResourcesController.Instance.jobs.Where(x => !x.isOccupied && x.scriptableObject.color == ResourceColors.Rest).ToList();
        RandomizeList(newBed);

        var bestBed = newBed.FirstOrDefault();


        var IsOccupied = newBed != null ? MapManager.Instance.map[bestBed.gridLocation].IsOccupied : false;
        if (bestBed == null || IsOccupied)
        {
            if (!isWalking)
            {
                var emptyTile = MapManager.Instance.GetRandomUnoccupiedTile();
                GetPath(emptyTile.gridLocation);
            }

            MoveAlongPath();
        }
        else
        {
            bed = new ActiveJob(bestBed, 0);
            isWalking = false;
            foreach (var item in nodes)
            {
                item.HideNode();
            }
            nodes.Clear();
            stateComplete = true;
        }
    }

    private void GoingToBed()
    {
        if (!isWalking)
        {
            GetPath(bed.job.gridLocation);
        }

        MoveAlongPath();

        if (activeTile.gridLocation == bed.job.gridLocation)
        {
            stateComplete = true;
            bed.job.isOccupied = true;
            bed.occupiedBy = gameObject;
        }

        if (bed.job.isOccupied && bed.occupiedBy != gameObject)
        {
            stateComplete = true;
            bed = null;
            isWalking = false;

            foreach (var item in nodes)
            {
                item.HideNode();
            }
            nodes.Clear();
        }
    }

    private void Rest()
    {
        step = step + Time.deltaTime;

        if (step > bed.job.scriptableObject.time)
        {
            step = 0f;
            Energy -= bed.job.scriptableObject.cost;
        }

        if (Energy >= MaxEnergy)
        {
            Energy = MaxEnergy;
            stateComplete = true;
            bed.job.isOccupied = false;
            bed = null;
        }
    }

    private void ClearNodes()
    {
        foreach (var item in nodes)
        {
            item.HideNode();
        }
        nodes.Clear();
    }

    private void LookForWork()
    {
        var highestPrioJob = ResourcesController.Instance.jobs.Where(x => !x.isOccupied && x.scriptableObject.cost < Energy && x.scriptableObject.color != ResourceColors.Rest).ToList();
        RandomizeList(highestPrioJob);
        var bestJob = highestPrioJob.FirstOrDefault();
        var bestValue = int.MaxValue;
        var bestPrio = 0;

        foreach (var item in highestPrioJob)
        {
            if (item.scriptableObject.priority > bestPrio)
            {
                bestPrio = item.scriptableObject.priority;
                bestJob = item;
                //var value = ResourcesController.Instance.resourceValues[item.scriptableObject.color];

                //if (value < bestValue)
                //{
                //    bestValue = value;
                //    bestJob = item;
                //}
            }
        }

        var IsOccupied = bestJob != null ? MapManager.Instance.map[bestJob.gridLocation].IsOccupied : false;

        if (bestJob == null || IsOccupied)
        {
            if (!isWalking)
            {
                var emptyTile = MapManager.Instance.GetRandomUnoccupiedTile();
                GetPath(emptyTile.gridLocation);
            }

            MoveAlongPath();
        }
        else
        {
            activeJob = new ActiveJob(bestJob, 0);
            isWalking = false;
            foreach (var item in nodes)
            {
                item.HideNode();
            }
            nodes.Clear();
            stateComplete = true;
        }
    }

    private void GoingToWork()
    {
        if (activeJob == null)
        {
            stateComplete = true;
            isWalking = false;

            foreach (var item in nodes)
            {
                item.HideNode();
            }
            nodes.Clear();

            return;
        }

        if (!isWalking)
        {
            GetPath(activeJob.job.gridLocation);
        }

        MoveAlongPath();

        if (activeTile.gridLocation == activeJob.job.gridLocation)
        {
            stateComplete = true;
            activeJob.job.isOccupied = true;
            activeJob.occupiedBy = gameObject;
        }

        if (activeJob.job.isOccupied && activeJob.occupiedBy != gameObject)
        {
            stateComplete = true;
            activeJob = null;
            isWalking = false;

            foreach (var item in nodes)
            {
                item.HideNode();
            }
            nodes.Clear();
        }
    }

    private void GetPath(Vector2Int gridLocation)
    {
        path = pathFinder.FindPath(MapManager.Instance.GetClosestUnoccupiedTile(transform.position), MapManager.Instance.map[gridLocation], new List<OverlayTile>());
        if (path.Count > 0)
        {
            path[path.Count - 1].ShowNode();
            nodes.Add(path[path.Count - 1]);
            isWalking = true;
        }
    }

    private void MoveAlongPath()
    {
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
                    nodes.Add(path[i]);
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

            var step = speed * Time.deltaTime;
            transform.position = Vector3.MoveTowards(transform.position, path[0].transform.position, step);

            if (Vector3.Distance(transform.position, path[0].transform.position) < 0.0001f)
            {
                //second last tile
                if (path.Count >= 2)
                {
                    facingDirection = new Vector2(path[1].gridLocation.x - path[0].gridLocation.x, path[1].gridLocation.y - path[0].gridLocation.y);
                    SetDirection(facingDirection);
                }

                //last tile
                if (path.Count == 1)
                {
                    PositionCharacterOnTile(path[0]);
                    isWalking = false;
                }

                path[0].HideNode();
                path.RemoveAt(0);
            }
        }
    }

    float step = 0f;
    private void Work()
    {
        step = step + Time.deltaTime;

        if (step > activeJob.job.scriptableObject.time)
        {
            activeJob.FinishJob();
            step = 0f;
            Energy -= activeJob.job.scriptableObject.cost;
        }

        if (activeJob.job.scriptableObject.cost > Energy)
        {
            stateComplete = true;
            activeJob.job.isOccupied = false;
            activeJob = null;
        }
    }

    private void SetDirection(Vector2 dir)
    {
        facingDirection = dir;
        animator.SetFloat("xDir", facingDirection.x);
        animator.SetFloat("yDir", facingDirection.y);
    }

    public void PositionCharacterOnTile(OverlayTile tile)
    {
        if (tile != null)
        {
            transform.position = new Vector3(tile.transform.position.x, tile.transform.position.y + 0.0001f, tile.transform.position.z);

            if (activeTile)
                activeTile.IsOccupied = false;

            tile.IsOccupied = true;
            activeTile = tile;
        }
    }

    void RandomizeList(List<Job> list)
    {
        System.Random random = new System.Random();

        int n = list.Count;
        for (int i = n - 1; i > 0; i--)
        {
            // Generate a random index between 0 and i (inclusive)
            int randIndex = random.Next(0, i + 1);

            // Swap the elements at randIndex and i
            Job temp = list[i];
            list[i] = list[randIndex];
            list[randIndex] = temp;
        }
    }
}