using UnityEngine;
using System;

public class HealthManager : MonoBehaviour
{

    public static HealthManager Instance { get; private set; }

    public event Action OnPlayerDeath;

    [Header("References")]
    [SerializeField] private PlayerHealthUI _playerHealthUI;
    [Header("Settings")]
    [SerializeField] private int maxHealth = 3;
    private int _currentHealth;

    private void Awake()
    {
        Instance = this;
    }
    private void Start()
    {
        _currentHealth = maxHealth;
    }

    public void Damage(int damageAmount)
    {
        if(_currentHealth > 0)
        {
            _currentHealth -= damageAmount;
            _playerHealthUI.AnimateDamage();
            if(_currentHealth <= 0)
            {
                OnPlayerDeath?.Invoke();
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
