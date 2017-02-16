using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

public class CubeMovement : MonoBehaviour {
    Rigidbody rb;

    private const float speed = 500f;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

	// Update is called once per frame
	void Update () {
        float horMovement = CrossPlatformInputManager.GetAxis("Horizontal");
        float verMovement = CrossPlatformInputManager.GetAxis("Vertical");
        rb.velocity = new Vector3(horMovement * speed * Time.deltaTime, 0, verMovement * speed * Time.deltaTime);
	}

}
