
using UnityEngine;
using UnityEngine.UI;

public class UISwapLeftBehaviour : MonoBehaviour
{

    // the player
    private PlayerManager playerManager;
    [SerializeField] private Text SwapLeftText;

    // Start is called before the first frame update
    void Start()
    {
        playerManager = PlayerManager.Instance;
    }

    // Update is called once per frame
    void Update()
    {
        SwapLeftText.text = playerManager.SwapsLeft.ToString();
    }
}
