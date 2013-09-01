using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LatticeLocation
{
	public static uint mLatticeLocationIndex = 0;
	
	public uint CreateTime
	{ get; set; }
	
	public DeformableModel Body
	{ get; set; }

    public Point3 mIndex = Point3.zero;
	
	//public SmCell Cell
	//{get;set;}
	
	public bool RegionExists
	{get;set;}
	
	public bool IsEdge
	{get;set;}
	
	//public SmParticle SmParticle
	//{get;set;}
	
	public SmRegion Region
	{get;set;}

    public uint TheTouch
    { get; set; }
	
	//Particles
	public SmParticle[] 			mParticles 				= new SmParticle[8];
	
	// The IMMEDIATE immediateNeighbors
	public List<LatticeLocation> 	mImmediateNeighbors 	= new List<LatticeLocation>();
	public LatticeLocation[,,]		mImmediateNeighborsGrid	= new LatticeLocation[3,3,3];
	// Generated
	public List<LatticeLocation> 	mNeighborhood 			= new List<LatticeLocation>();
    public List<Summation>[]        mSummations 			= new List<Summation>[2];
	
	
	public LatticeLocation()
	{
		mSummations[0] = new List<Summation>();
		mSummations[1] = new List<Summation>();
		
		CreateTime = mLatticeLocationIndex++;
	}
	
	public void CalculateNeighborhood()
	{
        mNeighborhood.Clear();
        mNeighborhood.TrimExcess();

        if (null == Body)
            return;

        if (Body.W == 1)
        {
            mNeighborhood = mImmediateNeighbors;
            mNeighborhood.Sort(LatticeLocation.CompareLatticeLocation);
        }
        else
        {
            uint newTouch = (uint)(Random.Range(0, int.MaxValue));
            Queue<LatticeLocation> next = new Queue<LatticeLocation>();
            int currentDepth = 0;
            int remainingAtThisDepth = 1;
            int elementsAtNextDepth = 0;

            next.Enqueue(this);

            TheTouch = newTouch;

            while (next.Count != 0)
            {
                LatticeLocation u = next.Dequeue();
                mNeighborhood.Add(u);

                if (currentDepth < Body.W)
                {
                    for (int i = 0; i != u.mImmediateNeighbors.Count; ++i)
                    {
                        LatticeLocation neighbor = u.mImmediateNeighbors[i];
                        if (neighbor.TheTouch != newTouch)
                        {
                            neighbor.TheTouch = newTouch;
                            next.Enqueue(neighbor);
                            ++elementsAtNextDepth;
                        }
                    }
                }

                --remainingAtThisDepth;
                if (remainingAtThisDepth == 0)
                {
                    ++currentDepth;
                    remainingAtThisDepth = elementsAtNextDepth;
                    elementsAtNextDepth = 0;
                }
            }

            mNeighborhood.Sort(LatticeLocation.CompareLatticeLocation);

            next.Clear();
            next.TrimExcess();
        }

        //
		RegionExists = true;

        for (int i = 0; i != Body.mLatticeLocationsWithExistentRegions.Count; ++i)
        {
            LatticeLocation check = Body.mLatticeLocationsWithExistentRegions[i];
            if (check.mNeighborhood.Count == mNeighborhood.Count)
            {
                bool bEqual = true;
                for (int j = 0; j != check.mNeighborhood.Count; ++j)
                {
                    LatticeLocation left    = check.mNeighborhood[j];
                    LatticeLocation right   = mNeighborhood[j];
                    if (left != right)
                    {
                        bEqual = false;
                        break;
                    }
                }

                if (bEqual)
                {
                    RegionExists = false;
                    break;
                }
            }
        }

        if (RegionExists)
        {
            Body.mLatticeLocationsWithExistentRegions.Add(this);
        }
	}

    public static int CompareLatticeLocation(LatticeLocation x, LatticeLocation y)
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
