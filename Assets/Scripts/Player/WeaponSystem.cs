using ClumsyWizard.Audio;
using System.Collections;
using UnityEngine;

public class WeaponSystem : MonoBehaviour
{
    private CW_AudioPlayer audioPlayer;

    [SerializeField] private Transform turretHolder;
    [SerializeField] private Transform shootPos;
    [SerializeField] private Projectile projectilePrefab;

    [Space]

    [SerializeField] private int damage;
    [SerializeField] private float fireRate;
    [SerializeField] private GameObject muzzleFlash;

    private bool canShoot;

    private void Start()
    {
        audioPlayer = GetComponent<CW_AudioPlayer>();
        canShoot = true;
        InputManager.Instance.OnShoot += OnShoot;
    }

    private void Update()
    {
        Vector2 diff = Camera.main.ScreenToWorldPoint(Input.mousePosition) - turretHolder.position;
        float rot = Mathf.Atan2(diff.y, diff.x) * Mathf.Rad2Deg;
        turretHolder.eulerAngles = new Vector3(0.0f, 0.0f, rot);
    }

    private void OnShoot()
    {
        if (!canShoot || GameManager.Instance.State == GameState.Station)
            return;

        canShoot = false;
        Invoke("ResetShoot", fireRate);

        if (PlayerDataManager.Instance.UseBullets(1))
        {
            audioPlayer.Play("Shoot");
            CameraShake.Instance.ShakeObject(0.2f, ShakeMagnitude.Small);
            Destroy(Instantiate(muzzleFlash, shootPos.position, shootPos.rotation), 1.0f);
            Instantiate(projectilePrefab, shootPos.position, shootPos.rotation).Initialize(damage);
        }
    }

    private void ResetShoot()
    {
        canShoot = true;
    }

    public void IncreaseDamage(int amount)
    {
        damage += amount;
    }

    //Clean up
    private void OnDestroy()
    {
        InputManager.Instance.OnShoot -= OnShoot;
    }
}