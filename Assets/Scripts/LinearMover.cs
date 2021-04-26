using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LinearMover : MonoBehaviour
{
    [SerializeField] Vector3 targetOffset;
    [SerializeField] float speed = 0.0f;

    Vector3 startPos;
    float s = 0.0f;
    bool moving = true;

    private void Start()
    {
        startPos = transform.localPosition;
    }

    // Update is called once per frame
    void Update()
    {
        if (!moving) return;

        s += speed * Time.deltaTime;
        transform.localPosition = Vector3.Lerp(startPos, startPos + targetOffset, s);
        if (s >= 1.0f) moving = false;

    }
}
