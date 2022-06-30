//using System.Collections;
//using UnityEngine;

//public class TestScript : MonoBehaviour
//{
//    public enum FireMode { Auto, Burst, Single }
//    public FireMode fireMode;

//    public Transform[] projectileSpawnPoint;
//    public Projectile projectile;

//    public float msBetweenShots = 100;
//    public float muzzleVelocity = 35;
//    public int burstCount;
//    public int projectilesPerMagazine;
//    bool isReloading;
//    public float reloadTime = 0.3f;

//    [Header("Recoil")]
//    public Vector2 kickMinMax = new Vector2(0.05f, 2f);
//    public Vector2 recoilAngleMinMax = new Vector2(3, 5);
//    public float timeOfReturnToPosition = 0.1f;
//    Vector3 recoilSmoothDampVelocity;
//    private float recoilRotSmoothDampVelocity;
//    private float recoilAngle;

//    private float nextShotTime;

//    private bool triggerReleasedSinceLastShot;
//    private int shotsRemainingInBurst;
//    private int projectilesRemainingInMagazine;

//    void Start()
//    {
//        shotsRemainingInBurst = burstCount;
//        projectilesRemainingInMagazine = projectilesPerMagazine;
//    }

//    void LateUpdate()
//    {
//        transform.localPosition = Vector3.SmoothDamp(transform.localPosition, Vector3.zero, ref recoilSmoothDampVelocity, timeOfReturnToPosition);
//        recoilAngle = Mathf.SmoothDamp(recoilAngle, 0, ref recoilRotSmoothDampVelocity, timeOfReturnToPosition);
//        transform.localEulerAngles = transform.localEulerAngles + Vector3.left * recoilAngle;

//        if (!isReloading && projectilesRemainingInMagazine == 0)
//        {
//            Reload();
//        }
//    }

//    void Shoot()
//    {
//        if (!isReloading && Time.time > nextShotTime && projectilesRemainingInMagazine > 0)
//        {
//            if (fireMode == FireMode.Burst)
//            {
//                if (shotsRemainingInBurst == 0)
//                {
//                    return;
//                }
//                shotsRemainingInBurst--;
//            }
//            else if (fireMode == FireMode.Single)
//            {
//                if (!triggerReleasedSinceLastShot)
//                {
//                    return;
//                }
//            }
//            for (int i = 0; i < projectileSpawnPoint.Length; i++)
//            {
//                if (projectilesRemainingInMagazine == 0)
//                {
//                    break;
//                }
//                projectilesRemainingInMagazine--;
//                nextShotTime = Time.time + msBetweenShots;
//                Projectile newProjectile = Instantiate(projectile, projectileSpawnPoint[i].position, projectileSpawnPoint[i].rotation) as Projectile;
//                newProjectile.SetSpeed(muzzleVelocity);
//            }
//            transform.localPosition -= Vector3.forward * Random.Range(kickMinMax.x, kickMinMax.y);
//            recoilAngle += Random.Range(recoilAngleMinMax.x, recoilAngleMinMax.y);
//            recoilAngle = Mathf.Clamp(recoilAngle, 0, 25);
//        }
//    }

//    public void Reload()
//    {
//        if (!isReloading && projectilesRemainingInMagazine != projectilesPerMagazine)
//        {
//            StartCoroutine(AnimateReload());
//        }
//    }

//    IEnumerator AnimateReload()
//    {
//        isReloading = true;
//        yield return new WaitForSeconds(0.2f);

//        float reloadSpeed = 1f / reloadTime;
//        float percent = 0;
//        Vector3 initialRot = transform.localEulerAngles;
//        float maxReloadAngle = 30;

//        while (percent < 1)
//        {
//            percent += Time.deltaTime * reloadSpeed;
//            float interpolation = (-Mathf.Pow(percent, 2) + percent) * 4;
//            float reloadAngle = Mathf.Lerp(0, maxReloadAngle, interpolation);
//            transform.localEulerAngles = initialRot + Vector3.left * reloadAngle;

//            yield return null;
//        }

//        isReloading = false;
//        projectilesRemainingInMagazine = projectilesPerMagazine;
//    }

//    public void Aim(Vector3 aimPoint)
//    {
//        if (!isReloading)
//        {
//            transform.LookAt(aimPoint);
//        }
//    }

//    public void OnTriggerHold()
//    {
//        Shoot();
//        triggerReleasedSinceLastShot = false;
//    }

//    public void OnTriggerRelease()
//    {
//        triggerReleasedSinceLastShot = true;
//        shotsRemainingInBurst = burstCount;
//    }
//}