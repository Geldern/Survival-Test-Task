using UnityEngine;
using UnityEngine.Events;

public class Weapon : MonoBehaviour
{
    [Tooltip("Delay between shots")]
    [SerializeField] private float _shootDelay;
    [Tooltip("Position where the bullets will spawn")]
    [SerializeField] private Transform _muzzlePoint;
    [Tooltip("Bullet prefab")]
    [SerializeField] private GameObject _bulletPrefab;

    private bool _isShooting; //current state of the weapon
    private float _shootTimer; //timer to next shot

    [Tooltip("Use this event to simply setup shoot feedback")]
    public UnityEvent OnShot;

    private void Update()
    {
        if (!_isShooting)
            return;

        //shoot after the timer expires
        if (_shootTimer > 0)
            _shootTimer -= Time.deltaTime;
        else
            Shoot();
    }

    private void Shoot()
    {
        //reset the timer
        _shootTimer = _shootDelay;
        //invoke OnShot event
        OnShot.Invoke();
        //release bullet from the pool
        ObjectPooler.Instance.SpawnFromPool("Bullet", _muzzlePoint.position, _muzzlePoint.rotation);
    }

    //change the weapon state
    public void ToggleShooting(bool shooting)
    {
        _isShooting = shooting;
    }
}
