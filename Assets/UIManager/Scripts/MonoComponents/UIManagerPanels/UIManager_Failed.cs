using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;


    
    public partial class UIManager
    {
        
        private void SetupStateFailed()
        {
            statesMap.AddState((int)State.Failed, StateFailedOnStart, StateFailedOnEnd);
        }
        private void StateFailedOnStart()
        {
            panelLevelFailed.OpenPanel();
        }
        private void StateFailedOnEnd(int stateTo)
        {
            panelLevelFailed.ClosePanel();
        }
    }

