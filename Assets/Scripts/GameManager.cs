using Photon.Pun;
using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviourPunCallbacks
{
    public static GameManager Instance;

    public GameObject playerPrefab;
    public GameObject gameCanvas;
    public GameObject sceneCamera;

    public TMP_Text PingText;

    public GameObject disconnectUI;
    private bool Off = false;

    public GameObject PlayerFeed;
    public GameObject FeedGrid;

    [HideInInspector] public GameObject LocalPlayer;
    public TMP_Text RespawnTimerText;
    public GameObject RespawnMenu;
    private float timerAmount = 5f;
    private bool runSpawnTimer = false;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        gameCanvas.SetActive(true);
    }
    private void Update()
    {
        CheckInput();
        PingText.text = "Ping: " + PhotonNetwork.GetPing();

        if(runSpawnTimer)
        {
            StartRespawn();
        }
    }

    public void EnableRespawn()
    {
        timerAmount = 5f;
        runSpawnTimer = true;
        RespawnMenu.SetActive(true);
    }
    private void StartRespawn()
    {
        timerAmount -= Time.deltaTime;
        RespawnTimerText.text = "Respawning in " + timerAmount.ToString("F0");

        if (timerAmount <= 0)
        {
            LocalPlayer.GetComponent<PhotonView>().RPC("Respawn", RpcTarget.AllBuffered);
            LocalPlayer.GetComponent<Health>().EnableInput();
            RespawnLocation();
            RespawnMenu.SetActive(false);
            runSpawnTimer = false;
        }
    }
    public void RespawnLocation() 
    {
        float randomValue = Random.Range(-3f ,5f);
        LocalPlayer.transform.localPosition = new Vector2(randomValue, 3f);
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
        obj.GetComponent<TMP_Text>().text = player.NickName + " joined the game";
        obj.GetComponent<TMP_Text>().color = Color.green;
    }
    public override void OnPlayerLeftRoom(Photon.Realtime.Player player)
    {
        GameObject obj = Instantiate(PlayerFeed, new Vector2(0,0), Quaternion.identity);
        obj.transform.SetParent(FeedGrid.transform, false);
        obj.GetComponent<TMP_Text>().text = player.NickName + " left the game";
        obj.GetComponent<TMP_Text>().color = Color.red;
    }
}
