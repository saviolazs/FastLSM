using UnityEngine;
using System.Collections;

public class Testbed2 : MonoBehaviour {

	// Use this for initialization
	void Start () 
	{
	
	}
	
	// Update is called once per frame
	void Update () 
	{
	}
	
	void OnGUI()
    {
		if (GUI.Button(new Rect(10, 10, 150, 100), "Add Force"))
        	AddForce();
		
		if (GUI.Button(new Rect(10, 110, 150, 100), "Add Force2"))
        	AddForce2();
	}
	
	void AddForce()
	{
		string[] smBodyName = new string[]{"Penguin", "rubberduck", "Soccerball", "plane_lower"};
		foreach (string strName in smBodyName)
		{
			GameObject goBody = GameObject.Find(strName);
			if (null == goBody)
				continue;
			
			SmBody sb = goBody.GetComponent<SmBody>();
			if (null == sb)
				continue;
			
			float velocityMax = 25.0f;
	        float velocityHalf = velocityMax / 2.0f;
			
	        Vector3 randomVelocity = new Vector3(0,
	            Random.Range(0.5f, 1.0f) * velocityMax - velocityHalf,
	            0);

			for (int i = 0; i != sb.mDeformableModel.mParticles.Count; ++i)
	        {
	            SmParticle particle = sb.mDeformableModel.mParticles[i];
	            //particle.mX.y += 5.0f;
	            particle.mV += (randomVelocity);
	        }
		}
	}
	
	void AddForce2()
	{
		string[] smBodyName = new string[]{"Penguin", "rubberduck", "Soccerball", "plane_lower"};
		foreach (string strName in smBodyName)
		{
			GameObject goBody = GameObject.Find(strName);
			if (null == goBody)
				continue;
			
			SmBody sb = goBody.GetComponent<SmBody>();
			if (null == sb)
				continue;
			
			float velocityMax = 500.0f;
	        float velocityHalf = velocityMax / 2.0f;
			
			/*
	        Vector3 randomVelocity = new Vector3(0,
	            Random.Range(0.5f, 1.0f) * velocityMax - velocityHalf,
	            0);
	            */
				
			Vector3 randomVelocity = new Vector3(Random.Range(0.5f, 1.0f) * velocityMax - velocityHalf,
	            Random.Range(0.5f, 1.0f) * velocityMax - velocityHalf,
	            Random.Range(0.5f, 1.0f) * velocityMax - velocityHalf);
			
			/*
	        for (int i = 0; i != sb.mDeformableModel.mParticles.Count; ++i)
	        {
	            SmParticle particle = sb.mDeformableModel.mParticles[i];
	            //particle.mX.y += 5.0f;
	            particle.mV += (randomVelocity);
	        }
	        */
			while (true)
			{
				int index = Random.Range(0, sb.mDeformableModel.mLatticeLocations.Count);
				LatticeLocation ll = sb.mDeformableModel.mLatticeLocations[index];
				if (ll.IsEdge)
				{
					for (int i = 0; i != ll.mParticles.Length; ++i)
					{
						SmParticle particle = ll.mParticles[i];
						particle.mF = randomVelocity;
					}
					break;
				}
			}
		}
	}
}
