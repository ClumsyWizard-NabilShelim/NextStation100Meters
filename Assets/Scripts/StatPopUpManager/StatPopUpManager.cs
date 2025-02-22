using ClumsyWizard.Core;
using ClumsyWizard.Utilities;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum StatPopUpColor
{
    Red,
    Green
}

public class StatPopUpManager : CW_Singleton<StatPopUpManager>
{
    [SerializeField] private StatPopUp statPopUp;
    [SerializeField] private CW_Dictionary<StatPopUpColor, Color> colors;
    [SerializeField] private float timeBetweenSpawn = 0.5f;

    private Queue<(Vector2 pos, string info, StatPopUpColor color)> popQueue = new Queue<(Vector2 pos, string info, StatPopUpColor color)>();
    private bool canSpawn = true;

    public void ShowStatPopUp(Vector2 position, string info, StatPopUpColor color)
    {
        if (canSpawn)
        {
            Instantiate(statPopUp, position + new Vector2(0.0f, 3.0f), Quaternion.identity).SetData(info, colors[color]);
            canSpawn = false;
            Invoke("ResetSpawn", timeBetweenSpawn);
        }
        else
        {
            popQueue.Enqueue((position, info, color));
        }
    }

    private void ResetSpawn()
    {
        canSpawn = true;
        if (popQueue.Count > 0)
        {
            (Vector2 pos, string info, StatPopUpColor color) = popQueue.Dequeue();
            ShowStatPopUp(pos, info, color);
        }
    }
}