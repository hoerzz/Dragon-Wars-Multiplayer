using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public class TimeOut : MonoBehaviourPun
{
    private float idleTimer = 10f;
    private float timer = 5;
    public GameObject TimeOutUI;
    public Text TimeOutUI_Text;
    private bool TimeOver = false;
    // Update is called once per frame
    void Update()
    {
        //if(photonView.IsMine)
        //{
            if(!TimeOver){
                if (Input.anyKey)
                {
                    idleTimer = 10;
                }
                idleTimer -= Time.deltaTime;
                if (idleTimer <= 0)
                {
                    playerNotMoving();
                }
                if (TimeOutUI.activeSelf)
                {
                    timer -= Time.deltaTime;
                    TimeOutUI_Text.text = "Disconnecting in : ["+timer.ToString("F0")+"]";
                    if (timer <= 0)
                    {
                        TimeOver = true;
                    }
                    else if(timer > 0 && Input.anyKey)
                    {
                        idleTimer = 10;
                        timer = 5;
                        TimeOutUI.SetActive(false);
                    }
                }
            }
            else
            {
                leaveGame();
            }
        //}
    }
    private void playerNotMoving()
    {
        TimeOutUI.SetActive(true);
    }
    private void leaveGame()
    {
        PhotonNetwork.LeaveRoom();
        PhotonNetwork.LoadLevel(0);
    }
}
