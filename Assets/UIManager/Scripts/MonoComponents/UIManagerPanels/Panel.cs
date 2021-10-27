using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using UnityEngine;
using System.Collections;


    [RequireComponent(typeof(CanvasGroup), typeof(Animator))]
    public class Panel : MonoBehaviour
    {
        [Serializable]
        public class PanelList
        {
            public List<Panel> list = new List<Panel>();

            public void OpenPanels()
            {
                foreach (Panel panel in list)
                    panel.OpenPanel();
            }
            public void ClosePanels()
            {
                foreach (Panel panel in list)
                    panel.ClosePanel();
            }
        }
        public enum FadeMode { In, Out }
        public enum State { None, Opened, Closed }

        public Animator animator;
        public bool fading;
        public bool hideOnStart = true;
        public bool closeOnShadowClick;
        public bool playShowHideAudio;
        public bool saveInitialPosition;

        public event Action OnPanelShow = () => { };
        public event Action OnPanelHide = () => { };

        public State CurState => curState;
        private State curState;

        private List<object> blockersToShow = new List<object>();

        public static Panel UpperShaded => curShowedPanels.FindLast(p => p.fading);
        public static readonly string EventKeyOnPanelOpen = "EventOnPanelOpen";
        public static readonly string EventKeyOnPanelClose = "EventOnPanelClose";
        public static readonly string AnimKeyPanelFadeIn = "Panel In";
        public static readonly string AnimKeyPanelFadeOut = "Panel Out";

        private static List<Panel> curShowedPanels = new List<Panel>();
        public void Start()
        {
            RectTransform rt = GetComponent<RectTransform>();
            if (rt != null && !saveInitialPosition)
                rt.anchoredPosition = Vector3.zero;

            //if (MonoUI.Default.panelLoading == this)
            //    return;
            if (curState == State.None)
            {
                if (hideOnStart)
                {
                    ClosePanel();
                    gameObject.SetActive(false);
                }
                else
                    OpenPanel();
            }
        }

        public void SetBlock(object blocker, bool arg)
        {
            if (arg && !blockersToShow.Contains(blocker))
                blockersToShow.Add(blocker);
            else if (!arg)
                blockersToShow.Remove(blocker);
        }
        public void TogglePanel(bool arg)
        {
            if (arg)
                OpenPanel();
            else
                ClosePanel();
        }

        //Do not place here logic as On Panel Show
        public void OpenPanel()
        {
            gameObject.SetActive(true);
            PanelFade(FadeMode.In);
        }
        //Do not place here logic as On Panels Hide
        public void ClosePanel()
        {
            PanelFade(FadeMode.Out);
        }
        private void PanelFade(FadeMode fadeMode)
        {
            IEnumerator WaitToShow()
            {
                while (blockersToShow.Count > 0)
                    yield return null;
                PanelFade(FadeMode.In);
            }
            if ((curState == State.Opened && fadeMode == FadeMode.In) ||
                (curState == State.Closed && fadeMode == FadeMode.Out))
                return;
            if (fadeMode == FadeMode.In)
            {
                if (blockersToShow.Count > 0)
                {
                    WaitToShow();
                    return;
                }
                curShowedPanels.Add(this);
                //if (MonoUI.Default.panelShadow != this && fading)
                //    MonoUI.Default.panelShadow.OpenPanel();
                transform.SetAsLastSibling();
                curState = State.Opened;
                OnPanelShow.Invoke();
            }
            else if (fadeMode == FadeMode.Out)
            {
                if (curShowedPanels.Contains(this))
                    curShowedPanels.Remove(this);
                if (fading)
                {
                    Panel panelWithFading = curShowedPanels.Find((p) => p.fading);
                    //if (curShowedPanels.Contains(MonoUI.Default.panelShadow) && panelWithFading == null)
                    //    MonoUI.Default.panelShadow.ClosePanel();
                }

                //List<MonoPanel> fadedPanels = new List<MonoPanel>(curShowedPanels.FindAll(p => p.fading));
                //for (int i = 0; i < fadedPanels.Count; i++)
                //{
                //    MonoPanel panel = fadedPanels[i];
                //    if (i + 1 == fadedPanels.Count)
                //        MonoUI.Default.panelShadow.transform.SetAsLastSibling();
                //    if (panel.fading)
                //        panel.transform.SetAsLastSibling();
                //}
                curState = State.Closed;
                OnPanelHide.Invoke();
            }

            //MonoUI.Default.panelLoading.transform.SetAsLastSibling();
            ProcessorDeferredOperation.Default.Add(() =>
            {
                if (curState == State.Closed)
                    gameObject.SetActive(false);
            }, true, false, 1f);
            animator.Play(fadeMode == FadeMode.In ? AnimKeyPanelFadeIn : AnimKeyPanelFadeOut);
        }
    }

