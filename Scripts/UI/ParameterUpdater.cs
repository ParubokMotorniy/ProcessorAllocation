using TMPro;
using UnityEngine;

namespace Simulation.UI
{
    public class ParameterUpdater : MonoBehaviour
    {
        [SerializeField] private TMP_Text textToAlter;
        [SerializeField] private string defaultText;
        public void ReceiveParameterChange(float actualValue)
        {
            textToAlter.text = defaultText + actualValue.ToString("0.##");
        }
    }
}

