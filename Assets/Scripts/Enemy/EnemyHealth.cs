using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class EnemyHealth : MonoBehaviour, IDamagable
{
    [SerializeField] private EnemyStats enemyStats;
    [SerializeField] private Slider healthBar;
    [SerializeField] private Image HealthBarFill;
    [SerializeField] private Color maxHealthColor;
    [SerializeField] private Color noHealthColor;
    [SerializeField] private GameObject damageTextPrefab;
    
    private int currentHealth;

    private void Start() 
    {
        currentHealth = enemyStats.maxHealth;
        SetHealthUI();
    }

    public void DealDamage(int damage)
    {
        currentHealth -= damage;
        Instantiate(damageTextPrefab, transform.position, Quaternion.identity).GetComponent<DamageText>().Initalise(damage);
        CheckIfDead();
        SetHealthUI();
    }

    private void CheckIfDead()
    {
        if(currentHealth <= 0)
        {
            Destroy(gameObject);
        }
    }

    private void SetHealthUI()
    {
        float healthpercentage = FindHealthPercentage();
        healthBar.value = FindHealthPercentage();
        HealthBarFill.color = Color.Lerp(noHealthColor,maxHealthColor,healthpercentage / 100);
    }

    private float FindHealthPercentage()
    {
        return ((float)currentHealth / (float)enemyStats.maxHealth) * 100;
    }
}
