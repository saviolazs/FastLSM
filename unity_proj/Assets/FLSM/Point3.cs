using UnityEngine;
using System.Collections;

public struct Point3
{
    public int x;
    public int y;
    public int z;
	
    public Point3(int x, int y, int z)
    {
        this.x = x;
        this.y = y;
        this.z = z;
    }

    public int this[int index]
    {
        get
        {
            switch (index)
            {
                case 0:
                    return this.x;

                case 1:
                    return this.y;

                case 2:
                    return this.z;
            }
            throw new System.IndexOutOfRangeException("Invalid Point3 index!");
        }
        set
        {
            switch (index)
            {
                case 0:
                    this.x = value;
                    break;

                case 1:
                    this.y = value;
                    break;

                case 2:
                    this.z = value;
                    break;

                default:
                    throw new System.IndexOutOfRangeException("Invalid Point3 index!");
            }
        }
    }
	
    public void Set(int new_x, int new_y, int new_z)
    {
        this.x = new_x;
        this.y = new_y;
        this.z = new_z;
    }
	
    public override int GetHashCode()
    {
        return ((this.x.GetHashCode() ^ (this.y.GetHashCode() << 2)) ^ (this.z.GetHashCode() >> 2));
    }

    public override bool Equals(object other)
    {
        if (!(other is Point3))
        {
            return false;
        }
        Point3 point = (Point3) other;
        return ((this.x.Equals(point.x) && this.y.Equals(point.y)) && this.z.Equals(point.z));
    }

    public override string ToString()
    {
        object[] args = new object[] { this.x, this.y, this.z };
        return string.Format("Point3({0:F1}, {1:F1}, {2:F1})", args);
    }

    public static Point3 zero
    {
        get
        {
            return new Point3(0, 0, 0);
        }
    }

    public static Point3 one
    {
        get
        {
            return new Point3(1, 1, 1);
        }
    }

    public static Point3 operator +(Point3 a, Point3 b)
    {
        return new Point3(a.x + b.x, a.y + b.y, a.z + b.z);
    }

    public static Point3 operator -(Point3 a, Point3 b)
    {
        return new Point3(a.x - b.x, a.y - b.y, a.z - b.z);
    }

    public static Point3 operator -(Point3 a)
    {
        return new Point3(-a.x, -a.y, -a.z);
    }

    public static Point3 operator *(Point3 a, int d)
    {
        return new Point3(a.x * d, a.y * d, a.z * d);
    }

    public static Point3 operator *(int d, Point3 a)
    {
        return new Point3(a.x * d, a.y * d, a.z * d);
    }

    public static bool operator ==(Point3 lhs, Point3 rhs)
    {
        return ( lhs.x == rhs.x && lhs.y == rhs.y && lhs.z == rhs.z );
    }

    public static bool operator !=(Point3 lhs, Point3 rhs)
    {
        return ( lhs.x != rhs.x || lhs.y != rhs.y || lhs.z != rhs.z );
    }
}