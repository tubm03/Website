using PetStoreProject.Helpers;

public class Matrix
{
    private readonly Item[,] matrix;

    public int Rows { get; private set; }
    public int Columns { get; private set; }

    public Matrix(int rows, int columns)
    {
        Rows = rows;
        Columns = columns;
        matrix = new Item[rows, columns];
    }

    public Item this[int row, int col]
    {
        get { return matrix[row, col]; }
        set { matrix[row, col] = value; }
    }

    /*public static Matrix operator +(Matrix a, Matrix b)
    {
        if (a.Rows != b.Rows || a.Columns != b.Columns)
            throw new InvalidOperationException("Matrices must have the same dimensions for addition.");

        Matrix result = new Matrix(a.Rows, a.Columns);
        for (int i = 0; i < a.Rows; i++)
        {
            for (int j = 0; j < a.Columns; j++)
            {
                result[i, j].Value = a[i, j].Value + b[i, j].Value;
            }
        }
        return result;
    }

    public static Matrix operator -(Matrix a, Matrix b)
    {
        if (a.Rows != b.Rows || a.Columns != b.Columns)
            throw new InvalidOperationException("Matrices must have the same dimensions for subtraction.");

        Matrix result = new Matrix(a.Rows, a.Columns);
        for (int i = 0; i < a.Rows; i++)
        {
            for (int j = 0; j < a.Columns; j++)
            {
                result[i, j].Value = a[i, j].Value - b[i, j].Value;
            }
        }
        return result;
    }

    public static Matrix operator *(Matrix a, Matrix b)
    {
        if (a.Columns != b.Rows)
            throw new InvalidOperationException("Number of columns in the first matrix must equal the number of rows in the second matrix.");

        Matrix result = new Matrix(a.Rows, b.Columns);
        for (int i = 0; i < result.Rows; i++)
        {
            for (int j = 0; j < result.Columns; j++)
            {
                for (int k = 0; k < a.Columns; k++)
                {
                    result[i, j].Value += a[i, k].Value * b[k, j].Value;
                }
            }
        }
        return result;
    }
*/
    public Matrix Transpose()
    {
        Matrix result = new Matrix(Columns, Rows);
        for (int i = 0; i < Rows; i++)
        {
            for (int j = 0; j < Columns; j++)
            {
                result[j, i] = new Item(matrix[i, j].UserId, matrix[i, j].ProductId, matrix[i, j].RatingValue);
            }
        }
        return result;
    }

    public Vector GetRow(int rowIndex)
    {
        if (rowIndex < 0 || rowIndex >= Rows)
            throw new ArgumentOutOfRangeException(nameof(rowIndex), "Row index is out of range.");

        Item[] row = new Item[Columns];
        for (int i = 0; i < Columns; i++)
        {
            row[i] = matrix[rowIndex, i];
        }
        return new Vector(row);
    }

    public Vector GetColumn(int columnIndex)
    {
        if (columnIndex < 0 || columnIndex >= Columns)
            throw new ArgumentOutOfRangeException(nameof(columnIndex), "Column index is out of range.");

        Item[] column = new Item[Rows];
        for (int i = 0; i < Rows; i++)
        {
            column[i] = matrix[i, columnIndex];
        }
        return new Vector(column);
    }

    static double[] AvaregeRating(Matrix matrix)
    {
        double[] averages = new double[matrix.Columns];
        for (int i = 0; i < matrix.Columns; i++)
        {
            double sum = 0;
            int num = 0;
            for (int j = 0; j < matrix.Rows; j++)
            {
                if (matrix[j, i].RatingValue != null)
                {
                    sum += (double)matrix[j, i].RatingValue;
                    num += 1;
                }
            }
            averages[i] = sum / num;
        }
        return averages;

    }

    static Matrix Normalized(Matrix matrix)
    {
        Matrix normalized = new Matrix(matrix.Rows, matrix.Columns);
        double[] averages = AvaregeRating(matrix);

        for (int i = 0; i < matrix.Rows; i++)
        {
            for (int j = 0; j < matrix.Columns; j++)
            {
                if (matrix[i, j].RatingValue == null)
                {
                    normalized[i, j] = new Item(matrix[i, j]);
                    normalized[i, j].RatingValue = 0;
                }
                else
                {
                    normalized[i, j] = new Item(matrix[i, j]);
                    normalized[i, j].RatingValue -= averages[j];
                }
            }
        }
        return normalized;
    }

    static Matrix Similarity(Matrix matrix)
    {
        Matrix similarity = new Matrix(matrix.Columns, matrix.Columns);
        Matrix normalized = Normalized(matrix);
        for (int i = 0; i < matrix.Columns; i++)
        {
            Vector a = new Vector(normalized.GetColumn(i));
            for (int j = 0; j < matrix.Columns; j++)
            {
                if (i == j)
                {
                    similarity[i, j] = new Item(matrix[0, j].UserId, matrix[0, i].UserId, 1);
                    continue;
                }
                Vector b = new Vector(normalized.GetColumn(j));
                var cosin = Vector.Cosine(a, b);
                similarity[i, j] = new Item(matrix[0, j].UserId, matrix[0, i].UserId, cosin);
            }
        }
        return similarity;
    }

    public static Matrix Pred(Matrix matrix, int userId, int k)
    {
        Matrix rating = new Matrix(matrix.Rows, matrix.Columns);
        Matrix normalized = Normalized(matrix);
        Matrix similarity = Similarity(matrix);
        double[] averages = AvaregeRating(matrix);

        int index = 0;

        for (int i = 0; i < matrix.Columns; i++)
        {
            if (matrix[0, i].UserId == userId)
            {
                index = i;
                break;
            }
        }

        Item[] listUser = similarity.GetColumn(index).ToArray();

        int[] topKUserId = listUser.Where(item => item.RatingValue.HasValue && item.UserId != userId)
                                    .OrderByDescending(item => item.RatingValue)
                                    .Take(k).Select(x => x.UserId)
                                    .ToArray();

        List<double[]> ds = new List<double[]>();

        for (int i = 0; i < matrix.Rows; i++)
        {
            if (matrix[i, index].RatingValue == null)
            {
                double numerator = 0;
                double denominator = 0;
                for (int j = 0; j < matrix.Columns; j++)
                {
                    if (topKUserId.Contains(matrix[i, j].UserId) && matrix[i, j].RatingValue != null)
                    {
                        numerator += (double)(similarity[j, index].RatingValue * normalized[i, j].RatingValue);
                        denominator += Math.Abs((double)similarity[j, index].RatingValue);
                        double[] x = [numerator, denominator];
                        ds.Append(x);
                    }
                }
                rating[i, index] = new Item(matrix[i, index].ProductId, matrix[i, index].UserId, numerator / denominator + averages[index]);
                if (Double.IsNaN((double)rating[i, index].RatingValue))
                {
                    rating[i, index].RatingValue = averages[index];
                }
            }
            else
            {
                rating[i, index] = new Item(normalized[i, index]);
                rating[i, index].RatingValue += averages[index];
            }
        }

        return rating;
    }

    public static Item[] GetListItem(Matrix matrix)
    {
        List<Item> items = new List<Item>();
        for (int i = 0; i < matrix.Rows; i++)
        {
            for (int j = 0; j < matrix.Columns; j++)
            {
                if (matrix[i, j] != null)
                {
                    items.Add(matrix[i, j]);
                }
            }
        }
        return items.ToArray();
    }
}
