using UnityEngine;
using UnityEngine.UI;

namespace Simulation.UI
{
    public class TresholdMirror : MonoBehaviour
    {
        [SerializeField] private Slider reflection;
        private Slider restrainedSlider;
        private static float minDistance = 10f;
        private void Start()
        {
            restrainedSlider = GetComponent<Slider>();
        }
        public void ReceiveChange(float newValue)
        {
            if (100 - newValue - reflection.value < minDistance)
            {
                restrainedSlider.interactable = false;
            }
            reflection.interactable = true;
        }
    }
}