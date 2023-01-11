using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class CharacterStats : NetworkBehaviour
{
    //character stats
    [SerializeField] protected int health;
    [SerializeField] protected int maxHealth;

    //objects to hide and show
    public GameObject objectToHide;
    public GameObject objectToShow;

    //bool
    [SerializeField, SyncVar(hook = nameof(OnDeathStateChanged))] protected bool isDead = false;
    [SerializeField] protected bool isBuilding;

    private void Start()
    {
        InitialVariables();
    }

    void OnDeathStateChanged(bool oldVal, bool newVal)
    {
        if (isDead == true && isBuilding == true)
        {
            HandleBuildingDeath();
        }
    }

    public virtual void HandleBuildingDeath()
    {
        //objectToHide.SetActive(false);
        MeshRenderer[] renderes = objectToHide.GetComponents<MeshRenderer>();
        MeshCollider[] colliders = objectToHide.GetComponents<MeshCollider>();

        foreach (var render in renderes)
        {
            render.enabled = false;
        }
        foreach (var colli in colliders)
        {
            colli.enabled = false;
        }

        objectToShow.SetActive(true);
    }

    public virtual void CheckHealth()
    {
        if(health <= 0 && isBuilding == false)
        {
            health = 0;
            Die();
        }

        if(health <= 0 && isBuilding == true)
        {
            HandleBuildingDeath();

            isDead = true;
        }

        if(health >= maxHealth)
        {
            health = maxHealth;
        }
    }

    public virtual void Die()
    {
        isDead = true;
    }

    public virtual void Building()
    {
        isBuilding = true;
    }

    public void SetHealthTo(int healthToSetTo)
    {
        health = healthToSetTo;
        CheckHealth();
    }

    public void TakeDamage(int damage)
    {
        //int healthAfterDamage = health - damage;
        //SetHealthTo(healthAfterDamage);

        ServerTakeDamage(damage);
    }

    [Command(requiresAuthority = false)]
    void ServerTakeDamage(int damage)
    {
        int healthAfterDamage = health - damage;
        SetHealthTo(healthAfterDamage);
    }

    public void Heal(int heal)
    {
        int healthAfterHeal = health + heal;
        SetHealthTo(healthAfterHeal);
    }

    public virtual void InitialVariables()
    {
        isBuilding = false;
        maxHealth = 100;
        SetHealthTo(100);
        isDead = false;
    }

    public bool IsDead()
    {
        return isDead;
    }
}