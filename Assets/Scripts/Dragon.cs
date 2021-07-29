using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.UI;
using System;

public class Dragon : MonoBehaviourPun
{
    public float MoveSpeed = 5;
    public GameObject playerCam;
    public SpriteRenderer sprite;
    public PhotonView photonview;
    public Animator anim;
    private bool AllowingMoving = true;

    public GameObject BulletPrefabs;
    public Transform BulletSpawnPointRight, BulletSpawnPointLeft;

    public Text PlayerName;
    public bool IsGrounded = false;
    public bool DisableInputs = false;
    private Rigidbody2D rb;
    public float jumpForce;
    [HideInInspector]
    public string MyName;

    //use this for initialization
    void Awake() 
    {
        if (photonView.IsMine){
            GameManager.instance.LocalPlayer = this.gameObject;
            playerCam.SetActive(true);
            PlayerName.text = "You : "+PhotonNetwork.NickName;
            PlayerName.color = Color.green;
            MyName = PhotonNetwork.NickName;
        }
        else
        {
            PlayerName.text = photonview.Owner.NickName;
            PlayerName.color = Color.red;
        }        
    }
    private void Start() {
        rb = GetComponent<Rigidbody2D>();
    }
    void Update()
    {
        if (photonView.IsMine && !DisableInputs)
        {
            checkInputs();
        }
    }

    private void checkInputs()
    {
        if(AllowingMoving){
        var movement = new Vector3 (Input.GetAxisRaw("Horizontal"), 0);
        transform.position += movement * MoveSpeed * Time.deltaTime;
        }

        if(Input.GetKeyDown(KeyCode.RightControl) && anim.GetBool("IsMove") == false)
        {
            if(sprite.flipX == false)
            {
                GameObject bullete = PhotonNetwork.Instantiate(BulletPrefabs.name, new Vector2(BulletSpawnPointRight.position.x, BulletSpawnPointRight.position.y), Quaternion.identity, 0);
                bullete.GetComponent<Bullet>().localPlayerObj = this.gameObject;
            }
            if(sprite.flipX == true)
            {
                GameObject bullete = PhotonNetwork.Instantiate(BulletPrefabs.name, new Vector2(BulletSpawnPointLeft.position.x, BulletSpawnPointLeft.position.y), Quaternion.identity, 0);
                bullete.GetComponent<Bullet>().localPlayerObj = this.gameObject;
                bullete.GetComponent<PhotonView>().RPC("ChangeDirection", RpcTarget.AllBuffered);
            }
            anim.SetBool("IsShot", true);
            AllowingMoving = false;
        }
        else if(Input.GetKeyUp(KeyCode.RightControl))
        {
            anim.SetBool("IsShot", false);
            AllowingMoving = true;
        }
        if (Input.GetKeyDown(KeyCode.Space) && IsGrounded)
        {
            Jump();
        }

        if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.A))
        {
            anim.SetBool("IsMove", true);
        }

        if (Input.GetKeyDown(KeyCode.D) && anim.GetBool("IsShot") == false)
        {
            playerCam.GetComponent<CameraFollow2D>().offset = new Vector3(1.3f, 2f, 0);
            anim.SetBool("IsMove", true);
            photonview.RPC("FlipSprite_Right", RpcTarget.AllBuffered);
        }else if (Input.GetKeyUp(KeyCode.D))
        {
            anim.SetBool("IsMove", false);
        }
        if (Input.GetKeyDown(KeyCode.A) && anim.GetBool("IsShot") == false)
        {
            playerCam.GetComponent<CameraFollow2D>().offset = new Vector3(-1.3f, 2f, 0);
            anim.SetBool("IsMove", true);
            photonview.RPC("FlipSprite_Left", RpcTarget.AllBuffered);
        }
        if (Input.GetKeyUp(KeyCode.A))
        {
            anim.SetBool("IsMove", false);
        }
    }

    [PunRPC]
    private  void FlipSprite_Right()
    {
        sprite.flipX = false;
    }
    [PunRPC]
    private  void FlipSprite_Left()
    {
        sprite.flipX = true;
    }

    private void OnCollisionEnter2D(Collision2D other) {
        if(other.gameObject.tag == "Ground")
        {
            IsGrounded = true;
        }
    }
    private void OnCollisionExit2D(Collision2D other) {
        if(other.gameObject.tag == "Ground")
        {
            IsGrounded = false;
        }
    }

    void Jump()
    {
        rb.AddForce(new Vector2(0, jumpForce * Time.deltaTime));
    }

}
