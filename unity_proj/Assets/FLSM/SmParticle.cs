using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SmParticle : Summation
{
	public static uint mParticleIndex = 0;
	
    public uint CreateTime //CellVertex.cpp Line29 <
    { get; set; }

    public float Mass
    { get; set; }

    public Vector3 mX0 = Vector3.zero;

    public float PerRegionMass
    { get; set; }

    public Vector3 mX = Vector3.zero;

    public Vector3 mV = Vector3.zero;

    public Vector3 mF = Vector3.zero;

    public Vector3 mG = Vector3.zero;

    public Matrix3x3 mR = Matrix3x3.zero;

    public SmParticle NextParticleInCollisionCell
    { get; set; }

    public List<LatticeLocation> mParentRegions = new List<LatticeLocation>();
	
	public SmParticle()
	{
		CreateTime = mParticleIndex++;
	}

    public static int CompareSmParticle(SmParticle x, SmParticle y)
    {
        if (x == null)
        {
            if (y == null)
            {
                return 0;
            }
            else
            {
                return -1;
            }
        }
        else
        {
            if (y == null)
            {
                return 1;
            }
            else
            {
                return x.CreateTime.CompareTo(y.CreateTime);
            }
        }
    }
}


 

