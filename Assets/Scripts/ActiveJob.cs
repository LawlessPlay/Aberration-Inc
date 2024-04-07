using System.Collections;
using UnityEngine;

namespace Assets.Scripts
{
    public class ActiveJob 
    {
        public Job job;
        public int timesCompleted;
        public GameObject occupiedBy;

        public ActiveJob(Job job, int timesCompleted)
        {
            this.job = job;
            this.timesCompleted = timesCompleted;
        }

        public void FinishJob()
        {
            if (job != null)
            {
                ResourcesController.Instance.UpdateResourceValue(new ResourceValue(job.scriptableObject.color, job.scriptableObject.value));
                timesCompleted++;
            }
        }
    }
}