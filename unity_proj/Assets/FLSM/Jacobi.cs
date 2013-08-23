using UnityEngine;
using System.Collections;

public static class Jacobi
{
    public static readonly float TOL = 1e-3f;
    public static readonly int MAX_SWEEPS = 50;
	public static int GetMatrix3x3ElementIndex(int iIndex)
	{
		/*
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
		*/

		switch (iIndex)
        {
            case 0://m00
				return 0;
            case 1://m01
				return 3;
            case 2://m02
				return 6;
			case 3://m10
				return 1;
			case 4://m11
				return 4;
			case 5://m12
				return 7;
			case 6://m20
				return 2;
			case 7://m21
				return 5;
			case 8://m22
				return 8;
			default:
                throw new System.IndexOutOfRangeException("Invalid matrix index!");

        }
	}
    public static void jacobi(int n, ref Matrix3x3 a, ref float[] d, ref Matrix3x3 v)
    {
        if (null == d)
            return;

        if (d.Length != 3)
            return;

        float onorm = 0.0f;
        float dnorm = 0.0f;
	    float b     = 0.0f;
        float dma   = 0.0f;
        float q     = 0.0f;
        float t     = 0.0f;
        float c     = 0.0f;
        float s     = 0.0f ;
	    float atemp = 0.0f;
        float vtemp = 0.0f;
        float dtemp = 0.0f;

        d[0] = a[GetMatrix3x3ElementIndex(0)];
        d[1] = a[GetMatrix3x3ElementIndex(4)];
        d[2] = a[GetMatrix3x3ElementIndex(8)];

        for (int l = 1; l <= MAX_SWEEPS; ++l)
        {
            dnorm = Mathf.Abs(d[0]) + Mathf.Abs(d[1]) + Mathf.Abs(d[2]);
            onorm = Mathf.Abs(a[GetMatrix3x3ElementIndex(1)]) + Mathf.Abs(a[GetMatrix3x3ElementIndex(2)]) + Mathf.Abs(a[GetMatrix3x3ElementIndex(5)]);

            if ((onorm / dnorm) <= TOL)
			{
				//Debug.Log("jacobi Times[" + l.ToString() + "]");
                return;
			}

            for (int j = 1; j < n; ++j)
		    {
			    for (int i = 0; i <= j - 1; ++i)
			    {

				    b = a[GetMatrix3x3ElementIndex(n*i+j)];
				    if(Mathf.Abs(b) > 0.0f)
				    {
					    dma = d[j] - d[i];
					    if((Mathf.Abs(dma) + Mathf.Abs(b)) <= Mathf.Abs(dma))
						    t = b / dma;
					    else
					    {
						    q = 0.5f * dma / b;
						    t = 1.0f/((float)Mathf.Abs(q) + (float)Mathf.Sqrt(1.0f+q*q));
						    if (q < 0.0)
							    t = -t;
					    }

					    c = 1.0f/(float)Mathf.Sqrt(t*t + 1.0f);
					    s = t * c;
					    a[GetMatrix3x3ElementIndex(n*i+j)] = 0.0f;

					    for (int k = 0; k <= i-1; ++k)
					    {
						    atemp = c * a[GetMatrix3x3ElementIndex(n*k+i)] - s * a[GetMatrix3x3ElementIndex(n*k+j)];
						    a[GetMatrix3x3ElementIndex(n*k+j)] = s * a[GetMatrix3x3ElementIndex(n*k+i)] + c * a[GetMatrix3x3ElementIndex(n*k+j)];
						    a[GetMatrix3x3ElementIndex(n*k+i)] = atemp;
					    }

					    for (int k = i+1; k <= j-1; ++k)
					    {
						    atemp = c * a[GetMatrix3x3ElementIndex(n*i+k)] - s * a[GetMatrix3x3ElementIndex(n*k+j)];
						    a[GetMatrix3x3ElementIndex(n*k+j)] = s * a[GetMatrix3x3ElementIndex(n*i+k)] + c * a[GetMatrix3x3ElementIndex(n*k+j)];
						    a[GetMatrix3x3ElementIndex(n*i+k)] = atemp;
					    }

					    for (int k = j+1; k < n; ++k)
					    {
						    atemp = c * a[GetMatrix3x3ElementIndex(n*i+k)] - s * a[GetMatrix3x3ElementIndex(n*j+k)];
						    a[GetMatrix3x3ElementIndex(n*j+k)] = s * a[GetMatrix3x3ElementIndex(n*i+k)] + c * a[GetMatrix3x3ElementIndex(n*j+k)];
						    a[GetMatrix3x3ElementIndex(n*i+k)] = atemp;
					    }

					    for (int k = 0; k < n; ++k)
					    {
						    vtemp = c * v[GetMatrix3x3ElementIndex(n*k+i)] - s * v[GetMatrix3x3ElementIndex(n*k+j)];
						    v[GetMatrix3x3ElementIndex(n*k+j)] = s * v[GetMatrix3x3ElementIndex(n*k+i)] + c * v[GetMatrix3x3ElementIndex(n*k+j)];
						    v[GetMatrix3x3ElementIndex(n*k+i)] = vtemp;
					    }

					    dtemp = c*c*d[i] + s*s*d[j] - 2.0f*c*s*b;
					    d[j] = s*s*d[i] + c*c*d[j] + 2.0f*c*s*b;
					    d[i] = dtemp;
				    } 
			    } 
		    }
        }
    }
}
