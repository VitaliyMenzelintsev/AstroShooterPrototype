using UnityEngine;
using UnityEngine.InputSystem;
using Cinemachine;

public class CameraSwitcher : MonoBehaviour
{
    [SerializeField]
    private PlayerInput PlayerInput;
    [SerializeField]
    private int priorityBoostAmount = 2;

    private CinemachineVirtualCamera virtualCamera;
    private InputAction aimAction;

    private void Awake()
    {
        virtualCamera = GetComponent<CinemachineVirtualCamera>();
        aimAction = PlayerInput.actions["Aim"];
    }

    private void OnEnable() // подписка на прицеливание
    {
        aimAction.performed += _ => StartAim();
        aimAction.canceled += _ => CancelAim();
    }

    private void OnDisable() // отписка от прицеливания
    {
        aimAction.performed -= _ => StartAim();
        aimAction.canceled -= _ => CancelAim();
    }

    private void StartAim() // прицеливание повышает приоритет вторичной камеры
    {
        virtualCamera.Priority += priorityBoostAmount;
    }

    private void CancelAim() // отмена прицеливания понижает приоритет
    {
        virtualCamera.Priority -= priorityBoostAmount;
    }
}
