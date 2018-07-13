using UnityEngine;

public class Position
{
    public int x
    {
        get; private set;
    }
    public int y
    {
        get; private set;
    }
    public int z
    {
        get; private set;
    }


    public Position( int x, int y, int z )
    {
        this.x = x;
        this.y = y;
        this.z = z;
    }

    public override bool Equals( object obj )
    {
        var position = obj as Position;
        return position != null &&
               x == position.x &&
               y == position.y &&
               z == position.z;
    }

    public override int GetHashCode()
    {
        var hashCode = 373119288;
        hashCode = hashCode * -1521134295 + x.GetHashCode();
        hashCode = hashCode * -1521134295 + y.GetHashCode();
        hashCode = hashCode * -1521134295 + z.GetHashCode();
        return hashCode;
    }

    public static bool operator==(Position p1, Position p2)
    {
        if(ReferenceEquals(p1, null) || ReferenceEquals(p2, null))
        {
            return false;
        }

        return (p1.x == p2.x && p1.y == p2.y && p1.z == p2.z);
    }

    public static bool operator !=( Position p1, Position p2 )
    {
        return !(p1 == p2);
    }

    public static Position operator +( Position p1, Position p2 )
    {
        return new Position(p1.x + p2.x, p1.y + p2.y, p1.z + p2.z);
    }

    public static Position FromVector(Vector3 vector)
    {
        int x = Mathf.FloorToInt(vector.x);
        int y = Mathf.FloorToInt(vector.y);
        int z = Mathf.FloorToInt(vector.z);

        return new Position(x, y, z);
    }
}
