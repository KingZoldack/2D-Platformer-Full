using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    //Player input
    PlayerControls_inputactions _playerControls;
    InputAction _move;
    InputAction _jump;

    //Components
    Rigidbody2D _rb;
    Animator _anim;

    [Header("Movement Variables")]
    [Tooltip("Set the movement speed of the player.")]
    [SerializeField]
    int _moveSpeed = 6;
    [Tooltip("Set the jump force of the player.")]
    [SerializeField]
    int jumpForce = 1;
    Vector2 _moveDirection;

    [SerializeField]
    LayerMask _whatIsGround;
    [SerializeField]
    float _distanceFromGround;
    [SerializeField]
    bool _isGrounded;
    bool _canDoubleJump = false;

    //Animation and State
    string _currentState;
    string _idleAnimation, _runAnimation, _jumpAnimation;

    enum AnimStates
    {
        idle,
        run,
        jump

    }

    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        _anim = GetComponent<Animator>();
        _playerControls = new PlayerControls_inputactions();
    }


    private void OnEnable()
    {
        _move = _playerControls.Player.Move;
        _move.Enable();

        _jump = _playerControls.Player.Jump;
        _jump.Enable();
        _jump.performed += Jump;
    }

    private void OnDisable()
    {
        _move.Disable();
        _jump.Disable();
    }

    // Start is called before the first frame update
    void Start()
    {
        //Setting Animations
        _idleAnimation = AnimStates.idle.ToString();
        _runAnimation = AnimStates.run.ToString();
        _jumpAnimation = AnimStates.jump.ToString();
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, new Vector3(transform.position.x, transform.position.y - _distanceFromGround, transform.position.z));
    }

    // Update is called once per frame
    void Update()
    {
        CollisionChecks();

        _moveDirection = _move.ReadValue<Vector2>();
    }

    private void FixedUpdate()
    {
        Move();
    }

    private void CollisionChecks()
    {
        _isGrounded = Physics2D.Raycast(transform.position, Vector2.down, _distanceFromGround, _whatIsGround);
    }
    private void Move()
    {
        _rb.velocity = new Vector2(_moveDirection.x * _moveSpeed, _rb.velocity.y);

        if (_rb.velocity.x != 0 && _rb.velocity.y == 0)
        {
            ChangeAnimationState(_runAnimation);

            if (_rb.velocity.x > 0)
                transform.localScale = new Vector2(1, 1);
            else
                transform.localScale = new Vector2(-1, 1);
        }
        if (_rb.velocity.x == 0 && _rb.velocity.y == 0)
            ChangeAnimationState(_idleAnimation);
    }

    void Jump(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            if (_isGrounded)
            {
                Jump();
                _canDoubleJump = true;
            }
            else if (_canDoubleJump)
            {
                _canDoubleJump = false;
                Jump();
            }
        }
        
    }

    private void Jump()
    {
        _rb.velocity = new Vector2(_rb.velocity.x, jumpForce);
        _anim.StopPlayback();
        ChangeAnimationState(_jumpAnimation);
    }

    public void ChangeAnimationState(string newState)
    {
        //stop the same animation from interrupting itself
        if (_currentState == newState)
            return;

        //play the animation
        _anim.Play(newState);

        //reassign the current state
        _currentState = newState;
    }
}
 