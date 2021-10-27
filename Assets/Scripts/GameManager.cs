using NetHands;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
namespace Dreamteck.Splines
{
    public class GameManager : MonoBehaviour
    {
        public GameObject TestLosePanel;
        public SplineFollower splineFollower;
        public GamePlay gamePlay;
        public static GameManager Instance;
        void Start()
        {
            if (Instance == null)
            {
                Instance = this;
            }
        }

        public void StartGame()
        {
            splineFollower.follow = true;
        }  
        public void TestReloadLevel()
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
            Debug.Log("123");
        }
        public void Lose()
        {
            //GameState.Instance.ChangeState(GameState.State.Lose);
            //gamePlay.ChangeSpeed(0);
            Debug.Log("GameManager: Lose()");
        }
     
        
    }
}

  
