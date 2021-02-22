using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class Movement : NetworkBehaviour
{
    [SerializeField] float moveSpeed;

    private void Start()
    {
        if (!isLocalPlayer) { return; }

        Camera.main.transform.SetParent(this.transform);
    }

    [Client]
    void Update()
    {
        if (!isLocalPlayer) { return; }

        if (Input.GetButton("Horizontal") || Input.GetButton("Vertical"))
        {
            Vector2 dir = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical")).normalized;
            dir *= moveSpeed;
            this.transform.position += ((Vector3)dir * Time.deltaTime);
        }
    }
}
