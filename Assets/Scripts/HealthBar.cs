using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    [SerializeField] private Health _health;
    [SerializeField] private Slider _slider;
    [SerializeField] private Image _fill;
    [SerializeField] private bool _faceToCamera;

    [SerializeField] private Color _green;
    [SerializeField] private Color _red;

    private Camera _camera;

    public void Start()
    {
        if(_faceToCamera)
            _camera = Camera.main;
    }

    private void OnEnable()
    {
        _slider.maxValue = _health.MaxHealth;
        _health.HealthChanged += OnHealthChanged;
        _slider.value = _health.CurrentHealth;
    }

    private void OnDisable()
    {
        _health.HealthChanged -= OnHealthChanged;
    }

    private void Update()
    {
        if (_faceToCamera)
            transform.LookAt(new Vector3(_camera.transform.position.x, _camera.transform.position.y, _camera.transform.position.z));

    }
    public void OnHealthChanged()
    {
        _slider.value = _health.CurrentHealth;
        _fill.color = Color.Lerp(_red, _green, _slider.value / _slider.maxValue);
    }

}
