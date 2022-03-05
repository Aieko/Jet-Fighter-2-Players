
using UnityEngine;
using Quaternion = UnityEngine.Quaternion;
using Random = UnityEngine.Random;
using Vector2 = UnityEngine.Vector2;
using Vector3 = UnityEngine.Vector3;

public class PlayerController : MonoBehaviour
{
    public class JetFighter
    {
        private readonly KeyCode _left;
        private readonly KeyCode _right;
        private readonly KeyCode _fire;

        private readonly GameObject _sceneObject;

        private readonly Weapon1 _minigun;
        private readonly Weapon2 _rocketLauncher;
        private readonly Weapon3 _bomb;

        private readonly int _playerNum;
        private int _currentWeaponNum;
        private Weapon _currentWeapon;

        private const float RotationSpeed = 220f;
        private float _speed;
        private float _switchCoolDown = 2f;
        private float _lastTimeSwitched;

        public JetFighter(int playerNum, GameObject sceneObject, KeyCode left, KeyCode right,
                          KeyCode fire)
        {
            _minigun = new Weapon1(playerNum);
            _rocketLauncher = new Weapon2(playerNum);
            _bomb = new Weapon3(playerNum);
            _currentWeapon = _minigun;
            _currentWeaponNum = 1;
            _speed = 70f;
            _playerNum = playerNum;
            _sceneObject = sceneObject;
            _left = left;
            _right = right;
            _fire = fire;
        }

        private void SwitchWeapon()
        {
            if (Time.time < _lastTimeSwitched+_switchCoolDown)
            {
                Debug.Log("Weapon is reloading, can't switch!");
                return;
            }

            switch (_currentWeaponNum)
            {
                case 1:
                    _currentWeaponNum = 2;
                    _currentWeapon = _rocketLauncher;
                    break;
                case 2:
                    _currentWeaponNum = 3;
                    _currentWeapon = _bomb;
                    break;
                case 3:
                    _currentWeaponNum = 1;
                    _currentWeapon = _minigun;
                    break;
            }
            _lastTimeSwitched = Time.time;
            Debug.Log("Player " + _playerNum + " switched weapon type to " + _currentWeapon);
        }

        private void Shoot()
        {
            var rotation = Quaternion.LookRotation(Vector3.forward, _sceneObject.transform.up);
            _currentWeapon.Fire(_sceneObject.transform, rotation);
        }

        public void Move()
        {
            Vector2 facingDirection = _sceneObject.transform.up;

            if (Input.GetKey(_left))
            {
               _sceneObject.transform.RotateAround(_sceneObject.transform.position, Vector3.forward, RotationSpeed * Time.deltaTime);
            }

            if (Input.GetKey(_right))
            {
                _sceneObject.transform.RotateAround(_sceneObject.transform.position, Vector3.back, RotationSpeed * Time.deltaTime);
            }

            _sceneObject.transform.Translate(facingDirection * _speed * Time.deltaTime, Space.World);
        }

        public void CheckSwitchWeapon()
        {
            if (Input.GetButtonDown("SwitchWeapon" + _playerNum))
            {
                SwitchWeapon();
            }
        }

        public void CheckFire()
        {
            if (Input.GetKey(_fire))
            {
                Shoot();
            }
        }

        public void SetSpeed(float speed)
        {
            if (speed <= 100f)
                this._speed = speed;
        }

    }

    public GameObject[] players;

    private JetFighter _p1;
    private JetFighter _p2;

    // Start is called before the first frame update
    private void Start()
    {
        foreach (var player in players)
        {
            player.transform.position = new Vector2(Random.Range(0f, GameManager.Instance.canvasWidth), Random.Range(0f,GameManager.Instance.canvasHeight));
            player.gameObject.transform.GetComponent<Animator>().SetBool("Move", true);
        }

        _p1 = new JetFighter(0, players[0],  KeyCode.LeftArrow, KeyCode.RightArrow, KeyCode.Keypad5);
        _p2 = new JetFighter(1, players[1], KeyCode.A, KeyCode.D, KeyCode.Space);

    }

    private void Update()
    {
        _p1.CheckFire();
        _p2.CheckFire();

        _p1.CheckSwitchWeapon();
        _p2.CheckSwitchWeapon();
    }

    private void FixedUpdate()
    {
        _p1.Move();
        _p2.Move();

        foreach (var player in players)
        {
            GameManager.Instance.ConstrainToMap(player);
        }
    }

    //DEPRECETED CODE WITH DIFFERENT MOVING SYSTEM

    /*


    private void Player1MovingController()
    {
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");

        Vector2 movementDirection = new Vector2(horizontalInput, verticalInput);
        Vector2 facingDirection = Players[0].transform.up;
        float inputMagnitude = Mathf.Clamp01(movementDirection.magnitude);

        if (Input.GetKey(KeyCode.Keypad5) && Time.time >= P1LastTimeFire + reloadTime)
        {
            Fire(facingDirection, 0);
        }

        GoForward(movementDirection, inputMagnitude, 0);
    }

    private void Player2MovingController()
    {
        float horizontalInput = Input.GetAxis("Horizontal1");

        Vector2 facingDirection = Players[1].transform.up;
        
        movementDirection = new Vector2(horizontalInput, 1f);

        float inputMagnitude = Mathf.Clamp01(movementDirection.magnitude);

        if (Input.GetKey(KeyCode.Space) && Time.time >= P2LastTimeFire+reloadTime)
        {
            Fire(facingDirection, 1);
        }

        GoForward(movementDirection, inputMagnitude, 1);
    }

    private void GoForward(Vector2 _md, float _im, int playerNum)
    {
        if (Mathf.Abs(_md.x) >= 0.01)
        {
            _md.y -= Mathf.Abs(_md.x);
        }

        if (_md.y <= 0f)
        {
            _md.y -= Mathf.Abs(_md.x);
        }

        Players[playerNum].gameObject.transform.Translate(_md*speed * _im * Time.deltaTime, Space.World);

        if (_md != Vector2.zero)
        {
            var toRotation = Quaternion.LookRotation(Vector3.forward, _md);
            Players[playerNum].gameObject.transform.rotation = Quaternion.RotateTowards(Players[playerNum].gameObject.transform.rotation, toRotation, rotationSpeed * Time.deltaTime);
        }
       
    }

 
    */
}
