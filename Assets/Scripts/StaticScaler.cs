using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StaticScaler : MonoBehaviour
{
    public Vector3 minScale;
    public Vector3 maxScale;

    // Start is called before the first frame update
    void Start()
    {
        transform.localScale = Vector3.Lerp(minScale, maxScale, Random.value);
    }
}
