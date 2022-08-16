/*
 * Author: Wong Chao Hao
 * Date Created: 31/7/2022
 * Description: Handles Player movements, looks and miscellaneous
 * Last Edit: 31/7/2022
 */

using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;
using UnityEngine.UIElements;
using Cursor = UnityEngine.Cursor;
using Debug = UnityEngine.Debug;
using Slider = UnityEngine.UI.Slider;

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

    /// <summary>
    /// contains the controller to the AI
    /// </summary>
    public Companion _aiController;

    /// <summary>
    /// contains the gamemanagerscript
    /// </summary>
    public GameManager _gameManager;

    /// <summary>
    /// bool if the player has recieved the AI
    /// </summary>
    public bool hasAI;

    /// <summary>
    /// contains the script for the playertasks
    /// </summary>
    public TaskBook storyUI;

    /// <summary>
    /// contains deathAnimator
    /// </summary>
    public Animator deathSequence;
    
    #endregion

    #region Objects

    /// <summary>
    /// Object that the player is currently holding
    /// </summary>
    public GameObject onHand;
    
    /// <summary>
    /// Objects that player can currently access
    /// </summary>
    public List<GameObject> inventory;

    /// <summary>
    /// All possible objects a player can pick up
    /// </summary>
    public List<GameObject> allItems;

    #endregion
    
    #region Looking Variables (Mouse-Camera)

    /// <summary>
    /// Handles Horizontal Look Axis (left <-> right)
    /// </summary>
    public float _horizontalLook;

    /// <summary>
    /// Handles Vertical Look Axis (up ^ | v down)
    /// </summary>
    public float _verticalLook;
    
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

    /// <summary>
    /// Sets distance of raycast from player
    /// </summary>
    public float interactionDistance = 1f;

    [SerializeField] private GameObject target;

    private float durationElapsed;

    private const float Speed = 1f;

    public int axisDirection = 1;

    
    #endregion

    #region Headbobbing Variables

    /// <summary>
    /// boolean to activate/deactive headbobbing effect
    /// </summary>
    public bool bobbingEffectActive = true;
    
    /// <summary>
    /// sets how "strong" the head bob effect will be
    /// </summary>
    [Range(0, 0.1f)] 
    public float amplitude = 0.0045f;

    private Vector3 _startPos;

    /// <summary>
    /// sets the time interval between head bobs
    /// </summary>
    [Range(0, 30)] 
    public float frequency = 8.5f;

    private float elapsedTime;
    
    #endregion

    #region Movement Variables (Keypress-Action)

    #region >>> Walking / BaseMovement <<<

    /// <summary>
    /// contains movement Speed value
    /// (NOTE: left-right movement should be 1/2 of front-back)
    /// </summary>
    /// Preset: Set to 3f (base)
    [SerializeField]
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

    /// <summary>
    /// takes note if player is currently running
    /// </summary>
    private bool _sprinting;
    
    /// <summary>
    /// speed of running player
    /// </summary>
    public float sprintingSpeed = 6f;

    public bool exhausted;

    public Slider staminaBar;

    public float drainSpeed = 0.1f;
    
    public float recoverSpeed = 0.05f;
    
    #endregion
    
    #endregion


    private void Awake()
    {
        _gameManager = FindObjectOfType<GameManager>();
        // resets move speed to defaults
        _moveSpeed = baseMoveSpeed;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        _startPos = playerCam.transform.localPosition;

        var playerInv = FindObjectOfType<inventoryLocator>().transform;
        if (hasAI) _aiController = FindObjectOfType<Companion>();
    }

    private void Update()
    {
        BobbingEffect();
        // FixedUpdate() > Update() > LateUpdate()
        // hover = false > hover = true (overridden) > color to be changed
        Interactions();
    }
    
    private void FixedUpdate()
    {
        // Frames
        //Debug.Log((1 / Time.deltaTime).ToString().Split(".")[0]);
        Looking();
        Movement();
        GroundedChecker();
        Recovery();
    }

    #region Movement and Looking Functions
    
    /// <summary>
    /// Handles player looking Actions
    /// leftRight look > Body
    /// upDown look > Head/Camera
    /// </summary>
    private void Looking()
    {
        //Debug.Log(axisDirection * _verticalLook * verticalRotationModifier * Time.deltaTime);
        // if the player is dead, do not let the rest of the code run
        if (isDead)
        {
            return;
        }
        // current angle + angle change + lookSpeed + deltatime; (Horizontal Rotation > body), (Vertical Rotation > Head)
        transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles + Vector3.up * _horizontalLook * horizontalRotationModifier * Time.deltaTime);
        playerCam.transform.localRotation = Quaternion.Euler(new Vector3(playerCam.transform.rotation.eulerAngles.x - axisDirection * _verticalLook * verticalRotationModifier * Time.deltaTime, 180, 0));
    }

    /// <summary>
    /// Handles player movements, WASD
    /// </summary>
    private void Movement()
    {
        StaminaBarUpdate();
        // Applies up down axis (front back axis in the case of the game) to FORWARD/BACK motion (W | S)
        _appliedMovement = transform.forward * _retrievedInput.y;
        // Applies left right axis to SIDE motion (A | D)
        _appliedMovement += transform.right * _retrievedInput.x;
        _appliedMovement.Normalize();
        playerContact.MovePosition(transform.position - _appliedMovement * _moveSpeed * Time.deltaTime);
    }

    private void Interactions(bool interact=false)
    {
        Debug.DrawLine(playerCam.transform.position, playerCam.transform.position + playerCam.transform.forward * interactionDistance);
        
        // player interactions may only interact with "Items to be picked up and UI
        var layerMasking = LayerMask.GetMask("ItemToPickup", "UI");
        if (Physics.Raycast(playerCam.transform.position, playerCam.transform.forward, out var hitInfo, interactionDistance, layerMasking, QueryTriggerInteraction.Collide))
        {
            // if raycast object is in UI layer
            if (hitInfo.transform.gameObject.layer == LayerMask.NameToLayer("UI"))
            {
                hitInfo.transform.GetComponent<ObjectPlacing>().hovering = true;

                // if raycast collides with trigger and interaction is done
                if (interact) hitInfo.transform.GetComponent<ObjectPlacing>().PlaceItem();
                return;
            }
            // if raycast object is interacted
            if (interact && hitInfo.transform.gameObject.layer == LayerMask.NameToLayer("ItemToPickup"))
            {
                //Debug.Log("Interacted with " + hitInfo.transform.gameObject.name);
                target = hitInfo.transform.gameObject;
                // if the item to be picked up is not a key item, add it to inventory
                if (target.tag == "companion")
                {
                    FindObjectOfType<companionPickup>().PickUp();
                    return;
                }
                if (target.tag != "keyItem")
                {
                    AddInventory(target.tag);
                }
                else
                {
                    var gameManager = FindObjectOfType<GameManager>();
                    var dictToCompare = gameManager.collectedKeyItems;
                    if (gameManager.sceneIndex == 0) //testingScene only for now
                    {
                        dictToCompare = gameManager.collectedKeyItems2;
                    }
                    var key = hitInfo.transform.GetComponent<keyItemIdentifier>().objectTypeText;
                    dictToCompare[key] = true;

                    var pedestal = FindObjectsOfType<PedestalController>();
                    foreach (var code in pedestal)
                    {
                        if (code.targetItemText != key) continue;;
                        code.swapUI();
                    }
                }
                target.SetActive(false);
            }
        }
    }

    /// <summary>
    /// Uses Raycast to check if player is grounded
    /// </summary>
    private void GroundedChecker()
    {
        // shows the presence of raycast
        Debug.DrawLine(transform.position, transform.position - (transform.up * (groundDetectDist + 0.1f)));

        var layerMasking = LayerMask.GetMask("Companion");

        // If the raycast pointing down from player passes through object(s), state that player is not in air
        if (Physics.Raycast(transform.position, Vector3.down,groundDetectDist + 0.1f, ~layerMasking))
        {
            _inAir = false;
        }
        // If the raycast pointing down from player passes through no object, state that player is in air
        else
        {
            _inAir = true;
        }
    }

    private void StaminaBarUpdate()
    {
        if (staminaBar.value >= 1) exhausted = false;
        if (staminaBar.value <= 0)
        {
            exhausted = true;
            _sprinting = false;
            _moveSpeed = baseMoveSpeed;
            amplitude = 0.0045f;
            frequency = 8.5f;
            return;
        }
        if (_sprinting && _retrievedInput != Vector2.zero)
        {
            staminaBar.value -= Time.deltaTime * drainSpeed;
            
        }
    }
    private void Recovery()
    {
        if (_sprinting && _retrievedInput != Vector2.zero) return;
        staminaBar.value += Time.deltaTime * recoverSpeed;
    }
    
    #endregion

    #region Headbobbing Functions
    
    /// <summary>
    /// Head bobbing effect when walking
    /// </summary>
    private void BobbingEffect()
    {
        // if head bob is not active, no head bob;
        if (!bobbingEffectActive) return;
        // cam reset should still work even if player is jumping
        ResetCam();
        // if player is in the air, no head bob;
        if (_inAir) return;
        // if player is not walking
        if (_retrievedInput == Vector2.zero) return;
        
        // calculates bobbing in both x and y axis (up-down && left-right)
        var bobMotion = Vector3.zero;
        //elapsedTime += Time.deltaTime;
        //  sine graph formulae : amplitude * (period) + lift
        // dev notes (remove this on final)
        // Try rotation of Lemniscate of Gerono curve, but cosine does not work
        // Tried Lissajous curve, extra /2 on amplitude makes bob too vigorous. (current implementation)
        // might just try simple cosine wave for xy direction (vector2) if there is time 
        bobMotion.y += Mathf.Sin(Time.time * frequency) * amplitude; // * Mathf.Cos(Time.time * frequency);
        bobMotion.x += Mathf.Sin(Time.time * frequency / 2) * amplitude; // 2 ;
        
        playerCam.transform.localPosition += bobMotion;
    }

    /// <summary>
    /// Brings back camera to starting position for next headbob loop
    /// </summary>
    private void ResetCam()
    {
        if (_retrievedInput == Vector2.zero)
        {
            playerCam.transform.localPosition = Vector3.Lerp(playerCam.transform.localPosition, _startPos, 12f * Time.deltaTime);
            return;
        }
        // if camera is not at starting pos, revert it back to starting pos with lerp
        if (playerCam.transform.localPosition == _startPos) return;
        playerCam.transform.localPosition = Vector3.Lerp(playerCam.transform.localPosition, _startPos, 1 * Time.deltaTime);
    }

    #endregion
    
    #region Inventory Management
        
    /// <summary>
    /// Sorts tag into identifiable index (based on allItems List)
    /// </summary>
    /// <param name="tag"></param>
    /// <returns>
    /// Index: tag
    /// 0 : TORCH
    /// </returns>
    private int TagToIndex(string tag)
    {
        tag = tag.ToUpper();
        if (tag == "TORCH")
        {
            storyUI.NextTask();
            return 0;
        }
        
        return 100;
    }

    /// <summary>
    /// adds object "picked up" to accessible inventory
    /// </summary>
    /// <param name="tag"></param>
    private void AddInventory(string tag)
    {
        if (onHand != null)
        {
            onHand.SetActive(false);
        }
        var newItem = allItems[TagToIndex(tag)];

        newItem.SetActive(true);
        inventory.Add(newItem);
        onHand = newItem;
    }
        
    #endregion
        
    #region Misc Functions
    
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
    
    #endregion

    
    
    #region UnityEvents

    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.tag == "Ghost")
        {
            deathSequence.SetBool("Scare", true);
        }
    }

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
        if (exhausted) return;
        var output = value.Get<float>();
        
        // If player is not in contact with the ground, you cannot start sprinting
        
        // if player is currently not running and output is true (is pressing shift)
        // running state starts
        if (!_sprinting && IntToBool(output) && !_inAir)
        {
            _moveSpeed = sprintingSpeed;
            _sprinting = true;
            amplitude = 0.01f;
            frequency = 12f;
            /*
            amplitude = 0.013f;
            frequency = 15f;
            */
        }
        
        // if player is currently running and output is false (no longer pressing shift)
        // running state stops
        if (_sprinting && !IntToBool(output))
        {
            _moveSpeed = baseMoveSpeed;
            _sprinting = false;
            amplitude = 0.0045f;
            frequency = 8.5f;
        }
    }

    private void OnFire()
    {
        if (onHand == null) return;
        if (onHand.gameObject.tag == "torch")
        {
            onHand.GetComponent<Torch>().ToggleLight();
        }
    }

    private void OnInteract()
    {
        Interactions(true);
    }

    private void OnUseCompanion()
    {
        if (!hasAI) return;
        if (_aiController == null) _aiController = FindObjectOfType<Companion>();
        _aiController.SetNewTarget();
    }

    private void OnToggleMenu()
    {
        _gameManager.ToggleMenu();
    }

    private void OnToggleBook()
    {
        storyUI.ToggleBook();
        Debug.Log("toggled");
    }
    
    #endregion
}