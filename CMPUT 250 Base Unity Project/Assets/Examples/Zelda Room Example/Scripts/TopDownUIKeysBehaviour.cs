using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UIKeysBehaviour : MonoBehaviour
{

    // text component
    private TextMeshProUGUI tmp;

    // the player
    private Rigidbody2D player;
    private PlayerBehaviour playerScript;

    // Start is called before the first frame update
    void Start()
    {
        tmp = (TextMeshProUGUI)GameObject.Find("Text").GetComponent<TextMeshProUGUI>();
        player = (Rigidbody2D)GameObject.Find("Player").GetComponent("Rigidbody2D");
        playerScript = (PlayerBehaviour)player.gameObject.GetComponent(typeof(PlayerBehaviour));
    }

    // Update is called once per frame
    void Update()
    {
        int currKeys = playerScript.getKeys();
        tmp.text = currKeys.ToString();
    }
}
