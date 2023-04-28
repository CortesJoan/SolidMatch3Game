using UnityEngine;

public class TileDestructionEffect : MonoBehaviour
{
    [Header("Effect Settings")]
    [SerializeField] private float destructionEffectDuration = 5f;

    private ParticleSystem particleSystem;

    private void Start()
    {
        particleSystem = GetComponent<ParticleSystem>();
        SetParticleSystemDuration();
    }

    private void SetParticleSystemDuration()
    {
        particleSystem.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
        var main = particleSystem.main;
        main.duration = destructionEffectDuration;
    }

    public void Play(Vector2 position)
    {
        transform.position = position;
        particleSystem.Play();
    }

    private void Update()
    {
        if (particleSystem && !particleSystem.IsAlive())
        {
            particleSystem.Clear();
            particleSystem.Stop();
        }
    }
}