/*
 * Author: Wong Chao Hao
 * Date Created: 31/7/2022
 * Description: Handles Player movements, looks and miscellaneous
 * Last Edit: 31/7/2022
 */

using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]
public class PlayerMove : MonoBehaviour
{
    #region General Variables

    /// <summary>
    /// contains the playercamera
    /// </summary>
    public GameObject playerCam;

    /// <summary>
    /// contains the player's rigidbody
    /// </summary>
    public Rigidbody playerContact;
    
    #endregion

    #region Looking Variables (Mouse-Camera)

    /// <summary>
    /// Handles Horizontal Look Axis (left <-> right)
    /// </summary>
    private float _horizontalLook;

    /// <summary>
    /// Handles Vertical Look Axis (up ^ | v down)
    /// </summary>
    private float _verticalLook;
    
    /// <summary>
    /// Player Death Boolean
    /// </summary>
    public bool isDead;

    /// <summary>
    /// Handles Horizontal Lookspeed
    /// </summary>
    /// Preset: Set to 12
    public float horizontalRotationModifier = 12f;

    /// <summary>
    /// Handles Horizontal Lookspeed
    /// </summary>
    /// Preset: Set to 12
    public float verticalRotationModifier = 12f;
    
    #endregion

    #region Movement Variables (Keypress-Action)

    #region >>> Walking / BaseMovement <<<

    /// <summary>
    /// contains movement Speed value
    /// (NOTE: left-right movement should be 1/2 of front-back)
    /// </summary>
    /// Preset: Set to 3f (base)
    private float _moveSpeed;
    public float baseMoveSpeed = 3f; 
        
    /// <summary>
    ///  contains Vector3 for movement (to be added into transform)
    /// </summary>
    private Vector3 _appliedMovement = Vector3.zero;

    /// <summary>
    ///  contains Vector2 Input from Unity OnMove Event
    /// </summary>
    private Vector2 _retrievedInput;
    
    #endregion

    #region >>> Jumping <<<

    /// <summary>
    /// contains bool to check if player is grounded;
    /// </summary>
    [SerializeField]
    private bool _inAir;

    /// <summary>
    /// contains raycast distance for grounded checker
    /// </summary>
    /// Preset: Set to 0.949f
    public float groundDetectDist = 0.95f;

    /// <summary>
    /// contains strength of jump
    /// </summary>
    /// Preset: Set to 5f
    public float jumpForce = 5f;
    
    #endregion

    #region >>> Sprinting <<<

    private bool _sprinting;
    public float sprintingSpeed = 6f;

    #endregion
    
    #endregion

    private void Awake()
    {
        // resets move speed to defaults
        _moveSpeed = baseMoveSpeed;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void Update()
    {
    }
    
    private void FixedUpdate()
    {
        // Frames
        //Debug.Log((1 / Time.deltaTime).ToString().Split(".")[0]);
        Looking();
        Movement();
        GroundedChecker();
    }

    /// <summary>
    /// Handles player looking Actions
    /// leftRight look > Body
    /// upDown look > Head/Camera
    /// </summary>
    private void Looking()
    {
        // if the player is dead, do not let the rest of the code run
        if (isDead)
        {
            return;
        }

        // current angle + angle change + lookSpeed + deltatime; (Horizontal Rotation > body), (Vertical Rotation > Head)
        transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles + Vector3.up * _horizontalLook * horizontalRotationModifier * Time.deltaTime);
        playerCam.transform.localRotation = Quaternion.Euler(new Vector3(playerCam.transform.rotation.eulerAngles.x - 1 * _verticalLook * verticalRotationModifier * Time.deltaTime, 180, 0));
    }

    /// <summary>
    /// Handles player movements, WASD
    /// </summary>
    private void Movement()
    {
        // Applies up down axis (front back axis in the case of the game) to FORWARD/BACK motion (W | S)
        _appliedMovement = transform.forward * _retrievedInput.y;
        // Applies left right axis to SIDE motion (A | D)
        _appliedMovement += transform.right * _retrievedInput.x;
        playerContact.MovePosition(transform.position - _appliedMovement * _moveSpeed * Time.deltaTime);
    }
    
    
    /// <summary>
    /// Uses Raycast to check if player is grounded
    /// </summary>
    private void GroundedChecker()
    {
        // shows the presence of raycast
        Debug.DrawLine(transform.position, transform.position - (transform.up * (groundDetectDist + 0.1f)));
        
        // If the raycast pointing down from player passes through object(s), state that player is not in air
        if (Physics.Raycast(transform.position, Vector3.down, out var hitInfo, groundDetectDist + 0.1f))
        {
            _inAir = false;
        }
        // If the raycast pointing down from player passes through no object, state that player is in air
        else
        {
            _inAir = true;
        }
    }

    /// <summary>
    /// static function for int to bool conversions
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    private static bool IntToBool(float input)
    {
        if (input == 1)
        {
            return true;
        }
        
        return false;
    }
    
    #region UnityEvents

    
    private void OnLook(InputValue value)
    {
        var actions = value.Get<Vector2>();
        _horizontalLook = actions.x;
        _verticalLook = actions.y;
    }

    private void OnMove(InputValue value)
    {
        _retrievedInput = value.Get<Vector2>();
    }

    private void OnJump()
    {
        // if player is still in the sky, dont allow player to jump
        if (_inAir)
        {
            return;
        }

        playerContact.AddForce(transform.up * jumpForce, ForceMode.Impulse);
    }

    private void OnSprint(InputValue value)
    {
        var output = value.Get<float>();
        
        // If player is not in contact with the ground, you cannot start sprinting
        if (_inAir) return;
        
        // if player is currently not running and output is true (is pressing shift)
        // running state starts
        if (!_sprinting && IntToBool(output))
        {
            _moveSpeed = sprintingSpeed;
            _sprinting = true;
        }
        
        // if player is currently running and output is false (no longer pressing shift)
        // running state stops
        else if (_sprinting && !IntToBool(output))
        {
            _moveSpeed = baseMoveSpeed;
            _sprinting = false;
        }
    }
    
    #endregion


}
