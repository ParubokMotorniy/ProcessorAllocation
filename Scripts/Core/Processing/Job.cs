namespace Simulation.Core.Processing
{
    public class Job
    {
        public uint ExecutionTime { get; private set; }
        public float ExecutionProgression { get; private set; }
        public uint ProcessorLoad { get; private set; }

        public Job(uint executionTime, uint processorLoad)
        {
            ExecutionTime = executionTime;
            ProcessorLoad = processorLoad;
        }

        public void Execute(float executionTimeFraction)
        {
            ExecutionProgression += executionTimeFraction;
        }
    }

}
