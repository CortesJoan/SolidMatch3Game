using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnDestructionEffect : MonoBehaviour
{
    private Tile currentTile;
    public GameObject destructionEffectPrefab;
 
    ParticleSystem cachedDestructionEffect;
    
    private void OnEnable()
    {
         
    }

    void Start()
    {
        if (destructionEffectPrefab != null)
        {
            GameObject instantiatedEffect = Instantiate(destructionEffectPrefab);
            cachedDestructionEffect = instantiatedEffect.GetComponent<ParticleSystem>();
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

 public   void PlayDestructionEffect()
    {
        cachedDestructionEffect?.Play(this.transform);

    }
}
