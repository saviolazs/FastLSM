using UnityEngine;
using System.Collections;

public static class Utility 
{
	public enum Axis
	{
		X = 0,
		Y = 1,
		Z = 2
	}
	
	static void Bounds_Triangle_Overlap_CHECK_FINDMINMAX(float x0, float x1, float x2,ref float min, ref float max)
	{
		min = max = x0;
		if(x1<min) 
			min=x1;
		
		if(x1>max) 
			max=x1;
		
		if(x2<min) 
			min=x2;
		
		if(x2>max) 
			max=x2;
	}
	
	
	public static bool Bounds_Plane_Overlap(ref Vector3 normal, ref Vector3 vert, ref Vector3 maxbox)
	{
		int q;
		Vector3 vmin = Vector3.zero;
		Vector3 vmax = Vector3.zero;
		float v = 0.0f;
		
		for(q=0;q<=2;q++)
		{
			v=vert[q];					
			if(normal[q]>0.0f)
			{
				vmin[q]=-maxbox[q] - v;	
				vmax[q]= maxbox[q] - v;	
			}
			else
			{
				vmin[q]= maxbox[q] - v;	
				vmax[q]=-maxbox[q] - v;	
			}
		}
		
		if(Vector3.Dot(normal,vmin)>0.0f) 
			return false;
		
		if(Vector3.Dot(normal,vmax)>=0.0f) 
			return true;
		
		return false;
	}
	
	public static bool Bounds_Triangle_Overlap(ref Bounds bounds, ref Vector3 v0, ref Vector3 v1, ref Vector3 v2)
	{
		/* move everything so that the boxcenter is in (0,0,0) */
		v0 = v0 - bounds.center;
		v1 = v1 - bounds.center;
		v2 = v2 - bounds.center;
		
		Vector3 boxhalfsize = bounds.size / 2.0f;
		
		/* compute triangle edges */
		Vector3 e0 = v1 - v0;
		Vector3 e1 = v2 - v1;
		Vector3 e2 = v0 - v2;

		float fex 	= 0.0f;
		float fey 	= 0.0f;
		float fez 	= 0.0f;
		float p0 	= 0.0f;
		float p1 	= 0.0f;
		float p2 	= 0.0f;
		float rad 	= 0.0f;
		float min	= 0.0f;
		float max 	= 0.0f;
		
		//------------------------------------------------------------
		//------------------------------------------------------------
		/* Bullet 3:  */
   		/*  test the 9 tests first (this was faster) */
		fex = Mathf.Abs(e0[0]);
		fey = Mathf.Abs(e0[1]);
		fez = Mathf.Abs(e0[2]);
		
		//AXISTEST_X01(e0[Z], e0[Y], fez, fey);
		p0 = e0[(int)Axis.Z]*v0[(int)Axis.Y] - e0[(int)Axis.Y]*v0[(int)Axis.Z];
		p2 = e0[(int)Axis.Z]*v2[(int)Axis.Y] - e0[(int)Axis.Y]*v2[(int)Axis.Z];
		if(p0<p2) 
		{
			min = p0; 
			max = p2;
		} 
		else 
		{
			min = p2; 
			max = p0;
		}

		rad = fez * boxhalfsize[(int)Axis.Y] + fey * boxhalfsize[(int)Axis.Z];
		if(min > rad || max <- rad) 
			return false;
		
		//AXISTEST_Y02(e0[Z], e0[X], fez, fex);
		p0 = -e0[(int)Axis.Z]*v0[(int)Axis.X] + e0[(int)Axis.X]*v0[(int)Axis.Z];
		p2 = -e0[(int)Axis.Z]*v2[(int)Axis.X] + e0[(int)Axis.X]*v2[(int)Axis.Z];
	    if(p0<p2) 
		{
			min=p0; 
			max=p2;
		} 
		else 
		{
			min=p2; 
			max=p0;
		}
	
		rad = fez * boxhalfsize[(int)Axis.X] + fex * boxhalfsize[(int)Axis.Z];
	
		if(min>rad || max<-rad) 
			return false;
		
   		//AXISTEST_Z12(e0[Y], e0[X], fey, fex);
		p1 = e0[(int)Axis.Y]*v1[(int)Axis.X] - e0[(int)Axis.X]*v1[(int)Axis.Y];
		p2 = e0[(int)Axis.Y]*v2[(int)Axis.X] - e0[(int)Axis.X]*v2[(int)Axis.Y];
	    if(p2<p1) 
		{
			min=p2; 
			max=p1;
		} 
		else 
		{
			min=p1; 
			max=p2;
		}
		rad = fey * boxhalfsize[(int)Axis.X] + fex * boxhalfsize[(int)Axis.Y];
		if(min>rad || max<-rad) 
			return false;
		
		//------------------------------------------------------------
		fex = Mathf.Abs(e1[0]);
		fey = Mathf.Abs(e1[1]);
		fez = Mathf.Abs(e1[2]);
		
		//AXISTEST_X01(e1[Z], e1[Y], fez, fey);
		p0 = e1[(int)Axis.Z]*v0[(int)Axis.Y] - e1[(int)Axis.Y]*v0[(int)Axis.Z];
		p2 = e1[(int)Axis.Z]*v2[(int)Axis.Y] - e1[(int)Axis.Y]*v2[(int)Axis.Z];
	    if(p0<p2) 
		{
			min=p0; 
			max=p2;
		} 
		else 
		{
			min=p2; 
			max=p0;
		}
		rad = fez * boxhalfsize[(int)Axis.Y] + fey * boxhalfsize[(int)Axis.Z];
		if(min>rad || max<-rad) 
			return false;

   		//AXISTEST_Y02(e1[Z], e1[X], fez, fex);
		p0 = -e1[(int)Axis.Z]*v0[(int)Axis.X] + e1[(int)Axis.X]*v0[(int)Axis.Z];
		p2 = -e1[(int)Axis.Z]*v2[(int)Axis.X] + e1[(int)Axis.X]*v2[(int)Axis.Z];
	    if(p0<p2) 
		{
			min=p0; 
			max=p2;
		} 
		else 
		{
			min=p2; 
			max=p0;
		}
	
		rad = fez * boxhalfsize[(int)Axis.X] + fex * boxhalfsize[(int)Axis.Z];
		if(min>rad || max<-rad) 
			return false;

   		//AXISTEST_Z0(e1[Y], e1[X], fey, fex);	   
		p0 = e1[(int)Axis.Y]*v0[(int)Axis.X] - e1[(int)Axis.X]*v0[(int)Axis.Y];
		p1 = e1[(int)Axis.Y]*v1[(int)Axis.X] - e1[(int)Axis.X]*v1[(int)Axis.Y];
	    if(p0<p1) 
		{
			min=p0; 
			max=p1;
		} 
		else 
		{
			min=p1; 
			max=p0;
		}
		rad = fey * boxhalfsize[(int)Axis.X] + fex * boxhalfsize[(int)Axis.Y];
		if(min>rad || max<-rad) 
			return false;
		
		//------------------------------------------------------------
		fex = Mathf.Abs(e2[0]);
		fey = Mathf.Abs(e2[1]);
		fez = Mathf.Abs(e2[2]);
		
		//#define AXISTEST_X2(a, b, fa, fb)			   
		p0 = e2[(int)Axis.Z]*v0[(int)Axis.Y] - e2[(int)Axis.Y]*v0[(int)Axis.Z];
		p1 = e2[(int)Axis.Z]*v1[(int)Axis.Y] - e2[(int)Axis.Y]*v1[(int)Axis.Z];
	    if(p0<p1) 
		{
			min=p0; 
			max=p1;
		} 
		else 
		{
			min=p1; 
			max=p0;
		} 
		rad = fez * boxhalfsize[(int)Axis.Y] + fey * boxhalfsize[(int)Axis.Z];
		if(min>rad || max<-rad) 
			return false;
	   	//AXISTEST_Y1(e2[Z], e2[X], fez, fex);	   
		p0 = -e2[(int)Axis.Z]*v0[(int)Axis.X] + e2[(int)Axis.X]*v0[(int)Axis.Z];
		p1 = -e2[(int)Axis.Z]*v1[(int)Axis.X] + e2[(int)Axis.X]*v1[(int)Axis.Z];
	    if(p0<p1) 
		{
			min=p0; 
			max=p1;
		} 
		else 
		{
			min=p1; 
			max=p0;
		}
		rad = fez * boxhalfsize[(int)Axis.X] + fex * boxhalfsize[(int)Axis.Z];
		if(min>rad || max<-rad) 
			return false;
	
	   	//AXISTEST_Z12(e2[Y], e2[X], fey, fex);	   
		p1 = e2[(int)Axis.Y]*v1[(int)Axis.X] - e2[(int)Axis.X]*v1[(int)Axis.Y];
		p2 = e2[(int)Axis.Y]*v2[(int)Axis.X] - e2[(int)Axis.X]*v2[(int)Axis.Y];
	    if(p2<p1) 
		{
			min=p2; 
			max=p1;
		} 
		else 
		{
			min=p1; 
			max=p2;
		}
		rad = fey * boxhalfsize[(int)Axis.X] + fex * boxhalfsize[(int)Axis.Y];
		if(min>rad || max<-rad) 
			return false;
		
		/* Bullet 1: */

	   	/*  first test overlap in the {x,y,z}-directions */
	
	   	/*  find min, max of the triangle each direction, and test for overlap in */
	
	   	/*  that direction -- this is equivalent to testing a minimal AABB around */
	
	   	/*  the triangle against the AABB */
	
	
	
	   	/* test in X-direction */
	   	Bounds_Triangle_Overlap_CHECK_FINDMINMAX(v0[(int)Axis.X],v1[(int)Axis.X],v2[(int)Axis.X],ref min,ref max);
	   	if(min>boxhalfsize[(int)Axis.X] || max<-boxhalfsize[(int)Axis.X]) 
			return false;
	
	
	
	   	/* test in Y-direction */
	
	   	Bounds_Triangle_Overlap_CHECK_FINDMINMAX(v0[(int)Axis.Y],v1[(int)Axis.Y],v2[(int)Axis.Y],ref min,ref max);
	
	   	if(min>boxhalfsize[(int)Axis.Y] || max<-boxhalfsize[(int)Axis.Y]) 
			return false;
	
	
	
	   	/* test in Z-direction */
	
	   	Bounds_Triangle_Overlap_CHECK_FINDMINMAX(v0[(int)Axis.Z],v1[(int)Axis.Z],v2[(int)Axis.Z],ref min,ref max);
	
	   	if(min>boxhalfsize[(int)Axis.Z] || max<-boxhalfsize[(int)Axis.Z]) 
			return false;
		
		/* Bullet 2: */

	   	/*  test if the box intersects the plane of the triangle */
	
	   	/*  compute plane equation of triangle: normal*x+d=0 */
	
	   	Vector3 normal = Vector3.Cross(e0,e1);
	
	   	if(false == Bounds_Plane_Overlap(ref normal, ref v0, ref boxhalfsize)) 
			return false;

	   	return true;   /* box and triangle overlaps */
	}
}
