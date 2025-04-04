using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Random = UnityEngine.Random;

public class TrainCompartment : MonoBehaviour
{
    public CompartmentManagement Management { get; private set; }

    protected Train train;
    public TrainHealthComponenet TrainStats { get; protected set; }
    [SerializeField] private Transform gfx;

    //Random Jitter
    private float jitterStrength = 0.02f;
    private float currentTime;

    private void Start()
    {
        gfx = transform.GetChild(0);
    }

    public virtual void Initialize(Train train)
    {
        this.train = train;
        Management = GetComponent<CompartmentManagement>();
        Management.Initialize(train);
        TrainStats = GetComponent<TrainHealthComponenet>();
        TrainStats.Initialize(train);
    }

    protected virtual void Update()
    {
        if(currentTime <= 0.0f)
        {
            gfx.transform.localPosition = Vector2.zero;
            gfx.transform.localPosition = Random.insideUnitCircle * jitterStrength;
        }
        else
        {
            currentTime -= Time.deltaTime;
        }
    }
}