using UnityEngine;

// Висит на каждом стреляющем персонаже
public class GunController : MonoBehaviour
{
    public Transform WeaponHold;             
    //public Gun[] AllGuns;
    public Gun StartingGun;

    private Gun _equippedGun;


    private void Start()
    {
        if (StartingGun != null)
            EquipGun(StartingGun);
    }
    public void EquipGun(Gun _gunToEquip)
    {
        if (_equippedGun != null) 
            Destroy(_equippedGun.gameObject);

        _equippedGun = Instantiate(_gunToEquip, WeaponHold.position, WeaponHold.rotation) as Gun;
        _equippedGun.transform.parent = WeaponHold;
    }

    //public void EquipGun(int _weaponIndex)
    //{
    //    EquipGun(AllGuns[_weaponIndex]);
    //}
    
    public void OnTriggerHold()
    {
        if (_equippedGun != null)
            _equippedGun.OnTriggerHold();
    }

    public void OnTriggerRelease()
    {
        if (_equippedGun != null)
            _equippedGun.OnTriggerRelease();
    }

    public float GunHeight
    {
        get { return WeaponHold.position.y; }
    }

    public void Aim(Vector3 _aimPoint)
    {
        if (_equippedGun != null)
            _equippedGun.Aim(_aimPoint);
    }

    public void Reload()
    {
        if (_equippedGun != null)
            _equippedGun.Reload();
    }
}