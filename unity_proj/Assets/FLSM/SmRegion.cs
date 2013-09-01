using UnityEngine;
using System.Collections;

public class SmRegion : Summation
{
    public Vector3 mEx0 = Vector3.zero;
    public Vector3 mC0 = Vector3.zero;
    public float mM = 0.0f;

    public Vector3 mC = Vector3.zero;
    public Matrix3x3 mA = Matrix3x3.zero;
    public Matrix3x3 mEigenVectors = Matrix3x3.zero;

    public Vector3 mT = Vector3.zero;
    public Matrix3x3 mR = Matrix3x3.zero;
	
	public Color color = new Color(Random.Range(0.0f, 1.0f), Random.Range(0.0f, 1.0f), Random.Range(0.0f, 1.0f), 1.0f);
}
