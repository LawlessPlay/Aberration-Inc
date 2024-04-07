using System.Collections;
using UnityEngine;

namespace Assets.Scripts
{
    [CreateAssetMenu]
    public class JobSO : ScriptableObject
    {
        public ResourceColors color;
        public int priority;
        public int time;
        public int value;
        public int cost;
    }
}