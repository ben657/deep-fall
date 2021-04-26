using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugUI : MonoBehaviour
{
    public Player player;
    public TMPro.TextMeshProUGUI ySpeed;
    public TMPro.TextMeshProUGUI xzSpeed;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Rigidbody body = player.GetComponent<Rigidbody>();
        ySpeed.text = "Y Speed: " + body.velocity.y.ToString("0.00");
        Vector3 xzVelocity = body.velocity;
        xzVelocity.y = 0;
        xzSpeed.text = "XZ Speed: " + xzVelocity.magnitude.ToString("0.00");
    }
}
