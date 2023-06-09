using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public interface IController
{
    float miningSpeed { get; set; }
    float speed { get; set; }
    float maxHealth { get; set; }
    float pickLayer { get; set; }

    IEnumerator ResetStat(float stat, Color statColor);
    void AddGem(GameObject _item);
    void Heal();
    void Damage(int damageBy);

    void CreateDoppleGanger();
    IEnumerator KillDoppleganger(float lifespan);
}
public class Gem : MonoBehaviour
{
    public GemScrObj gemData;

    [NonSerialized]
    public GemType gemType;
    [NonSerialized]
    public int score;
    [NonSerialized]
    public Color color;

    public GameObject gemText;

    //sfx
    AudioSource audioSource;

    private void Awake()
    {
        AttachData();
    }

    public void AttachData()
    {
        gemType = gemData.gemType;
        score = gemData.score;
        gameObject.GetComponent<SpriteRenderer>().sprite = gemData.sprite;
        color = gemData.color;
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
        if (collider.tag == "Empty") Destroy(gameObject);
    }

    void PickUp<T>(T controller) where T : MonoBehaviour, IController

    {
        var audioSource = GetComponent<AudioSource>();
        Manager.Instance.PlaySFX(audioSource, "gem");
        if (controller.gameObject.name == "Player") Manager.Instance.AddScore(score);
        else  Manager.Instance.AddRivalScore(score);
        controller.AddGem(gameObject);
        Destroy(gameObject);
    }

    void GemEffect<T>(T controller) where T : MonoBehaviour, IController
    {
        if (gemType == GemType.Potassium)
        {
            StartCoroutine(controller.ResetStat(controller.miningSpeed, color));
            controller.miningSpeed += 0.3f;
            Manager.Instance.ShowText(transform, "\n Mining rate increased!", color);
        }
        else if (gemType == GemType.Sapphire)
        {
            StartCoroutine(controller.ResetStat(controller.speed, color));
            controller.speed += 0.3f;
            Manager.Instance.ShowText( transform, "\n Speed increased!", color);
        }
        else if (gemType == GemType.Ruby)
        {
            StartCoroutine(controller.ResetStat(controller.maxHealth, color));
            controller.maxHealth += 1;
            controller.Heal();
            Manager.Instance.ShowText(transform, "\n Health increased!", color);
        }
        else if (gemType == GemType.Onyx)
        {
            if(controller.pickLayer > 0) controller.pickLayer--;
            Manager.Instance.ShowText(transform, "\n Pick strength increased!", color);
        }
        //else if (gemType == GemType.Mimik)
        //{
        //    controller.CreateDoppleGanger();
        //    StartCoroutine(controller.KillDoppleganger(5f));
        //    Manager.Instance.ShowText(transform, "\n Picked up Mimik!", color);
        //}
        else Manager.Instance.ShowText( transform, "Picked Up:" + gemType.ToString(), Color.white);
    }
} 
