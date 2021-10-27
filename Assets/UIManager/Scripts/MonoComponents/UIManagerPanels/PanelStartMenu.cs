using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;
using NetHands;

public class PanelStartMenu : MonoBehaviour
{
    [SerializeField] private Panel panel;
    [SerializeField] private Button buttonStart;

    public static event Action OnStartButtonPressed;


    private void OnEnable()
    {

    }

    public void Start()
    {
        buttonStart.onClick.RemoveAllListeners();

        buttonStart.onClick.AddListener(HandleButtonStartClick);
    }


    private void HandleButtonStartClick()
    {
        GameState.Instance.ChangeState(GameState.State.Playing);
        OnStartButtonPressed?.Invoke();
    }

    private void OnDisable()
    {

    }
}


