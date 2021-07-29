using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Photon.Pun;

public class DisManager : MonoBehaviourPunCallbacks
{
    public GameObject DisUI;
    public GameObject MenuButton;
    public GameObject ReconnectButton;
    public Text StatusText;

    private void Awake() 
    {
        DontDestroyOnLoad(this.gameObject);
    }
    private void Update() {
        if(Application.internetReachability == NetworkReachability.NotReachable)
        {
            DisUI.SetActive(true);

            if ( SceneManager.GetActiveScene().buildIndex == 0)
            {
                ReconnectButton.SetActive(true);
                StatusText.text = "Koneksi Anda Terputus, Menyambungkan Ulang";
            }
            if (SceneManager.GetActiveScene().buildIndex == 1)
            {
                MenuButton.SetActive(true);
                StatusText.text = "Koneksi Anda Terputus, Menyambungkan Ulang Menuju Main Menu";
            }
        }
    }
    //called by photon
    public override void OnConnectedToMaster()
    {
        if (DisUI.activeSelf)
        {
            MenuButton.SetActive(false);
            ReconnectButton.SetActive(false);
            DisUI.SetActive(false);
        }
    }

    public void OnClick_TryConnect()
    {
        PhotonNetwork.ConnectUsingSettings();
    }

    public void OnClick_Menu()
    {
        PhotonNetwork.LoadLevel(0);
    }
}
