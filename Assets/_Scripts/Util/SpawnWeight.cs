using System;
using UnityEngine;

[Serializable]
public class SpawnWeight
{
    [Tooltip("The game object to spawn")] [SerializeField]
    private GameObject prefab;

    [Tooltip("The weight of the spawn. The higher the weight, the more likely it is to spawn.")]
    [SerializeField]
    [Min(0)]
    private float weight;

    public GameObject Prefab => prefab;

    public float Weight => weight;
}