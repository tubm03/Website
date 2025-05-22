using PetStoreProject.Helpers;

public class Vector
{
    private readonly Item[] vector;

    public Vector()
    {
    }

    public Vector(Item[] vector)
    {
        this.vector = vector;
    }

    public Vector(Vector vector)
    {
        this.vector = vector.ToArray();
    }

    public int Length()
    {
        return vector.Length;
    }

    public Item ValueOf(int index)
    {
        return vector[index];
    }

    // Vector addition
    /*public static Vector Add(Vector a, Vector b)
    {
        Item[] result = new Item[a.Length()];
        for (int i = 0; i < a.Length(); i++)
        {
            result[i] = a.ValueOf(i) + b.ValueOf(i);
        }
        return new Vector(result);
    }

    // Vector subtraction
    public static Vector Subtract(Vector a, Vector b)
    {
        if (a.Length() != b.Length())
            throw new ArgumentException("Vectors must have the same dimensions for subtraction.");

        Item[] result = new Item[a.Length()];
        for (int i = 0; i < a.Length(); i++)
        {
            result[i] = a.ValueOf(i) - b.ValueOf(i);
        }
        return new Vector(result);
    }*/

    // Dot product
    public static double DotProduct(Vector a, Vector b)
    {
        if (a.Length() != b.Length())
            throw new ArgumentException("Vectors must have the same dimensions for dot product.");

        double result = 0;
        for (int i = 0; i < a.Length(); i++)
        {
            result += (double)(a.ValueOf(i).RatingValue * b.ValueOf(i).RatingValue);
        }
        return result;
    }

    public double Magnitude()
    {
        double sumOfSquares = vector.Sum(component => (double)component.RatingValue * (double)component.RatingValue);
        return Math.Sqrt(sumOfSquares);
    }

    public static double Cosine(Vector a, Vector b)
    {
        double dotProduct = DotProduct(a, b);
        double magnitudeA = a.Magnitude();
        double magnitudeB = b.Magnitude();
        var cosin = dotProduct / (magnitudeA * magnitudeB);
        return cosin;
    }

    public Item[] ToArray()
    {
        return vector;
    }
}