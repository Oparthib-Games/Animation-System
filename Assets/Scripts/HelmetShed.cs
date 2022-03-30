using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HelmetShed : MonoBehaviour
{
    public float speed = 0.02f;
           //float charSpeed = 0f;
    public float jumpSpeed = 0.1f;

    Animator anim;
    Rigidbody RB;

    bool canMove = true;
    bool isWalking;

    bool doSit;
    bool isSitting;

    bool isHanging;
    bool isShimmying;

    public GameObject Chair;
    public GameObject Root_Box;



    void Start()
    {
        anim = GetComponent<Animator>();
        RB = GetComponent<Rigidbody>();
    }

    void Update()
    {
        Update_ForChairSitStand();
        Update_ForClimbHanging();
    }


    public void GrabEdge(GameObject rootTarget)/// [rootTarget] is where the hand will hang to.
    {
        if (isHanging) return;

        anim.SetTrigger("doHang");
        anim.SetBool("isWalking", false);
        anim.SetFloat("charSpeed", 0);
        RB.isKinematic = true;

        canMove = false;
        Root_Box = rootTarget;

        Snap_Root_Box_to_Player_XZ_Pos();

        isHanging = true;
    }

    private void Snap_Root_Box_to_Player_XZ_Pos()
    {
        Plane xPlane = new Plane(
            Root_Box.transform.position,
            Root_Box.transform.position + Root_Box.transform.right,
            Root_Box.transform.position + Root_Box.transform.up
            );

        Vector3 desiredRootPos = new Vector3(transform.position.x, Root_Box.transform.position.y, transform.position.z);
        ///                         Cz WE NEED TO CHANGE THE X & Z POS OF THE ROOT_BOX. Y POS WILL REMAIN SAME.
        Ray ray_from_player_XZ = new Ray(desiredRootPos - Root_Box.transform.forward, Root_Box.transform.forward);

        float enter;

        if (xPlane.Raycast(ray_from_player_XZ, out enter))
        {
            Root_Box.transform.position = ray_from_player_XZ.GetPoint(enter);
        }
    }

    void Update_ForClimbHanging()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            anim.SetTrigger("doJump");
        }

        if(Input.GetKeyDown(KeyCode.W))
        {
            if(isHanging)
            {
                isHanging = false;
                anim.SetTrigger("doClimb");
            }
        }

        if(Input.GetKeyDown(KeyCode.A))
        {
            if(isHanging)
            {
                anim.SetTrigger("shimmyLeft");
                isShimmying = true;
            }
        }
        if(Input.GetKeyDown(KeyCode.D))
        {
            if(isHanging)
            {
                anim.SetTrigger("shimmyRight");
                isShimmying = true;
            }
        }
        if(Input.GetKeyDown(KeyCode.S))
        {
            if(isHanging)
            {
                anim.SetTrigger("dropHanging");
                End_Climb();
            }
        }


        AnimLerpClimb();
    }

    public void End_Climb()
    {
        isHanging = false;
        Root_Box = null;
        canMove = true;
        RB.isKinematic = false;
    }
    public void End_Shimmy()
    {
        Snap_Root_Box_to_Player_XZ_Pos();
        isShimmying = false;
    }

    void AnimLerpClimb()
    {
        if (!isHanging || isShimmying) return;

        transform.position = Vector3.Lerp(transform.position, Root_Box.transform.position, 0.1f);

        transform.rotation = Quaternion.Lerp(transform.rotation, Root_Box.transform.rotation, 0.1f);
    }

    /// --------------------------------------------------------------------------------------------------------//|                                                                          //|
    void Update_ForChairSitStand()
    {
        Movement();

        if (Input.GetKey(KeyCode.Return) && !isSitting) doSit = true;
        else if (Input.GetKey(KeyCode.Return) && isSitting) StandUpChair();

        if (doSit) SitDownChair();
        AnimLerpChair();
    }                                                                           //|
    void SitDownChair()
    {
        canMove = false;
        isSitting = true;

        anim.SetBool("isWalking", true);
        anim.SetFloat("charSpeed", 2);

        Vector3 chairDir = new Vector3(Chair.transform.position.x - transform.position.x, 0, Chair.transform.position.z - transform.position.z);

        Quaternion lookRot = Quaternion.LookRotation(chairDir);

        transform.rotation = Quaternion.Slerp(transform.rotation, lookRot, 0.1f);

        print(Vector3.Distance(transform.position, Chair.transform.position));

        if(Vector3.Distance(transform.position, Chair.transform.position) < 1f)
        {
            anim.SetBool("isSitting", true);
            anim.SetBool("isWalking", false);

            doSit = false;
        }
    }                                                                                      //|
    void StandUpChair()
    {
        isSitting = false;
        anim.SetBool("isSitting", false);
        canMove = true;
    }                                                                                      //|
    void AnimLerpChair()
    {
        if (!isSitting) return;

        Vector3 chairPosXZ = new Vector3(Chair.transform.position.x, 0f, Chair.transform.position.z);

        if (Vector3.Distance(transform.position, Chair.transform.position) > 0.01f)
        {
            transform.position = Vector3.Lerp(transform.position, chairPosXZ, 0.01f);

            transform.rotation = Quaternion.Lerp(transform.rotation, Chair.transform.rotation, 0.01f);
        }
        else
        {
            transform.position = chairPosXZ;
            transform.rotation = Chair.transform.rotation;
        }
    }                                                                                     //|
    /// --------------------------------------------------------------------------------------------------------//|



    void Movement()
    {
        if (!canMove) return;

        float x = Input.GetAxis("Horizontal");  //Shamne
        float z = Input.GetAxis("Vertical");    //Side e

        transform.Rotate(0, Mathf.Atan2(x, z) * 2, 0);

        //charSpeed += z;
        //anim.SetFloat("charSpeed", charSpeed);

        if (z != 0)
        {
            isWalking = true;
        }
        else
        {
            isWalking = false;
            //charSpeed = 0f;
        }

        anim.SetBool("isWalking", isWalking);
        anim.SetFloat("velocityX", x);
        anim.SetFloat("velocityZ", z);

    }
}
