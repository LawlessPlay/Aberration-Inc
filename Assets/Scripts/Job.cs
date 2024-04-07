using Assets.Scripts;
using UnityEngine;

public class Job
{
    public Vector2Int gridLocation;
    public Vector2 direction;
    public bool isComplete;
    public bool isOccupied;

    public JobSO scriptableObject;

    public Job(Vector2Int gridLocation, Vector2 direction, bool isComplete, bool isOccupied, JobSO scriptableObject)
    {
        this.gridLocation = gridLocation;
        this.direction = direction;
        this.isComplete = isComplete;
        this.isOccupied = isOccupied;
        this.scriptableObject = scriptableObject;
    }
}
