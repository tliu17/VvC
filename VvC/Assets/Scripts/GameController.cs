﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour {

	Player player;
	ShakeCamera camShake;
	Text scoreText;
	Text lifeText;
	Text buttonText;
	Rigidbody2D rb;

	public static int score;
	private int life;

	public bool paused;
	private bool died;

	public float speed;
	private float acceleration;

	private static float INITIAL_SPEED = -4.0f;
	private static float INITIAL_ACCELERATION = 0.01f;
	private static float ACCELERATION_DECAY = 0.9999f;

	public GameObject deathExplosion;

	// Use this for initialization
	void Awake () {
		player = GameObject.Find("Player").GetComponent<Player> ();
		rb = player.GetComponent<Rigidbody2D> ();
		camShake = GameObject.Find("MainCamera").GetComponent<ShakeCamera> ();
		scoreText = GameObject.Find("ScoreText").GetComponent<Text> ();
		lifeText = GameObject.Find("LifeText").GetComponent<Text> ();
		buttonText = GameObject.Find("PauseButtonText").GetComponent<Text> ();

		paused = false;
		died = false;
		score = 0;
		life = 3;
		speed = INITIAL_SPEED;
		acceleration = INITIAL_ACCELERATION;
	}

	// Update is called once per frame
	void Update () {
		if (!paused) {
			speed = speed - acceleration;
			acceleration = acceleration * ACCELERATION_DECAY;
		}

		if (died) {
			StartCoroutine(explode());
		}
	}

	public void AddScore () {
		score += 1;
		scoreText.text = "Score: " + score;
	}

	public void LoseLife () {
		life -= 1;
		lifeText.text = "Life: " + life;

		// Juice Effects
		LoseLifeEffects();

		if (life <= 0) {
			died = true;
		}
	}

	public void LoseLifeEffects () {
		// Pauses objects and decorations for 1 sec
		StartCoroutine(FreezeAndResume(1f));

		// Shakes camera (magnitude, duration)
		camShake.Shake(0.1f, 0.6f);

		// slow down by 20%
		speed *= 0.8f;
	}

	IEnumerator FreezeAndResume (float waitTime) {
		paused = true;
		player.GetComponent<TouchMovement>().enabled = false;

		float previousSpeed = speed;
		speed = 0; // pauses scrolling
		yield return new WaitForSeconds (waitTime);
		speed = previousSpeed;

		paused = false;
		player.GetComponent<TouchMovement>().enabled = true;
	}

	public void Pause () {

		paused = !paused;

		if (paused) {
			Time.timeScale = 0;
			buttonText.text = "Resume";
			player.GetComponent<TouchMovement>().enabled = false;
		}

		else if (!paused) {
			Time.timeScale = 1;
			buttonText.text = "Pause";
			player.GetComponent<TouchMovement>().enabled = true;
		}
	}

	//	public void death(){
	//		rb.velocity = new Vector2(0.0f, 3.0f);
	//		Destroy(player.GetComponent<BoxCollider2D> ());
	//	}

	IEnumerator explode(){
		yield return new WaitForSeconds (1f);
		//rb.velocity = new Vector2(0.0f, 0.0f);
		paused = true;
		speed = 0;
		player.GetComponent<TouchMovement>().enabled = false;

		Destroy(player.gameObject);	

		Instantiate (deathExplosion, new Vector3(rb.position.x, rb.position.y, 0), Quaternion.Euler(0, 0, 0));
		Invoke("LoadGameOver", 2);
	}

	public void LoadGameOver(){
		SceneManager.LoadScene ("GameOver",LoadSceneMode.Single);
	}
}