using UnityEngine;
using System.Collections;

public class Bounds_Triangle_Overlap_Test : MonoBehaviour 
{
	public Bounds 		mBounds			= new Bounds(Vector3.zero, Vector3.one);
	public Vector3[]	mTriangle		= new Vector3[3];
	public GameObject[]	mTriangleNode	= new GameObject[3];
	
	public Color		mBoundsNormalColor 	= Color.black;
	public Color		mBoundsOverlapColor	= Color.white;
	// Use this for initialization
	void Start () 
	{
		mTriangleNode[0] = new GameObject("v0");
		mTriangleNode[1] = new GameObject("v1");
		mTriangleNode[2] = new GameObject("v2");
		
		for (int i = 0; i != 3; ++i)
		{
			mTriangleNode[i].transform.position = mTriangle[i];
		}
	}
	
	// Update is called once per frame
	void Update () 
	{
		for (int i = 0; i != 3; ++i)
		{
			mTriangle[i] = mTriangleNode[i].transform.position;
		}
	}

	void OnDrawGizmos()
	{
		//Draw Triangle
		Gizmos.color = Color.green;
		Gizmos.DrawLine(mTriangle[0], mTriangle[1]);
		Gizmos.DrawLine(mTriangle[1], mTriangle[2]);
		Gizmos.DrawLine(mTriangle[2], mTriangle[0]);
		
		Gizmos.DrawSphere(mTriangle[0], 0.1f);
		Gizmos.DrawSphere(mTriangle[1], 0.1f);
		Gizmos.DrawSphere(mTriangle[2], 0.1f);
		
		if (Utility.Bounds_Triangle_Overlap(ref mBounds, ref mTriangle[0], ref mTriangle[1], ref mTriangle[2]))
			Gizmos.color = mBoundsOverlapColor;
		else
			Gizmos.color = mBoundsNormalColor;
		
		Gizmos.DrawCube(mBounds.center, mBounds.size);
	}
}
