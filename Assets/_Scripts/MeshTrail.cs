using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewBehaviourScript : MonoBehaviour
{
    public float activeTime = 2f;

    [Header("Mesh Related")] [SerializeField]
    private CountdownTimer meshRefreshRate;

    [SerializeField] private float meshDestroyDelay = 3f;
    [SerializeField] private Transform positionToSpawn;
    [SerializeField] private Vector3 meshSpawnOffset;
    [SerializeField] private float meshMoveAmount = 1f;

    [Header("Shader Related")] [SerializeField]
    private Material mat;

    [SerializeField] private string shaderVarRef;
    [SerializeField] private float shaderVarRate = 0.1f;
    [SerializeField] private float shaderVarRefreshRate = 0.05f;

    private bool _isTrailActive;

    private SkinnedMeshRenderer[] _skinnedMeshRenderers;

    private readonly HashSet<GameObject> _spawnedObjects = new HashSet<GameObject>();

    private void Start()
    {
        // Get the TestPlayerScript
        var testPlayerScript = GetComponent<TestPlayerScript>();

        // Connect to the boost event
        testPlayerScript.OnBoostStart += _ => _isTrailActive = true;
        testPlayerScript.OnBoostEnd += _ => _isTrailActive = false;

        // Connect to the countdown timer event
        meshRefreshRate.Reset();
        meshRefreshRate.SetActive(true);
        meshRefreshRate.OnTimerEnd += () => meshRefreshRate.Reset();
        meshRefreshRate.OnTimerEnd += () =>
        {
            if (_isTrailActive)
                SpawnTrailObject();
        };
    }

    // Update is called once per frame
    private void Update()
    {
        // Update the timer
        meshRefreshRate.Update(Time.deltaTime);

        // Update the spawned object positions
        UpdateSpawnedObjectPositions();
    }

    private void UpdateSpawnedObjectPositions()
    {
        foreach (var obj in _spawnedObjects)
            obj.transform.position += Vector3.forward * (meshMoveAmount * Time.deltaTime);
    }

    private void SpawnTrailObject()
    {
        Debug.Log($"Spawning trail object at {positionToSpawn.position}");

        if (_skinnedMeshRenderers == null)
            _skinnedMeshRenderers = GetComponentsInChildren<SkinnedMeshRenderer>();

        foreach (var currentRenderer in _skinnedMeshRenderers)
        {
            // Get the position, rotation, and scale of the current renderer's object
            var rotation = currentRenderer.transform.rotation;

            var gObj = new GameObject();
            // gObj.transform.SetPositionAndRotation(
            //     positionToSpawn.position + meshSpawnOffset,
            //     rotation
            // );

            // Add the spawned object to the spawned objects list
            _spawnedObjects.Add(gObj);

            gObj.transform.parent = positionToSpawn;
            gObj.transform.localPosition = meshSpawnOffset;
            gObj.transform.rotation = rotation;

            var meshRenderer = gObj.AddComponent<MeshRenderer>();
            var meshFilter = gObj.AddComponent<MeshFilter>();

            var mesh = new Mesh();
            currentRenderer.BakeMesh(mesh);

            meshFilter.mesh = mesh;
            meshRenderer.material = mat;

            StartCoroutine(AnimateMaterialFloat(meshRenderer.material, 0, shaderVarRate, shaderVarRefreshRate));

            // Destroy the spawned object after a delay
            StartCoroutine(DestroySpawnedObject(gObj));
        }
    }

    private IEnumerator DestroySpawnedObject(GameObject obj)
    {
        yield return new WaitForSeconds(meshDestroyDelay);

        // Remove the object from the spawned objects list
        _spawnedObjects.Remove(obj);

        // Destroy the object
        Destroy(obj);
    }

    // private IEnumerator ActivateTrail(float timeActive)
    // {
    //     while (timeActive > 0)
    //     {
    //         timeActive -= meshRefreshRate;
    //
    //         if (_skinnedMeshRenderers == null)
    //             _skinnedMeshRenderers = GetComponentsInChildren<SkinnedMeshRenderer>();
    //
    //         for (var i = 0; i < _skinnedMeshRenderers.Length; i++)
    //         {
    //             var currentRenderer = _skinnedMeshRenderers[i];
    //
    //             // Get the position, rotation, and scale of the current renderer's object
    //             var rotation = currentRenderer.transform.rotation;
    //
    //             var gObj = new GameObject();
    //             gObj.transform.SetPositionAndRotation(
    //                 positionToSpawn.position + meshSpawnOffset,
    //                 rotation
    //             );
    //
    //             var meshRenderer = gObj.AddComponent<MeshRenderer>();
    //             var meshFilter = gObj.AddComponent<MeshFilter>();
    //
    //             var mesh = new Mesh();
    //             currentRenderer.BakeMesh(mesh);
    //
    //             meshFilter.mesh = mesh;
    //             meshRenderer.material = mat;
    //
    //             StartCoroutine(AnimateMaterialFloat(meshRenderer.material, 0, shaderVarRate, shaderVarRefreshRate));
    //
    //             Destroy(gObj, meshDestroyDelay);
    //         }
    //
    //         yield return new WaitForSeconds(meshRefreshRate);
    //     }
    //
    //     _isTrailActive = false;
    // }

    private IEnumerator AnimateMaterialFloat(Material mat, float goal, float rate, float refreshRate)
    {
        var valueToAnimate = mat.GetFloat(shaderVarRef);

        while (valueToAnimate > goal)
        {
            valueToAnimate -= rate;
            mat.SetFloat(shaderVarRef, valueToAnimate);
            yield return new WaitForSeconds(refreshRate);
        }
    }
}