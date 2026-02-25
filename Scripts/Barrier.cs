using System.Collections;
using System.Collections.Generic;
using Unity.Burst.CompilerServices;
using UnityEngine;

public class Barrier : MonoBehaviour
{
    private int _barrierHealth = 3;
    private float _minRechargeTime = 5f;
    private float _maxRechargeTime = 10f;
    private float _timer;
    private bool _isBroken = false;

    public bool _isBusy = false;
    public GameObject Occupier; 

    [SerializeField]
    private AudioClip _barrierHit;
    [SerializeField]
    private AudioClip _barrierChargeUp;
    [SerializeField]
    private AudioClip _barrierChargeDown;
    [SerializeField]
    private AudioSource _barrierSource;

    private Collider barrierCollider;
    private Renderer barrierRenderer;

    // Start is called before the first frame update
    void Start()
    {
        barrierCollider = GetComponent<Collider>();
        if (barrierCollider == null)
        {
            Debug.Log("Component Collider No found");
        }
        barrierRenderer = GetComponent<Renderer>();
        if (barrierRenderer == null)
        {
            Debug.Log("Component Renderer No found");
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (!_isBroken)
            return;

        if (Time.time >= _timer && _isBroken == true)
        {
            GetComponent<Collider>().enabled = true;
            GetComponent<Renderer>().enabled = true;

            _barrierSource.PlayOneShot(_barrierChargeUp);

            _barrierHealth = 3;

            _isBroken = false;
        }
    }

    public void BarrierDamage()
    {
        if (_barrierHealth <= 0)
        return;

        _barrierHealth--;

        if (_barrierHealth <= 0)
        {
            _timer = Time.time + Random.Range(_minRechargeTime, _maxRechargeTime);

            _barrierSource.PlayOneShot(_barrierChargeDown);

            GetComponent<Collider>().enabled = false;
            GetComponent<Renderer>().enabled = false;

            _isBroken = true;
            return;
        }

        _barrierSource.PlayOneShot(_barrierHit);
    }

    //return barrier status to Enemy script
    public bool IsBroken()
    {
        return _isBroken;
    }

    //used in the enemy script: checks if the object is taken, and occupies it if not
    public void Occupy(GameObject enemy)
    {
        _isBusy = true;
        Occupier = enemy;
    }

    //used in the enemy script: if occupied then free it
    public void Free()
    {
        _isBusy = false;
        Occupier = null;
    }
}
