using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    private enum AiState
    {
        Idle,
        Moving,
        MovingToCover,
        Covering,
        Death
    }

    private int _health = 3;
    private int _score = 50;
    private int _killed = 1;
    private bool _isDead = false;
    private bool _deathProcessed = false;

    [SerializeField]
    private AiState _aiState;

    [SerializeField]
    private NavMeshAgent _agent;

    private int _currentPos = 0; //first movement point
    public float _stoppingDistance = 0.2f; //stop distance near moving points
    private float _searchRadius = 8f; //searching radius for cover
    private bool _isCovering = false; //covering status
    private bool _coverFind = false; //status if cover find
    private bool _startFind = false;

    private Transform[] _pointsPos; //set variable for positions of the movement points
    [SerializeField]
    private Transform[] _coverPos; //set variable for positions of the cover points

    [SerializeField]
    private Transform _targetCover; //set variable for the last positions of the cover point
    [SerializeField]
    private Transform _lastUsedCover; //create a variable to store the cover point for validation
    [SerializeField]
    private List<Transform> _forbiddenCovers = new List<Transform>(); //

    private float _coverTime;
    [SerializeField] private float _coverMinTime = 3f; //
    [SerializeField] private float _coverMaxTime = 6f; //

    [SerializeField]
    private Animator _enemyAnim;

    [SerializeField]
    private AudioClip _hitSound;
    [SerializeField]
    private AudioClip _deathSound;
    [SerializeField]
    private AudioClip _escapeSound;
    [SerializeField]
    private AudioSource _audioSource;

    void Awake()
    {
        _agent.autoBraking = false;

        //get all moivng points WayPoint script
        WayPoint points = FindFirstObjectByType<WayPoint>();
        //get all cover points from CoverPoint
        CoverPoint coverPoints = FindFirstObjectByType<CoverPoint>();

        _coverPos = coverPoints.points;

        

        if (points != null && points.points.Length > 0)
        {
            _pointsPos = points.points;

            _agent.SetDestination(_pointsPos[_currentPos].position);
            _aiState = AiState.Moving;
        }
        else
        {
            _aiState = AiState.Idle;
        }

        _forbiddenCovers.Clear();
    }

    // Update is called once per frame
    void Update()
    {
        switch (_aiState)
        {
            case AiState.Idle:
                Waiting();
            break;

            case AiState.Moving:

                _enemyAnim.SetBool("Move", true);
                _enemyAnim.SetBool("Cover", false);

                if (_isCovering)
                return;

                if (_startFind && Random.value < 0.4f)
                {
                    SearchCover();
                }

                if (_coverFind)
                return;

                if (!_agent.pathPending && _agent.remainingDistance < _stoppingDistance)
                {
                    GoToNextPoint();
                }

            break;

            case AiState.MovingToCover:
                if (!_agent.pathPending && _agent.remainingDistance < 1f)
                {
                    ArrivedAtCover();
                }
            break;

            case AiState.Covering:

                _enemyAnim.SetBool("Move", false);
                _enemyAnim.SetBool("Cover", true);

                CoveringState();

            break;

            case AiState.Death:
                if (_isDead == true && !_deathProcessed)
                {
                    Death();
                }
            break;

            default:
            break;
        }
    }

    public void Waiting()
    {
        _enemyAnim.SetBool("Move", false);
        _enemyAnim.SetBool("Cover", false);
    }

    public void GoToNextPoint()
    {
        _currentPos++;

        _startFind = true;

        Debug.Log(_currentPos + " -- " + _pointsPos.Length);

        if (_currentPos >= _pointsPos.Length)
        {
            Escaped();

            /*
            _currentPos = _pointsPos.Length - 1;
            _aiState = AiState.Idle;
            */
            return;
        }

        _agent.SetDestination(_pointsPos[_currentPos].position);
    }

    public void SearchCover()
    {
        /*
        Debug.Log(Random.value);

        if (Random.value > 0.8f)
        {
            return;
        }
        */

        Barrier best = null;

        foreach (var coverTransform in _coverPos)
        {
            Barrier cover = coverTransform.GetComponent<Barrier>();

            if (cover == null)
            continue;

            if (cover.IsBroken() == true)
            {
                _forbiddenCovers.Add(cover.transform);
                return;
            }

            float distance = Vector3.Distance(transform.position, cover.transform.position);

            if (_forbiddenCovers.Contains(cover.transform))
            continue;

            if (distance < _searchRadius && cover.transform != _lastUsedCover)
            {
                if (!cover._isBusy)
                {
                    best = cover;
                }
                else 
                {
                    _forbiddenCovers.Add(cover.transform);
                }
            }
        }

        if (best != null)
        {
            _targetCover = best.transform;
            best.Occupy(gameObject);
            _coverFind = true;
            _agent.SetDestination(best.transform.position);
            _aiState = AiState.MovingToCover;

            return;
        }
    }

    public void ArrivedAtCover()
    {
        _agent.isStopped = true;
        _agent.velocity = Vector3.zero;

        Vector3 dirRotation = (transform.position - _targetCover.position).normalized;
        dirRotation.y = 0;
        transform.rotation = Quaternion.LookRotation(dirRotation);

        _lastUsedCover = _targetCover;
        _forbiddenCovers.Add(_targetCover);

        _isCovering = true;
        _coverFind = false;
        _coverTime = Time.time + Random.Range(_coverMinTime, _coverMaxTime);
        _aiState = AiState.Covering;
    }


    void CoveringState()
    {
        float distance = Vector3.Distance(transform.position, _pointsPos[_currentPos].position);

        Barrier barrier = null;

        if (_targetCover != null)
        {
            barrier = _targetCover.GetComponent<Barrier>();
        }
        
        /*
        if (_isCovering && barrier.IsBroken() == true)
        {
            barrier.Free();

            _startFind = false;
            _agent.isStopped = false;
            _coverFind = false;
            _isCovering = false;
            _targetCover = null;
            _aiState = AiState.Moving;
            _agent.SetDestination(_pointsPos[_currentPos].position);

            return;
        }
        */

        if ((_isCovering && Time.time >= _coverTime) || barrier.IsBroken() == true)
        {
            barrier.Free();
            
            _startFind = false;
            _agent.isStopped = false;
            _coverFind = false;
            _isCovering = false;
            _targetCover = null;
            _aiState = AiState.Moving;

            if (distance < 5f)
            {
                _agent.SetDestination(_pointsPos[_currentPos + 1].position);
            }
            else
            {
                _agent.SetDestination(_pointsPos[_currentPos].position);
            }
        }
    }

    public void Death()
    {
        _deathProcessed = true;

        //If the enemy had cover then free cover point
        if (_targetCover != null)
        {
            Barrier barrier = _targetCover.GetComponent<Barrier>();
            if (barrier != null)
            {
                barrier.Free();
            }

            _targetCover = null;
        }

        _agent.isStopped = true;
        _agent.velocity = Vector3.zero;
        _agent.enabled = false;

        //GetComponent<NavMeshAgent>().enabled = false;
        GetComponent<Collider>().enabled = false;

        _enemyAnim.SetBool("Move", false);
        _enemyAnim.SetBool("Cover", false);
        _enemyAnim.SetTrigger("Die");
        _audioSource.PlayOneShot(_deathSound);

        //SpawnManager.Instance.ReturnEnemyToPool(this.gameObject);
        UIManager.Instance.UpdateScore(_score);
        UIManager.Instance.EnemyKilled(_killed);
    }

    //called from FPS script
    public void EnemyDamage()
    {
        _health--;

        if (_health <= 0)
        {
            _isDead = true;
            _aiState = AiState.Death;
            return;
        }

        _audioSource.PlayOneShot(_hitSound);
    }

    //Enemy death from explosion called from Barrel script
    public void ExplosionDamage()
    {
        _health = 0;
        _isDead = true;
        _aiState = AiState.Death;
    }

    public void Escaped()
    {
        _agent.isStopped = true;
        _agent.velocity = Vector3.zero;
        _agent.enabled = false;

        _aiState = AiState.Idle;

        _audioSource.PlayOneShot(_escapeSound);
        //AudioSource.PlayClipAtPoint(_escapeSound, transform.position);
        //SpawnManager.Instance.DestroyEnemyByOne(this.gameObject);
        UIManager.Instance.EnemyEscaped();
    }
}
