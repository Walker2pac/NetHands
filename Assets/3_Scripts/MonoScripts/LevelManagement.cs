using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using System;
using Dreamteck.Splines;
//using GameAnalyticsSDK;

public class LevelManagement : MonoBehaviour
{
    #region Singletone
    private static LevelManagement _default;
    public static LevelManagement Default { get => _default; }
    public LevelManagement() => _default = this;
    #endregion

    const string PREFS_KEY_LEVEL_ID = "CurrentLevelCount";
    const string PREFS_KEY_LAST_INDEX = "LastLevelIndex";


    public bool editorMode;
    public int CurrentLevelCount => PlayerPrefs.GetInt(PREFS_KEY_LEVEL_ID, 0) + 1;
    public int CurrentLevelIndex;
    public List<Level> Levels = new List<Level>();

    public delegate void RestartDelegate();
    public static event RestartDelegate onRestart;

    public event Action OnNextLevel;
    public event Action OnPrevLevel;
    public event Action OnLevelSelected;

    public void Start()
    {
        #if UNITY_EDITOR
        #else
            editorMode = false;
        #endif
        if (!editorMode) SelectLevel(PlayerPrefs.GetInt(PREFS_KEY_LAST_INDEX, 0), false);
    }

    public void StartGame()
    {
        SendStart();
        GameManager.Instance.StartGame();
    }

    public void RestartLevel()
    {
        SendRestart();
        SelectLevel(CurrentLevelIndex, false);
    }

    public void clearListAtIndex(int levelIndex)
    {
        Levels[levelIndex].LevelPrefab = null;
    }

    public void SelectLevel(int levelIndex, bool indexCheck = true)
    {
        if (indexCheck)
            levelIndex = GetCorrectedIndex(levelIndex);

        if (Levels[levelIndex].LevelPrefab == null)
        {
            Debug.Log("<color=red>There is no prefab attached!</color>");
            return;
        }

        var level = Levels[levelIndex];

        if (level.LevelPrefab)
        {
            SelLevelParams(level);
            CurrentLevelIndex = levelIndex;
        }

        OnLevelSelected?.Invoke();
    }

    public void NextLevel() 
    {
        SendComplete();
        if (!editorMode) 
            PlayerPrefs.SetInt(PREFS_KEY_LEVEL_ID, (PlayerPrefs.GetInt(PREFS_KEY_LEVEL_ID, 0) + 1) );
        SelectLevel(CurrentLevelIndex + 1);
        OnNextLevel?.Invoke();
    }

    public void PrevLevel()
    {
        SelectLevel(CurrentLevelIndex - 1);
        OnPrevLevel?.Invoke();
    }

    private int GetCorrectedIndex(int levelIndex)
    {
        if (editorMode)
            return levelIndex > Levels.Count - 1 || levelIndex <= 0 ? 0 : levelIndex;
        else
        {
            int levelId = PlayerPrefs.GetInt(PREFS_KEY_LEVEL_ID, 0);
            if (levelId > Levels.Count - 1) 
            {
                if (Levels.Count > 1) 
                {
                    while (true) 
                    {
                        levelId = UnityEngine.Random.Range(0, Levels.Count - 1);
                        if (levelId != CurrentLevelIndex) return levelId;
                    }
                }
                else return UnityEngine.Random.Range(0, Levels.Count - 1);
            }
            return levelId;
        }
    }
    private void SelLevelParams(Level level)
    {
        if (level.LevelPrefab)
        {
            ClearChilds();
#if UNITY_EDITOR
            if (Application.isPlaying)
            {
                Instantiate(level.LevelPrefab, transform);
                onRestart?.Invoke();
            }
            else
                PrefabUtility.InstantiatePrefab(level.LevelPrefab, transform);
#else
            Instantiate(level.LevelPrefab, transform);
#endif
        }

        if (level.SkyboxMaterial)
        {
            RenderSettings.skybox = level.SkyboxMaterial;
        }
    }

    private void ClearChilds()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            GameObject destroyObject = transform.GetChild(i).gameObject;

            if (Application.isEditor)
            {
                DestroyImmediate(destroyObject);
            }
            else
            {
                Destroy(destroyObject);
            }
        }
        Transform rayFireMan = GameObject.Find("RayFireMan")?.transform;
        if (rayFireMan != null)
        {
            foreach (Transform t in rayFireMan)
            {
                if (t.gameObject.name != "Pool_Fragments" && t.gameObject.name != "Pool_Particles")
                    Destroy(t.gameObject);
            }
        }
    }

    private void OnDestroy()
    {
        PlayerPrefs.SetInt(PREFS_KEY_LAST_INDEX, CurrentLevelIndex);
    }

    #region Analitics Events

    public void SendStart()
    {
        string content = (PlayerPrefs.GetInt(PREFS_KEY_LEVEL_ID, 0) + 1).ToString();
       //if (!editorMode) //////
            //GameAnalytics.NewProgressionEvent(GAProgressionStatus.Start, content);////////////
    }

    public void SendRestart()
    {
        string content = (PlayerPrefs.GetInt(PREFS_KEY_LEVEL_ID, 0) + 1).ToString();
        //if (!editorMode) GameAnalytics.NewProgressionEvent(GAProgressionStatus.Fail, content);//////////
        //Debug.Log("Analitics Restart " + content);
    }

    public void SendComplete()
    {
        string content = (PlayerPrefs.GetInt(PREFS_KEY_LEVEL_ID, 0) + 1).ToString();
        //if (!editorMode) GameAnalytics.NewProgressionEvent(GAProgressionStatus.Complete, content);////////
        //Debug.Log("Analitics Complete " + content);
    }

    #endregion
}

[System.Serializable]
public class Level
{
    public GameObject LevelPrefab;
    public Material SkyboxMaterial; 
}
