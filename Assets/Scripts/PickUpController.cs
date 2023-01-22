using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class PickUpController : NetworkBehaviour
{
    public WeaponController gunScript;
    public Rigidbody rigidBody;
    public BoxCollider coll;
    Transform player, gunContainer, _camera;

    public float pickUpRange;
    public float dropForwardForce, dropUpwardForce;


    public bool equipped = false;

    public bool slotFull = false;

    //controls
    [SerializeField] private KeyCode dropKey = KeyCode.G;
    [SerializeField] private KeyCode equipKey = KeyCode.F;

    private void Start()
    {
        _camera = Camera.main.transform;
        gunContainer = _camera.GetChild(0);

        player = NetworkedPlayer.Instance == null ? null : NetworkedPlayer.Instance.transform;

        if (player == null) return;

        if (!equipped)
        {
            gunScript.enabled = false;
            rigidBody.isKinematic = false;
            coll.isTrigger = false;
        }

        if (equipped)
        {
            //gunScript.enabled = true;
            //rigidBody.isKinematic = true;
            //coll.isTrigger = true;
            //slotFull = true;
            CmdPickUp();
        }
    }

    bool initialized = false;

    private void Update()
    {
        player = NetworkedPlayer.Instance == null ? null : NetworkedPlayer.Instance.transform;

        if (player == null) return;

        if (initialized == false)
        {
            initialized = true;
            Start();
        }

        WeaponPickUp();
    }

    public void WeaponPickUp()
    {
        Vector3 distanceToPlayer = player.position - transform.position;
        if (!equipped && distanceToPlayer.magnitude <= pickUpRange && Input.GetKeyDown(equipKey) && !slotFull)
            CmdPickUp();

        if (equipped && Input.GetKeyDown(dropKey))
            CmdDrop();
    }

    [Command(requiresAuthority = false)]
    private void CmdPickUp()
    {
        if (!isServer) return;
        equipped = true;
        slotFull = true;

        transform.SetParent(gunContainer);
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.Euler(Vector3.zero);
        transform.localScale = Vector3.one;

        rigidBody.isKinematic = true;
        rigidBody.interpolation = RigidbodyInterpolation.None;
        coll.isTrigger = true;
        gunScript.enabled = true;
    }

    [Command(requiresAuthority = false)]
    private void CmdDrop()
    {
        if (!isServer) return;
        equipped = false;
        slotFull = false;

        transform.SetParent(null);
        rigidBody.velocity = player.GetComponent<Rigidbody>().velocity;

        rigidBody.AddForce(_camera.forward * dropForwardForce, ForceMode.Impulse);
        rigidBody.AddForce(_camera.up * dropUpwardForce, ForceMode.Impulse);

        rigidBody.isKinematic = false;
        coll.isTrigger = false;
        gunScript.enabled = false;
        rigidBody.interpolation = RigidbodyInterpolation.Extrapolate;
    }
    private void EquippedChange(bool oldValue, bool newValue)
    {
        this.equipped = newValue;
        gunScript.enabled = newValue;
        rigidBody.isKinematic = newValue;
        coll.isTrigger = newValue;
        if (newValue)
        {
            transform.SetParent(gunContainer);
            transform.localPosition = Vector3.zero;
            transform.localRotation = Quaternion.Euler(Vector3.zero);
            transform.localScale = Vector3.one;
            slotFull = true;
        }
        else
        {
            transform.SetParent(null);
            slotFull = false;
        }
    }
}