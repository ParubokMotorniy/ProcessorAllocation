using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Simulation.Core.Processing;
using UnityEngine;

namespace Simulation.Core
{
    [ExecuteAlways]
    public class Recorder : SimulationSupervisorWrapper
    {
        public static Recorder Instance { get; private set; }
        [SerializeField] private int measurementInterval;
        [SerializeField] private float fadeTime;
        [SerializeField] TMPro.TMP_Text algorithmName;
        [SerializeField] GameObject finishMessage;

        private int totalQueries;
        private int totalMigrations;
        private List<LoadStatistics> statistics = new List<LoadStatistics>();
        private float startTime = 0;

        public void IncrementQueries()
        {
            totalQueries++;
        }
        public void IncrementMigrations()
        {
            totalMigrations++;
        }

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

        private IEnumerator NextMeasurement()
        {
            WaitForSeconds measurementTimer = new WaitForSeconds(measurementInterval);
            while (true)
            {
                yield return measurementTimer;
                CollectLoads();
            }
        }
        private IEnumerator ShowMessage()
        {
            yield return StartCoroutine(ScaleMessage(1));
            yield return new WaitForSeconds(2f);
            StartCoroutine(ScaleMessage(0));
        }
        private IEnumerator ScaleMessage(int target)
        {
            RectTransform message = finishMessage.GetComponent<RectTransform>();
            Vector3 targetScale = new Vector3(target, target, message.localScale.z);
            float scaleDelta = Mathf.Abs(target - message.localScale.x) / fadeTime;
            while (targetScale != message.localScale)
            {
                message.localScale = Vector3.MoveTowards(message.localScale, targetScale, scaleDelta * Time.deltaTime);
                yield return null;
            }
        }
        private void CollectLoads()
        {
            int loadSum = 0;
            foreach (GameObject processor in ProcessorsManager.Instance)
            {
                loadSum += (int)processor.GetComponent<ProcessorContainer>().ContainedProcessor.ProcessorLoad;
            }
            float average = loadSum / ProcessorsManager.Instance.NumberOfProcessors;

            float deviationSquared = 0;
            foreach (GameObject processor in ProcessorsManager.Instance)
            {
                deviationSquared += Mathf.Pow((processor.GetComponent<ProcessorContainer>().ContainedProcessor.ProcessorLoad - average), 2);
            }
            deviationSquared /= ProcessorsManager.Instance.NumberOfProcessors;
            deviationSquared = Mathf.Sqrt(deviationSquared);

            statistics.Add(new LoadStatistics(TrimFloat(average), TrimFloat(deviationSquared), TrimFloat(Time.time - startTime)));
        }

        override public void OnSimulationStart()
        {
            startTime = Time.time;
            totalMigrations = 0;
            totalQueries = 0;
            StartCoroutine(NextMeasurement());
        }

        [ContextMenu("Menu/Func")]
        override public void OnSimulationFinish()
        {
            StopAllCoroutines();
            StartCoroutine(ShowMessage());

            float totalAverage = 0;
            foreach (LoadStatistics entry in statistics)
            {
                totalAverage += entry.AverageLoad;
            }
            totalAverage /= statistics.Count;

            string data = JsonUtility.ToJson(new StatisticsOutputWrapper(statistics, algorithmName.text, totalQueries, TrimFloat(totalAverage), totalMigrations), true);
            DumpStats(Path.Combine(Environment.GetFolderPath(System.Environment.SpecialFolder.DesktopDirectory), "LoadDistributionSimulationOutput"), algorithmName.text, data);
        }
        private float TrimFloat(float originalFloat)
        {
            return float.Parse(originalFloat.ToString("0.##"));
        }
        private void DumpStats(string dataOutputPath, string fileName, string statsString)
        {
            if (!Directory.Exists(dataOutputPath))
            {
                Directory.CreateDirectory(dataOutputPath);
            }

            string rootName = string.Concat(fileName, ".json");
            int maxIndex = -1;

            foreach (string file in Directory.GetFiles(dataOutputPath))
            {
                string inspectedFilename = Path.GetFileName(file);
                if (inspectedFilename.Contains(rootName))
                {
                    int index = int.Parse(inspectedFilename.Substring(0, inspectedFilename.Length - rootName.Length));
                    if (index > maxIndex)
                    {
                        maxIndex = index;
                    }
                }
            }
            string outputPath = Path.Combine(dataOutputPath, string.Concat((maxIndex + 1).ToString(), rootName));
            using (FileStream newOutput = File.Create(outputPath)) { }
            using (StreamWriter sw = new StreamWriter(outputPath))
            {
                sw.WriteLine(statsString);
            }
        }

        [System.Serializable]
        private class StatisticsOutputWrapper
        {
            [SerializeField] private List<LoadStatistics> loadsRecordings;
            [SerializeField] private string algorithmName;
            [SerializeField] private int numOfQueries;
            [SerializeField] private int numOfMigartions;
            [SerializeField] private float finalAverageLoad;

            public StatisticsOutputWrapper(List<LoadStatistics> loadsRecordings, string algorithmName, int numOfQueries, float finalAverageLoad, int numOfMigartions)
            {
                this.loadsRecordings = loadsRecordings;
                this.algorithmName = algorithmName;
                this.numOfQueries = numOfQueries;
                this.finalAverageLoad = float.Parse(finalAverageLoad.ToString("0.00"));
                this.numOfMigartions = numOfMigartions;
            }
        }


        [System.Serializable]
        private class LoadStatistics
        {
            [SerializeField]
            private float timeStamp;
            [SerializeField]
            private float averageLoad;
            [SerializeField]
            private float stdDeviation;
            public float StandardDeviation { get => stdDeviation; set => stdDeviation = value; }
            public float AverageLoad { get => averageLoad; set => averageLoad = value; }
            public float TimeStamp { get => timeStamp; set => timeStamp = value; }

            public LoadStatistics(float mean, float stdDeviation, float timeStamp)
            {
                TimeStamp = timeStamp;
                StandardDeviation = stdDeviation;
                AverageLoad = mean;
            }
        }
    }
}