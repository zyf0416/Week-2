using UnityEngine;
using System.Collections;

public class SwordSwing : MonoBehaviour 
{
	Animator a;

	void Start () 
	{
		a = GetComponent<Animator>();
		Screen.orientation = ScreenOrientation.LandscapeLeft;
	}

	void Update () 
	{
		// code to trigger Swing method on relative movement downards
	}

	void Swing()
	{
		a.Play ("Slash", -1, 0.0f);
	}
}
