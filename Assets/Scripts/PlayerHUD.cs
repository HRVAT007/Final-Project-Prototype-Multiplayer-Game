using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Mirror;

public class PlayerHUD : NetworkBehaviour
{
    [SerializeField] private Text currentHealthText;
    [SerializeField] private Text maxHealthText;
    [SerializeField] private Text currentZombieCountText;

    void Awake()
    {
        currentHealthText = GameObject.Find("CurrentHealthText").GetComponent<Text>();
        maxHealthText = GameObject.Find("MaxHealthText").GetComponent<Text>();
        currentZombieCountText = GameObject.Find("CurrentZombieCountText").GetComponent<Text>();
    }

    void Update()
    {
        if (!isOwned) return;
        if (isServerOnly) return;


        ZombieCounter();
    }

    public void UpdateHealth(int currentHealth, int maxHealth)
    {
        currentHealthText.text = currentHealth.ToString();
        maxHealthText.text = maxHealth.ToString();
    }

    private void ZombieCounter()
    {
        int numberOfZombies = GameObject.FindGameObjectsWithTag("Enemy").Length;
        currentZombieCountText.text = numberOfZombies.ToString();
    }
}
