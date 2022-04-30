using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{

	public CharacterController2D controller;
	public Animator animator;

	public float runSpeed = 40f;

	float horizontalMove = 0f;
	bool jump = false;
	bool jumping = false;
	bool dash = false;
	bool crouch = false;

	void Update()
	{
		horizontalMove = Input.GetAxisRaw("Horizontal") * runSpeed;

		animator.SetFloat("Speed", Mathf.Abs(horizontalMove));

		if (Input.GetKeyDown(KeyCode.Z))
		{
			jump = true;
		}

		if (Input.GetKey(KeyCode.Z))
		{
			jumping = true;
		}

		if (Input.GetKeyDown(KeyCode.C))
		{
			dash = true;
		}

		if (Input.GetKeyDown(KeyCode.DownArrow))
		{
			crouch = true;
		}
		else if (Input.GetKeyUp(KeyCode.DownArrow))
		{
			crouch = false;
		}
		/*if (Input.GetAxisRaw("Dash") == 1 || Input.GetAxisRaw("Dash") == -1) //RT in Unity 2017 = -1, RT in Unity 2019 = 1
		{
			if (dashAxis == false)
			{
				dashAxis = true;
				dash = true;
			}
		}
		else
		{
			dashAxis = false;
		}
		*/

	}

	public void OnFall()
	{
		animator.SetBool("IsJumping", true);
	}

	public void OnLanding()
	{
		animator.SetBool("IsJumping", false);
	}

	void FixedUpdate()
	{
		// Move our character
		controller.Move(horizontalMove * Time.fixedDeltaTime, jump, jumping, dash, crouch);
		jump = false;
		dash = false;
		jumping = false;
	}
}
