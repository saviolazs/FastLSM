using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DeformableModel
{
	public bool IsDirty
	{
		get;
		private set;
	}
	
	public float Alpha
	{get;set;}

	public int W
	{
		get
		{
			return mW;
		}
		set
		{
			if (mW != value)
			{
				mW = value;
			}
		}
	}

    public Vector3 mSpacing = Vector3.zero;

    public float DefaultParticleMass
    { get; set; }
	
	public float RegionDamping
	{get;set;}
	
	protected int mW = 1;
	protected Dictionary<Point3, LatticeLocation> 	mLattice 								= new Dictionary<Point3, LatticeLocation>();
	public List<LatticeLocation> 					mLatticeLocations						= new List<LatticeLocation>();	
	public List<LatticeLocation> 					mLatticeLocationsWithExistentRegions	= new List<LatticeLocation>();
	public List<SmParticle>							mParticles								= new List<SmParticle>();
	public List<SmRegion>							mRegions								= new List<SmRegion>();
	public List<SmCell>								mCells									= new List<SmCell>();
    public List<Summation>[]                        mSummations                             = new List<Summation>[2];

    // Generation
	public DeformableModel(Vector3 vSpacing)
	{
		mSummations[0] = new List<Summation>();
		mSummations[1] = new List<Summation>();
		
		mSpacing 		= vSpacing;
		Alpha			= 1.0f;
		DefaultParticleMass = 1.0f;
		mW				= 3;
		RegionDamping	= 0.5f;
		
		IsDirty			= true;
	}
	
	public void AddParticle(Point3 index)
	{
		LatticeLocation l = new LatticeLocation();
		mLatticeLocations.Add(l);
		mLattice[index] = l;
        l.mIndex = index;
        l.Body = this;
        l.Region = null;

        for (int xo = -1; xo <= 1; ++xo)
        {
            for (int yo = -1; yo <= 1; ++yo)
            {
                for (int zo = -1; zo <= 1; ++zo)
                {
                    Point3 check = new Point3(index.x + xo, index.y + yo, index.z + zo);
                    if (!(xo == 0 && yo == 0 && zo == 0) && GetLatticeLocation(check) != null)
                    {
                        l.mImmediateNeighbors.Add(GetLatticeLocation(check));
                        l.mImmediateNeighborsGrid[xo + 1, yo + 1, zo + 1] = GetLatticeLocation(check);
                        GetLatticeLocation(check).mImmediateNeighbors.Add(l);
                        GetLatticeLocation(check).mImmediateNeighborsGrid[-xo + 1, -yo + 1, -zo + 1] = l;
                    }
                    else
                    {
                        l.mImmediateNeighborsGrid[xo + 1, yo + 1, zo + 1] = null;
                    }
                }
            }
        }

        l.SmParticle = new SmParticle();
        mParticles.Add(l.SmParticle);
        l.SmParticle.LP = l;
        l.SmParticle.mX0 = Vector3.Scale(new Vector3((float)index.x, (float)index.y, (float)index.z), mSpacing);
        l.SmParticle.Mass = DefaultParticleMass;
        l.SmParticle.mX = l.SmParticle.mX0;
        l.SmParticle.mV = Vector3.zero;
        l.SmParticle.mF = Vector3.zero;
        
	}

    public void Complete()
    {
        for (int i = 0; i != mLatticeLocations.Count; ++i)
        {
            LatticeLocation l = mLatticeLocations[i];
            l.IsEdge = (l.mImmediateNeighbors.Count != 26);
            l.CalculateNeighborhood();
        }

        GenerateSMRegions();

        for (int i = 0; i != mRegions.Count; ++i)
        {
            SmRegion r = mRegions[i];
            for (int j = 0; j != r.mParticles.Count; ++j)
            {
                SmParticle p = r.mParticles[j];
                p.mParentRegions.Add(r.LP);
            }
        }

        CalculateInvariants();

        InitializeCells();		// Cells help with rendering
    }

    // Simulation
    public void ShapeMatch()
    {
        if (IsDirty)
        {
            CalculateInvariants();
            IsDirty = false;
        }

        for (int i = 0; i != mParticles.Count; ++i)
        {
            SmParticle particle = mParticles[i];
            particle.mSumData.mV = particle.PerRegionMass * particle.mX;
            particle.mSumData.mM = Matrix3x3.MultiplyWithTranspose(particle.PerRegionMass * particle.mX, particle.mX0);
        }

        SumParticlesToRegions();

        for (int i = 0; i != mRegions.Count; ++i)
        {
            SmRegion r = mRegions[i];
            Vector3 Fmixi = r.mSumData.mV;
            Matrix3x3 Fmixi0T = r.mSumData.mM;

            r.mC = (1 / r.mM) * Fmixi;
            r.mA = Fmixi0T - Matrix3x3.MultiplyWithTranspose(r.mM * r.mC, r.mC0);

            Matrix3x3 S = r.mA.transpose * r.mA;
            S = r.mEigenVectors.transpose * S * r.mEigenVectors;
            float[] eigenValues = new float[3];
            Jacobi.jacobi(3, ref S, ref eigenValues, ref r.mEigenVectors);

            for (int j = 0; j != 3; ++j)
            {
                if (eigenValues[j] <= 0.0f)
                {
                    eigenValues[j] = 0.05f;
                }

                eigenValues[j] = 1.0f / Mathf.Sqrt(eigenValues[j]);
            }

            Matrix3x3 DPrime = new Matrix3x3(eigenValues[0], 0, 0, 0, eigenValues[1], 0, 0, 0, eigenValues[2]);
            S = r.mEigenVectors * DPrime * r.mEigenVectors.transpose;

            r.mR = r.mA * S;

            if (r.mR.determinant < 0)
            {
                r.mR *= (-1.0f);
            }

            r.mT = r.mC - r.mR * r.mC0;

            r.mSumData.mM = r.mR;
            r.mSumData.mV = r.mT;
        }

        SumRegionsToParticles();

        for (int i = 0; i != mParticles.Count; ++i)
        {
            SmParticle particle = mParticles[i];

            float invNumParentRegions = 1.0f / particle.mParentRegions.Count;

            particle.mG = (invNumParentRegions * particle.mSumData.mM) * particle.mX0 + invNumParentRegions * particle.mSumData.mV;

            particle.mR = invNumParentRegions * particle.mSumData.mM;
        }
    }

    public void CalculateParticleVelocities(float h)
    {
        for (int i = 0; i != mParticles.Count; ++i)
        {
            SmParticle particle = mParticles[i];
            if (particle.mParentRegions.Count == 0)
            {
                particle.mV = particle.mV + h * (particle.mF / particle.Mass);
                particle.mG = particle.mX;
            }
            else
            {
                particle.mV = particle.mV + Alpha * (particle.mG - particle.mX) / h + h * (particle.mF / particle.Mass);
            }

            particle.mF = Vector3.zero;
        }
    }

    public void PerformRegionDamping()
    {
        if (RegionDamping == 0.0f)
            return;

        for (int i = 0; i != mParticles.Count; ++i)
        {
            SmParticle particle = mParticles[i];
            particle.mSumData.mV = particle.PerRegionMass * particle.mV;
            particle.mSumData.mM.SetColumn(0, Vector3.Cross(particle.mX, particle.mSumData.mV));

            Vector3 x = particle.mX;
            particle.mSumData.mM.m21 = particle.PerRegionMass * (x.z * x.z + x.y * x.y);
            particle.mSumData.mM.m01 = particle.PerRegionMass * (-x.x * x.y);
            particle.mSumData.mM.m02 = particle.PerRegionMass * (-x.x * x.z);
            particle.mSumData.mM.m11 = particle.PerRegionMass * (x.z * x.z + x.x * x.x);
            particle.mSumData.mM.m12 = particle.PerRegionMass * (-x.z * x.y);
            particle.mSumData.mM.m22 = particle.PerRegionMass * (x.y * x.y + x.x * x.x);
        }

        SumParticlesToRegions();

        for (int i = 0; i != mRegions.Count; ++i)
        {
            SmRegion region = mRegions[i];
            Vector3     v = Vector3.zero;
            Vector3     L = Vector3.zero;
            Matrix3x3   I = Matrix3x3.zero;

            Matrix3x3 FmixixiT = new Matrix3x3(
                region.mSumData.mM.m21, region.mSumData.mM.m01, region.mSumData.mM.m02, 
                region.mSumData.mM.m01, region.mSumData.mM.m11, region.mSumData.mM.m12, 
                region.mSumData.mM.m02, region.mSumData.mM.m12, region.mSumData.mM.m22);

            v = (1.0f / region.mM) * region.mSumData.mV;
            L = region.mSumData.mM.GetColumn(0) - Vector3.Cross(region.mC, region.mSumData.mV);
            I = FmixixiT - region.mM * MrMatrix(ref region.mC);

            Vector3 w = I.inverse * L;

            region.mSumData.mV = v;
            region.mSumData.mM.SetColumn(0, w);
            region.mSumData.mM.SetColumn(1, Vector3.Cross(w, region.mC));
        }

        SumRegionsToParticles();

        for (int i = 0; i != mParticles.Count; ++i)
        {
            SmParticle particle = mParticles[i];
            if (particle.mParentRegions.Count > 0)
            {
                Vector3 Fv = particle.mSumData.mV;
                Vector3 Fw = particle.mSumData.mM.GetColumn(0);
                Vector3 Fwc = particle.mSumData.mM.GetColumn(1);
                Vector3 dv = (1.0f / particle.mParentRegions.Count) * (Fv + Vector3.Cross(Fw, particle.mX) - Fwc - (float)particle.mParentRegions.Count * particle.mV);

                particle.mV = particle.mV + RegionDamping * dv;
            }
        }
    }

    public void ApplyParticleVelocities(float h)
    {
        for (int i = 0; i != mParticles.Count; ++i)
        {
            SmParticle particle = mParticles[i];
            particle.mX = particle.mX + h * particle.mV;
        }
    }

    public void DoFracturing()
    {
    }

    public void UpdateCellPositions()
    {
        for (int i = 0; i != mCells.Count; ++i)
        {
            SmCell cell = mCells[i];
            cell.UpdateVertexPositions();
        }
    }

    // Fast summation
    public void SumParticlesToRegions()
    {
        for (int i = 0; i != mSummations[0].Count; ++i)
        {
            Summation bar = mSummations[0][i];
            bar.SumFromChildren();
        }

        for (int i = 0; i != mSummations[1].Count; ++i)
        {
            Summation plate = mSummations[1][i];
            plate.SumFromChildren();
        }

        for (int i = 0; i != mRegions.Count; ++i)
        {
            SmRegion region = mRegions[i];
            region.SumFromChildren();
        }
    }

    public void SumRegionsToParticles()
    {
        for (int i = 0; i != mSummations[1].Count; ++i)
        {
            Summation plate = mSummations[1][i];
            plate.SumFromParents();
        }

        for (int i = 0; i != mSummations[0].Count; ++i)
        {
            Summation bar = mSummations[0][i];
            bar.SumFromParents();
        }

        for (int i = 0; i != mParticles.Count; ++i)
        {
            SmParticle particle = mParticles[i];
            particle.SumFromParents();
        }
    }

    // Helper functions;
    public void GenerateSMRegions()
    {
        for (int i = 0; i != mLatticeLocationsWithExistentRegions.Count; ++i)
        {
            LatticeLocation l = mLatticeLocationsWithExistentRegions[i];
            l.Region = new SmRegion();
            mRegions.Add(l.Region);
            l.Region.LP = l;

            for (int j = 0; j != l.mNeighborhood.Count; ++j)
            {
                LatticeLocation l2 = l.mNeighborhood[j];
                l.Region.mParticles.Add(l2.SmParticle);
            }

            l.Region.mParticles.Sort(SmParticle.CompareSmParticle);

            l.Region.mEigenVectors = Matrix3x3.identity;
        }

        for (int i = 0; i != mRegions.Count; ++i)
        {
            SmRegion region = mRegions[i];
            region.GenerateChildSums(1);
        }
    }

    public void CalculateInvariants()
    {
		Debug.Log("Calculating invariants...");
		
        for (int i = 0; i != mParticles.Count; ++i)
        {
            SmParticle particle = mParticles[i];
            particle.PerRegionMass = particle.Mass / particle.mParentRegions.Count;
        }

        for (int i = 0; i != mParticles.Count; ++i)
        {
            SmParticle p = mParticles[i];
            p.mSumData.mM.m00 = p.PerRegionMass;
            p.mSumData.mV = p.PerRegionMass * p.mX0;
        }

        SumParticlesToRegions();

        for (int i = 0; i != mRegions.Count; ++i)
        {
            SmRegion r = mRegions[i];
            r.mM = r.mSumData.mM.m00;
            r.mEx0 = r.mSumData.mV;
            r.mC0 = r.mEx0 / r.mM;
        }
		
		Debug.Log(" done.");
    }

    public void InitializeCells()
    {
        for (int i = 0; i != mLatticeLocations.Count; ++i)
        {
            LatticeLocation l = mLatticeLocations[i];
            l.Cell = new SmCell();
            mCells.Add(l.Cell);
            l.Cell.Center = l;
        }

        for (int i = 0; i != mCells.Count; ++i)
        {
            SmCell cell = mCells[i];
            cell.Initialize();
        }

        for (int i = 0; i != mCells.Count; ++i)
        {
            SmCell cell = mCells[i];
            cell.Initialize2();
        }
    }

    public void RebuildRegions(ref List<LatticeLocation> regen)
    {
    }

    public LatticeLocation GetLatticeLocation(Point3 index)
	{
		LatticeLocation result = null;
		if (mLattice.TryGetValue(index, out result))
		{
			return result;
		}
		
		return null;
	}

    public static Matrix3x3 MrMatrix(ref Vector3 v)
    {
        Matrix3x3 ret;
        ret.m00 = (v.z * v.z + v.y * v.y);
        ret.m01 = (-v.x * v.y);
        ret.m02 = (-v.x * v.z);
        ret.m11 = (v.z * v.z + v.x * v.x);
        ret.m12 = (-v.z * v.y);
        ret.m22 = (v.y * v.y + v.x * v.x);
        ret.m10 = ret.m01;
        ret.m20 = ret.m02;
        ret.m21 = ret.m12;
        return ret;
    }
}
