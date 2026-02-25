using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Barrel : MonoBehaviour
{
    private int _health = 3;
    [SerializeField]
    private GameObject _wholeBarrel; //whole barrel gameobject that will be used before explosion
    [SerializeField]
    private GameObject _explodeBarrel; //broken barrel gameobject that will be used for the explosion

    [SerializeField]
    private float _explosionRadius = 5f; //explosion radius
    [SerializeField]
    private float _explosionForce = 700f; //explosion force that will be used for AddExplosionForce()
    [SerializeField]
    private Rigidbody[] _pieces; //set pieces count from_explodeBarrel object

    [SerializeField]
    private AudioClip _barrelHit;
    [SerializeField]
    private AudioSource _barrelSource;

    [SerializeField]
    private LayerMask _enemyLayer; //will be check only enemy collider

    [SerializeField]
    private GameObject _explosionPrefab;

    private void Awake()
    {
        _explodeBarrel.SetActive(false); 
    }

    //Call method From FPS script when Raycast hit
    public void BarrelDamage()
    {
        _health--;

        //chech how much health left, IF 0
        if (_health <= 0)
        {
            //then call method
            Explode();
            return;
        }

        _barrelSource.PlayOneShot(_barrelHit);
    }

    //Call method
    public void Explode()
    {
        _wholeBarrel.SetActive(false);
        _explodeBarrel.SetActive(true);

        //check how many _pieces the ExplodeBarrel object has and loop them
        for (int i = 0; i < _pieces.Length; i++)
        {
            if (_pieces[i] != null)
            {
                //apply a force to create a spread of pieces
                _pieces[i].AddExplosionForce(_explosionForce, transform.position, _explosionRadius);
            }
        }

        //instantiate explosion and save it into reference
        GameObject explosion = Instantiate(_explosionPrefab, transform.position, Quaternion.identity);

        //check if enemy is close to the explosion
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, _explosionRadius, _enemyLayer);

        foreach (Collider hit in hitColliders)
        {
            if (hit != null)
            {
                //call damage method from Enemy script
                hit.gameObject.GetComponent<Enemy>().ExplosionDamage();
            }
        }

        Destroy(explosion, 5f);
        //switch off gameobject collider and destroy the object afer 5sec
        GetComponent<Collider>().enabled = false;
        Destroy(this.gameObject, 5f);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, _explosionRadius);
    }
}
