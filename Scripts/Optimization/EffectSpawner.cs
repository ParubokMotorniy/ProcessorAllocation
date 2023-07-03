using UnityEngine;

namespace Assets.Scripts.Optimization
{
    public class EffectSpawner : MonoBehaviour, IPooledObject
    {
        [SerializeField] ParticleSystem effectParentSystem;
        public void OnSpawnedFromPool()
        {
            effectParentSystem.Play();
        }
    }
}