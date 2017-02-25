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

		if (currentState == FREE_MOVEMENT || currentState == STRETCHING_ARM) {
			Vector3 force = new Vector3 (0f, 0f, 0f);
			Vector3 torque = new Vector3 (0f, 0f, 0f);
			torque.z = Input.GetAxis ("Horizontal") * -moveForce;
			torque.x = Input.GetAxis ("Vertical") * moveForce;
			force.x = Input.GetAxis ("Horizontal") * moveForce;
			force.z = Input.GetAxis ("Vertical") * moveForce;

			// Don't bother trying to add force if the force vector is zero
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
			if (Vector3.Distance (leftArm.position, centerSpine.position) < 1f) {
				Debug.Log ("Reached Arm");
//				foreach (Rigidbody rb in rigidbodies) {
//					rb.velocity = Vector3.zero;
//				}
				leftArmJoint.connectedBody = centerSpine.GetComponent<Rigidbody> ();
				currentState = FREE_MOVEMENT;
			}

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

//		if (currentState == FREE_MOVEMENT || currentState == STRETCHING_ARM)
//		{
			if (Input.GetMouseButtonDown (0)) {

				RaycastHit hitInfo;
				if (Physics.Raycast (Camera.main.ScreenPointToRay (Input.mousePosition), out hitInfo, 50f)) {
					if (leftArmJoint != null) {
						leftArmAgent.enabled = true;
						leftArmJoint.breakForce = 0;
					}
					leftArmAgent.SetDestination (hitInfo.point);
					currentState = STRETCHING_ARM;
				}
			}
//		}

		if (currentState == STRETCHING_ARM)
		{
			// If the arm has reached its position.
			if (Vector3.Distance (leftArm.position, leftArmAgent.destination) < 1f) {
				leftArmAgent.Stop ();
				leftArmAgent.enabled = false;
				leftArm.gameObject.AddComponent<CharacterJoint> ();
				leftArmJoint = leftArm.GetComponent<CharacterJoint> ();
				currentState = MOVING_TOWARDS_ARM;
			}
		}
	}
}
