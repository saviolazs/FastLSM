using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Testbed : MonoBehaviour 
{
    static float    h = 1.0f / 30.0f;
    static Vector3  vGravity = new Vector3(0.0f, -9.8f, 0.0f);
    static Material mMaterial = null;
	static bool 	mEnableRender = true;
	
	// Use this for initialization
	void Start () 
    {
        if (null == mMaterial)
        {
            mMaterial = new Material("Shader \"Lines/Colored Blended\" {" +
               "SubShader { Pass { " +
               "    Blend SrcAlpha OneMinusSrcAlpha " +
               "    ZWrite On Cull Off Fog { Mode Off } " +
               "    BindChannels {" +
               "      Bind \"vertex\", vertex Bind \"color\", color }" +
               "} } }");
            mMaterial.hideFlags = HideFlags.HideAndDontSave;
            mMaterial.shader.hideFlags = HideFlags.HideAndDontSave;
        }
		
		Camera.main.transform.position = new Vector3(0.0f, 20.0f, 40.0f);
		Camera.main.transform.LookAt (Vector3.zero);
	}
	
	// Update is called once per frame
	void Update () 
    {
        for (int i = 0; i != mBodies.Count; ++i)
        {
            DeformableModel body = mBodies[i];

            body.ShapeMatch();
            body.CalculateParticleVelocities(h);
            body.PerformRegionDamping();
            body.ApplyParticleVelocities(h);

            for (int j = 0; j != body.mParticles.Count; ++j)
            {
                SmParticle particle = body.mParticles[j];
                particle.mF += vGravity;

                if (particle.mX.y < 0.0f)
                {
                    particle.mF.y -= particle.mX.y;
                    particle.mV = Vector3.zero;
                    particle.mX.y = 0.0f;
                }
            }
        }
	}
	
	void GLVertex3(ref Vector3 v)
	{
		GL.Vertex3(v.x, v.y, v.z);
	}
	
    void OnPostRender()
    {
		if (false == mEnableRender)
			return;
		
        if (null == mMaterial)
            return;

        GL.PushMatrix();
        mMaterial.SetPass(0);
        //GL.LoadProjectionMatrix();
        GL.Begin(GL.QUADS);
        GL.Color(Color.white);
        GL.Vertex3(-9999, -100, -9999);
        GL.Vertex3(-9999, -100, 9999);
        GL.Vertex3(9999, -100, 9999);
        GL.Vertex3(9999, -100, -9999);
        GL.End();
		
		GL.Begin(GL.QUADS);
		GL.Color(Color.blue);
		for (int i = 0; i != mBodies.Count; ++i)
		{
			DeformableModel body = mBodies[i];
			body.UpdateCellPositions();
			
			for (int j = 0; j != body.mCells.Count; ++j)
			{
				SmCell c = body.mCells[j];
				if (c.Center.IsEdge)
				{
					// Bottom face
					GLVertex3(ref c.mVertices[0].mPosition);
					GLVertex3(ref c.mVertices[4].mPosition);
					GLVertex3(ref c.mVertices[5].mPosition);
					GLVertex3(ref c.mVertices[1].mPosition);
	
					// Top face
					GLVertex3(ref c.mVertices[2].mPosition);
					GLVertex3(ref c.mVertices[3].mPosition);
					GLVertex3(ref c.mVertices[7].mPosition);
					GLVertex3(ref c.mVertices[6].mPosition);
	
					// Far face
					GLVertex3(ref c.mVertices[0].mPosition);
					GLVertex3(ref c.mVertices[2].mPosition);
					GLVertex3(ref c.mVertices[6].mPosition);
					GLVertex3(ref c.mVertices[4].mPosition);
					
					// Right face
					GLVertex3(ref c.mVertices[4].mPosition); 
					GLVertex3(ref c.mVertices[6].mPosition);
					GLVertex3(ref c.mVertices[7].mPosition);
					GLVertex3(ref c.mVertices[5].mPosition);
	
	
					// Front face
					GLVertex3(ref c.mVertices[1].mPosition);
					GLVertex3(ref c.mVertices[5].mPosition);
					GLVertex3(ref c.mVertices[7].mPosition);
					GLVertex3(ref c.mVertices[3].mPosition);
	
					// Left Face
					GLVertex3(ref c.mVertices[0].mPosition);
					GLVertex3(ref c.mVertices[1].mPosition);
					GLVertex3(ref c.mVertices[3].mPosition);
					GLVertex3(ref c.mVertices[2].mPosition);
				}
			}
		}
		GL.End();
		
		GL.Begin(GL.LINES);
		GL.Color(Color.red);
		for (int i = 0; i != mBodies.Count; ++i)
		{
			DeformableModel body = mBodies[i];
			for (int j = 0; j != body.mCells.Count; ++j)
			{
				SmCell c = body.mCells[j];
				if (c.Center.IsEdge)
				{
					// Bottom face.
					GLVertex3(ref c.mVertices[0].mPosition);
					GLVertex3(ref c.mVertices[4].mPosition);
					GLVertex3(ref c.mVertices[4].mPosition);
					GLVertex3(ref c.mVertices[5].mPosition);
					GLVertex3(ref c.mVertices[5].mPosition);
					GLVertex3(ref c.mVertices[1].mPosition);
					GLVertex3(ref c.mVertices[1].mPosition);
					GLVertex3(ref c.mVertices[0].mPosition);
	
					// Top face.
					GLVertex3(ref c.mVertices[2].mPosition);
					GLVertex3(ref c.mVertices[3].mPosition);
					GLVertex3(ref c.mVertices[3].mPosition);
					GLVertex3(ref c.mVertices[7].mPosition);
					GLVertex3(ref c.mVertices[7].mPosition);
					GLVertex3(ref c.mVertices[6].mPosition);
					GLVertex3(ref c.mVertices[6].mPosition);
					GLVertex3(ref c.mVertices[2].mPosition);
	
					// Far face.
					GLVertex3(ref c.mVertices[0].mPosition);
					GLVertex3(ref c.mVertices[2].mPosition);
					GLVertex3(ref c.mVertices[2].mPosition);
					GLVertex3(ref c.mVertices[6].mPosition);
					GLVertex3(ref c.mVertices[6].mPosition);
					GLVertex3(ref c.mVertices[4].mPosition);
					GLVertex3(ref c.mVertices[4].mPosition);
					GLVertex3(ref c.mVertices[0].mPosition);
	
					// Right face.
					GLVertex3(ref c.mVertices[4].mPosition); 
					GLVertex3(ref c.mVertices[6].mPosition);
					GLVertex3(ref c.mVertices[6].mPosition);
					GLVertex3(ref c.mVertices[7].mPosition);
					GLVertex3(ref c.mVertices[7].mPosition);
					GLVertex3(ref c.mVertices[5].mPosition);
					GLVertex3(ref c.mVertices[5].mPosition);
					GLVertex3(ref c.mVertices[4].mPosition);
	
					// Front face.
					GLVertex3(ref c.mVertices[1].mPosition);
					GLVertex3(ref c.mVertices[5].mPosition);
					GLVertex3(ref c.mVertices[5].mPosition);
					GLVertex3(ref c.mVertices[7].mPosition);
					GLVertex3(ref c.mVertices[7].mPosition);
					GLVertex3(ref c.mVertices[3].mPosition);
					GLVertex3(ref c.mVertices[3].mPosition);
					GLVertex3(ref c.mVertices[1].mPosition);
	
					// Left Face.
					GLVertex3(ref c.mVertices[0].mPosition);
					GLVertex3(ref c.mVertices[1].mPosition);
					GLVertex3(ref c.mVertices[1].mPosition);
					GLVertex3(ref c.mVertices[3].mPosition);
					GLVertex3(ref c.mVertices[3].mPosition);
					GLVertex3(ref c.mVertices[2].mPosition);
					GLVertex3(ref c.mVertices[2].mPosition);
					GLVertex3(ref c.mVertices[0].mPosition);
				}
			}
		}
		GL.End();
		
        GL.PopMatrix();
    }

    void OnGUI()
    {
		 if (GUI .Button(new Rect(10, 10, 150, 100), "Add Body"))
            AddBody();
		
		if (mEnableRender)
		{
			if (GUI.Button(new Rect(10, 110, 150, 100), "Disable Render"))
            	mEnableRender = false;
		}
		else
		{
			if (GUI.Button(new Rect(10, 110, 150, 100), "Enable Render"))
            	mEnableRender = true;
		}
    }

    void OnDestroy()
    {
        if (null != mMaterial)
        {
            Object.Destroy(mMaterial);
        }
    }

    void AddBody()
    {
        DeformableModel body = new DeformableModel(Vector3.one);
        body.W = 1;
		//body.W = Random.Range(1, 8);
        body.Alpha = 0.75f;
        body.RegionDamping = 0.25f;
        mBodies.Add(body);
		
		
        int width   = Random.Range(0, 3) + 2;
        int height  = Random.Range(0, 10) + 2;
        int depth 	= Random.Range(0, 3) + 2;
        
		//int width   = 3;
        //int height  = 11;
        //int depth 	= 4;

        for (int x = 0; x != width; ++x)
        {
            for (int y = 0; y != height; ++y)
            {
                for (int z = 0; z != depth; ++z)
                {
                    body.AddParticle(new Point3(x, y, z));
                }
            }
        }

        body.Complete();

        float velocityMax = 25.0f;
        float velocityHalf = velocityMax / 2.0f;
		
		
        Vector3 randomVelocity = new Vector3(Random.Range(0.0f, 1.0f) * velocityMax - velocityHalf,
            Random.Range(0.0f, 1.0f) * velocityMax - velocityHalf,
            Random.Range(0.0f, 1.0f) * velocityMax - velocityHalf);
		
		//Vector3 randomVelocity = new Vector3(-6.4f, -8.4f, -1.74f);
        

        for (int i = 0; i != body.mParticles.Count; ++i)
        {
            SmParticle particle = body.mParticles[i];
            particle.mX.y += 5.0f;
            particle.mV += randomVelocity;
        }

		Debug.LogWarning("Add Body:W[" + body.W.ToString() + "], width[" + width.ToString() + "], height[" + height.ToString() + "], depth[" + depth.ToString() + "], randomVelocity[" + randomVelocity.ToString() + "]");
    }

    protected List<DeformableModel> mBodies = new List<DeformableModel>();
}
