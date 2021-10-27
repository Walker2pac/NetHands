using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TMPro;
using NetHands;

public class PanelFailed : MonoBehaviour
{
    [SerializeField] private Panel panel;
    [SerializeField] private Button buttonRestart;

    private void OnEnable()
    {
        buttonRestart.gameObject.SetActive(true);
    }


    public void Start()
    {
        buttonRestart.onClick.AddListener(HandleButtonRestartClick);
    }

    private void HandleButtonRestartClick()
    {
        LevelManagement.Default.RestartLevel();
        GameState.Instance.ChangeState(GameState.State.MainMenu);
    }

    IEnumerator Delay()
    {
        yield return new WaitForSeconds(2f);

    }

    private void OnDisable()
    {
        StopAllCoroutines();
    }
}

