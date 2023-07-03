using UnityEngine;

namespace Simulation.Core
{
    [ExecuteAlways]
    public class Configurator : MonoBehaviour
    {
        public int SimulationDuration { get; private set; } = 1;
        public int MaxMigrationProbes { get; private set; } = 4;
        public int JobArrivalMaxInterval { get; private set; } = 3;

        public int TresholdMax { get { return treshOldMax; } set { if (value < 100 && value > TresholdMin) { treshOldMax = value; } } }
        private int treshOldMax = 95;

        public int TresholdMin { get { return treshOldMin; } set { if (value < TresholdMax && value > 0) { treshOldMin = value; } } }
        private int treshOldMin = 15;


        public static Configurator Instance { get; private set; }
        private void Update()
        {
            if (Instance != null && Instance != this)
            {
                DestroyImmediate(this);
            }
            else
            {
                Instance = this;
            }
        }
        public void SetDuration(float newDuration)
        {
            SimulationDuration = (int)newDuration;
        }
        public void SetProbes(float maxMigrationProbes)
        {
            MaxMigrationProbes = (int)maxMigrationProbes;
        }
        public void SetTresholdMin(float minTreshold)
        {
            TresholdMin = (int)minTreshold;
        }
        public void SetTresholdMax(float maxTreshold)
        {
            TresholdMax = (int)maxTreshold;
        }
        public void OnIntervalChange(float newInterval)
        {
            JobArrivalMaxInterval = (int)newInterval;
        }
    }
}

