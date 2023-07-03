using System.Collections.Generic;
using UnityEngine;

namespace Simulation.Core.Processing
{
    class ProcessorSenderMax : Processor
    {
        public override void SpawnJob(Job arrivingJob)
        {
            if (arrivingJob.ProcessorLoad + ProcessorLoad > Configurator.Instance.TresholdMax)
            {
                List<int> randomProcessors = new List<int>();
                for (int i = 0; i < ProcessorsManager.Instance.NumberOfProcessors; i++)
                {
                    randomProcessors.Add(i);
                }

                for (int i = 0; i < Mathf.Min(Configurator.Instance.MaxMigrationProbes, ProcessorsManager.Instance.NumberOfProcessors); i++)
                {
                    int randomIndex = UnityEngine.Random.Range(0, randomProcessors.Count);
                    ProcessorContainer migrationTarget = ProcessorsManager.Instance.GetProcessorByIndex(randomProcessors[randomIndex]).GetComponent<ProcessorContainer>();
                    Recorder.Instance.IncrementQueries();

                    if (migrationTarget.ContainedProcessor.ProcessorLoad < Configurator.Instance.TresholdMin)
                    {
                        MigrateDuck(migrationTarget.transform);
                        migrationTarget.ContainedProcessor.ReceiveMigration(arrivingJob);
                        return;
                    }
                    randomProcessors.RemoveAt(randomIndex);
                }
                postponedQueue.Add(arrivingJob);
            }
            else
            {
                StartJob(arrivingJob);
            }
        }
    }
}
