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

    // Update is called once per frame
    void Update()
    {
        _moveDirection = _move.ReadValue<Vector2>();
    }

    private void FixedUpdate()
    {
        _rb.velocity = new Vector2(_moveDirection.x * _moveSpeed, _rb.velocity.y);
    }

    void Jump(InputAction.CallbackContext context)
    {
        if(context.performed)
        _rb.velocity = new Vector2(_rb.velocity.x, jumpForce);
    }
}
