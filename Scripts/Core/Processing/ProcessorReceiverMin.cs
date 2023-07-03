using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Simulation.Core.Processing
{
    class ProcessorReceiverMin : Processor
    {
        public override void SpawnJob(Job arrivingJob)
        {
            if ((arrivingJob.ProcessorLoad + ProcessorLoad) > 100)
            {
                postponedQueue.Add(arrivingJob);
            }
            else
            {
                StartJob(arrivingJob);
            }
        }
        public override void OnSimulationStarted()
        {
            base.OnSimulationStarted();
            parent.StartCoroutine(LateUpdate());
        }
        private IEnumerator LateUpdate()
        {
            while (true)
            {
                if (ProcessorLoad < Configurator.Instance.TresholdMin)
                {
                    LookForJob();
                }
                yield return null;
            }
        }
        private void LookForJob()
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

                List<Job> migratedWork = migrationTarget.ContainedProcessor.ShareWork((int)(Configurator.Instance.TresholdMax - ProcessorLoad));
                if (migratedWork.Count > 0)
                {
                    parent.StartCoroutine(MigrateDucks(migratedWork.Count, migrationTarget.transform));
                    foreach (Job job in migratedWork)
                    {
                        Debug.Log("Receiving work from" + migrationTarget.name);
                        ReceiveMigration(job);
                    }
                    return;
                }
                randomProcessors.RemoveAt(randomIndex);
            }
        }

        private IEnumerator MigrateDucks(int numOfDucks, Transform target)
        {
            WaitForSeconds duckInterval = new WaitForSeconds(0.75f);
            while (numOfDucks > 0)
            {
                MigrateDuck(target);
                numOfDucks--;
                yield return duckInterval;
            }
        }
        public override void ReceiveMigration(Job jobToMigrate)
        {
            base.ReceiveMigration(jobToMigrate);
            StartJob(jobToMigrate);
        }

        public override List<Job> ShareWork(int loadRoom)
        {
            List<Job> migratedWork = new List<Job>();

            if (ProcessorLoad > Configurator.Instance.TresholdMax)
            {
                Comparison<Job> jobComparer = (job1, job2) => { if ((job1.ExecutionProgression / job1.ExecutionTime) < (job2.ExecutionProgression / job2.ExecutionTime)) { return -1; } return 1; };
                activeJobs.Sort(jobComparer);
                List<Job> updatedActive = new List<Job>();
                foreach (Job job in activeJobs)
                {
                    if (loadRoom - job.ProcessorLoad >= 0)
                    {
                        migratedWork.Add(job);
                        loadRoom -= (int)job.ProcessorLoad;
                        FinishJob(job);
                    }
                    else
                    {
                        updatedActive.Add(job);
                    }
                }
                activeJobs = updatedActive;
            }
            return migratedWork;
        }
    }
}
