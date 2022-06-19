using System.Collections;
using UnityEngine;


// висит на каждой пушке. Характеристики настраиваются индивидуально
public class Gun : MonoBehaviour
{
    public enum FireMode { Auto, Burst, Single }   
    public FireMode FireModeOfGun;

    public Transform[] ProjectileSpawnPoint;       
    public Projectile Projectile;     
    public int BurstCount;                
    public int ProjectilesPerMagazine;  
    public float ReloadTime = 0.3f;
    public float MsBetweenShots = 100;    
    public float MuzzleVelocity = 35;  

    private bool _isReloading; 

    [Header("Recoil")]
    //public Vector2 kickMinMax = new Vector2(0.005f, .2f);       
    //public Vector2 recoilAngleMinMax = new Vector2(.1f, .2f);     
    //public float timeOfReturnToPosition = 0.1f;
    //private Vector3 recoilSmoothDampVelocity;    
    //private float recoilRotSmoothDampVelocity;   
    //private float recoilAngle;                  

    private float _nextShotTime;
    private bool _triggerReleasedSinceLastShot;  
    private int _shotsRemainingInBurst;           
    private int _projectilesRemainingInMagazine;

    private void Start()
    {
        _shotsRemainingInBurst = BurstCount;
        _projectilesRemainingInMagazine = ProjectilesPerMagazine;
    }

    private void LateUpdate()          
    {
        //transform.localPosition = Vector3.SmoothDamp(transform.localPosition, Vector3.zero, ref recoilSmoothDampVelocity, timeOfReturnToPosition);
        //recoilAngle = Mathf.SmoothDamp(recoilAngle, 0, ref recoilRotSmoothDampVelocity, timeOfReturnToPosition);           
        //transform.localEulerAngles = transform.localEulerAngles + Vector3.left * recoilAngle;

        if (!_isReloading && _projectilesRemainingInMagazine == 0)
            Reload();    
    }

    private void Shoot()
    {
        if (!_isReloading && Time.time > _nextShotTime && _projectilesRemainingInMagazine > 0)
        {
            if (FireModeOfGun == FireMode.Burst)
            {
                if (_shotsRemainingInBurst == 0)
                {
                    return;
                }
                _shotsRemainingInBurst--;
            }
            else if (FireModeOfGun == FireMode.Single)
            {
                if (!_triggerReleasedSinceLastShot)
                {
                    return;
                }
            }
            for (int i = 0; i < ProjectileSpawnPoint.Length; i++)
            {
                if (_projectilesRemainingInMagazine == 0)
                {
                    break;
                }
                _projectilesRemainingInMagazine--;
                _nextShotTime = Time.time + MsBetweenShots / 1000;
                Projectile _newProjectile = Instantiate(Projectile, ProjectileSpawnPoint[i].position, ProjectileSpawnPoint[i].rotation) as Projectile;
                _newProjectile.SetSpeed(MuzzleVelocity);
            }
            //transform.localPosition -= Vector3.forward * Random.Range(kickMinMax.x, kickMinMax.y);
            //recoilAngle += Random.Range(recoilAngleMinMax.x, recoilAngleMinMax.y);
            //recoilAngle = Mathf.Clamp(recoilAngle, 0, 5);
        }
    }

    public void Reload()
    {
        if (!_isReloading && _projectilesRemainingInMagazine != ProjectilesPerMagazine)
            StartCoroutine(AnimateReload());
    }

    IEnumerator AnimateReload()
    {
        _isReloading = true;
        yield return new WaitForSeconds(0.2f);

        float reloadSpeed = 1f / ReloadTime;
        float percent = 0;
        Vector3 initialRot = transform.localEulerAngles;
        float maxReloadAngle = 30;

        while (percent < 1)
        {
            percent += Time.deltaTime * reloadSpeed;
            float interpolation = (-Mathf.Pow(percent, 2) + percent) * 4;
            float reloadAngle = Mathf.Lerp(0, maxReloadAngle, interpolation);
            transform.localEulerAngles = initialRot + Vector3.left * reloadAngle;

            yield return null;
        }

        _isReloading = false;
        _projectilesRemainingInMagazine = ProjectilesPerMagazine;
    }

    public void Aim(Vector3 _aimPoint)   
    {
        if (!_isReloading)
            transform.LookAt(_aimPoint);
    }

    public void OnTriggerHold()      
    {
        Shoot();
        _triggerReleasedSinceLastShot = false;
    }

    public void OnTriggerRelease()    
    {
        _triggerReleasedSinceLastShot = true;
        _shotsRemainingInBurst = BurstCount;
    }
}