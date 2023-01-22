using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class NetworkedPlayer : NetworkBehaviour
{
    public static NetworkedPlayer Instance;

    public override void OnStartAuthority()
    {
        Instance = this;

        Camera.main.transform.SetParent(transform);
        Camera.main.transform.position = new Vector3(0, 1.32f, 0);
    }


    void Awake() => gameObject.name = "Charachter";
    void Start() => gameObject.name = "Charachter";
}