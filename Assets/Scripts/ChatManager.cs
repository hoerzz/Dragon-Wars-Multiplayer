using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public class ChatManager : MonoBehaviourPun, IPunObservable
{
    public PhotonView photonView;
    public GameObject BubblSpeech;
    public Text ChatText;

    public Dragon player;
    InputField ChatInput;
    private bool DisableSend;

    private void Awake() {
        ChatInput = GameObject.Find("ChatInputField").GetComponent<InputField>();
    }
    private void Update() 
    {
        if (photonView.IsMine)
        {
            if (ChatInput.isFocused)
            {
                player.DisableInputs = true;
            }
            else
            {
                player.DisableInputs = false;
            }

            if (!DisableSend && ChatInput.isFocused)
            {
                if (ChatInput.text != "" && ChatInput.text.Length > 1 && Input.GetKeyDown(KeyCode.Space))
                {
                    photonView.RPC("SendMsg", RpcTarget.AllBuffered, ChatInput.text);
                    BubblSpeech.SetActive(true);
                    ChatInput.text = "";
                    DisableSend = true;
                }
            }
        }
    }

    [PunRPC]
    void SendMsg(string msg)
    {
        ChatText.text = msg;
        StartCoroutine(hiddenBubbleSpeech());
    }
    IEnumerator hiddenBubbleSpeech()
    {
        yield return new WaitForSeconds(3);
        BubblSpeech.SetActive(false);
        DisableSend = false;
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info) 
    {
        if (stream.IsWriting)
        {
            stream.SendNext(BubblSpeech.activeSelf);
            //
        }
        else if(stream.IsReading)
        {
            BubblSpeech.SetActive((bool)stream.ReceiveNext());
            //
        }
    }
}
