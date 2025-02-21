using System.Collections;
using UnityEngine;

public abstract class Enemy : MonoBehaviour
{
    protected Animator animator;
    protected EnemyHealthComponenet enemyStats;
    private Vector2 targetPosition;
    private Vector2 startingPosition;
    [SerializeField] private float timeToTarget;
    private float currentMoveTime;

    private bool retreating;

    public virtual void Initialize(Vector2 targetPosition)
    {
        animator = GetComponent<Animator>();

        this.targetPosition = targetPosition;
        startingPosition = transform.position;

        enemyStats= GetComponent<EnemyHealthComponenet>();
        enemyStats.Initialize(this);
    }

    private void Update()
    {
        if(currentMoveTime < timeToTarget)
        {
            transform.position = Vector2.Lerp(startingPosition, targetPosition, currentMoveTime / timeToTarget);
            currentMoveTime += Time.deltaTime;
            return;
        }

        if(retreating)
        {
            Destroy(gameObject);
            retreating = false;
            return;
        }

        Combat();
    }

    public virtual void Killed()
    {
        EnemyManager.Instance.RemoveEnemy(this);
        Destroy(gameObject);
    }

    public virtual void Retreat()
    {
        currentMoveTime = 0;
        Vector2 temp = targetPosition;
        targetPosition = startingPosition;
        startingPosition = temp;
        retreating = true;
    }

    protected abstract void Combat();
}