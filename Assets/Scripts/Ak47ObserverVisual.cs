using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class Ak47ObserverVisual : NetworkBehaviour
{
    [SerializeField] GameObject visualObject;
    [SerializeField, SyncVar(hook = nameof(EquippedChange))] bool _isEquipped = false;


    void Update()
    {
        if (!isOwned) return;
        if (isServerOnly) return;

        //ChnageValue(PickUpController.equipped);
    }

    [Command]
    void ChnageValue(bool newValue)
    {
        _isEquipped = newValue;
    }

    void EquippedChange(bool oldValue, bool newValue)
    {
        if (isOwned)
        {
            visualObject.SetActive(false);
            return;
        }
        visualObject.SetActive(_isEquipped);
    }
}