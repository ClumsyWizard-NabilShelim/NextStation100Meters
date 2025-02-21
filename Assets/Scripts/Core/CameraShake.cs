using ClumsyWizard.Core;
using UnityEngine;


public enum ShakeMagnitude
{
    Small = 2,
    Medium = 4,
    Large = 6
}

public class CameraShake : CW_Singleton<CameraShake>
{
    private bool shake;

    private float duration;
    private float magnitude;

    private Camera cam;
    private Vector3 originalPos;

    private void Start()
    {
        cam = Camera.main;
        originalPos = cam.transform.localPosition;
    }

    void Update()
    {
        if (shake)
        {
            if (duration > 0)
            {
                Vector3 randomPosition = Random.insideUnitSphere;
                cam.transform.localPosition = Vector3.Slerp(cam.transform.localPosition, originalPos + new Vector3(randomPosition.x, randomPosition.y, 0) * magnitude, Time.deltaTime);
                duration -= Time.deltaTime;
            }
            else
            {
                ShakeOver();
            }
        }
        else
        {

        }
    }

    private void ShakeOver()
    {
        duration = 0;
        cam.transform.localPosition = originalPos;
        cam.transform.localRotation = Quaternion.identity;
        shake = false;
    }

    public void ShakeObject(float duration, ShakeMagnitude magnitude)
    {
        this.duration = duration;
        this.magnitude = (int)magnitude;

        shake = true;
    }
}
