using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PlayerControl : MonoBehaviour {

	Rigidbody[] rigidbodies;

	public float moveForce = 20f;

	public Transform centerSpine;
	public Transform leftArm;
	NavMeshAgent leftArmAgent;
	CharacterJoint leftArmJoint;

	// Behavior states
	const int FREE_MOVEMENT = 0;
	const int STRETCHING_ARM = 1;
	const int MOVING_TOWARDS_ARM = 2;
    const int ARM_RETURNING_TO_PLAYER = 3;
	int currentState = 0;


	void Start ()
	{
		rigidbodies = GetComponentsInChildren<Rigidbody> ();
		leftArmAgent = leftArm.GetComponent<NavMeshAgent> ();
		leftArmJoint = leftArm.GetComponent<CharacterJoint> ();
		leftArmAgent.enabled = false;
	}


	void Update ()
	{
		/* MOVEMENT */

        if (currentState == FREE_MOVEMENT || currentState == STRETCHING_ARM || currentState == ARM_RETURNING_TO_PLAYER)
        {
            /* ADD FORCE AND TORQUE SO THE PLAYER RAGDOLL FLOPS AROUND */

			Vector3 force = new Vector3 (0f, 0f, 0f);
			Vector3 torque = new Vector3 (0f, 0f, 0f);

			torque.z = Input.GetAxis ("Horizontal") * -moveForce;
			torque.x = Input.GetAxis ("Vertical") * moveForce;

			force.x = Input.GetAxis ("Horizontal") * moveForce;
			force.z = Input.GetAxis ("Vertical") * moveForce;

            // Don't bother trying to add force if the force vector is zero (because that means a movement key wasn't being pressed)
			if (force != Vector3.zero) {
				// Apply force to all rigidbodies.
				foreach (Rigidbody rb in rigidbodies) {
					rb.AddForce (force);
					rb.AddTorque (torque, ForceMode.Impulse);
				}
			}
		}

		else if (currentState == MOVING_TOWARDS_ARM)
		{
           // If player has reached arm position, reattach arm.
			if (Vector3.Distance (leftArm.position, centerSpine.position) < 0.25f) {
				leftArmJoint.connectedBody = centerSpine.GetComponent<Rigidbody> ();
				currentState = FREE_MOVEMENT;
			}

            // Move player towards arm.
			Vector3 direction = leftArm.position - centerSpine.position;
			direction.Normalize();
			Vector3 torqueDirection = direction;
			torqueDirection.y = 0f;
			torqueDirection = Quaternion.Euler (0f, 90f, 0f) * torqueDirection;
			foreach (Rigidbody rb in rigidbodies) {
				rb.AddForce (direction * moveForce*10f);
				rb.AddTorque (torqueDirection * 9999999f, ForceMode.Impulse);
			}
		}
			

		/* ARM STRETCHING */

		if (Input.GetMouseButtonDown (0))
        {
			RaycastHit hitInfo;
			if (Physics.Raycast (Camera.main.ScreenPointToRay (Input.mousePosition), out hitInfo, 50f))
            {
				if (leftArmJoint != null)
                {
					leftArmJoint.breakForce = 0;
				}

				leftArmAgent.enabled = true;
				leftArmAgent.SetDestination (hitInfo.point);
				currentState = STRETCHING_ARM;
			}
		}


        if (currentState == STRETCHING_ARM)
        {
            // If the arm has reached its position.
            if (Vector3.Distance(leftArm.position, leftArmAgent.destination) < 1f)
            {
                // Test whether the arm has reached a grass tile.
                RaycastHit hit;
                if (Physics.Raycast(leftArm.position, Vector3.down, out hit, 1.5f))
                {
                    // If it has reached a grass tile, pull the player towards the arm position.
                    if (hit.collider.tag == "Grabbable")
                    {
                        Debug.Log("Gripped");

                        // Stop the arm and restore its joints & stuff.
                        leftArm.gameObject.AddComponent<CharacterJoint>();
                        leftArmJoint = leftArm.GetComponent<CharacterJoint>();
                        leftArmAgent.Stop();
                        leftArmAgent.enabled = false;

                        currentState = MOVING_TOWARDS_ARM;
                    }

                    // If it has not reached a grass tile, return to the player
                    else
                    {
                        leftArmAgent.SetDestination(centerSpine.position);
                        currentState = ARM_RETURNING_TO_PLAYER;
                    }
                }
            }
        }

        else if (currentState == ARM_RETURNING_TO_PLAYER)
        {
            // See if the arm has reached the player's position.
            if (Vector3.Distance(leftArm.position, centerSpine.position) <= 1f)
            {
                // Reattach arm & stop it's navigation.
                leftArm.gameObject.AddComponent<CharacterJoint>();
                leftArmJoint = leftArm.GetComponent<CharacterJoint>();
                leftArmJoint.connectedBody = centerSpine.GetComponent<Rigidbody> ();

                leftArmAgent.Stop();
                leftArmAgent.enabled = false;

                currentState = FREE_MOVEMENT;
            }
        }
	}
}
