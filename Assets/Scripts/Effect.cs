using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Effect : MonoBehaviour
{
    public Player targetPlayer = null;
    public Card sourceCard = null;
    public Image effectImage = null;

    public void EndTrigger()
    {
        int damage = sourceCard.cardData.damage;
        if (!targetPlayer.isPlayer)
        {
            if (sourceCard.cardData.damageType == CardData.DamageType.Fire && targetPlayer.isFire)
                damage = damage / 2;
            if (sourceCard.cardData.damageType == CardData.DamageType.Ice && !targetPlayer.isFire)
                damage = damage / 2;
        }
        targetPlayer.health -= damage;

        //todo check for death

        GameController.instance.isPlayable = true;

        Destroy(gameObject);
    }
}
