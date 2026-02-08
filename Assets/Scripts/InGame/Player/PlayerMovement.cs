using Photon.Pun;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(Rigidbody))]
public class PlayerMovement : MonoBehaviourPun
{
    public InputActionAsset _inputActions;
    
    InputAction _moveAction;
    InputAction _jumpAction;
    InputAction _interactAction;

    InputAction _pauseActionPlayer;
    InputAction _pauseActionUI;

    Vector2 _movement;
    
    public float _moveSpeed = 3f;
    public float _rotateSpeed = 3f;
    public float _jumpSpeed = 3f;

    Animator _animator;
    Rigidbody _rigidbody;

    public GameObject MenuCanvas;
    public GameObject PauseDisplay;

    bool _isGround= true;
    LayerMask _layerMask = 7;
    float raycastDistance = 0.4f;
    RaycastHit _raycastHit;


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
        MenuCanvas = GameObject.Find("MenuCanvas");
        PauseDisplay = MenuCanvas?.transform?.GetChild(0)?.gameObject;
        MenuCanvas.SetActive(false);

        if (!photonView.IsMine) return;
        
        _moveAction = InputSystem.actions.FindAction("Move");
        _jumpAction = InputSystem.actions.FindAction("Jump");
        _interactAction = InputSystem.actions.FindAction("Interact");

        _pauseActionPlayer = InputSystem.actions.FindAction("Player/Pause");
        _pauseActionUI = InputSystem.actions.FindAction("UI/Pause");

        _rigidbody = GetComponent<Rigidbody>();
        _animator = GetComponent<Animator>();
    }

    private void Update()
    {
        if (!photonView.IsMine) return;

        _movement = _moveAction.ReadValue<Vector2>();

        if (_isGround && _jumpAction.WasPressedThisFrame())
        {
            Jump();
        }

        // 점프 횟수 제한
        if(Physics.Raycast(transform.position, Vector3.down, out _raycastHit, raycastDistance))
            _isGround = true;
        else
            _isGround = false;

        //if (_interactAction.WasPressedThisFrame())
        //{
        //    _animator.SetBool("IsHolding", true);
        //    //Interact();
        //}

        //if (_interactAction.WasReleasedThisFrame())
        //{
        //    _animator.SetBool("IsHolding", false);
        //}

        DiplayPause();
    }

    private void FixedUpdate()
    {
        if (!photonView.IsMine) return;

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
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, _rotateSpeed * Time.deltaTime);
            //transform.Rotate(0, this._rotateSpeed * 2, 0, Space.World);
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
        _isGround = false;
    }

    public void Interact(GameObject puzzle)
    {
        DisplayButton displayButton = puzzle.GetComponent<DisplayButton>();
        Debug.Log("상호작용");
        _animator.SetTrigger("Interact");
        displayButton?.ClearPuzzle();
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("InteractablePuzzle") && _interactAction.IsPressed())
        {
            Interact(other.gameObject);
        }
    }

    // 게임 일시 정지 후 UI 입력/Player 입력 활성화
    private void DiplayPause()
    {
        if (_pauseActionPlayer.WasPressedThisFrame())
        {
            PauseDisplay.SetActive(true);
            PauseGame();
        }
        else if (_pauseActionUI.WasPressedThisFrame())
        {
            PauseDisplay.SetActive(false);
            ResumeGame();
        }
    }

    public void PauseGame()
    {
        _inputActions.FindActionMap("Player").Disable();
        _inputActions.FindActionMap("UI").Enable();
    }

    public void ResumeGame()
    {
        _inputActions.FindActionMap("UI").Disable();
        _inputActions.FindActionMap("Player").Enable();
    }
}
