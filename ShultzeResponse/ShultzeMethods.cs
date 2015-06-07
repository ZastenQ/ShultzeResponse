using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShultzeResponse
{
    public static class ShultzeMethods
    {
        private static Random random = new Random(DateTime.Now.Millisecond);

        static void Main(string[] args)
        {
            //Int32[,] testMatrix = new Int32[5, 5] { 
            //                                    { 0, 20, 26, 30, 22 }, 
            //                                    { 25, 0, 16, 33, 18},
            //                                    { 19, 29, 0, 17, 24 }, 
            //                                    { 15, 12, 28, 0, 14 }, 
            //                                    { 23, 27, 21, 31, 0 } };

            //PrintMatrix(testMatrix);
            //PrintMatrix(RunSchulzeMethod(testMatrix));
            //Int32[] winners = GetWinner(RunSchulzeMethod(testMatrix)).ToArray();
            //winners.ToList().ForEach(x => Console.Write("{0, 5} ", x));

            string connectionString = @"Server=LENOVO-PC\SQLEXPRESS;Database=Shultze; User Id=sa;  Password=su;";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                //CreateTheme(connection, "Confitures", "Lemon", "Orange", "Apple", "Strawberry", "Raspberry",
                //                        "Blackberry", "Blueberry", "Cherry", "Banana", "Pineapple", "Tangerine",
                //                        "Cranberry");
                //CreateTheme(connection, "Fruits", "Lemon", "Orange", "Apple", "Cherry", "Banana", "Pineapple", "Tangerine");
                //CreateTheme(connection, "Berries", "Strawberry", "Raspberry", "Blackberry", "Blueberry", "Cranberry");

                //  FillResponses(connection);
                IEnumerable<Int32[,]> responses = ProcessResponses(connection);

                foreach (Int32[,] matrix in responses)
                {
                    PrintMatrix(matrix);
                    PrintMatrix(RunSchulzeMethod(matrix));
                    Console.WriteLine("Winners!!!");
                    Int32[] winners = GetWinner(RunSchulzeMethod(matrix)).ToArray();
                    winners.ToList().ForEach(x => Console.WriteLine("{0, 5} ", x));
                    Console.WriteLine();
                }
                Console.ReadKey();
            }
        }

        public static void CreateTheme(SqlConnection sqlconn, string themeName, params string[] optionNames)
        {
            Int32 id = 0;

            // define INSERT query with parameters
            string themeQuery = "INSERT INTO THEME (NAME) VALUES (@NAME); SELECT CAST(scope_identity() AS int);";
            string optionQuery = "INSERT INTO [OPTION] (NAME, THEME_ID) VALUES(@NAME, @ID)";

            // create connection and command
            using (SqlCommand cmd = new SqlCommand(themeQuery, sqlconn))
            {
                // define parameters and their values
                cmd.Parameters.Add("@NAME", SqlDbType.NVarChar).Value = themeName;
                // open connection, execute INSERT, close connection
                id = (Int32)cmd.ExecuteScalar();
            }

            // define parameters and their values
            foreach (string name in optionNames)
            {
                using (SqlCommand cmd = new SqlCommand(optionQuery, sqlconn))
                {
                    cmd.Parameters.Add("@NAME", SqlDbType.NVarChar).Value = name;
                    cmd.Parameters.Add("@ID", SqlDbType.Int).Value = id;
                    // open connection, execute INSERT, close connection
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public static void FillResponses(SqlConnection sqlconn)
        {
            String allOptionsQuery = "SELECT O.THEME_ID, O.ID FROM [OPTION] O INNER JOIN [THEME] T ON O.THEME_ID = T.ID";
            String insertResponse = "INSERT INTO [RESPONSE] (THEME_ID, OPTION_ID, PRIORITY, SESSION) VALUES(@THEME, @OPTION, @PRIOR, @SES)";
            DataSet result = new DataSet();
            Int32 session = 0;

            using (SqlCommand cmd = new SqlCommand(allOptionsQuery, sqlconn))
            {
                using (SqlDataAdapter adapter = new SqlDataAdapter(cmd))
                {
                    adapter.Fill(result);
                }
            }

            using (SqlCommand cmd = new SqlCommand(insertResponse, sqlconn))
            {
                cmd.Parameters.Add("@THEME", SqlDbType.Int);
                cmd.Parameters.Add("@OPTION", SqlDbType.Int);
                cmd.Parameters.Add("@PRIOR", SqlDbType.Int);
                cmd.Parameters.Add("@SES", SqlDbType.Int);

                foreach (var group in result.Tables[0].AsEnumerable().GroupBy(x => x["THEME_ID"]))
                {
                    for (Int32 i = 0; i < 1000; i++)
                    {
                        List<Int32> priorities = Enumerable.Range(0, group.Count()).ToList();

                        cmd.Parameters["@THEME"].Value = group.Key;
                        cmd.Parameters["@SES"].Value = session++;

                        foreach (DataRow row in group)
                        {
                            cmd.Parameters["@OPTION"].Value = row["ID"];
                            cmd.Parameters["@PRIOR"].Value = PopRandom(priorities);
                            cmd.ExecuteNonQuery();
                        }
                    }
                }
            }
        }

        private static Int32 PopRandom(List<Int32> input)
        {
            Int32 index = random.Next(input.Count());
            Int32 output = input[index];
            input.RemoveAt(index);
            return output;
        }

        private static void PrintMatrix(Int32[,] getMatrix)
        {
            for (Int32 i = 0; i < getMatrix.GetLength(0); i++)
            {
                for (Int32 j = 0; j < getMatrix.GetLength(1); j++)
                {
                    Console.Write("{0, 5} ", getMatrix[i, j]);
                }
                Console.WriteLine();
            }
            Console.WriteLine();
        }

        private static Int32[,] GetPriorityMatrix(IEnumerable<ResponseData> source)
        {
            Int32[,] output = null;
            foreach (var group in source.GroupBy(x => x.Session))
            {
                if (output == null)
                {
                    Int32 arraySize = group.Max(x => x.Priority) + 1;
                    output = new Int32[arraySize, arraySize];
                }

                Int32 i = 0, j = 0;

                foreach (var itemA in group)
                {
                    foreach (var itemB in group)
                    {
                        if (itemA != itemB && itemA.Priority < itemB.Priority)
                        {
                            output[i, j] += 1;
                        }
                        j++;
                    }
                    i++;
                    j = 0;
                }
            }
            return output;
        }



        public static IEnumerable<Int32[,]> ProcessResponses(SqlConnection sqlconn)
        {
            String allOptionsQuery = @"
            SELECT R.OPTION_ID, R.[PRIORITY], R.THEME_ID, R.[SESSION]
              FROM [OPTION] O
                   INNER JOIN RESPONSE R ON R.OPTION_ID = O.ID
                   INNER JOIN THEME T ON T.ID = R.THEME_ID";

            DataSet result = new DataSet();

            using (SqlCommand cmd = new SqlCommand(allOptionsQuery, sqlconn))
            {
                using (SqlDataAdapter adapter = new SqlDataAdapter(cmd))
                {
                    adapter.Fill(result);
                }
            }

            return //IEnumerable<Int32[,]> responses = 
                 result.Tables[0].AsEnumerable()
                 .Select(x => new ResponseData(x))
                 .GroupBy(x => x.ThemeID)
                 .Select(x => GetPriorityMatrix(x))
                 .ToList();
        }

        public static Int32[,] ProcessResponses(SqlConnection sqlconn, Int32 themeID)
        {
            String allOptionsQuery = @"
            SELECT R.OPTION_ID, R.[PRIORITY], R.THEME_ID, R.[SESSION]
              FROM [OPTION] O
                   INNER JOIN RESPONSE R ON R.OPTION_ID = O.ID
             WHERE R.THEME_ID = @THEME";

            DataSet result = new DataSet();

            using (SqlCommand cmd = new SqlCommand(allOptionsQuery, sqlconn))
            {
                cmd.Parameters.Add("@THEME", SqlDbType.Int).Value = themeID;

                using (SqlDataAdapter adapter = new SqlDataAdapter(cmd))
                {
                    adapter.Fill(result);
                }
            }

            return GetPriorityMatrix(result.Tables[0].AsEnumerable().Select(x => new ResponseData(x)));
        }

        public static Int32[,] RunSchulzeMethod(Int32[,] inputMatrix)
        {
            Int32[,] outputMatrix = new Int32[inputMatrix.GetLength(0), inputMatrix.GetLength(1)];

            for (Int32 i = 0; i < inputMatrix.GetLength(0); i++)
            {
                for (Int32 j = 0; j < inputMatrix.GetLength(1); j++)
                {
                    if (i != j && inputMatrix[i, j] > inputMatrix[j, i])
                    {
                        outputMatrix[i, j] = inputMatrix[i, j];
                    }
                    else
                    {
                        outputMatrix[i, j] = 0;
                    }
                }
            }

            for (Int32 i = 0; i < outputMatrix.GetLength(0); i++)
            {
                for (Int32 j = 0; j < outputMatrix.GetLength(1); j++)
                {
                    if (i != j)
                    {
                        for (Int32 k = 0; k < outputMatrix.GetLength(0); k++)
                        {
                            if (i != k && j != k)
                            {
                                outputMatrix[j, k] = Math.Max(outputMatrix[j, k], Math.Min(outputMatrix[j, i], outputMatrix[i, k]));
                            }
                        }
                    }
                }
            }

            return (outputMatrix);
        }

        public static IEnumerable<Int32> GetWinner(Int32[,] shultzeMatrix)
        {
            for (Int32 i = 0; i < shultzeMatrix.GetLength(0); i++)
            {
                Boolean k = true;

                for (Int32 j = 0; j < shultzeMatrix.GetLength(1); j++)
                {
                    if (i != j && shultzeMatrix[i, j] <= shultzeMatrix[j, i])
                    {
                        k = false;
                    }
                }

                if (k)
                {
                    yield return i;
                }
            }
        }

        public static IEnumerable<Theme> GetThemes(SqlConnection sqlconn)
        {
            String allOptionsQuery = @"select t.ID, t.NAME from dbo.THEME t";

            DataSet result = new DataSet();

            using (SqlCommand cmd = new SqlCommand(allOptionsQuery, sqlconn))
            {
                using (SqlDataAdapter adapter = new SqlDataAdapter(cmd))
                {
                    adapter.Fill(result);
                }
            }

            return (result.Tables[0].AsEnumerable().Select(x => new Theme(x)));
        }
    }
}
