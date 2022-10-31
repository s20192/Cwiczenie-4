using System.Data;
using System.Data.SqlClient;

namespace AnimalsSqlDb
{
    public interface IDatabaseService
    {
        IEnumerable<Animal> GetAnimals(string orderBy);
        Animal GetAnimal(int id);
        int AddAnimal(Animal animal);
        void UpdateAnimal(int id, Animal animal);
        void DeleteAnimal(int id);
    }

    public class SqlDatabaseService : IDatabaseService
    {
        private IConfiguration _configuration;

        public SqlDatabaseService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public int AddAnimal(Animal animal)
        {
            int id = -1;

            using (SqlConnection connection = new(_configuration.GetConnectionString("ProductionDb")))
            {
                SqlCommand cmd = new()
                {
                    Connection = connection,
                    CommandText = "INSERT INTO Animal(Name,Description,Category,Area) VALUES(@animalName, " +
                    "@animalDescription, @animalCategory, @animalArea)"
                };

                cmd.Parameters.AddWithValue("@animalName", animal.Name);
                cmd.Parameters.AddWithValue("@animalDescription", animal.Description);
                cmd.Parameters.AddWithValue("@animalCategory", animal.Category);
                cmd.Parameters.AddWithValue("@animalArea", animal.Area);

                connection.Open();
                int rows = cmd.ExecuteNonQuery();

                if (rows == 0)
                {
                    throw new AnimalNotFoundException($"Animal with id: {animal.IdAnimal} not found");
                }
                cmd.Parameters.Clear();
                cmd.CommandText = "SELECT id=IDENT_CURRENT('Animal')";
                using (var dr = cmd.ExecuteReader())
                {
                    if (dr.Read())
                    {
                        id = (int)(dr.GetDecimal("id"));
                    }
                }
            }
            return id;
        }

        public void DeleteAnimal(int idAnimal)
        {
            using (SqlConnection connection = new(_configuration.GetConnectionString("ProductionDb")))
            {
                SqlCommand cmd = new()
                {
                    Connection = connection,
                    CommandText = "DELETE FROM Animal WHERE IdAnimal=@animalId"
                };

                cmd.Parameters.AddWithValue("@animalId", SqlDbType.Int).Value = idAnimal;

                connection.Open();
                int row = cmd.ExecuteNonQuery();

                if (row == 0)
                {
                    throw new AnimalNotFoundException($"Animal with id: {idAnimal} not found");
                }
            }
        }

        public IEnumerable<Animal> GetAnimals(string orderBy)
        {
            var animals = new List<Animal>();
            using (SqlConnection connection = new(_configuration.GetConnectionString("ProductionDb")))
            {
                SqlCommand cmd = new()
                {
                    Connection = connection,
                    CommandText = "SELECT * FROM Animal"
                };

                string param = orderBy.ToLower();

                switch (param)
                {
                    case "description":
                        cmd.CommandText += " ORDER BY Description";
                        break;
                    case "category":
                        cmd.CommandText += " ORDER BY Category";
                        break;
                    case "area":
                        cmd.CommandText += " ORDER BY Area";
                        break;
                    default:
                        cmd.CommandText += " ORDER BY Name";
                        break;
                }

                connection.Open();
                SqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    animals.Add(new Animal
                    {
                        IdAnimal = dr.GetInt32(dr.GetOrdinal("IdAnimal")),
                        Name = dr["Name"].ToString(),
                        Description = dr["Description"].ToString(),
                        Category = dr["Category"].ToString(),
                        Area = dr["Area"].ToString()

                    });
                }
            }
            return animals;
        }

        public Animal GetAnimal(int idNumber)
        {
            Animal animal = null;
            using (SqlConnection connection = new(_configuration.GetConnectionString("ProductionDb")))
            {
                SqlCommand cmd = new()
                {
                    Connection = connection,
                    CommandText = "SELECT IdAnimal,Name,Description,Category,Area FROM Animal WHERE IdAnimal=@idNumber"
                };
                cmd.Parameters.AddWithValue("@idNumber", SqlDbType.Int).Value = idNumber;

                connection.Open();
                SqlDataReader dr = cmd.ExecuteReader();
          
                if (dr.Read())
                {
                    animal = (new Animal
                    {
                        IdAnimal = dr.GetInt32(dr.GetOrdinal("IdAnimal")),
                        Name = dr["Name"].ToString(),
                        Description = dr["Description"].ToString(),
                        Category = dr["Category"].ToString(),
                        Area = dr["Area"].ToString()
                    });
                }
                if(dr.Read())
                {
                    throw new Exception("Two animals with the same id!");
                }
            }

            if (animal != null)
            {
                return animal;
            }
            else
            {
                throw new AnimalNotFoundException($"Animal with id: {idNumber} not found");
            }

        }

        public void UpdateAnimal(int idAnimal, Animal animal)
        {
            using (SqlConnection connection = new(_configuration.GetConnectionString("ProductionDb")))
            {
                SqlCommand cmd = new();
                cmd.Connection = connection;
                cmd.CommandText = "UPDATE Animal" +
                    "              SET Name=@animalName," +
                    "                  Description=@animalDescription," +
                    "                  Category=@animalCategory," +
                    "                  Area=@animalArea" +
                    "              WHERE IdAnimal=@animalId";

                cmd.Parameters.AddWithValue("@animalName", animal.Name);
                cmd.Parameters.AddWithValue("@animalDescription", animal.Description);
                cmd.Parameters.AddWithValue("@animalCategory", animal.Category);
                cmd.Parameters.AddWithValue("@animalArea", animal.Area);
                cmd.Parameters.AddWithValue("@animalId", SqlDbType.Int).Value = idAnimal;

                connection.Open();
                int rows = cmd.ExecuteNonQuery();
                if(rows ==0)
                {
                    throw new AnimalNotFoundException($"Animal with id: {idAnimal} not found");
                }
            }
        }
    }
}
