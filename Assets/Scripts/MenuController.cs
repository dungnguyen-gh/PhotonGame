using UnityEngine;
using TMPro;
using Photon.Pun;
using Photon.Realtime;

public class MenuController : MonoBehaviourPunCallbacks
{
    [SerializeField] private string VersionName = "0.1";
    [SerializeField] private GameObject UsernameMenu;
    [SerializeField] private GameObject ConnectPanel;

    [SerializeField] private TMP_InputField UsernameInput;
    [SerializeField] private TMP_InputField CreateGameInput;
    [SerializeField] private TMP_InputField JoinGameInput;

    [SerializeField] private GameObject StartButton;

    void Awake()
    {
        PhotonNetwork.PhotonServerSettings.AppSettings.AppVersion = VersionName;
        PhotonNetwork.ConnectUsingSettings();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        UsernameMenu.SetActive(true);
    }

    public override void OnConnectedToMaster()
    {
        PhotonNetwork.JoinLobby(TypedLobby.Default);
        Debug.Log("connected");
    }

    public void ChangeUserNameInput()
    {
        if (UsernameInput.text.Length >= 3)
        {
            StartButton.SetActive(true);
        }
        else 
        {
            StartButton.SetActive(false);
        }
    }
    public void SetUserName()
    {
        UsernameMenu.SetActive(false);
        PhotonNetwork.NickName = UsernameInput.text;
        Debug.Log("Username set to: " + PhotonNetwork.NickName);
    }
}
