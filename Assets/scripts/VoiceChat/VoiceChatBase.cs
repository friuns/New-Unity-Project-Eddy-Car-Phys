using System;
using System.Linq;
using UnityEngine;

public class VoiceChatBase:MonoBehaviour
{
    float middle=1;
    public void NormalizeSample(float[] sample)
    {
        var max = Math.Max(middle, Mathf.Lerp(middle, sample.Max() * 10, .1f));
        if (max > middle && bs.isDebug) print("Set Max" + middle);
        middle = max;
        for (int i = 0; i < sample.Length; i++)
            sample[i] *= 20f / middle;
    }   
}