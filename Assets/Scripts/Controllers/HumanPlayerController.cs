using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HumanPlayerController : MonoBehaviour
{

    Camera headCamera;
    Human controlledHuman;
    
	void Start ()
    {

        controlledHuman = GetComponent<Human>();


        if(!controlledHuman)
        {
            Destroy(gameObject);
        }

        GameObject cameraObject = new GameObject("Player Camera");
        headCamera = cameraObject.AddComponent<Camera>();

        cameraObject.transform.parent = gameObject.transform;

        float y = controlledHuman.GetHeight();

        cameraObject.transform.localPosition = new Vector3(0, y - .5f, 0);
        cameraObject.transform.localRotation = Quaternion.identity;

        controlledHuman.OnHeadRotation += RotateCamera;
	}

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButton("Jump"))
        {
            controlledHuman.Jump();
        }


        //movement input
        Vector3 inputVector = Vector3.zero;

        inputVector.z = Input.GetAxis("Forward");
        inputVector.x = Input.GetAxis("Right");

        controlledHuman.Move(inputVector);

        //roation input
        inputVector = Vector3.zero;
        
        inputVector.x = -Input.GetAxis("LookUp");
        inputVector.y = Input.GetAxis("LookRight");

        controlledHuman.Rotate(inputVector);

	}

    void RotateCamera(float rot)
    {
        Vector3 headRot = headCamera.gameObject.transform.eulerAngles;
        headRot.x = rot;

        //TODO clamp camera

        headCamera.gameObject.transform.eulerAngles = headRot;
    }
}
