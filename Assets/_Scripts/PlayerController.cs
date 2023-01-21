using System.Collections;
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

    [Header("Component References")]
    [Tooltip("Set reference to StringReference script.")]
    [SerializeField]
    StringReferences _references;

    [Header("Movement Variables")]
    [Tooltip("Set the movement speed of the player.")]
    [SerializeField]
    int _moveSpeed = 6;
    [Tooltip("Set the jump force of the player.")]
    [SerializeField]
    int jumpForce = 1;
    Vector2 _moveDirection;

    [Header("Raycast Configurations")]
    [Tooltip("Select ground layer.")]
    [SerializeField]
    LayerMask _whatIsGround;
    [Tooltip("Calculate distance of raycast to ground")]
    [SerializeField]
    float _distanceFromGround;
    [Tooltip("Calculate distance of raycast to wall")]
    [SerializeField]
    float _distanceFromWall;
    int _facingDirection = 1;

    [Header("For Testing")]
    [SerializeField]
    bool _isGrounded;
    [SerializeField]
    bool _isWall;

    //Jump Parametes
    bool _canDoubleJump = false;
    float _jumpAnimationDelay = 0.03f;

    //Animation and State
    string _currentState;
    string _idleAnimation, _runAnimation, _jumpAnimation;
    string _verticalWallAnimation2;

    enum AnimStates
    {
        idle,
        run,
        jump,
        vertical_wall2

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
        _verticalWallAnimation2 = AnimStates.vertical_wall2.ToString();
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, new Vector3(transform.position.x, transform.position.y - _distanceFromGround, transform.position.z));
        Gizmos.DrawLine(transform.position, new Vector3(transform.position.x + _distanceFromWall * _facingDirection, transform.position.y, transform.position.z));
    }

    // Update is called once per frame
    void Update()
    {
        CollisionChecks();
        WallHang();

        _moveDirection = _move.ReadValue<Vector2>();
    }

    private void FixedUpdate()
    {
        Move();
    }

    private void CollisionChecks()
    {
        _isGrounded = Physics2D.Raycast(transform.position, Vector2.down, _distanceFromGround, _whatIsGround);

        _isWall = Physics2D.Raycast(transform.position, Vector2.right * _facingDirection, _distanceFromWall, _whatIsGround);
    }
    private void Move()
    {
        _rb.velocity = new Vector2(_moveDirection.x * _moveSpeed, _rb.velocity.y);

        if (_rb.velocity.x != 0)
        {
            //Handles animations for in the air and on the ground
            if (_rb.velocity.y == 0)
                ChangeAnimationState(_runAnimation);
            else if (_rb.velocity.y != 0 && !_isWall)
                ChangeAnimationState(_jumpAnimation);

            ChangeDirection();
        }

        if (_rb.velocity.x == 0 && _rb.velocity.y == 0)
            ChangeAnimationState(_idleAnimation);
    }

    private void ChangeDirection()
    {
        if (_rb.velocity.x > 0)
        {
            transform.localScale = new Vector2(1, 1); //Player face right
            _facingDirection = 1; //Raycast face right
        }

        else if (_rb.velocity.x < 0)
        {
            transform.localScale = new Vector2(-1, 1); //Player face left
            _facingDirection = -1; //Raycast face left
        }
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
                StartCoroutine(StartJumpAnimationRoutine()); //Replays jump animation
                Jump();
            }
        }
    }

    IEnumerator StartJumpAnimationRoutine()
    {
        _anim.SetBool(_references.DidDoubleJumpBool, true);
        yield return new WaitForSeconds(_jumpAnimationDelay);
        _anim.SetBool(_references.DidDoubleJumpBool, false);
    }

    private void Jump()
    {
        if(!_isWall) //Enables player to jump off walls but only when they face away
        _rb.velocity = new Vector2(_rb.velocity.x, jumpForce);
    }

    private void WallHang()
    {
        if (_isWall && !_isGrounded)
        {
            _rb.velocity = new Vector2(_rb.velocity.x, _rb.velocity.y * 0.99f);
            ChangeAnimationState(_verticalWallAnimation2);
            _canDoubleJump = true;
        }
    }

    public void ChangeAnimationState(string newState)
    {
        _anim.StopPlayback();

        //stop the same animation from interrupting itself
        if (_currentState == newState)
            return;

        //play the animation
        _anim.Play(newState);

        //reassign the current state
        _currentState = newState;
    }
}
 