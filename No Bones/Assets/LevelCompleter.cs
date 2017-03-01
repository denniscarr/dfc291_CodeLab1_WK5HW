using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelCompleter : MonoBehaviour {

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            GameObject.Find("Game Manager").SendMessage("AdvanceScene");
        }
    }
}
