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

    IEnumerator ResetStat(float stat, Color statColor);
    void AddGem(GameObject _item);
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
            PickUp(collider.gameObject.name == "Player" ? playerController : rivalController);
        }
    }

    void PickUp<T>(T controller) where T : MonoBehaviour, IController

    {
        if (controller.gameObject.name == "Player") Manager.Instance.AddScore(score);
        else  Manager.Instance.AddRivalScore(score);
        controller.AddGem(gameObject);
        Destroy(gameObject);
    }

    void GemEffect<T>(T controller) where T : MonoBehaviour, IController
    {
        if (gemType == GemType.Potassium)
        {
            Color statColor = Color.green;
            StartCoroutine(controller.ResetStat(controller.miningSpeed, statColor));
            controller.miningSpeed += 1;
            Manager.Instance.ShowText(transform, "\n Mining rate increased!", statColor);
        }
        else if (gemType == GemType.Sapphire)
        {
            Color statColor = Color.cyan;
            StartCoroutine(controller.ResetStat(controller.speed, statColor));
            controller.speed += 1;
            Manager.Instance.ShowText( transform, "\n Speed increased!", statColor);
        }
        else Manager.Instance.ShowText( transform, "Picked Up:" + gemName, Color.white);
    }
} 
