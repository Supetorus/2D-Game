using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
	[SerializeField] oldController characterController;
	[SerializeField] Animator animator;
	[SerializeField] float speed;

	Vector2 direction = Vector2.zero;
	bool jump = false;
	bool crouch = false;

	void Update()
	{
		direction.x = Input.GetAxis("Horizontal") * speed;
		animator.SetFloat("Speed", Mathf.Abs(direction.x));

		if (Input.GetButtonDown("Jump"))
		{
			animator.SetBool("Jumping", true);
			jump = true;
		}

		characterController.Move(direction.x, false, false, jump, false);
	}

	private void FixedUpdate()
	{
		characterController.Move(direction.x * Time.fixedDeltaTime, crouch, false, jump, false);
		jump = false;
	}

	public void OnLand()
	{
		animator.SetBool("Jumping", false);
	}
}
