using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class playOneShotBehaviour : StateMachineBehaviour
{
    public AudioClip sound;
    [Range(0f, 1f)] public float volume = 1f;

    public bool playOnEnter = true;
    public bool playOnExit = false;
    public bool playAfterDelay = false;

    public float playDelay = 0.25f;

    private float timeSinceEntered = 0f;
    private bool hasDelayedSoundPlay = false;

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (playOnEnter)
        {
            PlaySound(animator.transform.position);
        }
        timeSinceEntered = 0f;
        hasDelayedSoundPlay = false;
    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (playAfterDelay && !hasDelayedSoundPlay)
        {
            timeSinceEntered += Time.deltaTime;
            if (timeSinceEntered >= playDelay)
            {
                PlaySound(animator.transform.position);
                hasDelayedSoundPlay = true;
            }
        }
    }

    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (playOnExit)
        {
            PlaySound(animator.transform.position);
        }
    }

    private void PlaySound(Vector3 position)
    {
        if (sound == null) return;

        GameObject tempGO = new GameObject("TempAudio");
        tempGO.transform.position = position;

        AudioSource audioSource = tempGO.AddComponent<AudioSource>();
        audioSource.clip = sound;
        audioSource.volume = volume;
        audioSource.spatialBlend = 0f; // 0 = 2D, 1 = 3D
        audioSource.rolloffMode = AudioRolloffMode.Linear;
        audioSource.dopplerLevel = 0f;
        audioSource.playOnAwake = false;

        audioSource.Play();

        Object.Destroy(tempGO, sound.length + 0.1f);
    }
}
