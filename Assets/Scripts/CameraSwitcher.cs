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

    private void OnEnable() // �������� �� ������������
    {
        aimAction.performed += _ => StartAim();
        aimAction.canceled += _ => CancelAim();
    }

    private void OnDisable() // ������� �� ������������
    {
        aimAction.performed -= _ => StartAim();
        aimAction.canceled -= _ => CancelAim();
    }

    private void StartAim() // ������������ �������� ��������� ��������� ������
    {
        virtualCamera.Priority += priorityBoostAmount;
    }

    private void CancelAim() // ������ ������������ �������� ���������
    {
        virtualCamera.Priority -= priorityBoostAmount;
    }
}
