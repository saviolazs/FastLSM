using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Summation 
{
    public LatticeLocation LP
    { get; set; }

    public int mMinDim = 0;

    public int mMaxDim = 0;

    public SummationData 		mSumData;
    public List<SmParticle> 	mParticles 		= new List<SmParticle>();
    protected List<Summation> 	mChildren 		= new List<Summation>();
    protected List<Summation> 	mParents 		= new List<Summation>();
    protected List<Summation>[] mConnections 	= new List<Summation>[2];

    public Summation()
    {
        mConnections[0] = mChildren;
        mConnections[1] = mParents;
    }

    public void FindParticleRange(int dimension, ref int minDim, ref int maxDim)
    {
	    minDim = int.MaxValue;
	    maxDim = int.MinValue;

        for (int i = 0; i != mParticles.Count; ++i)
        {
            SmParticle p = mParticles[i];
            if (p.LP.mIndex[dimension] < minDim)
                minDim = p.LP.mIndex[dimension];

            if (p.LP.mIndex[dimension] > maxDim)
                maxDim = p.LP.mIndex[dimension];
        }
    }

    public List<Summation> GenerateChildSums(int childLevel)
    {
        int splitDimension = childLevel;
        FindParticleRange(splitDimension, ref mMinDim, ref mMaxDim);

        List<Summation> newSums = new List<Summation>();
        Summation[] childArray = new Summation[mMaxDim - mMinDim + 1];
        for (int i = 0; i != mMaxDim - mMinDim + 1; ++i)
        {
            childArray[i] = new Summation();
        }

        for (int i = 0; i != mParticles.Count; ++i)
        {
            SmParticle p = mParticles[i];
            childArray[p.LP.mIndex[splitDimension] - mMinDim].mParticles.Add(p);
        }

        for (int i = mMinDim; i <= mMaxDim; ++i)
        {
            Summation child = childArray[i - mMinDim];

            if (child.mParticles.Count == 0)
            {
                child = null;
                childArray[i - mMinDim] = null;
            }
            else
            {
                child.mParticles.Sort(SmParticle.CompareSmParticle);
                child.LP = child.mParticles[0].LP;

                Summation identical = FindIdenticalSummation(ref child.mParticles, childLevel);

                if (null != identical)
                {
                    child = null;
                    childArray[i - mMinDim] = null;

                    mChildren.Add(identical);
                    identical.mParents.Add(this);
                }
                else
                {
                    newSums.Add(child);
                    mChildren.Add(child);
                    child.mParents.Add(this);
                    child.LP.mSummations[childLevel].Add(child);
                    LP.Body.mSummations[childLevel].Add(child);

                    if (childLevel > 0)
                    {
                        child.GenerateChildSums(childLevel - 1);
                    }
                    else
                    {
                        for (int m = 0; m != child.mParticles.Count; ++m)
                        {
                            SmParticle p = child.mParticles[m];
                            child.mChildren.Add(p);
                            p.mParents.Add(child);
                        }

                    }
                }
            }
        }

        System.Array.Clear(childArray, 0, childArray.Length);
        System.Array.Resize(ref childArray, 0);
        return newSums;
    }

    public void SumFromChildren()
    {
        PerformSummation(0);
    }

    public void SumFromParents()
    {
        PerformSummation(1);
    }

    public void PerformSummation(int direction)
    {
        List<Summation> source = mConnections[direction];
        if (source.Count == 0)
        {
            return;
        }
        else if (source.Count == 1)
        {
            mSumData.mV = source[0].mSumData.mV;
            mSumData.mM = source[0].mSumData.mM;
        }
        else
        {
            mSumData.mV = Vector3.zero;
            mSumData.mM = Matrix3x3.zero;

            for (int i = 0; i != source.Count; ++i)
            {
                mSumData.mV += (source[i].mSumData.mV);
                mSumData.mM += (source[i].mSumData.mM);
            }
        }
    }

    public static Summation FindIdenticalSummation(ref List<SmParticle> particles, int myLevel)
    {
        for (int m = 0; m != particles.Count; ++m)
        {
            LatticeLocation checkLp = particles[m].LP;

            for (int q = 0; q != checkLp.mSummations[myLevel].Count; ++q)
            {
                List<SmParticle> checkParticles = checkLp.mSummations[myLevel][q].mParticles;

                if (checkParticles.Count == particles.Count)
                {
                    bool bEqual = true;
                    for (int i = 0; i != checkParticles.Count; ++i)
                    {
                        SmParticle left = checkParticles[i];
                        SmParticle right = particles[i];
                        if (left != right)
                        {
                            bEqual = false;
                            break;
                        }
                    }

                    if (bEqual)
                    {
                        return checkLp.mSummations[myLevel][q];
                    }
                }

            }
        }

        return null;
    }
}
