using UnityEngine;

namespace Simulation.Core
{
    public abstract class SimulationSupervisorWrapper : MonoBehaviour, ISimulationSupervisor
    {
        public abstract void OnSimulationStart();
        public abstract void OnSimulationFinish();
    }
}