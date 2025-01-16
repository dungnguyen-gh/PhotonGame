using Photon.Pun;
using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviourPunCallbacks
{
    public GameObject playerPrefab;
    public GameObject gameCanvas;
    public GameObject sceneCamera;

    public TMP_Text PingText;

    public GameObject disconnectUI;
    private bool Off = false;

    public GameObject PlayerFeed;
    public GameObject FeedGrid;

    private void Awake()
    {
        gameCanvas.SetActive(true);
    }
    private void Update()
    {
        CheckInput();
        PingText.text = "Ping: " + PhotonNetwork.GetPing();
    }
    private void CheckInput()
    {
        if (Off && Input.GetKeyDown(KeyCode.Escape))
        {
            disconnectUI.SetActive(false);
            Off = false;
        }
        else if (!Off && Input.GetKeyDown(KeyCode.Escape))
        {
            disconnectUI.SetActive(true);
            Off = true;
        }
    }
    public void SpawnPlayer()
    {
        float randomValue = Random.Range(-1f, 1f);

        PhotonNetwork.Instantiate(playerPrefab.name, 
        new Vector2(this.transform.position.x *randomValue, this.transform.position.y), 
        Quaternion.identity,0);

        gameCanvas.SetActive(false);
        sceneCamera.SetActive(false);
    }

    public void LeaveRoom()
    {
        PhotonNetwork.LeaveRoom();
        PhotonNetwork.LoadLevel("MainMenu");
    }

    public override void OnPlayerEnteredRoom(Photon.Realtime.Player player)
    {
        GameObject obj = Instantiate(PlayerFeed, new Vector2(0,0), Quaternion.identity);
        obj.transform.SetParent(FeedGrid.transform, false);
        obj.GetComponent<TMP_Text>().text = player.NickName + "joined the game";
        obj.GetComponent<TMP_Text>().color = Color.green;
    }
    public override void OnPlayerLeftRoom(Photon.Realtime.Player player)
    {
        GameObject obj = Instantiate(PlayerFeed, new Vector2(0,0), Quaternion.identity);
        obj.transform.SetParent(FeedGrid.transform, false);
        obj.GetComponent<TMP_Text>().text = player.NickName + "left the game";
        obj.GetComponent<TMP_Text>().color = Color.red;
    }
}
