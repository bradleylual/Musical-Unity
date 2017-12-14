//======= Copyright (c) Valve Corporation, All rights reserved. ===============
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(SteamVR_TrackedObject))]
public class SteamVR_TestThrowMusicalUnity : MonoBehaviour
{
    // Variable initialization
	public GameObject prefab;
	public Rigidbody attachPoint;
    //public GameObject noteObj;
    [HideInInspector]
    public bool hold;
    [HideInInspector]
    public List<GameObject> notes = new List<GameObject>();
    [HideInInspector]
    public float volume;
    [HideInInspector]
    public float pitch;
    [HideInInspector]
    public float timbre;
    [HideInInspector]
    public AudioSource audio1 = new AudioSource();
    [HideInInspector]
    public AudioSource audio2 = new AudioSource();
    [HideInInspector]
    public AudioSource audio3 = new AudioSource();
    [HideInInspector]
    public AudioSource audio4 = new AudioSource();

    SteamVR_TrackedObject trackedObj;
	FixedJoint joint;

    // When environment starts
    void Awake()
	{
		trackedObj = GetComponent<SteamVR_TrackedObject>();
        //AudioSource audio = go.GetComponent<AudioSource>();
        //volume = audio.volume;
    }

    // When environment updates
	void FixedUpdate()
	{

		var device = SteamVR_Controller.Input((int)trackedObj.index);
        // Checks for match in contoller position & note position for every already created note
        foreach (var note in notes)
        {
            var objCollision = note.GetComponent<Collider>();
            var noteXMax = objCollision.bounds.max.x;
            var noteXMin = objCollision.bounds.min.x;
            var noteYMax = objCollision.bounds.max.y;
            var noteYMin = objCollision.bounds.min.y;
            var noteZMax = objCollision.bounds.max.z;
            var noteZMin = objCollision.bounds.min.z;
            // Beginning to move note when trigger clicks & controller model is inside note model
            if (joint == null && device.GetTouchDown(SteamVR_Controller.ButtonMask.Trigger) && 
                (attachPoint.transform.position.x <= noteXMax && attachPoint.transform.position.x > noteXMin) &&
                (attachPoint.transform.position.y <= noteYMax && attachPoint.transform.position.y > noteYMin) &&
                (attachPoint.transform.position.z <= noteZMax && attachPoint.transform.position.z > noteZMin))
            {
                note.transform.position = attachPoint.transform.position;
                joint = note.AddComponent<FixedJoint>();
                joint.connectedBody = attachPoint;
                hold = true;
                //Debug.Log(hold);
            }
            // Moves note when trigger is unclicked, changes volume, pitch, timbre based on new position
            else if (joint != null && hold == true && device.GetTouchUp(SteamVR_Controller.ButtonMask.Trigger))
            {
                note.transform.position = attachPoint.transform.position;
                volume = (note.transform.position.x / 2) - .1F;
                pitch = (note.transform.position.y * 2) - (note.transform.position.y / 2);
                timbre = note.transform.position.z;
                if (volume <= 0)
                {
                    volume = 0.01F;
                }
                if (pitch < -3)
                {
                    pitch = -3F;
                }
                if (pitch > 3)
                {
                    pitch = 3F;
                }
                // Switches Audio Source based on z position
                var audioSources = note.GetComponents<AudioSource>();
                audio1 = audioSources[0];
                audio2 = audioSources[1];
                audio3 = audioSources[2];
                audio4 = audioSources[3];
                if (timbre <= .85F)
                {
                    audio1.Stop();
                    audio2.Stop();
                    audio3.Stop();
                    audio4.Stop();
                    audio1.volume = volume;
                    audio1.pitch = pitch;
                    audio1.Play();
                }
                else if (timbre > .85F && timbre <= 1.70F)
                {
                    audio1.Stop();
                    audio2.Stop();
                    audio3.Stop();
                    audio4.Stop();
                    audio2.volume = volume;
                    audio2.pitch = pitch;
                    audio2.Play();
                }
                else if (timbre > 1.70F && timbre <= 2.55F)
                {
                    audio1.Stop();
                    audio2.Stop();
                    audio3.Stop();
                    audio4.Stop();
                    audio3.volume = volume;
                    audio3.pitch = pitch;
                    audio3.Play();
                }
                else
                {
                    audio1.Stop();
                    audio2.Stop();
                    audio3.Stop();
                    audio4.Stop();
                    audio4.volume = volume;
                    audio4.pitch = pitch;
                    audio4.Play();
                }
                // Separates note from controller
                var go = joint.gameObject;
                var rigidbody = go.GetComponent<Rigidbody>();
                Object.DestroyImmediate(joint);
                joint = null;
                hold = false;
            }
        }

        // Creates note when trigger is *otherwise* clicked
        if (joint == null && device.GetTouchDown(SteamVR_Controller.ButtonMask.Trigger))
        {
            var go = GameObject.Instantiate(prefab);
			go.transform.position = attachPoint.transform.position;


			joint = go.AddComponent<FixedJoint>();
			joint.connectedBody = attachPoint;
		}
		else if (joint != null && hold == false && device.GetTouchUp(SteamVR_Controller.ButtonMask.Trigger))
		{
			var go = joint.gameObject;
			var rigidbody = go.GetComponent<Rigidbody>();
			Object.DestroyImmediate(joint);
			joint = null;

            // Can only create four notes with each controller to prevent cluttering
            if (notes.Count < 4)
            {
                notes.Add(go);
                //Debug.Log(notes.Count);
                volume = (go.transform.position.x / 2) - .1F;
                pitch = (go.transform.position.y * 2) - (go.transform.position.y / 2);
                timbre = go.transform.position.z;
                if (volume <= 0)
                {
                    volume = 0.01F;
                }
                if (pitch < -3)
                {
                    pitch = -3F;
                }
                if (pitch > 3)
                {
                    pitch = 3F;
                }
                var audioSources = go.GetComponents<AudioSource>();
                audio1 = audioSources[0];
                audio2 = audioSources[1];
                audio3 = audioSources[2];
                audio4 = audioSources[3];
                if (timbre <= .85F)
                {
                    audio1.volume = volume;
                    audio1.pitch = pitch;
                    audio1.Play();
                }
                else if (timbre > .85F && timbre <= 1.70F)
                {
                    audio2.volume = volume;
                    audio2.pitch = pitch;
                    audio2.Play();
                }
                else if (timbre > 1.70F && timbre <= 2.55F)
                {
                    audio3.volume = volume;
                    audio3.pitch = pitch;
                    audio3.Play();
                }
                else
                {
                    audio4.volume = volume;
                    audio4.pitch = pitch;
                    audio4.Play();
                }
                Debug.Log(volume);
                var origin = trackedObj.origin ? trackedObj.origin : trackedObj.transform.parent;
                if (origin != null)
                {
                    rigidbody.velocity = origin.TransformVector(device.velocity);
                    rigidbody.angularVelocity = origin.TransformVector(device.angularVelocity);
                }
                else
                {
                    rigidbody.velocity = device.velocity;
                    rigidbody.angularVelocity = device.angularVelocity;
                }

                rigidbody.maxAngularVelocity = rigidbody.angularVelocity.magnitude;
            }
            // any additional notes are immediately destroyed
            else
            {
                Object.DestroyImmediate(go);
                var audio = GetComponent<AudioSource>();
                audio.Stop();
            }
			// Object.Destroy(go, 15.0f);

		}

	}
}
