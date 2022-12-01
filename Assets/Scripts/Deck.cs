using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Deck
{
    //randomized deck
    public List<CardData> cardDatas = new List<CardData>();

    //create a randomized deck of cards
    public void Create()
    {
        //create a list of cardData for the deck
        List<CardData> cardDataInOrder = new List<CardData>();

        foreach (CardData cardData in GameController.instance.cards)
        {
            for (int i = 0; i < cardData.numberInDeck; i++)
                cardDataInOrder.Add(cardData);
        }

        //randomize cards in deck
        while (cardDataInOrder.Count > 0)
        {
            int randomIndex = Random.Range(0, cardDataInOrder.Count);
            cardDatas.Add(cardDataInOrder[randomIndex]);
            cardDataInOrder.RemoveAt(randomIndex);
        }
    }

    //get the first card from the deck
    private CardData GetCard()
    {
        if (cardDatas.Count == 0)
            Create();

        CardData result = cardDatas[0];
        cardDatas.RemoveAt(0);

        return result;
    }

    //create a card with the properties from GetCard method
    private Card CreateNewCard(Vector3 position, string animName)
    {
        GameObject newCard = GameObject.Instantiate(GameController.instance.cardPrefab, 
                                                    GameController.instance.canvas.gameObject.transform);
        newCard.transform.position = position;

        Card card = newCard.GetComponent<Card>();

        if (card)
        {
            card.cardData = GetCard();
            card.Initialize();

            Animator animator = newCard.GetComponentInChildren<Animator>();

            if (animator)
            {
                animator.CrossFade(animName, 0);
            }
            else
            {
                Debug.LogError("No Animator found!");
            }
            return card;
        }
        else
        {
            Debug.LogError("No card component found!");
            return null;
        }
    }

    //put a card from the deck into a free space in hand
    internal void DealCard(Hand hand)
    {
        for (int h = 0; h < 3; h++)
        {
            if (hand.cards[h] == null)
            {
                if (hand.isPlayers)
                    GameController.instance.player.PlayDealSound();
                else
                    GameController.instance.enemy.PlayDealSound();

                hand.cards[h] = CreateNewCard(hand.positions[h].position, hand.animNames[h]);
                return;
            }
        }
    }
}
