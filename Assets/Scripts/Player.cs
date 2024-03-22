using Invector.vCharacterController;
using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent (typeof(Health))]
[RequireComponent (typeof(AutoAim))]
[RequireComponent (typeof(vThirdPersonController))]
public class Player : MonoBehaviour
{
    public static Player Instance;

    [Tooltip("The damage that the player will do with a shot")]
    [SerializeField] private int _damage;
    public int Damage => _damage;
    [Tooltip("Our weapon object")]
    [SerializeField] private Weapon _weapon;


    private Animator _animator;
    private Health _health;
    private AutoAim _autoAim;
    private vThirdPersonController _thirdPersonController;
    private Transform _target;

    private void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        //initialize our components and subscribe required events
        _health = GetComponent<Health>();
        _autoAim = GetComponent<AutoAim>();
        _animator = GetComponent<Animator>();
        _thirdPersonController = GetComponent<vThirdPersonController>();

        _health.HealthChanged += OnTakeDamage;
        _autoAim.TargetFound += OnTargetFound;
        _autoAim.TargetLost += OnTargetLost;
    }
    private void OnDisable()
    {
        _health.HealthChanged -= OnTakeDamage;
    }

    public void OnTargetFound(Transform target)
    {
        //set our target and change locomotion to strafing
        _target = target;
        _thirdPersonController.SetRotateTarget(_target);
        _thirdPersonController.isStrafing = true;
        //switch weapon state
        _weapon.ToggleShooting(true);
    }

    public void OnTargetLost()
    {
        //reset target, locomotion and weapon
        _target = null;
        _thirdPersonController.isStrafing = false;
        _weapon.ToggleShooting(false);
    }

    public void OnAnimatorIK(int layerIndex)
    {
        //set body IK to lock on target
        if(_target)
        {
            _animator.SetLookAtPosition(new Vector3(_target.position.x, 1, _target.position.z));
            _animator.SetLookAtWeight(1, 1, 0);
        }    
    }

    //processing incoming damage
    public void Hit(int damage)
    { 
        _health.ReduceHealth(damage);
    }

    private void OnTakeDamage()
    {
        //check if health equals zero and invoke death method if true
        if (_health.CurrentHealth == 0)
            Die();
    }

    private void Die()
    {
        //reload scene
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
