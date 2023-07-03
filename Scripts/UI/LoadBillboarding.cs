using UnityEngine;

namespace Simulation.UI
{
    public class LoadBillboarding : MonoBehaviour
    {
        void Update()
        {
            transform.rotation = Camera.main.transform.rotation;
            transform.Rotate(new Vector3(0, 180, 0));
        }
    }
}

