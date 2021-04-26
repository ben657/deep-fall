using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StaticOffset : MonoBehaviour
{
    [SerializeField] float min;
    [SerializeField] float max;
    [SerializeField] Vector3 direction;
    [SerializeField] bool integer = false;

    // Start is called before the first frame update
    void Start()
    {
        if(integer)
            transform.localPosition += direction * Random.Range((int)min, (int)max);
        else
            transform.localPosition += direction * Random.Range(min, max);
    }
}
