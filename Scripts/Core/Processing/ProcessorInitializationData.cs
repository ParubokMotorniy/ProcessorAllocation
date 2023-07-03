using UnityEngine;
using UnityEngine.UI;

namespace Simulation.Core.Processing
{
    public class ProcessorInitializationData : MonoBehaviour
    {
        [SerializeField] private Slider loadBar;

        public Slider LoadBar { get => loadBar; }
    }
}