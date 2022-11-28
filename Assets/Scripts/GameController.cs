using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour
{
    static public GameController instance;

    public Deck playerDeck = new Deck();
    public Deck enemyDeck  = new Deck();

    public Hand playerHand = new Hand();
    public Hand enemyHand  = new Hand();

    public Sprite[] costNumbers   = new Sprite[10];
    public Sprite[] damageNumbers = new Sprite[10];

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
}
