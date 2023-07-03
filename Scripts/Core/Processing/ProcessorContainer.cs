using UnityEngine;
using UnityEngine.UI;

namespace Simulation.Core.Processing
{
    public class ProcessorContainer : MonoBehaviour
    {
        [SerializeField] public Slider loadBar;
        public Slider LoadBar { get => loadBar; }
        public Processor ContainedProcessor { get => containedProcessor; }

        private Processor containedProcessor;

        public void AssignProcessor(Processor newProcessor)
        {
            containedProcessor = newProcessor;
        }
        private void Update()
        {
            containedProcessor.Update();
        }
    }
}