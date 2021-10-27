using NetHands;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Shooting : MonoBehaviour
{
    public AnimationCurve we;
    public Transform GunPosLeft;
    public Transform GunPosRihgt;

    public Animator LeftHandAnimator;
    public Animator RightHandAnimator;

    public GameObject Bullet;

    private bool leftHand;

    public float Distance;
    public float ShootForce;

    [SerializeField] private float verticalOffset = 0f;
    [SerializeField] private float animationDelayBeforeShoot = 0.2f;
    [SerializeField] private BulletType[] BulletTypes;
    private int currentBulletTypeIndex = 0;

    private void Start()
    {
        ChangeBulletType(currentBulletTypeIndex);
    }

    private void OnEnable()
    {
        UIManager.Default.OnChangeWeaponClicked += OnChangeWeaponClicked;
    }

    private void OnDisable()
    {
        if (UIManager.Default != null)
        {
            UIManager.Default.OnChangeWeaponClicked -= OnChangeWeaponClicked;
        }
    }

    private void OnChangeWeaponClicked()
    {
        currentBulletTypeIndex++;
        if (currentBulletTypeIndex >= BulletTypes.Length)
        {
            currentBulletTypeIndex = 0;
        }
        ChangeBulletType(currentBulletTypeIndex);
    }

    void Update()
    {
        if (!GameState.Instance.CompareState(GameState.State.Playing)) return;

        if (Input.GetMouseButtonDown(0) && !EventSystem.current.IsPointerOverGameObject())
        {
            Shoot();
        }
    }
    void Shoot()
    {

        var MousePos = Input.mousePosition;
        MousePos.z = Distance;
        MousePos.y += verticalOffset;
        MousePos = Camera.main.ScreenToWorldPoint(MousePos);

        var MouseDir = MousePos - transform.position;
        MouseDir = MouseDir.normalized;

        Transform SpawnTransform;
        if (leftHand)
        {
            SpawnTransform = GunPosLeft;

            LeftHandAnimator.SetTrigger("Atack");
        }
        else
        {
            SpawnTransform = GunPosRihgt;

            RightHandAnimator.SetTrigger("Atack");
        }
        leftHand = !leftHand;

        StartCoroutine(creatingBulletCoroutine(SpawnTransform, MousePos, MouseDir));
    }

    private IEnumerator creatingBulletCoroutine(Transform SpawnTransform, Vector3 MousePos, Vector3 MouseDir)
    {
        yield return new WaitForSeconds(animationDelayBeforeShoot);

        GameObject bullet = Instantiate(Bullet, SpawnTransform.position, Quaternion.Euler(0f, 0f, 0f));
        bullet.transform.LookAt(MousePos);
        bullet.GetComponent<Rigidbody>().AddForce(MouseDir * ShootForce);

        if (bullet.TryGetComponent(out Bullet bulletComponent))
        {
            Debug.DrawRay(SpawnTransform.position, (MousePos - SpawnTransform.position).normalized * 10f, Color.red, 5f);
            bulletComponent.SetDirection((MousePos - SpawnTransform.position).normalized);
        }
    }
   
    private void ChangeBulletType(int index)
    {
        Bullet = BulletTypes[index].Prefab.gameObject;
        if (UIManager.Default != null)
        {
            UIManager.Default.ChangeWeaponButtonIcon(GetNextBulletType());
        }
    }

    private BulletType GetNextBulletType()
    {
        int nextIndex = currentBulletTypeIndex + 1;
        if (nextIndex >= BulletTypes.Length)
        {
            nextIndex = 0;
        }
        return BulletTypes[nextIndex];
    }
}

[System.Serializable]
public class BulletType
{
    [SerializeField] private string name;

    [SerializeField] private Bullet _prefab;
    public Bullet Prefab => _prefab;

    [SerializeField] private Sprite _icon;
    public Sprite Icon => _icon;
}
