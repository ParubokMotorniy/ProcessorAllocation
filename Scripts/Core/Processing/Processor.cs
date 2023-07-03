using System.Collections;
using System.Collections.Generic;
using Assets.Scripts.Optimization;
using Simulation.Misc;
using UnityEngine;

namespace Simulation.Core.Processing
{
    public abstract class Processor
    {
        public uint ProcessorLoad { get { return processorLoad; } protected set { parent.LoadBar.value = value; processorLoad = value; } }
        protected List<Job> postponedQueue = new List<Job>();
        protected List<Job> activeJobs = new List<Job>();

        protected ProcessorContainer parent;
        protected uint processorLoad = 0;

        public void InitializeProcessor(ProcessorContainer parent)
        {
            this.parent = parent;
        }
        public void ResetProcessor()
        {
            postponedQueue = new List<Job>();
            activeJobs = new List<Job>();
        }
        public abstract void SpawnJob(Job arrivingJob);
        public virtual void ReceiveMigration(Job jobToMigrate)
        {
            Recorder.Instance.IncrementMigrations();
            if ((jobToMigrate.ProcessorLoad + ProcessorLoad) > 100)
            {
                postponedQueue.Add(jobToMigrate);
            }
            else
            {
                StartJob(jobToMigrate);
            }
        }

        protected virtual void StartJob(Job jobToStart)
        {
            activeJobs.Add(jobToStart);
            ProcessorLoad += jobToStart.ProcessorLoad;
        }
        protected virtual void FinishJob(Job jobToFinish)
        {
            ProcessorLoad -= jobToFinish.ProcessorLoad;
        }
        protected IEnumerator JobsGenerator()
        {
            while (true)
            {
                float arrivalInterval = Random.Range(0f, Configurator.Instance.JobArrivalMaxInterval);
                yield return new WaitForSeconds(arrivalInterval);

                uint execution = (uint)Random.Range(1, 6);
                uint load = (uint)Random.Range(5, 31);
                Job newJob = new Job(execution, load);

                SpawnJob(newJob);
            }
        }

        public virtual void OnSimulationStarted()
        {
            parent.StartCoroutine(JobsGenerator());
        }
        public virtual void OnSimulationFinished()
        {
            ProcessorLoad = 0;
            parent.StopAllCoroutines();
        }
        public virtual void Update()
        {
            List<Job> updatedQueue = new List<Job>();
            foreach (Job job in postponedQueue)
            {
                if (job.ProcessorLoad + ProcessorLoad <= 100)
                {
                    StartJob(job);
                }
                else
                {
                    updatedQueue.Add(job);
                }
            }
            postponedQueue = updatedQueue;

            List<Job> updatedActive = new List<Job>();
            foreach (Job job in activeJobs)
            {
                job.Execute(Time.deltaTime);
                if (job.ExecutionProgression >= job.ExecutionTime)
                {
                    FinishJob(job);
                }
                else
                {
                    updatedActive.Add(job);
                }
            }
            activeJobs = updatedActive;
        }

        public virtual List<Job> ShareWork(int loadRoom) { return null; }
        protected void MigrateDuck(Transform target)
        {
            GameObject duck = ObjectPooling.Instance.SpawnFromPool("Duck", parent.transform.position, Quaternion.LookRotation(target.position - parent.transform.position, Vector3.up));
            DuckController duckController = duck.GetComponent<DuckController>();
            duckController.StartCoroutine(duckController.Migrate(parent.transform, target));
        }
    }
}