﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VolatilePlatform : MonoBehaviour
{
	public float timeBeforeFalling = 1f;

	private bool breaking = false;

	private void Start()
	{
		isBreaking = false;
		spriteRenderer = GetComponent<SpriteRenderer>();
		originalPosition = transform.position;
		originalColor = spriteRenderer.color;
		originalScale = transform.localScale;
	}

	private void OnEnable()
	{
		EventManager.StartListening("PlayerDeath", OnPlayerDeath);
	}

	private void OnDisable()
	{
		EventManager.StopListening("PlayerDeath", OnPlayerDeath);
	}

	private void OnPlayerDeath()
	{
		StopAllCoroutines();
		transform.position = originalPosition;
		spriteRenderer.enabled = true;
		spriteRenderer.color = originalColor;
		transform.rotation = Quaternion.identity;
		transform.localScale = Vector3.one;
		GetComponent<Collider2D>().enabled = true;
		isBreaking = false;
		gameObject.SetActive(true);
	}

	public void Break()
	{
		if (!breaking)
		{
			breaking = true;
			StartCoroutine(AnimateBreak()); 
		}
	}

	private IEnumerator AnimateBreak()
	{
		const float flashPeriod = 0.15f;
		float timer = 0;
		SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
		while (timer < timeBeforeFalling)
		{
			timer += Time.deltaTime;
			spriteRenderer.color = Mathf.Repeat(timer, flashPeriod) > 0.5f * flashPeriod ? Color.white : Color.grey;
			yield return null;
		}

		spriteRenderer.color = Color.white;

		float scale = 1;
		float duration = 2f;
		float speed = 0f;
		float gravity = 20f;
		float rotateSpeed = 50f;
		GetComponent<Collider2D>().enabled = false;
		Vector3 baseScale = transform.localScale;
		while (scale > 0)
		{
			scale -= Time.deltaTime / duration;
			transform.localScale = baseScale * scale;

			speed += Time.deltaTime * gravity;
			transform.position += Vector3.down * speed * Time.deltaTime;

			transform.rotation *= Quaternion.Euler(0, 0, rotateSpeed * Time.deltaTime);

			yield return null;
		}

		gameObject.SetActive(false);
	}
}
