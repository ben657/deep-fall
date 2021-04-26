using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerControllerMouse : MonoBehaviour
{
    Player player;

    private void Awake()
    {
        player = GetComponent<Player>();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Plane intersect = new Plane(Vector3.up, player.transform.position);
        Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());

        float dist = 0.0f;
        if (intersect.Raycast(ray, out dist))
        {
            Vector3 worldPos = ray.GetPoint(dist);
            player.TargetPos = worldPos;
        }
    }

    void OnDive(InputValue value)
    {
        if (value.isPressed)
            player.StartDive();
        else
            player.EndDive();
    }
}
