using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIManager : MonoBehaviour
{
    [SerializeField]
    private Vitals _playerVitals;
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

    [SerializeField]
    private TextMeshProUGUI _textBullets;
    private int _bulletCount;
    [SerializeField]
    private BaseGun _playersGun;


    [SerializeField]
    private BaseCharacter _player;
    [SerializeField]
    private Slider _targetHPBar;
    [SerializeField]
    private TextMeshProUGUI _targetName;
    public GameObject _target;
    private float _targetVitals;


    private void Start()
    {
        _playerHealth = _playerVitals.GetCurrentHealth();
        _firstCompanionHealth = _firstCompanion.GetCurrentHealth();
        _secondCompanionHealth = _secondCompanion.GetCurrentHealth();

        SetMaxHealth(_playerVitals.GetMaxHealth());
        SetMaxHealth(_firstCompanion.GetMaxHealth());
        SetMaxHealth(_secondCompanion.GetMaxHealth());

        _bulletCount = _playersGun.BulletsInMagazine();
        _textBullets.text = "AMMO: " + _bulletCount;

        _target = _player.GetMyTarget();
        _targetVitals = _target.GetComponent<Vitals>().GetCurrentHealth();
    }

    private void Update()
    {
        _playerHealth = _playerVitals.GetCurrentHealth();
        _firstCompanionHealth = _firstCompanion.GetCurrentHealth();
        _secondCompanionHealth = _secondCompanion.GetCurrentHealth();

        SetHealth(_playerHPBar, _playerHealth);
        SetHealth(_firstCompanionHPBar, _firstCompanionHealth);
        SetHealth(_secondCompanionHPBar, _secondCompanionHealth);

        _bulletCount = _playersGun.BulletsInMagazine();
        _textBullets.text = "AMMO: " + _bulletCount;


        _target = _player.GetMyTarget();
        
        if (_target != null)
        {
            _targetVitals = _target.GetComponent<Vitals>().GetCurrentHealth();
            _targetHPBar.gameObject.SetActive(true);
            _targetHPBar.maxValue = _target.GetComponent<Vitals>().GetMaxHealth();
            SetHealth(_targetHPBar, _targetVitals);
            _targetName.text = _target.name;
        }
        else
        {
            _targetHPBar.gameObject.SetActive(false);
        }
    }


    private void SetHealth(Slider _hpBar, float _health)
    {
        _hpBar.value = _health;
    }


    public void SetMaxHealth(float _health)
    {
        _playerHPBar.maxValue = _health;
        _playerHPBar.value = _health;
    }
}
