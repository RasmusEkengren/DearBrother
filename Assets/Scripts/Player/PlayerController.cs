﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
	public float horizontalInputAxis { get; private set; }
	public bool isJumpInputHeld { get; private set; }
	public bool isJumpInputPressed { get; private set; }
	public bool isSprintInputHeld { get; private set; }
	public bool isCrouchInputHeld { get; private set; }
	public Rigidbody2D rb2d { get; private set; }
	public new Collider2D collider { get; private set; }
	public PlayerState previousState { get; private set; }
	public PlayerState currentState { get; private set; }

	[Header("States")]
	public PlayerStandingState standingState;
	public PlayerWalkingState walkingState;
	public PlayerState crouchingState;
	public PlayerState crawlingState;
	public PlayerJumpingState jumpingState;
	public PlayerFallingState fallingState;
	public PlayerLandingLagState landingLagState;

	[Header("Debug")]
	public Vector2 velocity;

	private const float castDistance = 0.05f;

	private int solidMask;

	private IEnumerable<PlayerState> IterateStates()
	{
		yield return standingState;
		yield return walkingState;
		yield return crouchingState;
		yield return crawlingState;
		yield return jumpingState;
		yield return fallingState;
		yield return landingLagState;
	}

	// Start is called before the first frame update
	private void Start()
	{
		rb2d = GetComponent<Rigidbody2D>();
		collider = GetComponent<BoxCollider2D>();
		solidMask = LayerMask.GetMask("Solid");

		foreach (PlayerState state in IterateStates())
		{
			state.player = this;
		}

		TransitionState(standingState);

		currentState.Start();
	}

	// Update is called once per frame
	private void Update()
	{
		ReadInput();

		currentState.Update();
	}

	private void FixedUpdate()
	{
		currentState.FixedUpdate();

		rb2d.velocity = velocity;
	}

	public void MoveHorizontally(float speed, float acceleration, float deceleration)
	{
		if (Mathf.Abs(rb2d.velocity.x) > speed || horizontalInputAxis == 0)
		{
			velocity.x = Mathf.MoveTowards(velocity.x, 0, deceleration * Time.deltaTime);
		}
		else
		{
			float effectiveAcceleration = acceleration;
			if (Mathf.Sign(horizontalInputAxis) == -Mathf.Sign(velocity.x))
			{
				effectiveAcceleration += deceleration;
			}
			velocity.x = Mathf.MoveTowards(velocity.x, speed * Mathf.Sign(horizontalInputAxis), effectiveAcceleration * Time.deltaTime);
		}
	}

	public bool CheckBoxcast(Vector2 direction)
	{
		Bounds bounds = collider.bounds;

		RaycastHit2D hit = Physics2D.BoxCast(bounds.center, bounds.size, 0f, direction, castDistance, solidMask);

		return hit.collider != null;
	}

	public void TransitionState(PlayerState newState)
	{
		if (currentState != null)
		{
			currentState.isCurrentState = false; 
			currentState.Exit();
		}
		previousState = currentState;
		currentState = newState;
		if (currentState != null)
		{
			currentState.Enter();
			currentState.isCurrentState = true; 
		}
	}

	private void ReadInput()
	{
		horizontalInputAxis = Input.GetAxisRaw("Horizontal");
		isJumpInputHeld = Input.GetKey(KeyCode.Space);
		isJumpInputPressed = Input.GetKeyDown(KeyCode.Space);
		isSprintInputHeld = Input.GetKey(KeyCode.LeftShift);
		isCrouchInputHeld = Input.GetAxisRaw("Vertical") < 0;
	}
}