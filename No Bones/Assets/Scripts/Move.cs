using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Move : MonoBehaviour {

	Rigidbody[] rigidbodies;

	public float moveForce = 20f;

	void Start () {
		rigidbodies = GetComponentsInChildren<Rigidbody> ();
	}
	
	void Update ()
	{
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
}
