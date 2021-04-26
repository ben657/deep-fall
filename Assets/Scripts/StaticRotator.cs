using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum RotatorType
{
    EightSide,
    SixteenSide,
    Free
}

public class StaticRotator : MonoBehaviour
{
    [SerializeField] RotatorType type;

    // Start is called before the first frame update
    void Start()
    {
        float deg = type == RotatorType.EightSide ? Random.Range(0, 8) * 45.0f : type == RotatorType.SixteenSide ? Random.Range(0, 16) * 22.5f : Random.Range(0.0f, 360.0f);
        transform.rotation = Quaternion.AngleAxis(deg, Vector3.up);
    }
}
