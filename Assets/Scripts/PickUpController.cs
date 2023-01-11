using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickUpController : MonoBehaviour
{
    public WeaponController gunScript;
    public Rigidbody rigidBody;
    public BoxCollider coll;
    Transform player, gunContainer, _camera;

    public float pickUpRange;
    public float dropForwardForce, dropUpwardForce;

    public static bool equipped = true;
    public static bool slotFull;

    //controls
    [SerializeField] private KeyCode dropKey = KeyCode.G;
    [SerializeField] private KeyCode equipKey = KeyCode.E;

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
            PickUp();
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
            PickUp();

        if (equipped && Input.GetKeyDown(dropKey))
            Drop();
    }

    private void PickUp()
    {
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

    private void Drop()
    {
        equipped = false;
        slotFull = false;

        transform.SetParent(null);
        rigidBody.velocity = player.GetComponent<Rigidbody>().velocity;

        rigidBody.AddForce(_camera.forward * dropForwardForce, ForceMode.Impulse);
        rigidBody.AddForce(_camera.up * dropUpwardForce, ForceMode.Impulse);

        float random = Random.Range(-1f, 1f);
        rigidBody.AddTorque(new Vector3(random, random, random) * 10);

        rigidBody.isKinematic = false;
        coll.isTrigger = false;
        gunScript.enabled = false;
        rigidBody.interpolation = RigidbodyInterpolation.Extrapolate;
    }
}