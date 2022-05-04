using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class CharacterController2D : MonoBehaviour
{

	[SerializeField, Tooltip("Whether or not a player can steer while jumping")] public bool airControlAllowed = false;
	[SerializeField, Tooltip("How much force is added when the player jumps off a wall")] public float wallJumpForce = 800f;
	[SerializeField, Tooltip("How much to smooth out the movement"), Range(0, .3f)] public float movementSmoothing = .05f;
	[SerializeField, Tooltip("A mask determining what is ground to the character")] public LayerMask groundLayers;
	[SerializeField, Tooltip("A position marking where to check if the player is grounded.")] public Transform groundCheckPosition;
	[SerializeField, Tooltip("A mask determining what is ceiling to the character while crouching")] public LayerMask ceilingLayers;
	[SerializeField, Tooltip("A position to check if the player can stand while crouching")] public Transform crouchBlockCheck;
	[SerializeField, Tooltip("A mask determining what is wall to the character")] public LayerMask wallLayers;
	[SerializeField, Tooltip("A position marking where to check if the player is against the wall")] public Transform wallCheckPosition;
	[SerializeField, Tooltip("The collider(s) used while standing")] public GameObject standingCollider;
	[SerializeField, Tooltip("The collider(s) used while crouching")] public GameObject crouchingCollider;
	[SerializeField, Tooltip("How fast the player moves while running")] public float runSpeed = 15f;
	[SerializeField, Tooltip("How fast the player moves while in the air")] public float airSpeed = 10f;
	[SerializeField, Tooltip("How fast the player moves while crouching")] public float crouchSpeed = 5f;
	[SerializeField, Tooltip("How fast to move while dashing")] public float dashspeed = 25f;
	[SerializeField, Tooltip("If the player is facing right (Check if your sprite's default direction is right")] public bool isFacingRight = true;
	[SerializeField, Tooltip("Particles used when the player jumps")] public ParticleSystem jumpParticles;
	[SerializeField, Tooltip("Particles used when the player lands")] public ParticleSystem landingParticles;

	[HideInInspector, Tooltip("Radius of a circle used for the crouch blocked check")] public float ceilingCheckRadius = .15f;
	[HideInInspector, Tooltip("Radius of a circle used for the ground check")] public float groundCheckRadius = .15f;
	[HideInInspector, Tooltip("Radius of a circle used for the wall check")] public float wallCheckRadius = .15f;


	public Text debugText;
	//[SerializeField, Tooltip("")] public float limitFallSpeed = 25f; // Limit fall speed
	//[HideInInspector] public float prevGravityScale;

	[HideInInspector] public bool wasCrouchCeiling; // If the ceiling check was true last fixed update frame;
	[HideInInspector] public bool wasGrounded; // If the ground check was true last fixed update frame;
	[HideInInspector] public bool wasWall; // If the wall check was true last fixed update frame;

	[HideInInspector] public bool wallPressing = false;
	[HideInInspector] public bool isGrounded;            // Whether or not the player is grounded.
	[HideInInspector] public bool isWall; //If there is a wall in front of the player
	[HideInInspector] public bool isCrouchBlocked; //If there is a ceiling in the player's headspace.

	[HideInInspector] public bool canDash = true;
	[HideInInspector] public bool isDashing = false; //If player is dashing
	[HideInInspector] public bool isWallSliding = false; //If player is sliding in a wall
	[HideInInspector] public bool prevWallSliding = false; //If player is sliding on a wall in the previous frame
	[HideInInspector] public bool canDoubleJump = true; //If the player can jump again while in air.
	[HideInInspector] public float secondJumpMultiplierWhileAscending = 0.6f; // The amount of impulse force an air jump will have compared to regular jump
	[HideInInspector] public float secondJumpMultiplierWhileDescending = 2f; // The amount of impulse force an air jump will have compared to regular jump. It seems the player needs more if they are falling.

	[HideInInspector] public bool canCheck = false; //For check if player is wallsliding
	[HideInInspector] public bool m_wasCrouching = false; // If the player was crouching last frame.
	[HideInInspector] public bool timeLockedCrouching = false; // If the player is locked crouching because of cooldown.

	[HideInInspector] public float jumpWallStartX = 0;
	[HideInInspector] public float jumpWallDistX = 0; //Distance between player and wall
	[HideInInspector] public bool limitVelOnWallJump = false; //For limit wall jump distance with low fps

	[HideInInspector] public Vector3 velocity = Vector3.zero;

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

		public void Update()
		{
			lateralMovement = Input.GetAxisRaw("Horizontal");
			jumpPressed = Input.GetKeyDown(KeyCode.Z);
			jumpHeld = Input.GetKey(KeyCode.Z);
			doDash = Input.GetKey(KeyCode.C);
			doCrouch = Input.GetKey(KeyCode.DownArrow);
			doMelee = Input.GetKeyDown(KeyCode.X);
		}
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
	[HideInInspector] public Running running;

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
		running = GetComponent<Running>();

		standingCollider.SetActive(true);
		crouchingCollider.SetActive(false);
	}

	private void Start()
	{
		ChangeState(idling);
	}

	private void Update()
	{
		debugText.text = "State: " + CurrentMState.ToString();

		// Get player input
		pi.Update();
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

		wasCrouchCeiling = isCrouchBlocked;
		isCrouchBlocked = false;
		foreach (var col in Physics2D.OverlapCircleAll(crouchBlockCheck.position, ceilingCheckRadius, ceilingLayers))
		{
			if (col.gameObject != gameObject) { isCrouchBlocked = true; break; }
		}

		CurrentMState.FixedUpdateState();
	}

	private void OnDrawGizmos()
	{
		Gizmos.DrawWireSphere(crouchBlockCheck.position, ceilingCheckRadius);
		Gizmos.DrawWireSphere(groundCheckPosition.position, groundCheckRadius);
		Gizmos.DrawWireSphere(wallCheckPosition.position, wallCheckRadius);
	}
}