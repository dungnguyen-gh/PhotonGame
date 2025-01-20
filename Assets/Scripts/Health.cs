using Photon.Pun;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class Health : MonoBehaviourPun
{
    public float HealthAmount;
    public Image FillImage;

    public Player playerMove;

    public Rigidbody2D rb;
    public BoxCollider2D bc;
    public SpriteRenderer sr;

    public GameObject playerCanvas;

    private void Awake() {
        if (photonView.IsMine)
        {
            GameManager.Instance.LocalPlayer = this.gameObject;
        }
    }

    [PunRPC]
    public void ReduceHealth(float amount)
    {
        ModifyHealth(amount);
    }

    private void CheckHealth()
    {
        if(HealthAmount <= 0 && photonView.IsMine)
        {
            GameManager.Instance.EnableRespawn();
            playerMove.disableInput = true;
            photonView.RPC("Dead", RpcTarget.AllBuffered);
        }
    }
    public void EnableInput() 
    {
        playerMove.disableInput = false;
    }

    [PunRPC]
    private void Dead()
    {
        rb.gravityScale = 0;
        bc.enabled = false;
        sr.enabled = false;
        playerCanvas.SetActive(false);
    }
    [PunRPC]
    private void Respawn()
    {
        rb.gravityScale = 1;
        bc.enabled = true;
        sr.enabled = true;
        playerCanvas.SetActive(true);
        FillImage.fillAmount = 1f;
        HealthAmount = 100f;
    }

    private void ModifyHealth(float amount)
    {
        HealthAmount -= amount;

        FillImage.fillAmount = HealthAmount / 100f;

        CheckHealth();
    }
}
