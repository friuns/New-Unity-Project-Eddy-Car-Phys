using UnityEngine;

public partial class bs
{

    public static void PlayOneShotGui(AudioClip sound, float volume = 1)
    {
        if (!_Loader.audio.isPlaying)
            _Loader.audio.PlayOneShot(sound, volume * res.volumeFactor);
    }
    public void PlayOneShot(AudioClip a, float volume = 1, bool forcePlay = true, float pitch = 1)
    {
        if (!audio.isPlaying || forcePlay)
        {
            audio.clip = a;
            audio.pitch = pitch;
            audio.volume = res.volumeFactor;
            //var magnitude = (CameraMainTransform.position - pos).magnitude;
            //audio.priority = magnitude < 20 ? 128 : 128 + (int)magnitude;
            audio.Play();
        }
    }
    public static void PlayClipAtPoint(AudioClip clip, Vector3 position, float volume = 1.0F)
    {
        GameObject obj2 = new GameObject("One shot audio");
        obj2.hideFlags = HideFlags.HideInHierarchy;
        obj2.transform.position = position;
        AudioSource source = (AudioSource)obj2.AddComponent(typeof(AudioSource));
        source.clip = clip;
        source.volume = volume;
        var magnitude = (CameraMainTransform.position - position).magnitude;
        source.priority = 128 + (int)magnitude;
        source.priority = magnitude < 20 ? 128 : 128 + (int)magnitude;
        source.Play();
        Object.Destroy(obj2, clip.length * Time.timeScale);
    }

}

public class bsSound : bs
{
    //public AudioSource[] srs;
    //public void Start()
    //{
    //    srs = transform.GetComponentsInChildren<AudioSource>();
    //}
    //public void Update()
    //{
    //    if (!AudioListener.pause)
    //        foreach (var a in srs)
    //            a.priority = 128 + (int)(CameraMainTransform.position - pos).magnitude;
    //}
}