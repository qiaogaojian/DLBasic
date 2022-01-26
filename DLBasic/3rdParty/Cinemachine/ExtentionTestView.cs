using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ExtentionTestView : MonoBehaviour
{
    private float shakeTime = 0;

    public float Delay = 0;
    public Vector3 Strength = new Vector3(0.3f, 0.3f, 0);
    public float CycleTime = 0.2f;
    public float Duration = 0.1f;


    float countTime;
    private void Update()
    {
        if (Input.GetKey(KeyCode.Space) && Time.time - shakeTime > 0.3f)
        {
            shakeTime = Time.time;
            ShakeManager.AddShake(Delay, Strength, CycleTime, Duration);
        }

        if (isOpen)
        {
            countTime -= Time.deltaTime;
            if (countTime <= 0)
            {
                isOpen = false;
            }
        }

        if (isVIb)
        {
            vibCount -= Time.deltaTime;
            if (vibCount <= 0)
            {
                isVIb = false;
            }
        }
    }
    bool isOpen;
    public void Sneak()
    {
        if (isOpen) return;
        countTime = Duration;
        isOpen = true;
        shakeTime = Time.time;
        ShakeManager.AddShake(Delay, Strength, CycleTime, Duration);
    }
    [Button]
    public void Sneak(Vector3 strength, float circleTime, float duration)
    {
        if (isOpen) return;
        countTime = Duration;
        isOpen = true;
        shakeTime = Time.time;
        ShakeManager.AddShake(Delay, strength, circleTime, duration);
    }

    float vibCount;
    bool isVIb;
    public void Vibrations()
    {
        if (isVIb) return;
        vibCount = 0.2f;
        isVIb = true;

        MoreMountains.NiceVibrations.MMVibrationManager.Haptic(MoreMountains.NiceVibrations.HapticTypes.Selection);
    }
}
