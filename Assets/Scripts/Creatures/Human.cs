using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CapsuleCollider))]
public class Human : BaseCreature
{

    public delegate void HeadRotate( float newRotation );
    public event HeadRotate OnHeadRotation;

    float headPitch;

    [SerializeField]
    float jumpForce = 2500;

    private void Start()
    {
        Init();

        headPitch = 0;
    }

    public float GetHeight()
    {
        return GetComponent<CapsuleCollider>().height;
    }

    protected override Vector3 GroundCheckStart()
    {
        Vector3 ret = transform.position;
        ret.y -= GetHeight() / 2;

        return ret;
    }

    public void Jump()
    {
        print("Attempt jump");
        if(IsGrounded())
        {
            Vector3 vel = rb.velocity;
            vel.y = jumpForce;
            rb.velocity = vel;
        }
        else
        {
            print("Not on ground");
        }
    }

    public override void Move( Vector3 direction )
    {
        Vector3 rotatedVector = Vector3.zero;

        direction.y = 0;
        
        //clamp movement input to a max of 1 to not allow sqrt(2) speed
        direction = Vector3.ClampMagnitude(direction, 1);
        direction *= moveSpeed;

        rotatedVector += direction.z * transform.forward;
        rotatedVector += direction.x * transform.right;

        rotatedVector.y = rb.velocity.y;

        rb.velocity = rotatedVector;
    }
    

    public override void Rotate( Vector3 rot )
    {
        Vector3 rotation = transform.eulerAngles;

        rotation.y += rot.y;
        

        //clamp camera rotation
        if(headPitch + rot.x >= 85)
        {
            headPitch = 85;
        }
        else if(headPitch + rot.x <= -85)
        {
            headPitch = -85;
        }
        else
        {
            headPitch += rot.x;
        }
        

        transform.eulerAngles = rotation;

        OnHeadRotation.Invoke(headPitch);
    }
}
