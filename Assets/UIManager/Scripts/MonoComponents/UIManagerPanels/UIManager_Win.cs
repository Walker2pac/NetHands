using DG.Tweening;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;


    public partial class UIManager
    {
        private void SetupStateWin()
        {
            statesMap.AddState((int)State.Win, StateWinOnStart, StateWinOnEnd);
        }
        private void StateWinOnStart()
        {
            panelLevelWin.OpenPanel();
        }
        private void StateWinOnEnd(int stateTo)
        {
            panelLevelWin.ClosePanel();
        }
    }

