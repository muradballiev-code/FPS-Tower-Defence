using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField]
    private float _speed = 100f; //bullet speed
    [SerializeField]
    private float _lifeTime = 3f; //bullet lifeime after launch

    private Rigidbody _bulletRb; 
    private TrailRenderer _trail; //bullet trail component

    private void Awake()
    {
        _bulletRb = GetComponent<Rigidbody>();
        _trail = GetComponent<TrailRenderer>();
    }

    // Start is called before the first frame update
    void Start()
    {
        Destroy(gameObject, _lifeTime);
    }

    public void BulletShoot(Vector3 direction)
    {
        _bulletRb.velocity = direction * _speed;

        //here we remove the fired bullet effect
        if (_trail != null)
        {
            _trail.transform.parent = null;
            Destroy(_trail.gameObject, 0.2f);
        }
    }

    //plan B after Physics.Raycast
    private void OnTriggerEnter(Collider other)
    {
        /*
        if (other.gameObject.tag == "Enemy")
        {
            Enemy damage = other.gameObject.GetComponent<Enemy>();
            damage.EnemyDamage();
        }

        if (other.gameObject.tag == "Barrel")
        {
            Barrel damage = other.gameObject.GetComponent<Barrel>();
            damage.BarrelDamage();
        }

        if (other.gameObject.tag == "Barrier")
        {
            Barrier damage = other.gameObject.GetComponent<Barrier>();
            damage.BarrierDamage();

            Debug.Log("sadasd");
        }
        */
    }
}
