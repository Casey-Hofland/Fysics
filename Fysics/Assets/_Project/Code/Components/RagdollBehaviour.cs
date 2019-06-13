using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RagdollBehaviour : MonoBehaviour
{
	//private Rigidbody controller;
	private Animator animator;
	private Collider collider;
	private Rigidbody[] bones;

    // Start is called before the first frame update
    private void Start()
    {
		//controller = GetComponent<Rigidbody>();
		animator = GetComponent<Animator>();
		collider = GetComponent<Collider>();
		bones = transform.GetChild(2).GetComponentsInChildren<Rigidbody>(true);
		RagdollSetActive(false);

    }

	public void RagdollSetActive(bool value)
	{
		animator.enabled = !value;
		collider.enabled = !value;

		/*
		foreach (Rigidbody bone in bones)
		{
			bone.useGravity = value;
			bone.isKinematic = !value;
		}

		if (!value)
		{
			Debug.Log("Woesh");
		}
		*/
	}

	private void Update()
	{
#if UNITY_EDITOR
		// Used for Testing
		if (Input.GetKeyDown(KeyCode.R))
		{
			RagdollSetActive(true);
		}
		if (Input.GetKeyUp(KeyCode.T))
		{
			RagdollSetActive(false);
		}
#endif
	}
}
