using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DG.Tweening;
using NetHands;
using UnityEngine;
using UnityEngine.UI;

public partial class UIManager : MonoBehaviour
{

    public static UIManager Default 
    {
        get
        {
            if (_default == null)
            {
                _default = FindObjectOfType<UIManager>();
            }
            return _default;
        }
    }
    private static UIManager _default;


    public Panel panelProgress;

    public Panel panelLevelFailed;

    public Panel panelLevelWin;

    public Panel panelMainMenu;

    public Canvas mainCanvas;

    [SerializeField] private Button changeWeaponButton;
    [SerializeField] private Image changeWeaponImage;

    public event Action OnChangeWeaponClicked;

    private void OnEnable()
    {
        GameState.Instance.OnMainMenu += OpenMainManu;
        GameState.Instance.OnPlaying += CloseMainMenu;
        GameState.Instance.OnLose += OpenLosePanel;
        GameState.Instance.OnVictory += OpenVictoryPanel;

        changeWeaponButton.onClick.AddListener(HandleChangeWeaponButtonClick);
    }
    private void OnDisable()
    {
        if (GameState.Instance != null)
        {
            GameState.Instance.OnMainMenu -= OpenMainManu;
            GameState.Instance.OnPlaying -= CloseMainMenu;
            GameState.Instance.OnLose -= OpenLosePanel;
            GameState.Instance.OnVictory -= OpenVictoryPanel;
        }

        changeWeaponButton.onClick.RemoveListener(HandleChangeWeaponButtonClick);
    }

    public enum State { None, MainMenu, Play, Failed, Win }

    public State CurState
    {
        get => curState;
        set
        {
            if (curState == value ||
            !statesMap.ContainsKey((int)value) ||
            !statesMap[(int)value].Condition((int)curState))
                return;
            statesMap[(int)curState].OnEnd((int)value);
            statesMap[(int)value].OnStart();
            curState = value;
        }
    }
    private State curState;
    private Dictionary<int, StateDefault> statesMap = new Dictionary<int, StateDefault>();
    public void Awake()
    {
        _default = this;

        statesMap.AddState((int)State.None, () => { }, (a) => { });

        SetupStateMainMenu();
        SetupStatePlay();
        SetupStateFailed();
        SetupStateWin();
    }
    public void ExitGame()
    {
        Application.Quit();
    }
    public void SetHighlightUIElement(GameObject go, bool arg)
    {

        Canvas canvas = null;
        if (arg)
        {
            canvas = go.AddComponent<Canvas>();
            canvas.overrideSorting = true;
            canvas.sortingOrder = 1;
        }
        else
        {
            canvas = go.GetComponent<Canvas>();
            GameObject.Destroy(canvas);
        }
    }

    private void OpenMainManu()
    {
        CurState = State.MainMenu;
    }

    private void OpenVictoryPanel()
    {
        CurState = State.Win;
    }

    private void OpenLosePanel()
    {
        CurState = State.Failed;
    }

    private void CloseMainMenu()
    {
        CurState = State.Play;
    }

    private void HandleChangeWeaponButtonClick()
    {
        OnChangeWeaponClicked?.Invoke();
    }

    public void ChangeWeaponButtonIcon(BulletType bulletType)
    {
        changeWeaponImage.sprite = bulletType.Icon;
    }
}

