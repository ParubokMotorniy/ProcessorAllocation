namespace Simulation.Core
{
    interface ISimulationSupervisor
    {
        public void OnSimulationStart();
        public void OnSimulationFinish();
    }
}
