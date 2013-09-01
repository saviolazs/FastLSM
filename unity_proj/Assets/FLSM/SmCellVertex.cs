using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/*
public class SmCellVertex
{
    public Vector3  mPosition                   = Vector3.zero;
    public Vector3  mOriginalPosition           = Vector3.zero;
    public Vector3  mOriginalPositionOffset     = Vector3.zero;
    public Point3   mV2                         = Point3.zero;

    public SmCell Parent
    { get; set; }

    public SmParticle PositionArbiter
    { get; set; }

    public SmCellVertex PositionArbiterVertex
    { get; set; }

    public SmCell[] 		mShareVertexCells 	= new SmCell[8];
    public SmCellVertex[] 	mPartnerVertices 	= new SmCellVertex[8];

    public SmCellVertex()
    {
        Parent = null;
        PositionArbiter = null;
        PositionArbiterVertex = null;

        for (int i = 0; i != 8; ++i)
        {
            mShareVertexCells[i] = null;
            mPartnerVertices[i] = null;
        }
    }

    public void DeterminePositionArbiter()
    {
        SmCell positionArbiterCell = null;
        PositionArbiter = null;
        List<LatticeLocation> immediateNeighbors = Parent.Center.mImmediateNeighbors;
        for (int i = 0; i != 8; ++i)
        {
            if (null != mShareVertexCells[i])
            {
                if (null == PositionArbiter || mShareVertexCells[i].Center.SmParticle.CreateTime < PositionArbiter.CreateTime)
                {
                    if (mShareVertexCells[i] == Parent || immediateNeighbors.Contains(mShareVertexCells[i].Center))
                    {
                        PositionArbiterVertex = mPartnerVertices[i];
                        positionArbiterCell = mShareVertexCells[i];
                        PositionArbiter = mShareVertexCells[i].Center.SmParticle;
                        mOriginalPositionOffset = PositionArbiterVertex.mOriginalPosition - PositionArbiter.mX0;
                        mOriginalPosition = PositionArbiterVertex.mOriginalPosition;
                    }
                }
            }
        }
    }

    public void HandleVertexSharerFracture()
    {
        DeterminePositionArbiter();
    }

    public void UpdatePosition()
    {
        if (PositionArbiter.mParentRegions.Count <= 1)
        {
            mPosition = PositionArbiter.mG + mOriginalPositionOffset;
        }
        else
        {
            mPosition = PositionArbiter.mG + PositionArbiter.mR * mOriginalPositionOffset;
        }
    }
}
 
*/