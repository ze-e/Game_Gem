using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum GemType { Sphene, Potassium, Sapphire };
public interface IController
{
    float miningSpeed { get; set; }
    float speed { get; set; }
}
public class Gem : MonoBehaviour
{
    public GemType gemType;
    string gemName;
    public int score;

    private void Start()
    {
        gemName = gemType.ToString();
    }

    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.gameObject.name == "Player" || collider.gameObject.tag == "Rival")
        {
            PlayerController playerController = collider.gameObject.GetComponent<PlayerController>();
            RivalController rivalController = collider.gameObject.GetComponent<RivalController>();
            Special(playerController != null ? playerController : rivalController);
            PickUp(playerController != null);
        }
    }

    void PickUp(bool rival)
    {
        PickUpText();
        if(rival) Manager.Instance.AddScore(score);
        else Manager.Instance.AddRivalScore(score);
        Destroy(gameObject);
    }

 

    void Special<T>(T controller) where T : MonoBehaviour, IController
    {
        if (gemType == GemType.Potassium)
        {
            controller.miningSpeed += 1;
            ShowText("Mining rate increased!", Color.green);
        }
        if (gemType == GemType.Sapphire)
        {
            controller.speed += 1;
            ShowText("Speed increased!", Color.blue) ;
        }
    }

    void PickUpText()
    {
        ShowText("Picked Up:" + gemName);
    }

    void ShowText(string _text)
    {
        Debug.Log(_text);
        GameObject actiontext = new GameObject();
        actiontext.AddComponent<ActionText>();
        actiontext.AddComponent<Text>();
        actiontext.AddComponent<SpriteRenderer>();
        actiontext.GetComponent<Text>().text = _text;
        Instantiate(actiontext, transform.position, Quaternion.identity);
    }

    void ShowText(string _text, Color _color)
    {
        Debug.Log(_text);
        GameObject actiontext = new GameObject();
        actiontext.AddComponent<ActionText>();
        actiontext.AddComponent<Text>();
        actiontext.AddComponent<SpriteRenderer>();
        actiontext.GetComponent<Text>().text = _text;
        actiontext.GetComponent<Text>().color = _color;
        Instantiate(actiontext, transform.position, Quaternion.identity);
    }
}
