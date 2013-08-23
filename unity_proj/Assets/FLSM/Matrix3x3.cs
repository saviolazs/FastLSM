using UnityEngine;
using System.Collections;

public struct Matrix3x3
{
    public float m00;
    public float m10;
    public float m20;
    public float m01;
    public float m11;
    public float m21;
    public float m02;
    public float m12;
    public float m22;

	public static Matrix3x3 zero
    {
        get
        {
            return new Matrix3x3 
			{ 
				m00 = 0f, m01 = 0f, m02 = 0f,
				m10 = 0f, m11 = 0f, m12 = 0f, 
				m20 = 0f, m21 = 0f, m22 = 0f
			};
        }
    }
	
    public static Matrix3x3 identity
    {
        get
        {
            return new Matrix3x3 
			{ 
				m00 = 1f, m01 = 0f, m02 = 0f,
				m10 = 0f, m11 = 1f, m12 = 0f,
				m20 = 0f, m21 = 0f, m22 = 1f
			};
        }
    }
	
	public Matrix3x3 transpose
    {
        get
        {
            return Transpose(ref this);
        }
    }
	
	public Matrix3x3 inverse
	{
		get
		{
			Matrix3x3 inv;
			if (Inverse(ref this, out inv))
			{
				return inv;
			}
			return zero;
		}
	}
	
	public float determinant
	{
		get
		{
			return Determinant(ref this);
		}
	}
	
    public Matrix3x3(ref Matrix3x3 rkMatrix)
	{
		m00 = rkMatrix.m00;
		m01 = rkMatrix.m01;
		m02 = rkMatrix.m02;
		m10 = rkMatrix.m10;
		m11 = rkMatrix.m11;
		m12 = rkMatrix.m12;
		m20 = rkMatrix.m20;
		m21 = rkMatrix.m21;
		m22 = rkMatrix.m22;
	}
	
    public Matrix3x3(float fEntry00, float fEntry01, float fEntry02,
		             float fEntry10, float fEntry11, float fEntry12,
		             float fEntry20, float fEntry21, float fEntry22)
	{
		m00 = fEntry00;
		m01 = fEntry01;
		m02 = fEntry02;
		m10 = fEntry10;
		m11 = fEntry11;
		m12 = fEntry12;
		m20 = fEntry20;
		m21 = fEntry21;
		m22 = fEntry22;
	}
	
    public Matrix3x3(Vector3 x,
	                 Vector3 y,
	                 Vector3 z)
	{
		m00 = x.x;
		m10 = x.y;
		m20 = x.z;
		m01 = y.x;
		m11 = y.y;
		m21 = y.z;
		m02 = z.x;
		m12 = z.y;
		m22 = z.z;
	}
	
    public float this[int row, int column] 
	{ 
		get
	    {
	        return this[row + (column * 3)];
	    }
	    set
	    {
	        this[row + (column * 3)] = value;
		}
	}
	
    public float this[int index] 
	{ 
		get
	    {
	        switch (index)
	        {
	            case 0:
	                return this.m00;
	
	            case 1:
	                return this.m10;
	
	            case 2:
	                return this.m20;
				
				case 3:
					return this.m01;
					
				case 4:
					return this.m11;
					
				case 5:
					return this.m21;
					
				case 6:
					return this.m02;
					
				case 7:
					return this.m12;
					
				case 8:
					return this.m22;
				
				default:
	                throw new System.IndexOutOfRangeException("Invalid matrix index!");

	        }
	    }
	    set
	    {
	        switch (index)
	        {
	            case 0:
	                this.m00 = value;
	                break;
	
	            case 1:
	                this.m10 = value;
	                break;
	
	            case 2:
	                this.m20 = value;
	                break;
	
				case 3:
					this.m01 = value;
					break;
					
				case 4:
					this.m11 = value;
					break;
					
				case 5:
					this.m21 = value;
					break;
					
				case 6:
					this.m02 = value;
					break;
					
				case 7:
					this.m12 = value;
					break;
					
				case 8:
					this.m22 = value;
					break;
				
	            default:
	                throw new System.IndexOutOfRangeException("Invalid matrix index!");
	        }
	    }
	}
	
	public override int GetHashCode()
	{
		return (((this.GetColumn(0).GetHashCode() ^ (this.GetColumn(1).GetHashCode() << 2)) ^ (this.GetColumn(2).GetHashCode() >> 2)));
	}
	
    public override bool Equals(object other)
	{
		if (!(other is Matrix3x3))
	    {
	        return false;
	    }
		
	    Matrix3x3 matrixx = (Matrix3x3) other;
	    return (((this.GetColumn(0).Equals(matrixx.GetColumn(0)) && this.GetColumn(1).Equals(matrixx.GetColumn(1))) && this.GetColumn(2).Equals(matrixx.GetColumn(2))));
	}
	
	public Vector3 GetColumn(int i)
	{
		return new Vector3(this[0, i], this[1, i], this[2, i]);
	}
	
	public Vector3 GetRow(int i)
	{
		return new Vector4(this[i, 0], this[i, 1], this[i, 2]);
	}
	
	public void SetColumn(int i, Vector3 v)
	{
		this[0, i] = v.x;
	    this[1, i] = v.y;
	    this[2, i] = v.z;
	}
	
    public void SetRow(int i, Vector3 v)
	{
		this[i, 0] = v.x;
	    this[i, 1] = v.y;
	    this[i, 2] = v.z;
	}
	
	void Orthonormalize()
	{
	    // Algorithm uses Gram-Schmidt orthogonalization.  If 'this' matrix is
	    // M = [m0|m1|m2], then orthonormal output matrix is Q = [q0|q1|q2],
	    //
	    //   q0 = m0/|m0|
	    //   q1 = (m1-(q0*m1)q0)/|m1-(q0*m1)q0|
	    //   q2 = (m2-(q0*m2)q0-(q1*m2)q1)/|m2-(q0*m2)q0-(q1*m2)q1|
	    //
	    // where |V| indicates length of vector V and A*B indicates dot
	    // product of vectors A and B.
		
	    // compute q0
		float fInvLength = 1.0f/Mathf.Sqrt(m00*m00 + m10*m10 + m20*m20);
		
		m00 *= fInvLength;
		m10 *= fInvLength;
		m20 *= fInvLength;
		
		// compute q1
		float fDot0 = m00*m01 + m10*m11 + m20*m21;
		
		m01 -= fDot0*m00;
		m11 -= fDot0*m10;
		m21 -= fDot0*m20;
		
		fInvLength = 1.0f/Mathf.Sqrt(m01*m01 + m11*m11 + m21*m21);
		
		m01 *= fInvLength;
		m11 *= fInvLength;
		m21 *= fInvLength;
		
		// compute q2
		float fDot1 = m01*m02 + m11*m12 + m21*m22;
		
		fDot0 = m00*m02 + m10*m12 + m20*m22;
		
		m02 -= fDot0*m00 + fDot1*m01;
		m12 -= fDot0*m10 + fDot1*m11;
		m22 -= fDot0*m20 + fDot1*m21;
		
		fInvLength = 1.0f/Mathf.Sqrt(m02*m02 + m12*m12 + m22*m22);
		
		m02 *= fInvLength;
		m12 *= fInvLength;
		m22 *= fInvLength;
	}
	
	public static bool operator ==(Matrix3x3 lhs, Matrix3x3 rhs)
	{
		return ((((lhs.GetColumn(0) == rhs.GetColumn(0)) && (lhs.GetColumn(1) == rhs.GetColumn(1))) && (lhs.GetColumn(2) == rhs.GetColumn(2))));
	}
    
	public static bool operator !=(Matrix3x3 lhs, Matrix3x3 rhs)
	{
		return !(lhs == rhs);
	}
	
	public static Matrix3x3 operator+ (Matrix3x3 lhs, Matrix3x3 rhs)
	{
	    return new Matrix3x3
			(
				lhs.m00 + rhs.m00, lhs.m01 + rhs.m01, lhs.m02 + rhs.m02,
				lhs.m10 + rhs.m10, lhs.m11 + rhs.m11, lhs.m12 + rhs.m12,
				lhs.m20 + rhs.m20, lhs.m21 + rhs.m21, lhs.m22 + rhs.m22
			);
	}
	
	public static Matrix3x3 operator- (Matrix3x3 lhs, Matrix3x3 rhs)
	{
	    return new Matrix3x3
			(
				lhs.m00 - rhs.m00, lhs.m01 - rhs.m01, lhs.m02 - rhs.m02,
				lhs.m10 - rhs.m10, lhs.m11 - rhs.m11, lhs.m12 - rhs.m12,
				lhs.m20 - rhs.m20, lhs.m21 - rhs.m21, lhs.m22 - rhs.m22
			);
	}
	
	public static Matrix3x3 operator* (Matrix3x3 lhs, Matrix3x3 rhs)
	{
	    return new Matrix3x3
			(
				lhs.m00*rhs.m00 + lhs.m01*rhs.m10 + lhs.m02*rhs.m20, lhs.m00*rhs.m01 + lhs.m01*rhs.m11 + lhs.m02*rhs.m21 , lhs.m00*rhs.m02 + lhs.m01*rhs.m12 + lhs.m02*rhs.m22,
				lhs.m10*rhs.m00 + lhs.m11*rhs.m10 + lhs.m12*rhs.m20, lhs.m10*rhs.m01 + lhs.m11*rhs.m11 + lhs.m12*rhs.m21 , lhs.m10*rhs.m02 + lhs.m11*rhs.m12 + lhs.m12*rhs.m22,
				lhs.m20*rhs.m00 + lhs.m21*rhs.m10 + lhs.m22*rhs.m20, lhs.m20*rhs.m01 + lhs.m21*rhs.m11 + lhs.m22*rhs.m21 , lhs.m20*rhs.m02 + lhs.m21*rhs.m12 + lhs.m22*rhs.m22
			);
	}
	
	public static Vector3 operator* (Vector3 lhs, Matrix3x3 rhs)
	{
	    return new Vector3
			(
				lhs.x*rhs.m00 + lhs.y*rhs.m10 + lhs.z*rhs.m20, lhs.x*rhs.m01 + lhs.y*rhs.m11 + lhs.z*rhs.m21, lhs.x*rhs.m02 + lhs.y*rhs.m12 + lhs.z*rhs.m22
			);
	}
	
	public static Vector3 operator* (Matrix3x3 lhs, Vector3 rhs)
	{
	    return new Vector3
			(
				lhs.m00*rhs.x + lhs.m01*rhs.y + lhs.m02*rhs.z, lhs.m10*rhs.x + lhs.m11*rhs.y + lhs.m12*rhs.z, lhs.m20*rhs.x + lhs.m21*rhs.y + lhs.m22*rhs.z
			);
	}
	
	public static Matrix3x3 operator* (float lhs, Matrix3x3 rhs)
	{
	    return new Matrix3x3
			(
				lhs*rhs.m00, lhs*rhs.m01, lhs*rhs.m02,
				lhs*rhs.m10, lhs*rhs.m11, lhs*rhs.m12,
				lhs*rhs.m20, lhs*rhs.m21, lhs*rhs.m22
			);
	}
	
	public static Matrix3x3 operator* (Matrix3x3 lhs, float rhs)
	{
	    return new Matrix3x3
			(
				lhs.m00*rhs, lhs.m01*rhs, lhs.m02*rhs,
				lhs.m10*rhs, lhs.m11*rhs, lhs.m12*rhs,
				lhs.m20*rhs, lhs.m21*rhs, lhs.m22*rhs
			);
	}
	
	public static Matrix3x3 Transpose(ref Matrix3x3 inMatrix)
	{
	    Matrix3x3 transpose;
	    transpose.m00 = inMatrix.m00;
		transpose.m01 = inMatrix.m10;
		transpose.m02 = inMatrix.m20;
		transpose.m10 = inMatrix.m01;
		transpose.m11 = inMatrix.m11;
		transpose.m12 = inMatrix.m21;
		transpose.m20 = inMatrix.m02;
		transpose.m21 = inMatrix.m12;
		transpose.m22 = inMatrix.m22;
		
	    return transpose;
	}
	
	public static bool Inverse(ref Matrix3x3 inMatrix, out Matrix3x3 outMatrix)
	{
	    // Invert a 3x3 using cofactors.  This is about 8 times faster than
	    // the Numerical Recipes code which uses Gaussian elimination.

	    outMatrix.m00 = inMatrix.m11*inMatrix.m22 - inMatrix.m12*inMatrix.m21;
	    outMatrix.m01 = inMatrix.m02*inMatrix.m21 - inMatrix.m01*inMatrix.m22;
	    outMatrix.m02 = inMatrix.m01*inMatrix.m12 - inMatrix.m02*inMatrix.m11;
	    outMatrix.m10 = inMatrix.m12*inMatrix.m20 - inMatrix.m10*inMatrix.m22;
	    outMatrix.m11 = inMatrix.m00*inMatrix.m22 - inMatrix.m02*inMatrix.m20;
	    outMatrix.m12 = inMatrix.m02*inMatrix.m10 - inMatrix.m00*inMatrix.m12;
	    outMatrix.m20 = inMatrix.m10*inMatrix.m21 - inMatrix.m11*inMatrix.m20;
	    outMatrix.m21 = inMatrix.m01*inMatrix.m20 - inMatrix.m00*inMatrix.m21;
	    outMatrix.m22 = inMatrix.m00*inMatrix.m11 - inMatrix.m01*inMatrix.m10;
	
	    float fDet =
	        inMatrix.m00*outMatrix.m00 +
	        inMatrix.m01*outMatrix.m10 +
	        inMatrix.m02*outMatrix.m20;
	
	    if ( Mathf.Abs(fDet) <= Mathf.Epsilon )
	        return false;
	
	    float fInvDet = 1.0f/fDet;
		
	    outMatrix.m00 *= fInvDet;
		outMatrix.m01 *= fInvDet;
		outMatrix.m02 *= fInvDet;
		outMatrix.m10 *= fInvDet;
		outMatrix.m11 *= fInvDet;
		outMatrix.m12 *= fInvDet;
		outMatrix.m20 *= fInvDet;
		outMatrix.m21 *= fInvDet;
		outMatrix.m22 *= fInvDet;
		
	    return true;
	}
	
	public static float Determinant(ref Matrix3x3 inMatrix)
	{
	    float fCofactor00 = inMatrix.m11*inMatrix.m22 - inMatrix.m12*inMatrix.m21;
	    float fCofactor10 = inMatrix.m12*inMatrix.m20 - inMatrix.m10*inMatrix.m22;
	    float fCofactor20 = inMatrix.m10*inMatrix.m21 - inMatrix.m11*inMatrix.m20;
	
	    float fDet =
	        inMatrix.m00*fCofactor00 +
	        inMatrix.m01*fCofactor10 +
	        inMatrix.m02*fCofactor20;
	
	    return fDet;
	}
	
	public static void MultiplyWithTranspose(out Matrix3x3 outMatrix, Vector3 a, Vector3 b)
	{
		outMatrix.m00 = a.x * b.x;
		outMatrix.m01 = a.x * b.y;
		outMatrix.m02 = a.x * b.z;
		outMatrix.m10 = a.y * b.x;
		outMatrix.m11 = a.y * b.y;
		outMatrix.m12 = a.y * b.z;
		outMatrix.m20 = a.z * b.x;
		outMatrix.m21 = a.z * b.y;
		outMatrix.m22 = a.z * b.z;
	}
	
	public static Matrix3x3 MultiplyWithTranspose(Vector3 a, Vector3 b)
	{
		return new Matrix3x3(a.x * b.x,	a.x * b.y,	a.x * b.z,
							 a.y * b.x,	a.y * b.y,	a.y * b.z,
							 a.z * b.x,	a.z * b.y,	a.z * b.z);
	}
	
	public override string ToString()
	{
	    object[] args = new object[] { this.m00, this.m01, this.m02, this.m10, this.m11, this.m12, this.m20, this.m21, this.m22 };
	    return string.Format("{0:F5}\t{1:F5}\t{2:F5}\t{3:F5}\n{4:F5}\t{5:F5}\t{6:F5}\t{7:F5}\n{8:F5}\n", args);
	}
}

