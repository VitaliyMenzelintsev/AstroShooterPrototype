using UnityEngine;

[RequireComponent(typeof(PlayerController))]
[RequireComponent(typeof(GunController))]


// Висит на игроке
public class Player : LivingEntity
{
    public Transform Crosshair;

    private float _moveSpeed = 3.2f;
    private Camera _viewCamera;
    private PlayerController _controller;
    private GunController _gunController;

    private void Awake()
    {
        _controller = GetComponent<PlayerController>();
        _gunController = GetComponent<GunController>();
        _viewCamera = Camera.main;                                     
    }

    protected override void Start()
    {
        base.Start();
    }

    //private void OnNewWave(int _waveNumber)
    //{
    //    _health = _startingHealth;
    //    _gunController.EquipGun(_waveNumber - 1);
    //}

    private void Update()
    {
        // Movement input
        Vector3 _moveInput = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical")); 
        Vector3 _moveVelocity = _moveInput.normalized * _moveSpeed;
        _controller.Move(_moveVelocity);
       

        // Look input
        Ray _ray = _viewCamera.ScreenPointToRay(Input.mousePosition);     
        Plane _groundPlane = new Plane(Vector3.up, Vector3.up * _gunController.GunHeight);
        float _rayDistance;                                             

        if (_groundPlane.Raycast(_ray, out _rayDistance))                            
        {
            Vector3 _point = _ray.GetPoint(_rayDistance);                 
            _controller.LookAt(_point);
            Crosshair.position = _point;


            if ((new Vector2(_point.x, _point.z) - new Vector2(transform.position.x, transform.position.z)).sqrMagnitude > 1)
                _gunController.Aim(_point);
        }


        // Weapon input
        if (Input.GetMouseButton(0)) 
            _gunController.OnTriggerHold();
        

        if (Input.GetMouseButtonUp(0))
            _gunController.OnTriggerRelease();
        

        if (Input.GetKeyDown(KeyCode.R))
            _gunController.Reload();
    }
}