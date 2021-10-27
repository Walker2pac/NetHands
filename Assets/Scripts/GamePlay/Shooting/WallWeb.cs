using DG.Tweening;
using NetHands.ScriptableObjects;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NetHands.Gameplay.Shooting
{
    public class WallWeb : MonoBehaviour
    {
        [SerializeField] private BulletWebSettingsSO _settings;

        [Space]
        [SerializeField] private Transform _modelTransform;
        public Transform ModelTransform => _modelTransform;

        [Space]
        [Header("Bones")]
        [SerializeField] private Transform _tipPointBone;
        [SerializeField] private Transform _scaleBone;

        [Space]
        [SerializeField] private WallWebTransformation _transformationOnShoot;
        private WallWebTransformation _transformationOnStart;
        private WallWebTransformation _transformationOnStickedToWall;

        private Tween scaleBoneTween;
        private Tween tipBoneTween;
        private Tween tipBoneScaleTween;

        private float randomScale = 0f;

        private void Awake()
        {
            _transformationOnStart = new WallWebTransformation(_scaleBone.localScale, _tipPointBone.localScale, _tipPointBone.position.y);


            float stickedToWallRandomScale = Random.Range(_settings.StickedToWallRandomScale.x, _settings.StickedToWallRandomScale.y);
            _transformationOnStickedToWall = new WallWebTransformation(new Vector3(stickedToWallRandomScale, _scaleBone.localScale.y, stickedToWallRandomScale), _tipPointBone.localScale, _tipPointBone.position.y);
        }

        public void SetShootScale()
        {
            Vector3 mainBoneScale = new Vector3(randomScale, _transformationOnShoot.MainBoneScale.y, randomScale);

            scaleBoneTween = _scaleBone.DOScale(mainBoneScale, _settings.ChangeScaleTime).SetEase(Ease.InFlash);
            tipBoneScaleTween = _tipPointBone.DOScale(_transformationOnShoot.TipBoneScale, _settings.ChangeScaleTime).SetEase(Ease.InFlash);
            tipBoneTween = _tipPointBone.DOLocalMoveY(_transformationOnShoot.TipBoneYPosition, _settings.ChangeScaleTime).SetEase(Ease.InFlash);
        }

        public void SetRandomScale(float time = 0f)
        {
            randomScale = Random.Range(_settings.EndRandomScale.x, _settings.EndRandomScale.y);
            Vector3 scale = new Vector3(randomScale, _scaleBone.localScale.y, randomScale);
            _transformationOnStart.SetMainBoneScale(scale);

            float startRandomScale = Random.Range(_settings.StartRandomScale.x, _settings.StartRandomScale.y);

            _scaleBone.localScale = new Vector3(startRandomScale, startRandomScale, startRandomScale);

            if (time > 0f)
            {
                _scaleBone.localScale = new Vector3(0f, 0f, 0f);
                scaleBoneTween = _scaleBone.DOScale(scale, time).SetEase(Ease.InFlash);
            }
        }

        public void SetTipPointOnGroundCatchPosition()
        {
            Vector3 tipPointLocalPosition = _tipPointBone.localPosition;
            tipPointLocalPosition.x = 0f;
            tipPointLocalPosition.y = _settings.GroundCatchTipLength;
            tipPointLocalPosition.z = 0f;
            _tipPointBone.localPosition = tipPointLocalPosition;
        }

        public void SetTipPointPosition(Vector3 tipPosition)
        {
            _tipPointBone.position = tipPosition;

            Vector3 tipPointLocalPosition = _tipPointBone.localPosition;
            tipPointLocalPosition.x = 0f;
            tipPointLocalPosition.y = Mathf.Clamp(tipPointLocalPosition.y, 0f, _settings.MaximumTipLength);
            tipPointLocalPosition.z = 0f;
            _tipPointBone.localPosition = tipPointLocalPosition;
        }

        public void ResetScale()
        {
            if (scaleBoneTween != null && scaleBoneTween.active) scaleBoneTween.Kill();
            if (tipBoneScaleTween != null && tipBoneScaleTween.active) tipBoneScaleTween.Kill();
            if (tipBoneTween != null && tipBoneTween.active) tipBoneTween.Kill();

            _tipPointBone.localScale = _transformationOnStickedToWall.TipBoneScale;
            _scaleBone.localScale = _transformationOnStickedToWall.MainBoneScale;
            _tipPointBone.localPosition = Vector3.zero;
        }

        public void ResetTipPoint()
        {
            _tipPointBone.DOLocalMove(Vector3.zero, _settings.TimeToResetTipPoint);
        }

        public Transform GetTipPointTransform()
        {
            return _tipPointBone;
        }

        public void PlayShakingAnimation()
        {
            float startTipPointBonePosition = _tipPointBone.localPosition.y;
            StartCoroutine(shakingAnimationCoroutine(startTipPointBonePosition));
        }

        private IEnumerator shakingAnimationCoroutine(float startTipPointBonePosition)
        {
            _tipPointBone.DOLocalMove(new Vector3(_tipPointBone.localPosition.x, startTipPointBonePosition + 0.03f, _tipPointBone.localPosition.z), 0.1f);
            yield return new WaitForSeconds(0.1f);
            _tipPointBone.DOLocalMove(new Vector3(_tipPointBone.localPosition.x, startTipPointBonePosition, _tipPointBone.localPosition.z), 0.1f);
            yield return new WaitForSeconds(0.1f);
            _tipPointBone.DOLocalMove(new Vector3(_tipPointBone.localPosition.x, startTipPointBonePosition + 0.015f, _tipPointBone.localPosition.z), 0.1f);
            yield return new WaitForSeconds(0.1f);
            _tipPointBone.DOLocalMove(new Vector3(_tipPointBone.localPosition.x, startTipPointBonePosition, _tipPointBone.localPosition.z), 0.1f);
        }

        private void OnDestroy()
        {
            scaleBoneTween.Kill();
            tipBoneTween.Kill();
            tipBoneScaleTween.Kill();
        }
    }

    [System.Serializable]
    public struct WallWebTransformation
    {
        [SerializeField] private Vector3 _mainBoneScale;
        public Vector3 MainBoneScale => _mainBoneScale;

        [SerializeField] private Vector3 _tipBoneScale;
        public Vector3 TipBoneScale => _tipBoneScale;

        [SerializeField] private float _tipBoneYPosition;
        public float TipBoneYPosition => _tipBoneYPosition;

        public WallWebTransformation(Vector3 mainBoneScale, Vector3 tipBoneScale, float tipBoneYPosition)
        {
            _mainBoneScale = mainBoneScale;
            _tipBoneScale = tipBoneScale;
            _tipBoneYPosition = tipBoneYPosition;
        }

        public void SetMainBoneScale(Vector3 scale)
        {
            _mainBoneScale = scale;
        }
    }
}