using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrainCompartment : MonoBehaviour
{
    protected Train train;
    public virtual void Initialize(Train train)
    {
        this.train = train;
    }
}
