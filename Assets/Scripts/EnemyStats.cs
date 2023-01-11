using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyStats : CharacterStats
{
    //enemy stats
    [SerializeField] private int damage;
    public float attackSpeed;

    [SerializeField] private bool canAttack;

    public void DealDamage(CharacterStats damageStats)
    {
        damageStats.TakeDamage(damage);
    }

    public override void Die()
    {
        base.Die();
        Destroy(gameObject);
    }

    public override void InitialVariables()
    {
        maxHealth = 200;
        SetHealthTo(maxHealth);
        isDead = false;

        damage = 10;
        attackSpeed = 1.5f;
        canAttack = true;
    }
}