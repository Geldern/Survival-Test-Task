using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Bullet : MonoBehaviour
{
    [Tooltip("The Rigidbody component on bullet")]
    [SerializeField] private Rigidbody _rigidbody;
    [Tooltip("Bullet move speed")]
    [SerializeField] private float _speed;

    private void FixedUpdate()
    {
        //moving the bullet forward at a given speed
        _rigidbody.velocity = transform.forward * _speed;
    }
}
