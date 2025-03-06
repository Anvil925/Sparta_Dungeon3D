using System;
using UnityEngine;

public class PlayerCondition : MonoBehaviour
{
    public UICondition uiCondition;

    Condition health { get { return uiCondition.health; } }
    public Condition energy { get { return uiCondition.energy; } }
    Condition water { get { return uiCondition.water; } }

    public float noWaterHealthDecay = 1f;
    public event Action onTakeDamage;

    private void Update()
    {
        water.Add(water.passiveValue * Time.deltaTime);
        if(water.curValue < 0f)
        {
            health.Subtract(noWaterHealthDecay * Time.deltaTime);
        }

        if(health.curValue < 0f)
        {
            Die();
        }
    }

    public void Heal(float amount)
    {
        health.Add(amount);
    }

    public void Eat(float amount)
    {
        water.Add(amount);
    }

    public void Die()
    {
        Debug.Log("플레이어가 죽었다.");
    }
}