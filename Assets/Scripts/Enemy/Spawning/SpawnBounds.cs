using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnBounds : MonoBehaviour
{
    [SerializeField] Vector3 extents = new Vector3();

    Vector3 Min { get => transform.position - extents; }
    Vector3 Max { get => transform.position + extents; }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireCube(transform.position, extents * 2);
    }

    /// <summary>
    /// creates the location for the enmy to spawn at
    /// </summary>
    /// <returns>Vector3 location for enemy to spawn at</returns>
    public Vector3 SpawnLoc()
    {
        Vector3 randomSpot;
        randomSpot.x = Random.Range(Min.x, Max.x);
        randomSpot.y = transform.position.y;
        randomSpot.z = Random.Range(Min.z, Max.z);

        return randomSpot;
    }
}
