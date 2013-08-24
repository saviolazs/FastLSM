using UnityEngine;
using System.Collections;

public class TI_Test : MonoBehaviour 
{
	public GameObject[] mVertices = new GameObject[8];
	public MeshFilter	mMeshFilter = null;
	Vector3[] mMeshVertices = null;
	// Use this for initialization
	void Start () 
	{
		mMeshFilter = GetComponent<MeshFilter>();
		if (null == mMeshFilter)
			return;
		
		Debug.Log(mMeshFilter.mesh.bounds.min.ToString());
		Debug.Log(mMeshFilter.mesh.bounds.max.ToString());
		
		mMeshVertices = new Vector3[mMeshFilter.mesh.vertices.Length];
		for (int i = 0; i != mMeshFilter.mesh.vertices.Length; ++i)
		{
			mMeshVertices[i] = mMeshFilter.mesh.vertices[i];
			mMeshVertices[i] += (new Vector3(0.5f, 0.5f, 0.5f));
		}
		
		for (int i = 0; i != 8; ++i)
		{
			mVertices[i] = new GameObject(i.ToString());
			mVertices[i].transform.parent = transform;
			mVertices[i].transform.localPosition = Vector3.zero;
		}
		
		mVertices[0].name = "V000";
		mVertices[0].transform.localPosition = mMeshFilter.mesh.bounds.min;
		
		mVertices[1].name = "V100";
		mVertices[1].transform.localPosition = new Vector3(mMeshFilter.mesh.bounds.max.x, mMeshFilter.mesh.bounds.min.y, mMeshFilter.mesh.bounds.min.z);
		
		mVertices[2].name = "V010";
		mVertices[2].transform.localPosition = new Vector3(mMeshFilter.mesh.bounds.min.x, mMeshFilter.mesh.bounds.max.y, mMeshFilter.mesh.bounds.min.z);
		
		mVertices[3].name = "V001";
		mVertices[3].transform.localPosition = new Vector3(mMeshFilter.mesh.bounds.min.x, mMeshFilter.mesh.bounds.min.y, mMeshFilter.mesh.bounds.max.z);
		
		mVertices[4].name = "V101";
		mVertices[4].transform.localPosition = new Vector3(mMeshFilter.mesh.bounds.max.x, mMeshFilter.mesh.bounds.min.y, mMeshFilter.mesh.bounds.max.z);
		
		mVertices[5].name = "V011";
		mVertices[5].transform.localPosition = new Vector3(mMeshFilter.mesh.bounds.min.x, mMeshFilter.mesh.bounds.max.y, mMeshFilter.mesh.bounds.max.z);
		
		mVertices[6].name = "V110";
		mVertices[6].transform.localPosition = new Vector3(mMeshFilter.mesh.bounds.max.x, mMeshFilter.mesh.bounds.max.y, mMeshFilter.mesh.bounds.min.z);
		
		mVertices[7].name = "V111";
		mVertices[7].transform.localPosition = mMeshFilter.mesh.bounds.max;
	}
	
	// Update is called once per frame
	void Update () 
	{
		if (null == mMeshFilter)
			return;
		
		Vector3[] vertices = mMeshFilter.mesh.vertices;
		
		/*
		 Vxyz =	V000 (1 - x) (1 - y) (1 - z) +
				V100 x (1 - y) (1 - z) + 
				V010 (1 - x) y (1 - z) + 
				V001 (1 - x) (1 - y) z +
				V101 x (1 - y) z + 
				V011 (1 - x) y z + 
				V110 x y (1 - z) + 
				V111 x y z
		*/
		
        for (int i = 0; i != vertices.Length; ++i)
		{
			vertices[i] = 	mVertices[0].transform.localPosition * (1.0f - mMeshVertices[i].x) * (1.0f - mMeshVertices[i].y) * (1.0f - mMeshVertices[i].z) +
						 	mVertices[1].transform.localPosition * (mMeshVertices[i].x) * (1.0f - mMeshVertices[i].y) * (1.0f - mMeshVertices[i].z) +
						  	mVertices[2].transform.localPosition * (1.0f - mMeshVertices[i].x) * (mMeshVertices[i].y) * (1.0f - mMeshVertices[i].z) +
							mVertices[3].transform.localPosition * (1.0f - mMeshVertices[i].x) * (1.0f - mMeshVertices[i].y) * (mMeshVertices[i].z) +
							mVertices[4].transform.localPosition * (mMeshVertices[i].x) * (1.0f - mMeshVertices[i].y) * (mMeshVertices[i].z) +
							mVertices[5].transform.localPosition * (1.0f - mMeshVertices[i].x) * (mMeshVertices[i].y) * (mMeshVertices[i].z) +
							mVertices[6].transform.localPosition * (mMeshVertices[i].x) * (mMeshVertices[i].y) * (1.0f - mMeshVertices[i].z) +
							mVertices[7].transform.localPosition * (mMeshVertices[i].x) * (mMeshVertices[i].y) * (mMeshVertices[i].z);
							
		}
		
        mMeshFilter.mesh.vertices = vertices;
		//mMeshFilter.mesh.RecalculateNormals();
	}
	
	void OnDrawGizmos ()
	{
		if (null == mMeshFilter)
			return;
		
		Gizmos.color = Color.yellow;
		
		for (int i = 0; i != 8; ++i)
		{
			if (null != mVertices[i])
			{
        		Gizmos.DrawSphere (mVertices[i].transform.position, 0.1f);
			}
		}
		
		Gizmos.color = Color.green;
		
		Gizmos.DrawLine(mVertices[0].transform.position, mVertices[1].transform.position);
		Gizmos.DrawLine(mVertices[1].transform.position, mVertices[6].transform.position);
		Gizmos.DrawLine(mVertices[6].transform.position, mVertices[2].transform.position);
		Gizmos.DrawLine(mVertices[2].transform.position, mVertices[0].transform.position);
		
		Gizmos.DrawLine(mVertices[3].transform.position, mVertices[4].transform.position);
		Gizmos.DrawLine(mVertices[4].transform.position, mVertices[7].transform.position);
		Gizmos.DrawLine(mVertices[7].transform.position, mVertices[5].transform.position);
		Gizmos.DrawLine(mVertices[5].transform.position, mVertices[3].transform.position);
		
		Gizmos.DrawLine(mVertices[0].transform.position, mVertices[3].transform.position);
		Gizmos.DrawLine(mVertices[1].transform.position, mVertices[4].transform.position);
		Gizmos.DrawLine(mVertices[6].transform.position, mVertices[7].transform.position);
		Gizmos.DrawLine(mVertices[2].transform.position, mVertices[5].transform.position);
	}
	
	void OnDestroy()
	{
		for (int i = 0; i != 8; ++i)
		{
			if (null != mVertices[i])
			{
				GameObject.Destroy(mVertices[i]);
				mVertices[i] = null;
			}
		}
		
		mVertices = null;
	}
}
