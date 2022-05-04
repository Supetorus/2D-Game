using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class CharacterController2D : MonoBehaviour
{
	public Text debugText;

	[SerializeField, Tooltip("Amount of horizontal force added when the player jumps off a wall.")] public float m_WallJumpForce = 800f;
	[SerializeField] public float m_DashForce = 25f;
	[SerializeField, Tooltip("How fast the player moves while crouching. 1 = 100%.")] public float m_CrouchSpeed = 0.5f;
	[Range(0, .3f)] [SerializeField] public float m_MovementSmoothing = .05f;  // How much to smooth out the movement
	[SerializeField] public bool airControlAllowed = false;                         // Whether or not a player can steer while jumping;
	[SerializeField] public LayerMask groundLayers;                          // A mask determining what is ground to the character
	[SerializeField] public LayerMask ceilingLayers;                          // A mask determining what is ground to the character
	[SerializeField] public LayerMask wallLayers;                          // A mask determining what is ground to the character
	[SerializeField] public Transform groundCheckPosition;                           // A position marking where to check if the player is grounded.
	[SerializeField] public Transform wallCheckPosition;                             //Posicion que controla si el personaje toca una pared
	[SerializeField] public Transform ceilingCheckPosition;                          //Position to check for ceiling.
	[SerializeField] public CapsuleCollider2D m_CrouchDisableCollider;         // The collider disabled by crouching.
	[SerializeField] public CapsuleCollider2D m_CrouchEnableCollider;          // The collider enabled by crouching.
	public float groundSpeed = 15f;
	public float airSpeed = 10f;

	public float ceilingCheckRadius = .15f; // Radius of the overlap circle to determine if under ceiling.
	public float groundCheckRadius = .15f; // Radius of the overlap circle to determine if grounded
	public float wallCheckRadius = .15f; // Radius of the overlap circle to determine if touching the wall.
	public bool isFacingRight = true;  // For determining which way the player is currently facing.
	public Vector3 velocity = Vector3.zero;
	public float limitFallSpeed = 25f; // Limit fall speed
	[HideInInspector] public float prevGravityScale;

	[HideInInspector] public bool wasCrouchCeiling; // If the ceiling check was true last fixed update frame;
	[HideInInspector] public bool wasGrounded; // If the ground check was true last fixed update frame;
	[HideInInspector] public bool wasWall; // If the wall check was true last fixed update frame;

	[HideInInspector] public bool wallPressing = false;
	[HideInInspector] public bool isGrounded;            // Whether or not the player is grounded.
	[HideInInspector] public bool isWall; //If there is a wall in front of the player
	[HideInInspector] public bool isCrouchCeiling; //If there is a ceiling in the player's headspace.

	[HideInInspector] public bool canDash = true;
	[HideInInspector] public bool isDashing = false; //If player is dashing
	[HideInInspector] public bool isWallSliding = false; //If player is sliding in a wall
	[HideInInspector] public bool prevWallSliding = false; //If player is sliding in a wall in the previous frame
														   //private float prevVelocityX = 0f;
	[HideInInspector] public bool canCheck = false; //For check if player is wallsliding
	[HideInInspector] public bool m_wasCrouching = false; // If the player was crouching last frame.
	[HideInInspector] public bool timeLockedCrouching = false; // If the player is locked crouching because of cooldown.

	[HideInInspector] public float jumpWallStartX = 0;
	[HideInInspector] public float jumpWallDistX = 0; //Distance between player and wall
	[HideInInspector] public bool limitVelOnWallJump = false; //For limit wall jump distance with low fps


	public ParticleSystem particleJumpUp; //Trail particles
	public ParticleSystem particleJumpDown; //Explosion particles

	[Header("Events")]
	[Space]

	public UnityEvent OnFallEvent;
	public UnityEvent OnLandEvent;
	public BoolEvent OnCrouchEvent;

		//Player input status
	public struct PlayerInput
	{
		[HideInInspector] public float lateralMovement;
		[HideInInspector] public bool jumpPressed;
		[HideInInspector] public bool jumpHeld;
		[HideInInspector] public bool doDash;
		[HideInInspector] public bool doCrouch;
		[HideInInspector] public bool doMelee;
	}

	public PlayerInput pi;

	// States
	[HideInInspector] public Idling idling;
	[HideInInspector] public Crouching crouching;
	[HideInInspector] public Jumping jumping;
	[HideInInspector] public Falling falling;
	[HideInInspector] public Meleeing meleeing;
	[HideInInspector] public Dashing dashing;
	[HideInInspector] public TakingDamage takingDamage;
	[HideInInspector] public Dead dead;
	[HideInInspector] public WallSliding wallSliding;
	[HideInInspector] public Running runningOnGround;

	[System.Serializable]
	public class BoolEvent : UnityEvent<bool> { }
	public CharacterMoveState CurrentMState { get; private set; }

	public void ChangeState(CharacterMoveState newState)
	{
		if (newState != null && newState != CurrentMState)
		{
			CurrentMState?.ExitState();
			CurrentMState = newState;
			CurrentMState.EnterState();
		}
	}

	private void Awake()
	{
		idling = GetComponent<Idling>();
		crouching = GetComponent<Crouching>();
		jumping = GetComponent<Jumping>();
		falling = GetComponent<Falling>();
		meleeing = GetComponent<Meleeing>();
		dashing = GetComponent<Dashing>();
		takingDamage = GetComponent<TakingDamage>();
		dead = GetComponent<Dead>();
		wallSliding = GetComponent<WallSliding>();
		runningOnGround = GetComponent<Running>();
	}

	private void Start()
	{
		ChangeState(idling);
	}

	private void Update()
	{
		debugText.text = "State: " + CurrentMState.ToString();

		// Get player input
		pi.lateralMovement = Input.GetAxisRaw("Horizontal");
		pi.jumpPressed = Input.GetKeyDown(KeyCode.Z);
		pi.jumpHeld = Input.GetKey(KeyCode.Z);
		pi.doDash = Input.GetKey(KeyCode.C);
		pi.doCrouch = Input.GetKey(KeyCode.DownArrow);
		pi.doMelee = Input.GetKeyDown(KeyCode.X);
		wallPressing = !isGrounded && ((pi.lateralMovement < 0.01f && !isFacingRight) || (pi.lateralMovement > 0.01f && isFacingRight)); // Not grounded and pressing toward the wall.

		// Update state
		CurrentMState.UpdateState();
	}

	private void FixedUpdate()
	{
		wasGrounded = isGrounded;
		isGrounded = false;
		foreach (var col in Physics2D.OverlapCircleAll(groundCheckPosition.position, groundCheckRadius, groundLayers))
		{
			if (col.gameObject != gameObject) { isGrounded = true; break; }
		}

		wasWall = isWall;
		isWall = false;
		foreach (var col in Physics2D.OverlapCircleAll(wallCheckPosition.position, wallCheckRadius, wallLayers))
		{
			if (col.gameObject != gameObject) { isWall = true; break; }
		}

		wasCrouchCeiling = isCrouchCeiling;
		isWall = false;
		foreach (var col in Physics2D.OverlapCircleAll(ceilingCheckPosition.position, ceilingCheckRadius, ceilingLayers))
		{
			if (col.gameObject != gameObject) { isWall = true; break; }
		}

		CurrentMState.FixedUpdateState();
	}

	private void OnDrawGizmos()
	{
		Gizmos.DrawWireSphere(ceilingCheckPosition.position, ceilingCheckRadius);
		Gizmos.DrawWireSphere(groundCheckPosition.position, groundCheckRadius);
		Gizmos.DrawWireSphere(wallCheckPosition.position, wallCheckRadius);
	}
}