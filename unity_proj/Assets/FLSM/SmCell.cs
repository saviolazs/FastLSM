using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/*
public class SmCell
{
    public LatticeLocation Center
    { get; set; }

    public SmCellVertex[] 	mVertices 		= new SmCellVertex[8];
    public List<SmCell> 	mConnectedCells = new List<SmCell>();

    public static readonly Point3[] mVertexOffset = new Point3[8] { 
        new Point3(0, 0, 0), 
        new Point3(0, 0, 1), 
        new Point3(0, 1, 0), 
        new Point3(0, 1, 1), 
        new Point3(1, 0, 0), 
        new Point3(1, 0, 1), 
        new Point3(1, 1, 0), 
        new Point3(1, 1, 1) };

    public SmCell()
    {
        Center = null;
    }

    public void Initialize()                              
    {
        Point3 p2 = Center.mIndex * 2;
        Vector3 spacing = Center.Body.mSpacing;

        for (int i = 0; i != 8; ++i)
        {
            if (null == mVertices[i])
            {
                mVertices[i] = new SmCellVertex();
            }
			SmCellVertex v = mVertices[i];
            v.Parent = this;
            v.mOriginalPosition = Center.SmParticle.mX0 +
                new Vector3(spacing.x * ((float)mVertexOffset[i].x - 0.5f),
                    spacing.y * ((float)mVertexOffset[i].y - 0.5f),
                    spacing.z * ((float)mVertexOffset[i].z - 0.5f));

            Point3 v2 = p2 + mVertexOffset[i] * 2 - Point3.one;
		    v.mV2 = v2;

		    for(int j = 0; j != 8; ++j)
		    {
                Point3 s2 = v2 + mVertexOffset[j] * 2 - Point3.one;
			    Point3 s = new Point3(s2.x / 2, s2.y / 2, s2.z / 2);

                if (Center.Body.GetLatticeLocation(s) != null)
                {
                    v.mShareVertexCells[j] = Center.Body.GetLatticeLocation(s).Cell;
                }
		    }
        }

        for (int i = 0; i != Center.mImmediateNeighbors.Count; ++i)
        {
            LatticeLocation lp = Center.mImmediateNeighbors[i];
            mConnectedCells.Add(lp.Cell);
        }

        mConnectedCells.Add(this);
    }

    public void Initialize2()
    {
        for (int i = 0; i != 8; ++i)
        {
            if (null == mVertices[i])
            {
                mVertices[i] = new SmCellVertex();
            }
			SmCellVertex v = mVertices[i];

            for (int j = 0; j != 8; ++j)
            {
                SmCell shared = v.mShareVertexCells[j];
                if (null != shared)
                {
                    for (int l = 0; l != 8; ++l)
                    {
                        if (shared.mVertices[l].mV2 == v.mV2)
                        {
                            v.mPartnerVertices[j] = shared.mVertices[l];
                        }
                    }

                    if (null == v.mPartnerVertices[j])
                    {
                        Debug.LogError("Error Initialize2");
                    }
                }
            }

            v.DeterminePositionArbiter();

            if (null == v.PositionArbiter)
            {
                Debug.LogError("Error Initialize2");
            }
        }
    }

    public void HandleVertexSharerFracture()
    {
        for (int i = 0; i != 8; ++i)
        {
            mVertices[i].HandleVertexSharerFracture();
        }
    }

    public void UpdateVertexPositions()
    {
        for (int i = 0; i != 8; ++i)
        {
            mVertices[i].UpdatePosition();
        }
    }
}
*/