﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class NPC : MonoBehaviour {

	NavMeshAgent navMeshAgent;

	// Behavior states
	int currentState;
	const int GET_TARGET_POSITION = 0;
	const int MOVE_TO_TARGET_POSITION = 1;

	bool pickupObjects = false;

	// Movement types
	int movementType;
	const int WANDER_RANDOMLY = 0;
	const int WANDER_RANDOMLY_NOISE = 1;

	// How far the NPC looks for a new point to wander to.
	public float wanderRange = 10f;

	// Noise variables
    float noiseSpeed = 0.04f;
	float noiseX = 0.0f;
	float noiseY = 0.0f;
	float noiseXOffset = 100f;
	float noiseYOffset = 1000f;


	void Start()
	{
		navMeshAgent = GetComponent<NavMeshAgent> ();

		currentState = GET_TARGET_POSITION;
	}


	void Update()
	{
		if (currentState == GET_TARGET_POSITION) {
			GetTargetPosition ();
			currentState = MOVE_TO_TARGET_POSITION;
		}

		else if (currentState == MOVE_TO_TARGET_POSITION)
		{
			// See if I've reached my target position
			if (navMeshAgent.velocity.magnitude == 0.0f)
			{
				currentState = GET_TARGET_POSITION;
			}
		}
	}

	void GetTargetPosition()
	{
		Vector3 targetPosition = transform.localPosition;

		if (movementType == WANDER_RANDOMLY_NOISE)
		{
			// Get a random direction with perlin noise.
			Vector3 direction = new Vector3 (
				MyMath.Map (Mathf.PerlinNoise (noiseX, 0.0f), 0f, 1f, -1f, 1f),
				transform.localPosition.y,
				MyMath.Map (Mathf.PerlinNoise(0.0f, noiseY), 0f, 1f, -1f, 1f)
			);

			direction.Normalize ();

			targetPosition += direction;

			noiseX += noiseSpeed;
			noiseY += noiseSpeed;
		}

		if (movementType == WANDER_RANDOMLY)
		{
			// Get a random direction with perlin noise.
			Vector2 randomCircle = Random.insideUnitCircle*wanderRange;
			targetPosition.x += randomCircle.x;
			targetPosition.z += randomCircle.y;
		}

		navMeshAgent.SetDestination(targetPosition);
	}
}
