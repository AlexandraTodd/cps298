using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundRandomizer : MonoBehaviour
{
    public AudioClip[] clips;
    public float repeatTime = 0.25f;
    public float volume = 0.2f;
    private AudioSource source;
    private Coroutine coro = null;

    // Start is called before the first frame update
    void Start()
    {
        source = GetComponent<AudioSource>();
    }

    private IEnumerator Timer() {
        while (true) {
            Trigger();
            yield return new WaitForSeconds(repeatTime);
        }
    }

    public void StartSound() {
        if (coro == null) coro = StartCoroutine(Timer());
    }

    public void StopSound() {
        if (coro != null) {
            StopCoroutine(coro);
            coro = null;
        }
    }

    public void Trigger() {
        int idx = Random.Range(0, clips.Length);
        source.PlayOneShot(clips[idx], volume);
    }
}
