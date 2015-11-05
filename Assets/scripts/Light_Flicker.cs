using UnityEngine;

public class Light_Flicker : bs
{

    public float time = .2f;
    public float min = .5f;
    public float max = 5f;
    public bool useSmooth = false;
    public float smoothTime = 10;
    void Awake()
    {
        OnQualityChanged();
    }
    void Start()
    {

        if (useSmooth == false && light)
        {
            InvokeRepeating("OneLightChange", time, time);
        }
    }
    public override void OnQualityChanged()
    {        
        enabled = !bs.lowestQuality;
    }
    void OneLightChange()
    {
        light.intensity = Random.Range(min, max);
    }

    void Update()
    {
        if (useSmooth && light)
        {
            light.intensity = Mathf.Lerp(light.intensity, Random.Range(min, max), Time.deltaTime * smoothTime);
        }
        if (light == false)
        {
            if (Application.isEditor)
                print("Please add a light component for light flicker");
        }
    }
}