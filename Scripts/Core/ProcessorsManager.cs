using System;
using System.Collections;
using Simulation.Core.Processing;
using UnityEngine;

namespace Simulation.Core
{
    public class ProcessorsManager : SimulationSupervisorWrapper, IEnumerable
    {
        public int NumberOfProcessors { get; private set; } = 0;
        [SerializeField] private GameObject processorPrefab;
        [SerializeField] private int fieldSize;
        [SerializeField] private TMPro.TMP_Dropdown modeSelector;

        [SerializeField] private Material processorMaterialDisabled;
        [SerializeField] private Material processorMaterialEnabled;
        public static ProcessorsManager Instance;
        private int previousNumberOfProcessors = 0;
        private GameObject[] pooledProcessors = new GameObject[100];
        void Awake()
        {
            int posDelta = fieldSize / 9;
            int posX = fieldSize / 2;
            for (int i = 0; i < 100; i += 10)
            {
                int posZ = fieldSize / 2;
                for (int j = 0; j < 10; j++)
                {
                    GameObject newProcessor = GameObject.Instantiate(processorPrefab, new Vector3(posX, 0, posZ), Quaternion.identity);
                    pooledProcessors[i + j] = newProcessor;
                    newProcessor.GetComponent<MeshRenderer>().material = processorMaterialDisabled;
                    newProcessor.SetActive(false);

                    posZ -= posDelta;
                }
                posX -= posDelta;
            }
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

        public void OnProcessorsChange(float newNumberOfProcessors)
        {
            int numberDelta = (int)newNumberOfProcessors - previousNumberOfProcessors;
            if (numberDelta > 0)
            {
                for (int i = previousNumberOfProcessors; i < newNumberOfProcessors; i++)
                {
                    pooledProcessors[i].SetActive(true);
                }
            }
            else if (numberDelta < 0)
            {
                for (int i = (previousNumberOfProcessors - 1); i >= newNumberOfProcessors; i--)
                {
                    pooledProcessors[i].SetActive(false);
                }
            }
            previousNumberOfProcessors = (int)newNumberOfProcessors;
        }

        override public void OnSimulationStart()
        {
            switch (modeSelector.value)
            {
                case 0:
                    WireUpProcessors<ProcessorRandom>();
                    break;
                case 1:
                    WireUpProcessors<ProcessorSenderMax>();
                    break;
                case 2:
                    WireUpProcessors<ProcessorReceiverMin>();
                    break;
            }
        }
        private void WireUpProcessors<T>() where T : Processor, new()
        {
            for (int i = 0; i < NumberOfProcessors; i++)
            {
                ProcessorContainer processorContainer = pooledProcessors[i].GetComponent<ProcessorContainer>();

                T newProcessor = new T();
                processorContainer.AssignProcessor(newProcessor);

                newProcessor.InitializeProcessor(processorContainer);

                processorContainer.enabled = true;
                pooledProcessors[i].GetComponent<MeshRenderer>().material = processorMaterialEnabled;
            }
            for (int i = 0; i < NumberOfProcessors; i++)
            {
                pooledProcessors[i].GetComponent<ProcessorContainer>().ContainedProcessor.OnSimulationStarted();
            }
        }

        override public void OnSimulationFinish()
        {
            for (int i = 0; i < NumberOfProcessors; i++)
            {
                ProcessorContainer processorContainer = pooledProcessors[i].GetComponent<ProcessorContainer>();
                processorContainer.ContainedProcessor.OnSimulationFinished();

                processorContainer.enabled = false;
                pooledProcessors[i].GetComponent<MeshRenderer>().material = processorMaterialDisabled;
            }
        }

        public void SetProcessors(float numberOfProcessors)
        {
            NumberOfProcessors = (int)numberOfProcessors;
        }

        public IEnumerator GetEnumerator()
        {
            return new ActiveProcessorsEnumerator(pooledProcessors);
        }
        private class ActiveProcessorsEnumerator : IEnumerator
        {
            private int inspectedProcessor = -1;
            private GameObject[] pooledProcessors;
            public ActiveProcessorsEnumerator(GameObject[] pooledProcessors)
            {
                this.pooledProcessors = pooledProcessors;
            }
            public bool MoveNext()
            {
                if ((++inspectedProcessor) < Instance.NumberOfProcessors)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }

            public void Reset()
            {
                inspectedProcessor = 0;
            }

            public object Current
            {
                get
                {
                    try
                    {
                        return pooledProcessors[inspectedProcessor];
                    }
                    catch
                    {
                        throw new IndexOutOfRangeException();
                    }
                }
            }
        }
        public GameObject GetProcessorByIndex(int index)
        {
            if (index >= 0 && index < NumberOfProcessors)
            {
                return pooledProcessors[index];
            }
            else
            {
                throw new IndexOutOfRangeException();
            }
        }
    }
}