using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

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

    public GameObject gemText;

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
            GemEffect(collider.gameObject.name == "Player" ? playerController : rivalController);
            PickUp(collider.gameObject.name == "Player");
        }
    }

    void PickUp(bool player)
    {
        if (player) Manager.Instance.AddScore(score);
        else Manager.Instance.AddRivalScore(score);
        Destroy(gameObject);
    }

 

    void GemEffect<T>(T controller) where T : MonoBehaviour, IController
    {
        if (gemType == GemType.Potassium)
        {
            controller.miningSpeed += 1;
            ShowText("\n Mining rate increased!", Color.green);
        }
        else if (gemType == GemType.Sapphire)
        {
            controller.speed += 1;
            ShowText("\n Speed increased!", Color.cyan);
        }
        else ShowText("Picked Up:" + gemName, Color.white);
    }


    void ShowText(string _text, Color _color)
    {
        var healthObj = Instantiate(gemText, transform.position, Quaternion.identity);
        Transform[] children = healthObj.GetComponentsInChildren<Transform>();
        foreach (var child in children)
        {
            if (child.gameObject.name == "text")
            {
                var textObj = child.gameObject.GetComponentInChildren<TMP_Text>();
                textObj.text = _text;
                textObj.color = _color;
            }
        }
    }
}
