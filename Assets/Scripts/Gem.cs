using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Gem : MonoBehaviour
{
    public string gemName;
    public int score;
    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.gameObject.name == "Player")
        {
            PickUp();
        }
    }

    void PickUp()
    {
        PickUpText();
        Manager.Instance.AddScore(score);
        Debug.Log(score);
        Destroy(gameObject);
    }

    void PickUpText()
    {
        GameObject actiontext = new GameObject();
        actiontext.AddComponent<ActionText>();
        actiontext.AddComponent<Text>();
        actiontext.AddComponent<SpriteRenderer>();
        actiontext.GetComponent<Text>().text = "Picked Up:" + gemName;
        Instantiate(actiontext, transform.position, Quaternion.identity);
    }
}
