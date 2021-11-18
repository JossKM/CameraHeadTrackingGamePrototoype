using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.Events;

public class PickupItem : MonoBehaviour
{
    [SerializeField] private AudioSource audio;
    [SerializeField] private Animator animator;
    [SerializeField] private AudioClip[] soundPool;
    public UnityEvent<PickupItem> onPickupEvent;
    
    public void PickUp()
    {
        onPickupEvent.Invoke(this);
        audio.clip = soundPool[Random.Range(0, soundPool.Length)];
        audio.Play(0);
        Destroy(this, audio.clip.length + 3.0f);
        animator.Play("picked_up");
        GetComponent<Collider>().enabled = false;
    }

    //IEnumerator Fade(Renderer toFade, float time)
    //{
    //    for (float timer = time; timer > 0; timer -= Time.deltaTime)
    //    {
    //        Color col = toFade.material.color;
    //        toFade.material.color = new ;
    //        yield return new WaitForEndOfFrame();
    //    }
    //}
}
