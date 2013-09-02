using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SmBody : MonoBehaviour 
{
	//----------------------------------------------------------------------
	
	static float    h = 1.0f / 30.0f;
	static Vector3  vGravity = new Vector3(0.0f, -9.8f, 0.0f);
	
	public struct VertexPosition
	{
		public int 		mMeshIndex;
		public int 		mVertexIndex;
		public Vector3	mOriginPosition;
	}
	
	public class SmCellInfo
	{
		public bool					mValid		= false;
		public Bounds				mBounds;
		public List<VertexPosition>	mVertices	= new List<VertexPosition>();
		
	}
	
	public float Spacing
	{
		get
		{
			return mSpacing.x;
		}
		set
		{
			if (mSpacing.x != value)
			{
				if (value <= 0.0f)
				{
					Debug.LogError("Spceing must greater then 0.");
					throw new System.Exception("Spceing must greater then 0.");
				}
				
				mSpacing.x = value;
				mSpacing.y = value;
				mSpacing.z = value;
			}
		}
	}
	
	public int				W = 1;
	public Vector3			mSpacing 			= new Vector3(0.3f, 0.3f, 0.3f);
	protected Bounds		mBounds				= new Bounds(Vector3.zero, Vector3.zero);
	public SmCellInfo[,,]	mCellInfos			= null;
	
	public DeformableModel	mDeformableModel	= null;
	
	MeshFilter[] 	mMeshFilters 	= null;
	Transform[]		mMeshTransforms = null;
	Vector3[][]		mMeshVertices	= null;
	int[][]			mMeshTriangles	= null;
	
	Color mParticleColor 	= Color.red;
	Color mCellColor		= new Color(0.8f, 0.8f, 0.8f, 0.8f);
	
	void Clear()
	{
		if (null != mMeshFilters)
		{
			System.Array.Clear(mMeshFilters, 0, mMeshFilters.Length);
			mMeshFilters = null;
		}
		
		if (null != mMeshTransforms)
		{
			System.Array.Clear(mMeshTransforms, 0, mMeshTransforms.Length);
			mMeshTransforms = null;
		}
		
		if (null != mCellInfos)
		{
			int[] dimensions = new int[3]{mCellInfos.GetLength(0), mCellInfos.GetLength(1), mCellInfos.GetLength(2)};
			
			for (int x = 0; x != dimensions[0]; ++x)
			{
				for (int y = 0; y != dimensions[1]; ++y)
				{
					for (int z = 0; z != dimensions[2]; ++z)
					{
						SmCellInfo info = mCellInfos[x,y,z];
						info.mValid = false;
						info.mVertices.Clear();
						info.mVertices.TrimExcess();
						info = null;
					}
				}
			}
			System.Array.Clear(mCellInfos, 0, dimensions[0] * dimensions[1] * dimensions[2]);
			mCellInfos = null;
		}
		
		mBounds.SetMinMax(Vector3.zero, Vector3.zero);
	}
	
	void ValidateCells()
	{
		if (null == mCellInfos)
			return;
		
		int[] dimensions = new int[3]{mCellInfos.GetLength(0), mCellInfos.GetLength(1), mCellInfos.GetLength(2)};
		
		//if (dimensions[0] > 2 && dimensions[1] > 2 && dimensions[2] > 2)
		{
			for (int x = 0; x != dimensions[0]; ++x)
			{
				for (int y = 0; y != dimensions[1]; ++y)
				{
					for (int z = 0; z != dimensions[2]; ++z)
					{
						SmCellInfo info = mCellInfos[x,y,z];
						if (info.mValid)
							continue;
						
						if (IsValidCell(x, y, z))
							info.mValid = true;
					}
				}
			}
		}
	}
	
	bool IsValidCell(int x, int y, int z)
	{
		SmCellInfo info = mCellInfos[x,y,z];
		
		for (int i = 0; i != mMeshTriangles.Length; ++i)
		{
			for (int j = 0; j < mMeshTriangles[i].Length; j += 3)
			{
				Vector3 v0 = transform.TransformPoint(mMeshTransforms[i].InverseTransformPoint(mMeshVertices[i][mMeshTriangles[i][j]]));
				Vector3 v1 = transform.TransformPoint(mMeshTransforms[i].InverseTransformPoint(mMeshVertices[i][mMeshTriangles[i][j+1]]));
				Vector3 v2 = transform.TransformPoint(mMeshTransforms[i].InverseTransformPoint(mMeshVertices[i][mMeshTriangles[i][j+2]]));
				
				if (Utility.Bounds_Triangle_Overlap(ref info.mBounds, ref v0, ref v1, ref v2))
				{
					return true;
				}
			}
		}

		return false;
	}
	
	bool InitCells()
	{
		//mSpacing = new Vector3(0.3f, 0.3f, 0.3f);
		//mSpacing = new Vector3(2, 2, 2);
		//----------------------------------------------------------------
		//Clear
		Clear();
		
		//----------------------------------------------------------------
		//Calculate
		mMeshFilters = GetComponentsInChildren<MeshFilter>();
		if (mMeshFilters.Length == 0)
			return false;
		
		mMeshTransforms = new Transform[mMeshFilters.Length];
		
		mMeshVertices = new Vector3[mMeshFilters.Length][];
		
		mMeshTriangles = new int[mMeshFilters.Length][];
		
		for (int i = 0; i != mMeshFilters.Length; ++i)
		{
			MeshFilter mf = mMeshFilters[i];
			mMeshTransforms[i] 	= mf.transform;
			mMeshVertices[i]	= mf.mesh.vertices;
			mMeshTriangles[i]	= mf.mesh.triangles;
			
			if (0 == i)
			{
				mBounds.SetMinMax(
					transform.TransformPoint(mMeshTransforms[i].InverseTransformPoint(mf.mesh.bounds.min)), 
					transform.TransformPoint(mMeshTransforms[i].InverseTransformPoint(mf.mesh.bounds.max))
					);
			}
			else
			{
				Bounds tempBounds = new Bounds(Vector3.zero, Vector3.zero);
				tempBounds.SetMinMax(
					transform.TransformPoint(mMeshTransforms[i].InverseTransformPoint(mf.mesh.bounds.min)), 
					transform.TransformPoint(mMeshTransforms[i].InverseTransformPoint(mf.mesh.bounds.max))
					);
				mBounds.Encapsulate(tempBounds);
			}
		}
		
		//calculate cell groups
		int width 	= Mathf.CeilToInt(mBounds.size.x / Spacing);
		int height 	= Mathf.CeilToInt(mBounds.size.y / Spacing);
		int depth 	= Mathf.CeilToInt(mBounds.size.z / Spacing);
		
		mBounds.size = new Vector3(width * Spacing, height * Spacing, depth * Spacing);
		
		mCellInfos = new SmCellInfo[width, height, depth];

		for (int x = 0; x != width; ++x)
		{
			for (int y = 0; y != height; ++y)
			{
				for (int z = 0; z != depth; ++z)
				{
					mCellInfos[x,y,z] = new SmCellInfo();
					mCellInfos[x,y,z].mValid = false;
					mCellInfos[x,y,z].mBounds.center = mBounds.min 		+ 
						Vector3.Scale(mSpacing, new Vector3(x, y, z))	+
						new Vector3(Spacing * 0.5f, Spacing * 0.5f, Spacing * 0.5f);
					mCellInfos[x,y,z].mBounds.size	= mSpacing;
					mCellInfos[x,y,z].mVertices.Clear();
				}
			}
		}
		
		//divide vertices into cell groups
		Vector3 vLocal 		= Vector3.zero;
		Vector3 vOrigin		= Vector3.zero;
		Vector3 vInBounds	= Vector3.zero;
		
		for (int i = 0; i != mMeshVertices.Length; ++i)
		{	
			Vector3[] vertices = mMeshVertices[i];
			for (int j = 0; j != vertices.Length; ++j)
			{
				vOrigin = vertices[j];
				vLocal = transform.TransformPoint(mMeshTransforms[i].InverseTransformPoint(vOrigin));
				vInBounds = vLocal - mBounds.min;
				
				int x = Mathf.FloorToInt(vInBounds.x / Spacing);
				int y = Mathf.FloorToInt(vInBounds.y / Spacing);
				int z = Mathf.FloorToInt(vInBounds.z / Spacing);
				
				if (x < 0)	
					x = 0;
				if (x > mCellInfos.GetLength(0) - 1)	
					x = mCellInfos.GetLength(0) - 1;
				
				if (y < 0)	
					y = 0;
				if (y > mCellInfos.GetLength(1) - 1)	
					y = mCellInfos.GetLength(1) - 1;
				
				if (z < 0)	
					z = 0;
				if (z > mCellInfos.GetLength(2) - 1)	
					z = mCellInfos.GetLength(2) - 1;
				
				SmCellInfo info = mCellInfos[x,y,z];
				info.mValid = true;
				
				VertexPosition vp = new VertexPosition();
				vp.mMeshIndex = i;
				vp.mVertexIndex = j;
				vp.mOriginPosition = (vLocal - info.mBounds.min) / Spacing;
				
				info.mVertices.Add(vp);
			}
		}
		
		return true;
	}
	// Use this for initialization
	void Start () 
	{
		InitCells();
		
		ValidateCells();
		
		if (null == mDeformableModel)
		{
			mDeformableModel = new DeformableModel(mSpacing);
			mDeformableModel.W = W;//Random.Range(1, 6);
			mDeformableModel.Alpha = 0.9f;
			mDeformableModel.RegionDamping = 0.2f;
			
			int width   = mCellInfos.GetLength(0);
	        int height  = mCellInfos.GetLength(1);
	        int depth 	= mCellInfos.GetLength(2);
	
	        for (int x = 0; x != width; ++x)
	        {
	            for (int y = 0; y != height; ++y)
	            {
	                for (int z = 0; z != depth; ++z)
	                {
						SmCellInfo info = mCellInfos[x,y,z];
						if (info.mValid)
						{
							mDeformableModel.AddCell(new Point3(x, y, z));
						}
	                }
	            }
	        }
	
	        mDeformableModel.Complete();
	
			/*
	        float velocityMax = 25.0f;
	        float velocityHalf = velocityMax / 2.0f;
			
			
	        Vector3 randomVelocity = new Vector3(0,
	            Random.Range(0.5f, 1.0f) * velocityMax - velocityHalf,
	            0);
				
	        for (int i = 0; i != mDeformableModel.mParticles.Count; ++i)
	        {
	            SmParticle particle = mDeformableModel.mParticles[i];
	            particle.mX.y += 5.0f;
	            particle.mV += (randomVelocity);
	        }
	    
	
			Debug.LogWarning("W[" + mDeformableModel.W.ToString() + "], Width[" + width.ToString() + "], Height[" + height.ToString() + "], Depth[" + depth.ToString() + "], Velocity[" + randomVelocity.ToString() + "]");
			*/
			
		}
	}
	
	// Update is called once per frame
	void Update () 
	{
		if (null != mDeformableModel)
		{
			mDeformableModel.ShapeMatch();
            mDeformableModel.CalculateParticleVelocities(h);
            mDeformableModel.PerformRegionDamping();
            mDeformableModel.ApplyParticleVelocities(h);
			
			
            for (int j = 0; j != mDeformableModel.mParticles.Count; ++j)
            {
                SmParticle particle = mDeformableModel.mParticles[j];
                particle.mF += vGravity;

                if (particle.mX.y < 0.0f)
                {
                    particle.mF.y -= particle.mX.y;
                    particle.mV = Vector3.zero;
                    particle.mX.y = 0.0f;
                }
            }
            
		}
		
		//deform mesh
		for (int x = 0; x != mCellInfos.GetLength(0); ++x)
		{
			for (int y = 0; y != mCellInfos.GetLength(1); ++y)
			{
				for (int z = 0; z != mCellInfos.GetLength(2); ++z)
				{
					SmCellInfo info = mCellInfos[x,y,z];
					if (info.mValid)
					{
						LatticeLocation ll = mDeformableModel.GetLatticeLocation(new Point3(x, y, z));
						if (null == ll)
							continue;
						
						for (int w = 0; w != info.mVertices.Count; ++w)
						{
							VertexPosition vp = info.mVertices[w];
							
							mMeshVertices[vp.mMeshIndex][vp.mVertexIndex] = 
								ll.mParticles[0].mX * (1.0f - vp.mOriginPosition.x) * (1.0f - vp.mOriginPosition.y) * (1.0f - vp.mOriginPosition.z) +
							 	ll.mParticles[1].mX * (vp.mOriginPosition.x) * (1.0f - vp.mOriginPosition.y) * (1.0f - vp.mOriginPosition.z) +
							  	ll.mParticles[3].mX * (1.0f - vp.mOriginPosition.x) * (1.0f - vp.mOriginPosition.y) * (vp.mOriginPosition.z) +
								ll.mParticles[2].mX * (1.0f - vp.mOriginPosition.x) * (vp.mOriginPosition.y) * (1.0f - vp.mOriginPosition.z) +
								ll.mParticles[6].mX * (vp.mOriginPosition.x) * (vp.mOriginPosition.y) * (1.0f - vp.mOriginPosition.z) +
								ll.mParticles[5].mX * (1.0f - vp.mOriginPosition.x) * (vp.mOriginPosition.y) * (vp.mOriginPosition.z) +
								ll.mParticles[4].mX * (vp.mOriginPosition.x) * (1.0f - vp.mOriginPosition.y) * (vp.mOriginPosition.z) +
								ll.mParticles[7].mX * (vp.mOriginPosition.x) * (vp.mOriginPosition.y) * (vp.mOriginPosition.z);
						}
					}
				}
			}
		}
		
		Vector3 vLocal = Vector3.zero;
		Vector3 vOrigin = Vector3.zero;
		
		for (int i = 0; i != mMeshFilters.Length; ++i)
		{
			MeshFilter mf = mMeshFilters[i];
			
			
			for (int j = 0; j != mMeshVertices[i].Length; ++j)
			{
				vLocal = mMeshVertices[i][j];
				vOrigin = mMeshTransforms[i].InverseTransformPoint(transform.TransformPoint(vLocal));
				mMeshVertices[i][j]	= vOrigin;
			}
			
			mf.mesh.vertices = mMeshVertices[i];
			mf.mesh.RecalculateNormals();
			mf.mesh.RecalculateBounds();
		}
	}
	
	/*
	void OnDrawGizmos()
	{
	}
	*/
	
	void OnDrawGizmosSelected()
	{
		/*
		if (null == mMeshFilters)
			return;
		
		Gizmos.color = Color.blue;
		Gizmos.DrawWireCube(mBounds.center, mBounds.size);
		
		Gizmos.color = Color.green;
		for (int i = 0; i != mMeshFilters.Length; ++i)
		{
			MeshFilter mf = mMeshFilters[i];
			Gizmos.DrawWireCube(mf.mesh.bounds.center, mf.mesh.bounds.size);
		}
		
		if (null == mCellInfos)
			return;
			*/
		
		/*
		int[] dimensions = new int[3]{mCellInfos.GetLength(0), mCellInfos.GetLength(1), mCellInfos.GetLength(2)};
		for (int x = 0; x != dimensions[0]; ++x)
		{
			for (int y = 0; y != dimensions[1]; ++y)
			{
				for (int z = 0; z != dimensions[2]; ++z)
				{
					SmCellInfo info = mCellInfos[x,y,z];
					if (info.mValid)
					{
						Gizmos.color = new Color(0, 0, 0, 0.1f);
						Gizmos.DrawCube(info.mBounds.center, info.mBounds.size);
						
						Gizmos.color = Color.black;
						Gizmos.DrawWireCube(info.mBounds.center, info.mBounds.size);
						
						Gizmos.color = Color.red;
						Gizmos.DrawSphere(info.mBounds.center, 0.05f);
						
						for (int w = 0; w != mDeformableModel.mParticles.Count; ++w)
						{
							Gizmos.DrawSphere(mDeformableModel.mParticles[w].mX, 0.03f);
						}
						

						LatticeLocation ll = mDeformableModel.GetLatticeLocation(new Point3(x,y,z));
						Gizmos.color = Color.gray;
						for (int w = 0; w != info.mVertices.Count; ++w)
						{
							VertexPosition vp = info.mVertices[w];
							Vector3 vPos = 
								ll.mParticles[0].mX0 * (1.0f - vp.mOriginPosition.x) * (1.0f - vp.mOriginPosition.y) * (1.0f - vp.mOriginPosition.z) +
							 	ll.mParticles[1].mX0 * (vp.mOriginPosition.x) * (1.0f - vp.mOriginPosition.y) * (1.0f - vp.mOriginPosition.z) +
							  	ll.mParticles[3].mX0 * (1.0f - vp.mOriginPosition.x) * (vp.mOriginPosition.y) * (1.0f - vp.mOriginPosition.z) +
								ll.mParticles[2].mX0 * (1.0f - vp.mOriginPosition.x) * (1.0f - vp.mOriginPosition.y) * (vp.mOriginPosition.z) +
								ll.mParticles[6].mX0 * (vp.mOriginPosition.x) * (1.0f - vp.mOriginPosition.y) * (vp.mOriginPosition.z) +
								ll.mParticles[5].mX0 * (1.0f - vp.mOriginPosition.x) * (vp.mOriginPosition.y) * (vp.mOriginPosition.z) +
								ll.mParticles[4].mX0 * (vp.mOriginPosition.x) * (vp.mOriginPosition.y) * (1.0f - vp.mOriginPosition.z) +
								ll.mParticles[7].mX0 * (vp.mOriginPosition.x) * (vp.mOriginPosition.y) * (vp.mOriginPosition.z);
							
							Gizmos.DrawSphere(vPos, 0.01f);
						}
					}
				}
			}
		}
		*/

		if (null != mDeformableModel)
		{
			for (int j = 0; j != mDeformableModel.mParticles.Count; ++j)
			{
				SmParticle p = mDeformableModel.mParticles[j];
				Gizmos.color = Color.red;
				Gizmos.DrawSphere(transform.TransformPoint(p.mX), 0.03f);
			}
			
			for (int j = 0; j != mDeformableModel.mRegions.Count; ++j)
			{
				SmRegion r = mDeformableModel.mRegions[j];
				
				Bounds bounds = new Bounds(transform.TransformPoint(r.mParticles[0].mX), Vector3.zero);
				for (int k = 0; k != r.mParticles.Count; ++k)
				{
					bounds.Encapsulate (transform.TransformPoint(r.mParticles[k].mX));
				}
				
				Gizmos.color = new Color(r.color.r, r.color.g, r.color.b, 0.05f);
				Gizmos.DrawCube(bounds.center, bounds.size);
				
				Gizmos.color = r.color;
				Gizmos.DrawWireCube(bounds.center, bounds.size);
			}
		}
	}
}
