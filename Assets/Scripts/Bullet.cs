using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class Bullet : MonoBehaviourPun
{
    public bool MovingDirection;
    public float MoveSpeed = 8;
    public float DestroyTime = 2f;
    public float bulletDamage = 0.3f;

    public string killerName;
    [HideInInspector]
    public GameObject localPlayerObj;

    private void Start() {
        if(photonView.IsMine)
        killerName = localPlayerObj.GetComponent<Dragon>().MyName;
    }

    IEnumerator destroyBullet()
    {
        yield return new WaitForSeconds(DestroyTime);
        this.GetComponent<PhotonView>().RPC("Destroy", RpcTarget.AllBuffered);
    }
    private void Update() {
       if(!MovingDirection)
       {
           transform.Translate(Vector2.right * MoveSpeed * Time.deltaTime);
       } else
       {
           transform.Translate(Vector2.left * MoveSpeed * Time.deltaTime);
       }
    }
    [PunRPC]
    public void ChangeDirection(){
        MovingDirection = true;
    }
    [PunRPC]
    void Destroy()
    {
        Destroy(this.gameObject);
    }

    private void OnTriggerEnter2D(Collider2D other) {
        if (!photonView.IsMine)
        {
            return;
        }
        PhotonView target = other.gameObject.GetComponent<PhotonView>();
        if (target != null && (!target.IsMine || target.IsSceneView))
        {
            if(target.tag == "Player")
            {
                target.RPC("HealthUpdate", RpcTarget.AllBuffered, bulletDamage);
                target.GetComponent<HurtEffect>().GotHit();

                if (target.GetComponent<Health>().health <=0)
                {
                    Player GotKilled = target.Owner;
                    target.RPC("YouGotKilledBy",GotKilled, killerName);
                    target.RPC("YouGotKilled", localPlayerObj.GetComponent<PhotonView>().Owner, target.Owner.NickName);
                }
            }
            this.GetComponent<PhotonView>().RPC("Destroy", RpcTarget.AllBuffered);
        }
    }
}
