using Photon.Pun;
using UnityEngine;
using TMPro;
using System.Collections;

public class ChatManager : MonoBehaviourPun, IPunObservable
{
    public Player playerMove;
    public GameObject BubbleSpeechObject;
    public TMP_Text UpdatedText;

    private TMP_InputField ChatInputField;
    private bool DisableSend;

    void Awake()
    {
        ChatInputField = MainGameUi.Instance.ChatInputField;
        ChatInputField.onEndEdit.AddListener(HandleChatInput);
    }
    private void HandleChatInput(string input)
    {
        if (photonView.IsMine)
        {
            if (!DisableSend)
            {
                if (!string.IsNullOrEmpty(ChatInputField.text))
                {
                    photonView.RPC("SendMessage", RpcTarget.AllBuffered, ChatInputField.text);
                    BubbleSpeechObject.SetActive(true);

                    ChatInputField.text = "";
                    DisableSend = true;
                    ChatInputField.ActivateInputField();
                }
            }
        }
    }
    [PunRPC]
    private void SendMessage(string message)
    {
        UpdatedText.text = message;
        StartCoroutine(Remove());
    }
    IEnumerator Remove()
    {
        yield return new WaitForSeconds(4f);
        BubbleSpeechObject.SetActive(false);
        DisableSend = false;
    }
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(BubbleSpeechObject.activeSelf);
        }
        else if (stream.IsReading)
        {
            BubbleSpeechObject.SetActive((bool)stream.ReceiveNext());
        }
    }
}
