using System.Collections;
using Photon.Pun;
using UnityEngine;

public class Bullet : MonoBehaviourPun, IPunObservable
{
    public bool MoveDir = false; //false:right - true:left
    public float MoveSpeed = 15f;
    public float DestroyTime = 2f;
    public SpriteRenderer spriteRenderer;

    public float BulletDamage = 30f;

    private PhotonView shooterView; //track shooter

    private void Awake() 
    {
        StartCoroutine(DestroyByTime());
    }
    IEnumerator DestroyByTime()
    {
        yield return new WaitForSeconds(DestroyTime);
        this.GetComponent<PhotonView>().RPC("DestroyObject", RpcTarget.AllBuffered);
    }

    public void SetShooter(PhotonView shooter)
    {
        shooterView = shooter;
        Collider2D bulletCollider = GetComponent<Collider2D>();
        Collider2D shooterCollider = shooter.GetComponent<Collider2D>();
        if (bulletCollider != null && shooterCollider != null)
        {
            Physics2D.IgnoreCollision(bulletCollider, shooterCollider);
        }
    }
    [PunRPC]
    public void ChangeDir_Left()
    {
        MoveDir = true;
        UpdateBulletSpriteDirection();
    }
    [PunRPC]
    public void DestroyObject()
    {
        if (photonView.IsMine)
        {
            PhotonNetwork.Destroy(this.gameObject);
        }
    }
    public void Update()
    {
        // Move the bullet in the appropriate direction
        Vector2 movement = MoveDir ? Vector2.left : Vector2.right;
        transform.Translate(movement * MoveSpeed * Time.deltaTime);
    }
    private void OnTriggerEnter2D(Collider2D other) 
    {
        if (!photonView.IsMine)
        {
            return;
        }
        PhotonView targetView = other.gameObject.GetComponent<PhotonView>();

        //ignore the collision if the target is the shooter
        if (targetView == shooterView)
        {
            return;
        }

        if (targetView != null && (!targetView.IsMine || targetView.IsRoomView))
        {
            if (targetView.tag == "Player")
            {
                targetView.RPC("ReduceHealth", RpcTarget.AllBuffered, BulletDamage);
            }
            photonView.RPC("DestroyObject", RpcTarget.AllBuffered);
        }
    }
    private void UpdateBulletSpriteDirection()
    {
        if (spriteRenderer != null)
        {
            spriteRenderer.flipX = MoveDir;
        }
    }
    // Implementing IPunObservable to synchronize the state of the bullet
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            // Send data to other players
            stream.SendNext(MoveDir);
        }
        else
        {
            // Receive data from the owner
            MoveDir = (bool)stream.ReceiveNext();
            UpdateBulletSpriteDirection();
        }
    }
}
