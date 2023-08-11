//Name: Zakariya Osman
//Date: 2023-08-10
//Program Description: ConcertEvent Displayer. Display all or Search by Artist or Genre.
//Search Input: Artist Name or Genre
//Output: Coresponding Concert Events

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySqlConnector;

namespace ConcertEvents.Classes
{
    //Properties for the ConcertModel with attributes of a concert event
    public class ConcertModel
    {
        public int TicketCost { get; set; }
        public string Genre { get; set; }
        public string Artist { get; set; }
        public string Date { get; set; }
        public string Time { get; set; }
        public string Venue { get; set; }
        public string City { get; set; }
        public string Description { get; set; }

        //List to hold each concert instance
        public List<ConcertModel> allConcerts;

        //Establishes a connection to the database and creates ConcertEvents database if it doesen't already exist
        public void CreateDatabase()
        {
            MySqlConnectionStringBuilder builder = new MySqlConnectionStringBuilder()
            {
                Server = "localhost",
                UserID = "root",
                Password = "password",
            };

            using (MySqlConnection connection = new MySqlConnection(builder.ConnectionString))
            {
                connection.Open();

                string createDatabaseScript = "CREATE DATABASE IF NOT EXISTS ConcertEvents;";
                using (MySqlCommand cmd = new MySqlCommand(createDatabaseScript, connection))
                {
                    cmd.ExecuteNonQuery();
                }
            }
        }
        //Connects to ConcertEvent Database and with the use of the Assignment2.sql file populates it with data by splitting them and executing them individually.
        public void InsertDataFromSQLFile()
        {
            MySqlConnectionStringBuilder builder = new MySqlConnectionStringBuilder()
            {
                Server = "localhost",
                UserID = "root",
                Password = "password",
                Database = "ConcertEvents",
            };

            Console.WriteLine(builder.ToString());

            using (MySqlConnection connection = new MySqlConnection(builder.ConnectionString))
            {
                connection.Open();

                string sqlFilePath = "C:\\Users\\osman\\source\\repos\\MyFinalProject\\ConcertEvents\\Assignment2.sql";
                string sqlContent = File.ReadAllText(sqlFilePath);

                string[] sqlStatements = sqlContent.Split(';');

                foreach (string sqlStatement in sqlStatements)
                {
                    if (!string.IsNullOrWhiteSpace(sqlStatement))
                    {
                        using (MySqlCommand cmd = new MySqlCommand(sqlStatement, connection))
                        {
                            cmd.ExecuteNonQuery();
                        }
                    }
                }
            }
        }
        //Connects to database and get concert data from concert table and adds data into the allConcert List above
        public void LoadConcerts()
        {
            MySqlConnectionStringBuilder builder = new MySqlConnectionStringBuilder()
            {
                Server = "localhost",
                UserID = "root",
                Password = "password",
                Database = "ConcertEvents",
            };

            MySqlConnection connection = new MySqlConnection(builder.ConnectionString);

            try
            {
                connection.Open();

                string query = "SELECT * FROM concert;";
                MySqlCommand cmd = new MySqlCommand(query, connection);
                MySqlDataReader dataReader = cmd.ExecuteReader();

                allConcerts = new List<ConcertModel>();

                while (dataReader.Read())
                {
                    var concert = new ConcertModel
                    {
                        TicketCost = dataReader.GetInt32("concert_ticket_cost"),
                        Genre = dataReader.GetString("concert_genre"),
                        Artist = dataReader.GetString("concert_artist"),
                        Date = dataReader.GetString("concert_date"),
                        Time = dataReader.GetString("concert_time"),
                        Venue = dataReader.GetString("concert_venue"),
                        City = dataReader.GetString("concert_city"),
                        Description = dataReader.GetString("concert_description")
                    };

                    allConcerts.Add(concert);
                }

                dataReader.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                allConcerts = null;
            }
            finally
            {
                connection.Close();
            }
        }
        //Based on the query it finds matching data which is saved to searchResults and returns it
        public List<ConcertModel> SearchConcerts(string query)
        {
            List<ConcertModel> searchResults = new List<ConcertModel>();

            try
            {
                MySqlConnectionStringBuilder builder = new MySqlConnectionStringBuilder()
                {
                    Server = "localhost",
                    UserID = "root",
                    Password = "password",
                    Database = "ConcertEvents",
                };

                MySqlConnection connection = new MySqlConnection(builder.ConnectionString);
                connection.Open();

                string searchStatementt = "SELECT * FROM concert WHERE concert_artist LIKE @query OR concert_genre LIKE @query;";
                MySqlCommand cmd = new MySqlCommand(searchStatementt, connection);
                cmd.Parameters.AddWithValue("@query", $"%{query}%");

                using (MySqlDataReader dataReader = cmd.ExecuteReader())
                {
                    while (dataReader.Read())
                    {
                        var concert = new ConcertModel
                        {
                            TicketCost = dataReader.GetInt32("concert_ticket_cost"),
                            Genre = dataReader.GetString("concert_genre"),
                            Artist = dataReader.GetString("concert_artist"),
                            Date = dataReader.GetString("concert_date"),
                            Time = dataReader.GetString("concert_time"),
                            Venue = dataReader.GetString("concert_venue"),
                            City = dataReader.GetString("concert_city"),
                            Description = dataReader.GetString("concert_description")
                        };

                        searchResults.Add(concert);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                searchResults = null;
            }

            return searchResults;
        }


    }



}
