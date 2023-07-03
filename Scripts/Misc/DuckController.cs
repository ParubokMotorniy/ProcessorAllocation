using System.Collections;
using Assets.Scripts.Optimization;
using UnityEngine;

namespace Simulation.Misc
{
    public class DuckController : MonoBehaviour, IPooledObject
    {
        [SerializeField] private GameObject puffAppearEffect;
        [SerializeField] private GameObject puffDisappearEffect;

        [Range(0, 50)]
        [SerializeField] private float linearSpeed;

        [Range(0, 4)]
        [SerializeField] private float flightHeight;
        [SerializeField] private float yOffset;

        public IEnumerator Migrate(Transform startProcessor, Transform endProcessor)
        {
            Vector3 serverHeightOffset = new Vector3(0, yOffset, 0);
            Vector3 middle = (startProcessor.position + endProcessor.position) * 0.5f;
            float linearPath = Vector3.Distance(startProcessor.position, endProcessor.position);
            float middleDepth = (Mathf.Pow(linearPath / 2f, 2) - Mathf.Pow(flightHeight, 2)) / (2f * flightHeight);

            middle -= Vector3.up * Mathf.Abs(middleDepth);

            Vector3 startArm = startProcessor.position - middle;
            Vector3 endArm = endProcessor.position - middle;

            float timeElapsed = 0;
            float migrationTime = linearPath / linearSpeed;

            ObjectPooling.Instance.SpawnFromPool("AppearPuff", startProcessor.position + serverHeightOffset, Quaternion.identity);

            while ((timeElapsed / migrationTime) < 1)
            {
                timeElapsed += Time.deltaTime;
                transform.position = Vector3.Slerp(startArm, endArm, timeElapsed / migrationTime) + middle + serverHeightOffset;
                yield return null;
            }
            ObjectPooling.Instance.SpawnFromPool("DisappearPuff", transform.position, Quaternion.identity);

            gameObject.SetActive(false);
        }

        public void OnSpawnedFromPool()
        {

        }
    }
}