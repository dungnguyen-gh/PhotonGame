using UnityEngine;
using Photon.Pun;
using TMPro;
using Photon.Pun.Demo.Asteroids;
public class Player : MonoBehaviourPun
{
    public Rigidbody2D rb;
    public Animator anim;
    public GameObject PlayerCamera;
    public SpriteRenderer sr;
    public TMP_Text PlayerNameText;

    public bool isGrounded = false;
    public float moveSpeed;
    public float jumpForce;

    private TMP_InputField ChatInputField;

    public GameObject BulletObject;
    public Transform FirePosition;

    public bool disableInput = false;

    private void Awake()
    {
        if (photonView.IsMine)
        {
            PlayerCamera.SetActive(true);
            PlayerNameText.text = PhotonNetwork.NickName;
        }
        else
        {
            PlayerNameText.text = photonView.Owner.NickName;
            PlayerNameText.color = Color.cyan;
        }
        ChatInputField = MainGameUi.Instance.ChatInputField;
    }
    private void Update()
    {
        if (photonView.IsMine && !disableInput)
        {
            if (!ChatInputField.isFocused)
            {
                CheckInput();
            }
        }
    }
    private void CheckInput()
    {
        float moveInput = Input.GetAxisRaw("Horizontal");

        if (moveInput != 0)
        {
            transform.position += new Vector3(moveInput,0) * moveSpeed * Time.deltaTime;

            //Handle sprite flip
            if (moveInput < 0)
            {
                photonView.RPC("FlipTrue", RpcTarget.AllBuffered);
            }
            else if (moveInput > 0)
            {
                photonView.RPC("FlipFalse", RpcTarget.AllBuffered);
            }
            anim.SetBool("isRunning", true);
        }
        else
        {
            anim.SetBool("isRunning", false);
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            anim.SetTrigger("ShootTrigger");
            //shoot method is assigned in animation event shoot
        }
    }
    
    //called via animation event
    private void Shoot()
    {
        GameObject bullet = PhotonNetwork.Instantiate(BulletObject.name, 
        FirePosition.position, 
        Quaternion.identity, 0);

        //adjust bullet direction based on player direction
        if (sr.flipX)
        {
            bullet.GetComponent<PhotonView>().RPC("ChangeDir_Left", RpcTarget.AllBuffered);
        }
    }
    [PunRPC]
    private void FlipTrue()
    {
        sr.flipX = true;
        UpdateFirePosition();
    }
    [PunRPC]
    private void FlipFalse()
    {
        sr.flipX = false;
        UpdateFirePosition();
    }
    private void UpdateFirePosition()
    {
        if (sr.flipX)
        {
            FirePosition.localPosition = new Vector3(-Mathf.Abs(FirePosition.localPosition.x), 
            FirePosition.localPosition.y, 0);
        }
        else
        {
            FirePosition.localPosition = new Vector3(Mathf.Abs(FirePosition.localPosition.x), 
            FirePosition.localPosition.y, 0);
        }
    }
}
