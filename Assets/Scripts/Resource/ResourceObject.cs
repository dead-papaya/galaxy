using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceObject : MonoBehaviour
{
    public int health = 5; // Количество ударов, чтобы дерево исчезло
    public float shakeDuration = 0.5f;  // Длительность тряски
    public float shakeStrength = 0.2f;
    public Transform harvestTransform;
    [SerializeField] protected GameObject harvestSound;
    [SerializeField] protected GameObject harvestEndSound;
    [SerializeField] protected LayerMask playerLayerMask;
    [SerializeField] protected GameObject resourcePrefab;
    [SerializeField] protected float spawnRadius;
    
    public virtual async void TakeDamage(){}
    
    public bool IsDepleted()
    {
        return health <= 0;
    }
    
    protected virtual void Deplete() { }
}
