using UnityEngine;
using Photon.Pun;
using TMPro;
using System.Collections;
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

    private bool canShoot = true;
    private float shootCoolDown = 0.5f;

    public GameObject groundCheck;
    public float groundCheckRadius = 0.1f;
    public LayerMask groundLayer;

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
            CheckJumpFallState();
        }
    }
    private void CheckInput()
    {
        float moveInput = Input.GetAxisRaw("Horizontal");

        if (moveInput != 0)
        {
            rb.linearVelocity = new Vector2(moveInput * moveSpeed, rb.linearVelocity.y);

            //Handle sprite flip
            if (moveInput < 0)
            {
                photonView.RPC("FlipTrue", RpcTarget.AllBuffered);
            }
            else if (moveInput > 0)
            {
                photonView.RPC("FlipFalse", RpcTarget.AllBuffered);
            }
        }
        else
        {
            // Stop horizontal velocity when no input
            rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
            anim.SetBool("isRunning", false);
        }
        //control running animation
        if (isGrounded && moveInput != 0)
        {
            anim.SetBool("isRunning", true);
        }
        else 
        {
            anim.SetBool("isRunning", false);
        }

        if (Input.GetKeyDown(KeyCode.W) && isGrounded)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
            isGrounded = false;
            anim.SetBool("isJumping", true);
            anim.SetBool("isFalling", false);
        }

        if (Input.GetKeyDown(KeyCode.Space) && canShoot)
        {
            photonView.RPC("TriggerShootAnimation", RpcTarget.All);
            StartCoroutine(ShootingCoolDown());
            //shoot method is assigned in animation event shoot
        }
    }
    //cool down shooting, prevent shooting multiple times
    private IEnumerator ShootingCoolDown()
    {
        canShoot = false;
        yield return new WaitForSeconds(shootCoolDown);
        canShoot = true;
    }

    //called via animation event
    private void Shoot()
    {
        if (!photonView.IsMine)
        {
            return;
        }
        GameObject bullet = PhotonNetwork.Instantiate(BulletObject.name, 
        FirePosition.position, 
        Quaternion.identity, 0);

        //assign the shooter
        bullet.GetComponent<Bullet>().SetShooter(photonView);

        //adjust bullet direction based on player direction
        if (sr.flipX)
        {
            bullet.GetComponent<PhotonView>().RPC("ChangeDir_Left", RpcTarget.AllBuffered);
        }
    }
    [PunRPC]
    private void TriggerShootAnimation()
    {
        anim.SetTrigger("ShootTrigger");
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
    private void CheckJumpFallState()
    {
        if (!isGrounded)
        {
            if (rb.linearVelocity.y > 0)
            {
                anim.SetBool("isFalling", false);
                anim.SetBool("isJumping", true);
            }
            else if (rb.linearVelocity.y < 0)
            {
                anim.SetBool("isJumping", false);
                anim.SetBool("isFalling", true);
            }
            anim.SetBool("isRunning", false); //disable running
        }
        else
        {
            // Reset jump/fall states when grounded
            anim.SetBool("isJumping", false);
            anim.SetBool("isFalling", false);
        }
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;
        }
    }

private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = false;
        }
    }
}
