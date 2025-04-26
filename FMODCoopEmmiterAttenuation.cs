//     Copyright (c) 2023 Sawyer King. <sawyer.audio@gmail.com>.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;
using FMOD.Studio;

public class FMODCoopEmitterAttenuation : MonoBehaviour
{
    [Header("GameObject(s) to calculate distance from - should be Player GameObjects")] // add ability to make this 2-4, or just unlimited additions?? List<GameObject> ????
    //[field: SerializeField] private GameObject _gameObjectOne;
    //[field: SerializeField] private GameObject _gameObjectTwo;
    [SerializeField] private List<GameObject> targetGameObjects = new List<GameObject>();

    [Header("FMOD Parameter for Distance Between FMODEvent and closest GameObject")]
    [SerializeField] private string distanceParameter;

    [Header("FMOD 3D Emitter Event")]
    [SerializeField] private EventReference _3DEvent;

    // add a spot to set the distance you want to check from as a variable

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
            //distanceBetweenObjectOne = Vector3.Distance(transform.position, _gameObjectOne.transform.position);
            //distanceBetweenObjectTwo = Vector3.Distance(transform.position, _gameObjectTwo.transform.position);

            //decide which gameObject is closer and then use that as the distance parameter to send to FMOD
            // distanceFinal = (distanceBetweenObjectOne < distanceBetweenObjectTwo) ? distanceBetweenObjectOne : distanceBetweenObjectTwo;

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
            
            //only do if also playing event through this script, checking if EventReference field is null
            if (_3DEvent.Path.Length > 0)
            {
                if (distanceFinal <= 15)
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
                //stop and release sound if both players are more than 15 units away
                if (distanceFinal > 15)
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
                instance.setParameterByName(distanceParameter, distanceFinal);
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
/*
void Update()
    {
    #if UNITY_EDITOR
        distanceBetweenObjectOne = Vector3.Distance(transform.position, _gameObjectOne.transform.position);

        distanceBetweenObjectTwo = Vector3.Distance(transform.position, _gameObjectTwo.transform.position);

        //decide which gameObject is closer and then use that as the distance parameter to send to FMOD
        if (distanceBetweenObjectOne < distanceBetweenObjectTwo)
        {
            distanceFinal = distanceBetweenObjectOne;
        }
        else
        {
            distanceFinal = distanceBetweenObjectTwo;
        }
        //only do if also playing event through this script, checking if EventReference field is null
        if (_3DEvent.Path.Length > 0)
        {
            if (distanceFinal <= 15)
            {
                if (IsInstancePlaying == false)
                {
                    IsInstancePlaying = true;
                    //create an instance of the sound, set it's 3D transform, this script assumes that the sound is not going to move and can just play from a location
                    instance = RuntimeManager.CreateInstance(_3DEvent);
                    instance.set3DAttributes(RuntimeUtils.To3DAttributes(gameObject));
                    instance.start();
                }
            }
            //stop and release sound if both players are more than 15 units away
            if (distanceFinal > 15)
            {
                if (IsInstancePlaying == true)
                {
                    IsInstancePlaying = false;
                    instance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
                    instance.release();
                }
            }
        }

        instance.setParameterByName(distanceParameter, distanceFinal);
    #endif

    #if UNITY_STANDALONE
    
        distanceBetweenObjectOne = Vector3.Distance(transform.position, _gameObjectOne.transform.position);

        distanceBetweenObjectTwo = Vector3.Distance(transform.position, _gameObjectTwo.transform.position);

        //decide which gameObject is closer and then use that as the distance parameter to send to FMOD
        if (distanceBetweenObjectOne < distanceBetweenObjectTwo)
        {
            distanceFinal = distanceBetweenObjectOne;
        }
        else
        {
            distanceFinal = distanceBetweenObjectTwo;
        }
        //only do if also playing event through this script, checking if EventReference field is null
        if (_3DEvent.ToString() != null)
        {
            if (distanceFinal <= 15)
            {
                if (IsInstancePlaying == false)
                {
                    IsInstancePlaying = true;
                    //create an instance of the sound, set it's 3D transform, this script assumes that the sound is not going to move and can just play from a location
                    instance = RuntimeManager.CreateInstance(_3DEvent);
                    instance.set3DAttributes(RuntimeUtils.To3DAttributes(gameObject));
                    instance.start();
                }
            }
            //stop and release sound if both players are more than 15 units away
            if (distanceFinal > 15)
            {
                if (IsInstancePlaying == true)
                {
                    IsInstancePlaying = false;
                    instance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
                    instance.release();
                }
            }
        }

        instance.setParameterByName(distanceParameter, distanceFinal);
    }
    #endif
    */
    