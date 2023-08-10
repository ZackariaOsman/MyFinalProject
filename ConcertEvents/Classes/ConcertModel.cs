﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySqlConnector;

namespace ConcertEvents.Classes
{
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

        public List<ConcertModel> allConcerts;


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

                string searchStatement = "SELECT * FROM concert WHERE concert_artist LIKE @query OR concert_genre LIKE @query;";
                MySqlCommand cmd = new MySqlCommand(searchStatement, connection);
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