using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NetHands
{
    public class GameState : MonoBehaviour
    {
        public enum State
        {
            MainMenu,
            BeforePlaying,
            Playing,
            Lose,
            Victory
        }
        private State currentState = State.MainMenu;

        private static GameState instance;
        public static GameState Instance 
        {
            get
            {
                if (instance == null)
                {
                    instance = FindObjectOfType<GameState>();
                }
                return instance;
            }
        }

        public event Action OnMainMenu;
        public event Action OnBeforePlaying;
        public event Action OnPlaying;
        public event Action OnLose;
        public event Action OnVictory;

        private void Awake()
        {
            if (instance != null)
                instance = this;
        }

        private void OnEnable()
        {
            LevelManagement.Default.OnLevelSelected += OnLevelManagementLevelSelected;
        }

        private void OnDisable()
        {
            LevelManagement.Default.OnLevelSelected -= OnLevelManagementLevelSelected;
        }

        private void OnLevelManagementLevelSelected()
        {
            ChangeState(State.MainMenu);
        }

        private void Start()
        {
            ChangeState(State.MainMenu);
        }

        public void ChangeState(State state)
        {
            currentState = state;

            switch (currentState)
            {
                case State.MainMenu:
                    OnMainMenu?.Invoke();
                    break;
                case State.BeforePlaying:
                    OnBeforePlaying?.Invoke();
                    break;
                case State.Playing:
                    OnPlaying?.Invoke();
                    break;
                case State.Lose:
                    OnLose?.Invoke();
                    break;
                case State.Victory:
                    OnVictory?.Invoke();
                    break;
            }
        }

        public bool CompareState(State state)
        {
            return state == currentState;
        }
    }
}