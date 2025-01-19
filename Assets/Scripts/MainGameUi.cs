using UnityEngine;
using TMPro;

public class MainGameUi : MonoBehaviour
{
    public static MainGameUi Instance;
    public TMP_InputField ChatInputField;

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
    }
}
