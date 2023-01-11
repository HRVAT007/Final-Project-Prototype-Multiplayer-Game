using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class BuildingStats : CharacterStats
{
    void Update()
    {
        isBuilding = true;
    }

    public override void InitialVariables()
    {
        isBuilding = true;
        maxHealth = 100;
        SetHealthTo(maxHealth);
        isDead = false;
    }


}
