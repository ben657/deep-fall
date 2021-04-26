using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Segment : MonoBehaviour
{
    const float BaseHeight = 5.0f;

    public int spawnWeight = 100;
    public bool dynamicWeight = true;
    public int length = 1;

    [SerializeField] MeshRenderer[] colourableMeshes;

    public Vector3 GetWorldEnd()
    {
        return transform.position - (Vector3.up * BaseHeight * length);
    }

    public void UpdateTunnelData(float tunnelStart, float tunnelHeight)
    {
        foreach(var mesh in colourableMeshes)
        {
            mesh.material.SetFloat("TunnelStartY", tunnelStart);
            mesh.material.SetFloat("TunnelHeight", tunnelHeight);
        }
    }

    public void UpdateColourData(Color colour)
    {
        foreach (var mesh in colourableMeshes)
        {
            mesh.material.SetColor("BaseColour", colour);
        }
    }
}
