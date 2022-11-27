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

    public List<CardData> cards = new List<CardData>();

    public Sprite[] costNumbers   = new Sprite[10];
    public Sprite[] damageNumbers = new Sprite[10];

    private void Awake()
    {
        instance = this;

        playerDeck.Create();
        enemyDeck.Create();
    }

    public void Quit()
    {
        SceneManager.LoadScene(0);
    }

    public void SkipTurn()
    {
        //implement method
    }
}
