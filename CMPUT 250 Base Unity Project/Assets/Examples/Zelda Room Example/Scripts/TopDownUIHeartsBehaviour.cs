
using UnityEngine;

public class TopDownUIHeartsBehaviour : MonoBehaviour
{

    // the player
    private PlayerManager playerManager;

    // Start is called before the first frame update
    void Start()
    {
        playerManager = PlayerManager.Instance;
    }

    // Update is called once per frame
    void Update()
    {
        int currHealth = playerManager.SwapsLeft;
        for (int i = 0; i < transform.childCount; i++){
            if (i > currHealth - 1){
                transform.GetChild(i).gameObject.SetActive(false);
            }
        }
    }
}
