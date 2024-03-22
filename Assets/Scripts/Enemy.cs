using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;

[RequireComponent(typeof(Health))]
[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(CapsuleCollider))]
public class Enemy : MonoBehaviour
{
    [Tooltip("Character move speed")]
    [SerializeField] private float _moveSpeed = 3.5f;
    [Tooltip("This field is responsible for the path update frequency. A lower value will produce better quality, but can reduce performance")]
    [SerializeField] private float _pathUpdateFrequency = 0.5f;
    [Tooltip("Damage that will be dealt to the player when attacked")]
    [SerializeField] private int _damage = 15;
    [Tooltip("Pause between attacks")]
    [SerializeField] private float _attackDelay = 1f;
    [Tooltip("Animated model")]
    [SerializeField] private Animator _animator;

    private Player _player; 
    private Health _health; 
    private NavMeshAgent _agent; 
    private float _pathUpdateTimer;
    private float _attackTimer = 0.1f;
    private bool _isAttacking;

    public event UnityAction<Enemy> EnemyDied;

    private void OnEnable()
    {
        //initialize the fields if this has not been done before
        if (!_health)
            _health = GetComponent<Health>();


        if (!_agent)
        {
            _agent = GetComponent<NavMeshAgent>();
            _agent.speed = _moveSpeed;
        }

        if (!_player)
            _player = FindObjectOfType<Player>();

        //subscribe on the health changed event
        _health.HealthChanged += OnTakeDamage;
    }

    void Update()
    {
        if (!_player)
            return;

        //update path when the timer expires
        if (_pathUpdateTimer > 0)
            _pathUpdateTimer -= Time.deltaTime;
        else
        {
            _agent.SetDestination(_player.transform.position);
            _pathUpdateTimer = _pathUpdateFrequency;
        }

        //check for the attack state and attack when timer expires
        if (_isAttacking)
        {
            if (_attackTimer > 0)
                _attackTimer -= Time.deltaTime;
            else
                AttackPlayer();
        }

        //check if enemy has reached player
        if (_agent.remainingDistance <= _agent.stoppingDistance && !_isAttacking)
        {
            _isAttacking = true;
        }

        //if player moves away, stop attacking
        if (_agent.remainingDistance > _agent.stoppingDistance && _isAttacking)
        {
            _isAttacking = false;
            _animator.SetTrigger("Walk");
        }
    }

    void AttackPlayer()
    {
        //play attack animation
        _animator.SetTrigger("Attack");
        //apply damage to player
        _player.Hit(_damage);
        //reset attack timer
        _attackTimer = _attackDelay;
    }

    private void OnTriggerEnter(Collider other)
    {
        //apply damage and play feedback when collide with bullet
        if (other.tag == "Bullet")
        { 
            _health.ReduceHealth(Player.Instance.Damage); 
            ObjectPooler.Instance.SpawnFromPool("HitImpact", other.transform.position, other.transform.rotation);
            other.gameObject.SetActive(false);
        }
    }

    private void OnDisable()
    {
        //unsubscribe from damage event
        _health.HealthChanged -= OnTakeDamage;
    }

    private void OnTakeDamage()
    {
        //check if health equals zero and invoke death method if true
        if (_health.CurrentHealth == 0)
            Die();
    }

    private void Die()
    {
        EnemyDied?.Invoke(this);
        gameObject.SetActive(false);
    }
}
