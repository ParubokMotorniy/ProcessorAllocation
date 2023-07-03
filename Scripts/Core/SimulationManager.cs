using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Simulation.Core
{
    [ExecuteAlways]
    public class SimulationManager : MonoBehaviour
    {
        [SerializeField] private List<Selectable> uiToControl;
        [SerializeField] private List<SimulationSupervisorWrapper> orderedSupervisors = new List<SimulationSupervisorWrapper>();
        public uint NumberOfJobs { get; private set; } = 300;
        public static SimulationManager Instance { get; private set; }

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
        public void StartSimulation()
        {
            if (ProcessorsManager.Instance.NumberOfProcessors > 0)
            {
                foreach (Selectable uiElem in uiToControl)
                {
                    uiElem.interactable = false;
                }
                foreach (ISimulationSupervisor supervisor in orderedSupervisors)
                {
                    supervisor.OnSimulationStart();
                }
                Invoke("FinishSimulation", Configurator.Instance.SimulationDuration * 60);
            }
        }
        public void FinishSimulation()
        {
            foreach (ISimulationSupervisor supervisor in orderedSupervisors)
            {
                supervisor.OnSimulationFinish();
            }
            foreach (Selectable uiElem in uiToControl)
            {
                uiElem.interactable = true;
            }
        }
        public void SetJobs(float jobsCount)
        {
            NumberOfJobs = (uint)jobsCount;
        }
    }
}