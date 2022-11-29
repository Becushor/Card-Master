using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour
{
    static public GameController instance = null;

    public Deck playerDeck = new Deck();
    public Deck enemyDeck  = new Deck();

    public Hand playerHand = new Hand();
    public Hand enemyHand  = new Hand();

    public Player player = null;
    public Player enemy  = null;

    public Sprite[] costNumbers   = new Sprite[10];
    public Sprite[] damageNumbers = new Sprite[10];

    public Sprite fireBallImage = null;
    public Sprite iceBallImage  = null;
    public Sprite multiFireBallImage = null;
    public Sprite multiIceBallImage  = null;
    public Sprite fireAndIceBallImage = null;

    public GameObject effectFromLeftPrefab  = null;
    public GameObject effectFromRightPrefab = null;

    public List<CardData> cards = new List<CardData>();

    public GameObject cardPrefab = null;

    public Canvas canvas = null;

    public bool isPlayable = false;

    private void Awake()
    {
        instance = this;

        playerDeck.Create();
        enemyDeck.Create();

        StartCoroutine(DealHands());
    }

    public void Quit()
    {
        SceneManager.LoadScene(0);
    }

    public void SkipTurn()
    {
        //implement method
    }

    internal IEnumerator DealHands()
    {
        yield return new WaitForSeconds(1);
        for (int i = 0; i < 3; i++)
        {
            playerDeck.DealCard(playerHand);
            enemyDeck.DealCard(enemyHand);
            yield return new WaitForSeconds(1);
        }
        isPlayable = true;
    }

    internal bool UseCard(Card card, Player usingOnPlayer, Hand fromHand)
    {
        if (!IsCardValid(card, usingOnPlayer, fromHand))
            return false;

        isPlayable = false;

        CastCard(card, usingOnPlayer, fromHand);

        TurnGlowImagesOff();

        fromHand.BurnCard(card);

        return false;
    }
    
    //check if the card can be used in the wanted way
    internal bool IsCardValid(Card currentCard, Player usingOnPlayer, Hand fromHand)
    {
        bool valid = false;

        if (currentCard == null)
            return false;

        if (fromHand.isPlayers)
        {
            if (currentCard.cardData.cost <= player.mana)
            {
                if (usingOnPlayer.isPlayer && currentCard.cardData.isDefenseCard)
                    valid = true;
                if (!usingOnPlayer.isPlayer && !currentCard.cardData.isDefenseCard)
                    valid = true;
            }
        }
        else //from enemy hand
        {
            if (currentCard.cardData.cost <= enemy.mana)
            {
                if (!usingOnPlayer.isPlayer && currentCard.cardData.isDefenseCard)
                    valid = true;
                if (usingOnPlayer.isPlayer && !currentCard.cardData.isDefenseCard)
                    valid = true;
            }
        }
        return valid;
    }

    //implements the effects of the card
    internal void CastCard(Card card, Player usingOnPlayer, Hand fromHand)
    {
        if (card.cardData.isMirrorCard)
        {
            usingOnPlayer.SetMirror(true);
            isPlayable = true;
        }
        else
        {
            if (card.cardData.isDefenseCard) //Healling
            {
                usingOnPlayer.health += card.cardData.damage;

                if (usingOnPlayer.health > usingOnPlayer.maxHealth)
                    usingOnPlayer.health = usingOnPlayer.maxHealth;

                UpdateHealths();

                StartCoroutine(CastHealEffect(usingOnPlayer));
            }
            else //isAttackCard
            {
                CastAttackEffect(card, usingOnPlayer);
            }
        }

    }

    private IEnumerator CastHealEffect(Player usingOnPlayer)
    {
        yield return new WaitForSeconds(0.5f);
        isPlayable = true;
    }

    internal void CastAttackEffect(Card card, Player usingOnPlayer)
    {
        GameObject effectGO = null;

        if (usingOnPlayer.isPlayer)
            effectGO = Instantiate(effectFromRightPrefab, canvas.gameObject.transform);
        else
            effectGO = Instantiate(effectFromLeftPrefab, canvas.gameObject.transform);

        Effect effect = effectGO.GetComponent<Effect>();

        if (effect)
        {
            effect.targetPlayer = usingOnPlayer;
            effect.sourceCard = card;

            switch (card.cardData.damageType)
            {
                case CardData.DamageType.Fire:
                    if (card.cardData.isMultiCard)
                        effect.effectImage.sprite = multiFireBallImage;
                    else
                        effect.effectImage.sprite = fireBallImage;
                break;
                case CardData.DamageType.Ice:
                    if (card.cardData.isMultiCard)
                        effect.effectImage.sprite = multiIceBallImage;
                    else
                        effect.effectImage.sprite = iceBallImage;
                break;
                case CardData.DamageType.Both:
                    effect.effectImage.sprite = fireAndIceBallImage;
                break;
            }
        }
    }

    internal void UpdateHealths()
    {
        player.UpdateHealth();
        enemy.UpdateHealth();

        if (player.health <= 0)
        {
            //todo GameOver 
        }
        if (enemy.health <= 0)
        {
            //todo new enemy
        }
    }

    internal void TurnGlowImagesOff()
    {
        player.glowImage.gameObject.SetActive(false);
        enemy.glowImage.gameObject.SetActive(false);
    }
}
