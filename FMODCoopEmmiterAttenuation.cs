//     Copyright (c) 2023 Sawyer King. <sawyer.audio@gmail.com>.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;
using FMOD.Studio;

public class FMODCoopEmitterAttenuation : MonoBehaviour
{
    [Header("GameObject(s) to calculate distance from")]
    [SerializeField] private List<GameObject> targetGameObjects = new List<GameObject>();

    [Header("Length of Radius")]
    [SerializeField] private float distanceRadius = 10f;

    [Header("FMOD Parameter for Distance Between FMODEvent and closest GameObject")]
    [SerializeField] private string distanceParameter;

    [Header("FMOD 3D Emitter Event")]
    [SerializeField] private EventReference _3DEvent;


    private EventInstance instance;
    private float distanceBetweenObjectOne;
    private float distanceBetweenObjectTwo;
    private float distanceFinal;
    private bool IsInstancePlaying = false;
    private Coroutine distanceCheckCoroutine;


    
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && distanceCheckCoroutine == null)
        {
            distanceCheckCoroutine = StartCoroutine(CoopPlayerDistanceCheck());
        }
    }

    IEnumerator CoopPlayerDistanceCheck()
    {
        while (true)
        {  
            float closestDistance = float.MaxValue;

            foreach (GameObject target in targetGameObjects)
            {
                if (target != null)
                {
                    float distance = Vector3.Distance(transform.position, target.transform.position);
                    if (distance < closestDistance)
                    {
                        closestDistance = distance;
                    }
                }
            }
            
            if (_3DEvent.Path.Length > 0)
            {
                if (closestDistance <= distanceRadius)
                {
                    if (!IsInstancePlaying)
                    {
                        IsInstancePlaying = true;
                        //create an instance of the sound, set it's 3D transform, this script assumes that the sound is not going to move and can just play from a location
                        instance = RuntimeManager.CreateInstance(_3DEvent);
                        instance.set3DAttributes(RuntimeUtils.To3DAttributes(gameObject));
                        instance.start();
                    }
                }
                //stop and release sound if both players are more than 'distanceRadius' units away
                if (closestDistance > distanceRadius)
                {
                    if (IsInstancePlaying)
                    {
                        IsInstancePlaying = false;
                        instance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
                        instance.release();
                    }
                }
            }

            if (instance.isValid())
            {
                instance.setParameterByName(distanceParameter, closestDistance);
            }

            yield return new WaitForSeconds(0.25f);
        }
    }
    
    private void OnDestroy()
    {
         if (distanceCheckCoroutine != null)
        {
            StopCoroutine(distanceCheckCoroutine);
        }
        //stop and release the sound when the object is destroyed
        if (IsInstancePlaying)
                {
                    IsInstancePlaying = false;
                    instance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
                    instance.release();
                }
    }
}
