using UnityEngine;

public class HealthManager : MonoBehaviour
{
    [SerializeField] private int maxHealth = 100;
    private int _currentHealth;

    private void Start()
    {
        _currentHealth = maxHealth;
    }

    public void Damage(int damageAmount)
    {
        if(_currentHealth > 0)
        {
            _currentHealth -= damageAmount;
            if(_currentHealth <= 0)
            {
                //todo die
            }
        }
    }

    public void Heal(int healAmount)
    {
        if(_currentHealth < maxHealth)
        {
            _currentHealth = Mathf.Min(_currentHealth + healAmount, maxHealth);
        }
    }
}
