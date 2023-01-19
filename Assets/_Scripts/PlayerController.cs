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

    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
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
    }

    void Jump(InputAction.CallbackContext context)
    {
        if(context.performed && _isGrounded)
        _rb.velocity = new Vector2(_rb.velocity.x, jumpForce);
    }
}
 