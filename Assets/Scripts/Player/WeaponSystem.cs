using System.Collections;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI.Table;

public class WeaponSystem : MonoBehaviour
{
    [SerializeField] private Transform turretHolder;
    [SerializeField] private Transform shootPos;
    [SerializeField] private Projectile projectilePrefab;

    [Space]

    [SerializeField] private int damage;
    [SerializeField] private float fireRate;

    private bool canShoot;

    private void Start()
    {
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

        if(PlayerDataManager.Instance.UseBullets(1))
            Instantiate(projectilePrefab, shootPos.position, shootPos.rotation).Initialize(damage);
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