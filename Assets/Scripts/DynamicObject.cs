using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class DynamicObject : NetworkBehaviour
{
    private Rigidbody rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    [Server]
    public void PushBack(Vector3 gg, float power)
    {
        gg = transform.position - gg;
        gg = gg.normalized * power;

        rb.AddForce(gg, ForceMode.Impulse);
    }
}
