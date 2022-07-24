using UnityEngine;

public class SkillTargetFX : MonoBehaviour
{
    [SerializeField]
    private ParticleSystem FXEplosion;
    private Vector3 _positionToPlay;
    private ParticleSystem _myFXExplosion;
    private bool _myOwnerVitals;

    public void FXPlay()
    {
        _positionToPlay = new Vector3(transform.position.x, transform.position.y + 1.2f, transform.position.z);
        _myFXExplosion =  Instantiate(FXEplosion, _positionToPlay, Quaternion.identity);
        Invoke("StopFX", 3f);
    }


    private void Start()
    {
        _myOwnerVitals = GetComponent<Vitals>().IsAlive();

    }


    private void Update()
    {
        if(_myFXExplosion != null)
        _myFXExplosion.transform.position = new Vector3(this.gameObject.transform.position.x, this.gameObject.transform.position.y + 1.2f, this.gameObject.transform.position.z);

        if (!_myOwnerVitals)
            StopFX();
    }

    public void StopFX()
    {
        _myFXExplosion.Stop();
        Destroy(_myFXExplosion);
    }
}
