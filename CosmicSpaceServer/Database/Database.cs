using System.Data.SqlClient;

namespace Database
{
    public class Database
    {
        static string ConnectionString = "Server=DESKTOP-TI6I264;Database=CosmicSpace;Trusted_Connection=True;";
        public Database()
        {
            using (SqlConnection con = new SqlConnection())
            {
                con.Open();
                try
                {
                    using (SqlCommand command = new SqlCommand(
                        "CREATE TABLE Dogs1 (Weight INT, Name TEXT, Breed TEXT)", con))
                    {
                        command.ExecuteNonQuery();
                    }
                }
                catch
                {

                }
            }
        }
    }
}