using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour, IMovement
{
    public InputActionAsset _inputActions;
    
    InputAction _moveAction;
    InputAction _lookAction;
    InputAction _jumpAction;
    InputAction _attackAction;

    InputAction _pauseActionPlayer;
    InputAction _pauseActionUI;


    Vector2 _movement;
    Vector2 _look;

    Vector3 _direction;
    
    public float _moveSpeed = 3f;
    public float _rotateSpeed = 3f;
    public float _jumpSpeed = 3f;

    Animator _animator;
    Rigidbody _rigidbody;

    public GameObject PauseDisplay;

    private void OnEnable()
    {
        _inputActions.FindActionMap("Player").Enable();
    }

    private void OnDisable()
    {
        _inputActions.FindActionMap("Player").Disable();
    }

    void Awake()
    {
        _moveAction = InputSystem.actions.FindAction("Move");
        _lookAction = InputSystem.actions.FindAction("Look");
        _jumpAction = InputSystem.actions.FindAction("Jump");
        _attackAction = InputSystem.actions.FindAction("Attack");

        _pauseActionPlayer = InputSystem.actions.FindAction("Player/Pause");
        _pauseActionUI = InputSystem.actions.FindAction("UI/Pause");

        _rigidbody = GetComponent<Rigidbody>();
        _animator = GetComponent<Animator>();
    }

    private void Update()
    {
        _movement = _moveAction.ReadValue<Vector2>();
        _look = _lookAction.ReadValue<Vector2>();

        if (_jumpAction.WasPressedThisFrame())
        {
            Jump();
        }
        if (_attackAction.WasPressedThisFrame())
        {
            Attack();
        }

        DiplayPause();
    }

    private void FixedUpdate()
    {
        Move();
    }

    // 직선 이동, 좌우는 회전 후 전진
    public void Move()
    {
        _animator.SetFloat("Speed", _movement.magnitude);
        Vector3 input = new Vector3(_movement.x, 0, _movement.y);

        if (_movement.magnitude > 0) 
        {
            // toward는 이동 방향으로, upward는 위로 고정해서 회전 방향 지정
            Quaternion targetRotation = Quaternion.LookRotation(input, Vector3.up);
            // 회전
            transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, _rotateSpeed * Time.deltaTime);
        }
        // 이동
        _rigidbody.MovePosition(_rigidbody.position + input * _moveSpeed * Time.deltaTime);
    }

    // 오브젝트 점프 시키고
    // 점프 트리거로 점프 애니메이션 재생
    private void Jump()
    {
        _rigidbody.AddForceAtPosition(new Vector3(0, 5f, 0), Vector3.up, ForceMode.Impulse);
        _animator.SetTrigger("Jump");
    }

    public void Attack()
    {
        // 공격 호출
        Debug.Log("공격");
        _animator.SetTrigger("Attack");
    }

    // 게임 일시 정지 후 UI 입력/Player 입력 활성화
    private void DiplayPause()
    {
        if (_pauseActionPlayer.WasPressedThisFrame())
        {
            PauseDisplay.SetActive(true);
            _inputActions.FindActionMap("Player").Disable();
            _inputActions.FindActionMap("UI").Enable();
        }
        else if (_pauseActionUI.WasPressedThisFrame())
        {
            PauseDisplay.SetActive(false);
            _inputActions.FindActionMap("UI").Disable();
            _inputActions.FindActionMap("Player").Enable();
        }
    }
}
