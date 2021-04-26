using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Player : MonoBehaviour
{
    [SerializeField] Vector2 drag;
    [SerializeField] Transform model;
    [SerializeField] Animator animator;
    [SerializeField] float moveForce = 0.01f;
    [SerializeField] float minMoveForce = 0.0f;
    [SerializeField] float diveDragMultiplier = 0.5f;
    [SerializeField] float maxXZOffset = 0.0f;
    [SerializeField] float tiltAmount = 20.0f;
    [SerializeField] float maxXZVelocity = 5.0f;
    [SerializeField] float multiplierTime = 2.0f;
    [SerializeField] float dragLostPerSecond = 0.001f;
    [SerializeField] TrailRenderer[] trails;
    [SerializeField] LayerMask scorableLayers;

    public UnityEvent OnDeath = new UnityEvent();
    public Vector3 TargetPos { get; set; }

    float multiplier = 1.0f;
    public float Multiplier => multiplier;

    Rigidbody body;
    bool diving = false;
    
    Rigidbody[] ragdollBodies;

    // Start is called before the first frame update
    void Awake()
    {
        body = GetComponent<Rigidbody>();

        ragdollBodies = model.GetComponentsInChildren<Rigidbody>();
    }

    private void Start()
    {
        SetTrailsEnabled(false);
        SetRagdollEnabled(false);
    }

    void SetTrailsEnabled(bool enabled)
    {
        foreach (var trail in trails)
        {
            trail.enabled = enabled;
        }
    }

    void SetRagdollEnabled(bool enabled)
    {
        body.isKinematic = enabled;
        animator.enabled = !enabled;

        foreach (var body in ragdollBodies)
        {
            body.isKinematic = !enabled;
            body.GetComponent<Collider>().isTrigger = !enabled;
            body.velocity = Vector3.zero;
        }
    }

    public void StartDive()
    {
        diving = true;
        animator.SetBool("Diving", true);
        SetTrailsEnabled(true);
    }

    public void EndDive()
    {
        diving = false;
        animator.SetBool("Diving", false);
        SetTrailsEnabled(false);
    }

    IEnumerator AddMultiplier()
    {
        multiplier += 1;
        yield return new WaitForSeconds(multiplierTime);
        multiplier -= 1;
    }

    private void OnTriggerEnter(Collider other)
    {
        if(Util.IsLayerInMask(scorableLayers, other.gameObject.layer))
        {
            StartCoroutine(AddMultiplier());
        }
    }

    public void OnProxyTrigger(Collider other)
    {
        if (Util.IsLayerInMask(scorableLayers, other.gameObject.layer))
        {
            Die();
        }
    }

    void Die()
    {
        if (body.isKinematic) return;

        multiplier = 0.0f;
        SetRagdollEnabled(true);

        StopAllCoroutines();
        multiplier = 0.0f;

        OnDeath.Invoke();
    }

    private void Update()
    {
        if (body.isKinematic) return;

        Vector3 toTarget = TargetPos - transform.position;
        float moveDist2 = toTarget.magnitude;
        float force = moveForce * moveDist2;
        force = Mathf.Max(minMoveForce, force);
        body.AddForce(toTarget.normalized * force);

        Vector3 yVelocity = new Vector3(0.0f, body.velocity.y, 0.0f);
        Vector3 xzVelocity = body.velocity - yVelocity;

        float xDrag = 1.0f - drag.x * Time.deltaTime;
        if (xDrag < 0.0f) xDrag = 0.0f;
        xzVelocity *= xDrag;

        float yDrag = 1.0f - drag.y * (diving ? diveDragMultiplier : 1.0f) * Time.deltaTime;
        if (yDrag < 0.0f) yDrag = 0.0f;
        yVelocity *= yDrag;

        body.velocity = xzVelocity + yVelocity;

        Vector3 xzPos = transform.position;
        xzPos.y = 0.0f;

        if (xzPos.sqrMagnitude > maxXZOffset * maxXZOffset)
        {
            xzPos = xzPos.normalized * maxXZOffset;
            xzPos.y = transform.position.y;
            transform.position = xzPos;
        }

        xzVelocity = body.velocity;
        xzVelocity.y = 0.0f;
        float velocityMod = Mathf.Clamp01(xzVelocity.magnitude / maxXZVelocity);

        Vector3 axis = Vector3.Cross(Vector3.up, body.velocity);
        Quaternion targetRotation = Quaternion.AngleAxis(tiltAmount * velocityMod, axis);
        model.rotation = targetRotation;

        drag.y -= dragLostPerSecond * Time.deltaTime;
        drag.y = Mathf.Max(0.1f, drag.y);
    }

    private void LateUpdate()
    {
        
    }
}
