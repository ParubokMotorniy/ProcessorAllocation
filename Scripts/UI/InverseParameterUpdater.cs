using TMPro;
using UnityEngine;

namespace Simulation.UI
{
    public class InverseParameterUpdater : MonoBehaviour
    {
        [SerializeField] private TMP_Text textToAlter;
        [SerializeField] private string defaultText;
        public void ReceiveParameterChange(float actualValue)
        {
            textToAlter.text = defaultText + (100 - actualValue);
        }
    }
}