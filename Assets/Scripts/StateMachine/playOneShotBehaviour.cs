using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class playOneShotBehaviour : StateMachineBehaviour
{
    public AudioClip sound;
    public float volume = 1f;
    public bool playOnEnter = true, playOnExit = false, playAfterDelay = false;

    public float playDelay = 0.25f;
    private float timeSinceEntered = 0;
    private bool hasDelayedSoundPlay = false;
    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if(playOnEnter)
        {
            AudioSource.PlayClipAtPoint(sound, animator.gameObject.transform.position, volume);
        }
        timeSinceEntered = 0f;
        hasDelayedSoundPlay = false;
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if(playAfterDelay && !hasDelayedSoundPlay)
        {
            timeSinceEntered += Time.deltaTime;
            if(timeSinceEntered>playDelay)
            {
                AudioSource.PlayClipAtPoint(sound, animator.gameObject.transform.position, volume);
                hasDelayedSoundPlay = true;
            }

        }
    }

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (playOnExit)
        {
            AudioSource.PlayClipAtPoint(sound, animator.gameObject.transform.position, volume);
        }
    }

    
}
