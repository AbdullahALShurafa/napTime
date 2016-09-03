using UnityEngine;
using System.Collections.Generic;

public abstract class Character : MonoBehaviour
{
    protected float maxHealth;
    protected float maxSpeed;
    protected float maxDamage;
    protected bool isDead;
    

    public float GetHealth()
    {
        return maxHealth;
    }
    public void SetHealth(float a_health)
    {
        maxHealth = a_health;
    }
    public float GetSpeed()
    {
        return maxSpeed;
    }
    public void SetSpeed(float a_speed)
    {
        maxSpeed = a_speed;
    }
    public void IsDead(bool a_isdead)
    {
        isDead = a_isdead;
    }
    
}
