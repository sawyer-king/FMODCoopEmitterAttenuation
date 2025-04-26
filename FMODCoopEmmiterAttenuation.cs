//     Copyright (c) 2023 Sawyer King. <sawyer.audio@gmail.com>. All rights reserved.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;
using FMOD.Studio;

public class FMODCoopEmitterAttenuation : MonoBehaviour
{
    [Header("GameObject(s) to calculate distance from")] // add ability to make this 2-4, or just unlimited additions??
    [field: SerializeField] private GameObject _gameObjectOne;
    [field: SerializeField] private GameObject _gameObjectTwo;

    [Header("FMOD Parameter for Distance Between FMODEvent and gameObject")]
    [field: SerializeField] private string distanceParameter;

    [Header("FMOD 3D Event")]
    [field: SerializeField] private EventReference _3DEvent;

    private EventInstance instance;
    private float distanceBetweenObjectOne;
    private float distanceBetweenObjectTwo;
    private float distanceFinal;
    private bool IsInstancePlaying = false;

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
    private void OnDestroy()
    {
        //stop and release the sound when the object is destroyed
        if (IsInstancePlaying == true)
                {
                    IsInstancePlaying = false;
                    instance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
                    instance.release();
                }
    }
}