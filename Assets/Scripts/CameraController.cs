using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class CameraController : MonoBehaviour
{
    [SerializeField] Transform target;
    [SerializeField] float distance = 0.0f;
    [SerializeField] float zOffset = 0.0f;
    [SerializeField] float maxXZOffset = 0.0f;
    [SerializeField] float moveSpeed = 0.0f;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    private void Update()
    {
        
    }

    // Update is called once per frame
    void LateUpdate()
    {
        if (!target) return;

        Vector3 from = transform.position;
        from.y = target.position.y;

        Vector3 toTarget = target.position - from;
        float speed = moveSpeed * toTarget.sqrMagnitude;
        Vector3 movement = toTarget.normalized * speed * Time.deltaTime;
        if (movement.sqrMagnitude > toTarget.sqrMagnitude)
            movement = movement.normalized * toTarget.magnitude;
        Vector3 pos = from + movement;

        pos.y += distance;

        Vector3 xzPos = pos;
        xzPos.y = 0;

        if (xzPos.sqrMagnitude > maxXZOffset * maxXZOffset)
        {
            xzPos = xzPos.normalized * maxXZOffset;
            xzPos.y = pos.y;
            pos = xzPos;
        }

        pos.z -= zOffset;

        transform.position = pos;
    }
}
