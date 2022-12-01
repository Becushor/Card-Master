using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Player : MonoBehaviour, IDropHandler
{
    public Image playerImage = null;
    public Image mirrorImage = null;
    public Image healthImage = null;
    public Image glowImage   = null;

    public AudioSource dealAudio   = null;
    public AudioSource healAudio   = null;
    public AudioSource mirrorAudio = null;
    public AudioSource smashAudio  = null;

    public int maxHealth = 9;
    public int health = 9; //current health
    public int mana = 1;

    public bool isPlayer;
    public bool isFire; //whether is a fire element monster or not

    public GameObject[] manaBalls = new GameObject[5];

    private Animator animator;

    void Start()
    {
        animator = GetComponent<Animator>();
        UpdateHealth();
        UpdateManaBalls();
    }

    internal void PlayHitAnim()
    {
        if (animator != null)
            animator.SetTrigger("Hit");
    }

    public void OnDrop(PointerEventData eventData)
    {
        if (!GameController.instance.isPlayable)
            return;

        GameObject obj = eventData.pointerDrag;
        if (obj != null)
        {
            Card card = obj.GetComponent<Card>();
            if (card != null)
            {
                GameController.instance.UseCard(card, this, GameController.instance.playerHand);
            }
        }
    }

    internal void UpdateHealth()
    {
        //costNumbers are used for card cost and player health
        if (health >= 0 && health < GameController.instance.costNumbers.Length)
        {
            healthImage.sprite = GameController.instance.costNumbers[health];
        }
        else
        {
            healthImage.sprite = GameController.instance.costNumbers[0];
        }
    }

    internal void SetMirror(bool on)
    {
        mirrorImage.gameObject.SetActive(on);
    }

    internal bool HasMirror()
    {
        return mirrorImage.gameObject.activeInHierarchy;
    }

    internal void UpdateManaBalls()
    {
        for (int i = 0; i < 5; i++)
        {
            if (mana > i)
                manaBalls[i].SetActive(true);
            else
                manaBalls[i].SetActive(false);
        }
    }

    internal void PlayDealSound()
    {
        dealAudio.Play();
    }

    internal void PlayHealSound()
    {
        healAudio.Play();
    }

    internal void PlayMirrorSound()
    {
        mirrorAudio.Play();
    }

    internal void PlaySmashSound()
    {
        smashAudio.Play();
    }
}
