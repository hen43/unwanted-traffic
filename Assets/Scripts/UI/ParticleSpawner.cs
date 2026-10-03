using UnityEngine;

public class ParticleSpawner : MonoBehaviour
{
    [SerializeField] GameObject particle;

    public void SpawnParticle()
    {
        GameObject part = Instantiate(particle, transform);
        Destroy(part, 2f);
    }
}