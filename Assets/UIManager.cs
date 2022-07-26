using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [SerializeField]
    private Vitals _player;
    [SerializeField]
    private Slider _playerHPBar;
    private float _playerHealth;

    [SerializeField]
    private Vitals _firstCompanion;
    [SerializeField]
    private Slider _firstCompanionHPBar;
    private float _firstCompanionHealth;

    [SerializeField]
    private Vitals _secondCompanion;
    [SerializeField]
    private Slider _secondCompanionHPBar;
    private float _secondCompanionHealth;



    private void Start()
    {
        _playerHealth = _player.GetCurrentHealth();
        _firstCompanionHealth = _firstCompanion.GetCurrentHealth();
        _secondCompanionHealth = _secondCompanion.GetCurrentHealth();

        SetMaxHealth(_player.GetMaxHealth());
        SetMaxHealth(_firstCompanion.GetMaxHealth());
        SetMaxHealth(_secondCompanion.GetMaxHealth());
    }

    private void Update()
    {
        _playerHealth = _player.GetCurrentHealth();
        _firstCompanionHealth = _firstCompanion.GetCurrentHealth();
        _secondCompanionHealth = _secondCompanion.GetCurrentHealth();

        SetHealth(_playerHPBar, _playerHealth);
        SetHealth(_firstCompanionHPBar, _firstCompanionHealth);
        SetHealth(_secondCompanionHPBar, _secondCompanionHealth);
    }

    private void SetHealth(Slider _hpBar,  float _health)
    {
        _hpBar.value = _health;
    }

    public void SetMaxHealth(float _health)
    {
        _playerHPBar.maxValue = _health;
        _playerHPBar.value = _health;
    }
}
