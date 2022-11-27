using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour
{
    static public GameController instance;

    public Sprite[] costNumbers = new Sprite[10];
    public Sprite[] damageNumbers = new Sprite[10];

    private void Awake()
    {
        instance = this;
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
