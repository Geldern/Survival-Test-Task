using UnityEngine;
using UnityEngine.Events;

public class Health : MonoBehaviour
{
    [Tooltip("Start health value between 0 and MaxHealth")]
    [SerializeField] private int _health;
    public int CurrentHealth => _health;

    [Tooltip("Maximum health value")]
    [SerializeField] private int _maxHealth = 100;
    public int MaxHealth => _maxHealth;
    [Tooltip("Use this event to simply setup damage feedback")]
    [SerializeField] private UnityEvent OnTakeDamage;
    [Tooltip("Use this event to simply setup death feedback")]
    [SerializeField] private UnityEvent OnDeath;
    [Tooltip("Use this event to simply setup healing feedback")]
    [SerializeField] private UnityEvent OnRegainHealth;

    public event UnityAction HealthChanged;
    
    //prevert setting health below zero and upper then _maxHealth in the inspector;
    private void OnValidate() => _health = Mathf.Clamp(_health, 0, _maxHealth);

    private void OnEnable()
    {
        _health = _maxHealth;
    }

    //use this method to reduce health
    public void ReduceHealth(int amount)
    {
        //clamp health values between 0 and max health to prevent the value from going beyond the bounds 
        _health = Mathf.Clamp(_health - amount, 0, _maxHealth);
        //invoking event to all listeners
        HealthChanged?.Invoke();

        //invoking the required event in case of death or damage
        if (_health > 0)
            OnTakeDamage?.Invoke();
        else
            OnDeath?.Invoke();
    }

    public void AddHealth(int amount)
    {
        //clamp health values between 0 and max health to prevent the value from going beyond the bounds 
        _health = Mathf.Clamp(_health + amount, 0, _maxHealth);
        //invoking event to all listeners
        HealthChanged?.Invoke();
        //invoking regain health event
        OnRegainHealth.Invoke();
    }
}
