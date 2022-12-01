using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

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

    public Sprite fireDemon = null;
    public Sprite iceDemon  = null;

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

    public Text turnText  = null;
    public Text scoreText = null;

    public int playerScore = 0;
    public int playerKills = 0;

    public Image enemySkipTurn = null;

    public AudioSource playerDieAudio = null;
    public AudioSource enemyDieAudio  = null;

    public bool isPlayable = false;
    public bool playerTurn = true;

    private void Awake()
    {
        instance = this;

        SetUpEnemy();

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
        if (playerTurn && isPlayable)
            NextPlayerTurn();
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
            usingOnPlayer.PlayMirrorSound();
            NextPlayerTurn();
            isPlayable = true;
        }
        else
        {
            if (card.cardData.isDefenseCard) //Healling
            {
                usingOnPlayer.health += card.cardData.damage;
                usingOnPlayer.PlayHealSound();

                if (usingOnPlayer.health > usingOnPlayer.maxHealth)
                    usingOnPlayer.health = usingOnPlayer.maxHealth;

                UpdateHealths();
                NextPlayerTurn();
                isPlayable = true;
            }
            else //isAttackCard
            {
                CastAttackEffect(card, usingOnPlayer);
            }

            if (fromHand.isPlayers)
                playerScore += card.cardData.damage;

            UpdateScore();
        }

        if (fromHand.isPlayers)
        {
            GameController.instance.player.mana -= card.cardData.cost;
            GameController.instance.player.UpdateManaBalls();
        }
        else
        {
            GameController.instance.enemy.mana -= card.cardData.cost;
            GameController.instance.enemy.UpdateManaBalls();
        }
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
                    effect.PlayFireSound();
                    break;
                case CardData.DamageType.Ice:
                    if (card.cardData.isMultiCard)
                        effect.effectImage.sprite = multiIceBallImage;
                    else
                        effect.effectImage.sprite = iceBallImage;
                    effect.PlayIceSound();
                    break;
                case CardData.DamageType.Both:
                    effect.effectImage.sprite = fireAndIceBallImage;
                    effect.PlayFireSound();
                    effect.PlayIceSound();
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
            StartCoroutine(GameOver());
        }
        if (enemy.health <= 0)
        {
            playerKills++;
            playerScore += 10;
            UpdateScore();
            StartCoroutine(NewEnemy());
        }
    }

    private IEnumerator NewEnemy()
    {
        yield return new WaitForSeconds(1);
        enemy.gameObject.SetActive(false);
        enemyHand.ClearHand();
        yield return new WaitForSeconds(1);
        SetUpEnemy();
        enemy.gameObject.SetActive(true);
        StartCoroutine(DealHands());
    }

    private void SetUpEnemy()
    {
        enemy.mana = 0;
        enemy.health = 5;
        enemy.UpdateHealth();
        if (UnityEngine.Random.Range(0, 2) == 1)
        {
            enemy.isFire = false;
            enemy.playerImage.sprite = iceDemon;
        }
        else
        {
            enemy.isFire = true;
            enemy.playerImage.sprite = fireDemon;
        }
    }

    private IEnumerator GameOver()
    {
        yield return new WaitForSeconds(2);
        SceneManager.LoadScene(2);
    }

    internal void TurnGlowImagesOff()
    {
        player.glowImage.gameObject.SetActive(false);
        enemy.glowImage.gameObject.SetActive(false);
    }

    internal void NextPlayerTurn()
    {
        playerTurn = !playerTurn;

        bool enemyIsDead = false;

        if (playerTurn)
        {
            if (player.mana < 5)
                player.mana++;
        }
        else
        {
            if (enemy.health > 0)
            {
                if (enemy.mana < 5)
                    enemy.mana++;
            }
            else
                enemyIsDead = true;
        }
        
        if (enemyIsDead)
        {
            playerTurn = !playerTurn;
            if (player.mana < 5)
                player.mana++;
        }
        else
        {
            SetTurnText();

            if (!playerTurn)
                EnemyTurn();
        }

        player.UpdateManaBalls();
        enemy.UpdateManaBalls();
    }

    private void EnemyTurn()
    {
        Card card = AIChooseCard();
        StartCoroutine(EnemyCastCard(card));
    }

    private Card AIChooseCard()
    {
        List<Card> availableCards = new List<Card>();

        for (int i = 0; i < 3; i++)
        {
            if (IsCardValid(enemyHand.cards[i], enemy, enemyHand))
                availableCards.Add(enemyHand.cards[i]);
            else if (IsCardValid(enemyHand.cards[i], player, enemyHand))
                availableCards.Add(enemyHand.cards[i]);
        }

        if (availableCards.Count == 0) //none available
        {
            NextPlayerTurn();
            return null;
        }

        int choice = UnityEngine.Random.Range(0, availableCards.Count);
        return availableCards[choice];
    }

    private IEnumerator EnemyCastCard(Card card)
    {
        yield return new WaitForSeconds(1);

        if (card)
        {
            FlipCard(card);
            yield return new WaitForSeconds(2);

            if (card.cardData.isDefenseCard)
                UseCard(card, enemy, enemyHand);
            else
                UseCard(card, player, enemyHand);

            yield return new WaitForSeconds(1);
            enemyDeck.DealCard(enemyHand);
            yield return new WaitForSeconds(1);
        }
        else //enemy has no card to cast
        {
            enemySkipTurn.gameObject.SetActive(true);
            yield return new WaitForSeconds(2);
            enemySkipTurn.gameObject.SetActive(false);
        }
    }

    internal void FlipCard(Card card)
    {
        Animator animator = card.GetComponentInChildren<Animator>();

        animator.SetTrigger("Flip");
    }

    internal void SetTurnText()
    {
        if (playerTurn)
            turnText.text = "Merlin's Turn";
        else
            turnText.text = "Enemy's Turn";
    }

    private void UpdateScore()
    {
        scoreText.text = "Demons Killed: " + playerKills.ToString() + "  |  Score: " + playerScore.ToString();
    }

    internal void PlayPlayerDieSound()
    {
        playerDieAudio.Play();
    }

    internal void PlayEnemyDieSound()
    {
        enemyDieAudio.Play();
    }
}
