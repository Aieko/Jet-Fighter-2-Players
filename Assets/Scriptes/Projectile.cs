
using System.Collections;
using UnityEngine;
using Object = UnityEngine.Object;

public class Projectile : MonoBehaviour
{
    #region  components

    private Animator _animator;
    private SpriteRenderer _spriteRenderer;

    #endregion

    
    #region private variables

    private float _spawnTime;
    private float _currentSpeed;
    
    private int _clusters;

    private bool _isWhite;
    private bool _isAnimationFinished;
    private bool _hit;

    private Transform _target;

    #endregion

    [SerializeField]
    private float acceleration = 2f;
    [SerializeField]
    private float accelerationSpeed = 2f;
    [SerializeField]
    private float startSpeed = 20f;
    [SerializeField]
    private float maxSpeed = 150f;
    [SerializeField]
    private float lifeTime = 5f;
    [SerializeField]
    private bool shouldAccelerate;
    [SerializeField]
    private bool shouldExplode;
    [SerializeField]
    private bool homingMissile;
    [SerializeField]
    private float rotationSpeed = 150f;
    [SerializeField]
    private float missileActivationDelay = 0.5f;


    private void Awake()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _animator = GetComponent<Animator>();
        
    }

    // Start is called before the first frame update
    private void Start()
    {
        _isAnimationFinished = false;
        _spawnTime = Time.time;
        _currentSpeed = startSpeed;
        if (shouldExplode)
        {
            transform.localScale *= 2f;
        }
    }

    private void FixedUpdate()
    {
        Move();

        CheckExplosion();

        GameManager.Instance.ConstrainToMap(gameObject);
    }

    public void Shoot(int playerNum)
    {
        if (playerNum == 0)
        {
            _spriteRenderer.color = Color.black;
            _isWhite = false;
            _target = GameObject.FindGameObjectWithTag("Player2").transform;
            return;
        }

        _target = GameObject.FindGameObjectWithTag("Player1").transform;
        _isWhite = true;
    }

    private void Move()
    {
        if (_hit) return;

        if (_currentSpeed < maxSpeed && shouldAccelerate)
        {
            StartCoroutine(Accelerate());
        }

        if (homingMissile && Time.time >= _spawnTime + missileActivationDelay)
        {
            SelfHoming();
        }

        transform.Translate(transform.up * (_currentSpeed * Time.deltaTime), Space.World);
    }

    private void SelfHoming()
    {
        var direction = (Vector2)(_target.position - transform.position);

        if (direction.magnitude <= 10)
        {
            homingMissile = false;
            return;
        }

        direction.Normalize();
        float dotProduct = Vector2.Dot(direction, transform.right);

        if (dotProduct == 0)
        {
            return;
        }
        if (dotProduct > 0)
        {
            transform.RotateAround(transform.position, Vector3.back, rotationSpeed * Time.deltaTime);
        }
        else if (dotProduct < 0)
        {
            transform.RotateAround(transform.position, Vector3.forward, rotationSpeed * Time.deltaTime);
        }
    }

    private IEnumerator Accelerate()
    {
        _currentSpeed += acceleration;

           yield return new WaitForSeconds(accelerationSpeed);
    }

    private void Explose()
    {
        var bulletDirection = 0f;
        GameManager.Instance.PlayFireSound((AudioClip)Resources.Load("Sounds/bombExplosion"));
        for (int i = 0; i < 7; i++)
        {

            var spread = Quaternion.Euler(transform.rotation.eulerAngles + new Vector3(0f, 0f, bulletDirection));
            bulletDirection += 60f;
            _clusters++;
            var bullet = Instantiate((GameObject)Resources.Load("Projectiles/Bullet"), transform.position, spread);
            
            var playerNum = 0;
            if (_isWhite) playerNum = 1;
            bullet.transform.GetComponent<Projectile>().Shoot(playerNum);
        }
    }

    private void CheckExplosion()
    {
        if (Time.time >= _spawnTime + lifeTime || _hit)
        {
            if (shouldExplode && _clusters < 6)
            {
                _currentSpeed = 0f;
                maxSpeed = 0f;
                transform.localScale /= 2;
                Explose();
            }
            _animator.SetTrigger("Explosion");

            if (_isAnimationFinished)
            {
                Object.Destroy(gameObject);
            }

        }
    }

    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (!_isWhite)
        {
            if (collider.gameObject.transform.CompareTag("Player2"))
            {
                _hit = true;
                GameManager.Instance.SetScore(0);
            
            }
        }
        else
        {
            if (collider.gameObject.transform.CompareTag("Player1"))
            {
                _hit = true;
                GameManager.Instance.SetScore(1);
            }
        }
        
    }

    public void AnimationFinishedTrigger()
    {
        _isAnimationFinished = true;
    }
}
