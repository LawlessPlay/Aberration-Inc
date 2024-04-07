using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourcesController : MonoBehaviour
{
    public static ResourcesController _instance;
    public static ResourcesController Instance { get { return _instance; } }

    public Dictionary<ResourceColors, int> resourceValues = new Dictionary<ResourceColors, int>();

    public List<Job> jobs = new List<Job>();

    public int maxValue = 100;

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            _instance = this;
        }
    }

    public void UpdateResourceValue(ResourceValue resource)
    {
        int currentValue = resourceValues[resource.Color];
        int newValue = currentValue + resource.Value;

        if(newValue < 0)
            newValue = 0;

        resourceValues[resource.Color] = newValue;
    }

    public void AddJob(Job job)
    {
        jobs.Add(job);
    }

    // Start is called before the first frame update
    void Start()
    {
        resourceValues.Add(ResourceColors.Red, 0);
        resourceValues.Add(ResourceColors.Blue, 0);
        resourceValues.Add(ResourceColors.Green, 0);
        resourceValues.Add(ResourceColors.Rest, 0);
    }
}

public class ResourceValue
{
    public ResourceColors Color;
    public int Value;

    public ResourceValue(ResourceColors color, int value)
    {
        Color = color;
        Value = value;
    }
}


public enum ResourceColors
{
    Red,
    Green, 
    Blue,
    Rest
}
