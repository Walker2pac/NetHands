using NetHands;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class PanelWin : MonoBehaviour
{
    [SerializeField] private Panel panel;
    [SerializeField] private Button buttonNext;

    private void OnEnable()
    {
        //StartCoroutine(Delay());
    }

    public void Start()
    {
        buttonNext.onClick.AddListener(HandleButtonNextClick);
    }

    private void HandleButtonNextClick()
    {
        LevelManagement.Default.NextLevel();
        GameState.Instance.ChangeState(GameState.State.MainMenu);
    }

    public IEnumerator Delay()
    {
        yield return new WaitForSeconds(3f);
    }

    private void OnDisable()
    {

        StopAllCoroutines();
    }
}

