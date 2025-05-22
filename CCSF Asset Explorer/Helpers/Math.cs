
public class Vector4
{
    public Vector4() { }
    public Vector4(float x, float y, float z, float w)
    {
        this.X = x;
        this.Y = y;
        this.Z = z;
        this.W = w;
    }
    public float X, Y, Z, W; 
    public static Vector4 One { get => new Vector4() { X = 1, Y = 1, Z = 1, W = 1 }; }

    public static Vector4 Zero { get => new Vector4() { X = 0, Y = 0, Z = 0, W=0 }; }
}
public class Vector3
{
    internal float X { get; set; }
    internal float Y { get; set; }
    internal float Z { get; set; }
    public static Vector3 Multiply(Vector3 input, float produto) => new Vector3(input.X * produto,
            input.Y * produto,
            input.Z * produto);
    public static Vector3 Divide(Vector3 input, float divisor) => new Vector3(input.X / divisor,
        input.Y / divisor,
        input.Z / divisor);
    public Vector3() { }
    public Vector3(float x, float y, float z)
    {
        this.X = x;
        this.Y = y;
        this.Z = z;
    }
    public Vector3(float value)
    {
        this.X = value;
        this.Y = value;
        this.Z = value;
    }
        
    public static Vector3 One { get => new Vector3() { X = 1, Y = 1, Z = 1 }; }
    public static Vector3 Zero { get => new Vector3() { X = 0, Y = 0, Z=0 }; }
}
public class Vector2
{
    internal Vector2 Multiply(Vector2 input, float produto) => new Vector2(input.X * produto,
                input.Y * produto);
    internal Vector2 Divide(Vector2 input, float divisor) => new Vector2(input.X / divisor,
            input.Y / divisor);
    public Vector2() { }
    public Vector2(float x, float y)
    {
        this.X = x;
        this.Y = y;
    }
    public float X { get; set; }
    public float Y { get; set; }
    public static Vector2 One { get => new Vector2() { X = 1, Y = 1 }; }
    public static Vector2 Zero { get => new Vector2() { X = 0, Y = 0 }; }
}
public class Quaternion
{
    public Vector3 Xyz { get => new Vector3(X, Y, Z); }
    public Quaternion() { }
    public Quaternion(float x, float y, float z, float w)
    {
        this.X = x;
        this.Y = y;
        this.Z = z;
        this.W = w;
    }

    public float X, Y, Z, W;
    public static Quaternion Identity { get => new Quaternion() { X = 0, Y = 0, Z= 0, W=1 }; }
    public static Quaternion Zero { get => new Quaternion() { X = 0, Y = 0, Z= 0, W=0 }; }
}

