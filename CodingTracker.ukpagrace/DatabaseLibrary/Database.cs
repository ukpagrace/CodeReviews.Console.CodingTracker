﻿using Dapper;
using Microsoft.Data.Sqlite;
using Application.Entities;


namespace DatabaseLibrary
{
    public class Database
    {

        public void Create()
        {
            using var connectionString = new SqliteConnection(@"Data Source = Coding_Tracker.db");
            connectionString.Open();
            string createTable = @"CREATE TABLE IF NOT EXISTS codingTracker (
                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                StartDate TEXT,
                EndDate TEXT,
                Duration TEXT
             )";

            connectionString.Execute(createTable);
            connectionString.Close();
        }



        public async void Insert(DateTime startDate, DateTime endDate, TimeSpan duration)
        {
            using var connectionString = new SqliteConnection(@"Data Source = Coding_Tracker.db");

            var sql = "Insert INTO codingTracker (StartDate, EndDate, Duration) VALUES (@StartDate, @EndDate, @Duration)";
            UserEntity userEntity = new() { StartDate = startDate.ToString("yyyy-MM-dd hh:mm"), EndDate = endDate.ToString("yyyy-MM-dd hh:mm"), Duration = duration.ToString() };


            var affectedRows = await connectionString.ExecuteAsync(sql, userEntity);
            Console.WriteLine($"{affectedRows} row(s) inserted");
        }


        public async void Update(int id, DateTime startDate, DateTime endDate, TimeSpan duration)
        {
            using var connectionString = new SqliteConnection(@"Data Source = Coding_Tracker.db");

            var sql = @"UPDATE codingTracker SET
                        StartDate = @StartDate, EndDate = @EndDate,
                        Duration= @Duration WHERE Id = @Id";
            UserEntity userEntity = new() { Id = id, StartDate = startDate.ToString("yyyy-MM-dd hh:mm"), EndDate = endDate.ToString("yyyy-MM-dd hh:mm"), Duration = duration.ToString() };


            var affectedRows = await connectionString.ExecuteAsync(sql, userEntity);
            Console.WriteLine($"{affectedRows} row(s) inserted");
        }


        public void List()
        {
            string connectionString = @"Data Source = Coding_Tracker.db";
            using var connection = new SqliteConnection(connectionString);

            var sql = @"SELECT * FROM codingTracker";

            var codingDurations = connection.Query<UserEntity>(sql).ToList();
            Console.WriteLine("------------------------------------------------\n");
            Console.WriteLine("CODING TRACKER\n");

            foreach (UserEntity userEntity in codingDurations)
            {
                Console.WriteLine($"{userEntity.Id}, {userEntity.StartDate}, {userEntity.EndDate}, {userEntity.Duration}");
            }

            Console.WriteLine("------------------------------------------------\n");
        }

        public UserEntity GetOne(int id)
        {
            string connectionString = @"Data Source = Coding_Tracker.db";
            using var connection = new SqliteConnection(connectionString);

            var sql = @"SELECT * FROM codingTracker where Id = @Id";

            var codingDurations = connection.Query<UserEntity>(sql,new {Id = id}).ToList();

            return codingDurations[0];
        }

        public void Filter(string format, string firstRange, string secondRange, string order)
        {
            string connectionString = @"Data Source = Coding_Tracker.db";
            using var connection = new SqliteConnection(connectionString);

            var sql = @$"SELECT *
                        FROM codingTracker
                        WHERE strftime('{format}', EndDate)
                        BETWEEN '{firstRange}' AND '{secondRange}' ORDER BY EndDate {order}";

            var codingDurations = connection.Query<UserEntity>(sql).ToList();

            foreach (UserEntity userEntity in codingDurations)
            {
                Console.WriteLine($"{userEntity.Id}, {userEntity.StartDate}, {userEntity.EndDate}, {userEntity.Duration}");
            }
            Console.WriteLine("------------------------------------------------\n"); ;

        }

        public void Filter(string format, string value, string order)
        {
            string connectionString = @"Data Source = Coding_Tracker.db";
            using var connection = new SqliteConnection(connectionString);

            var sql = @$"SELECT *
                        FROM codingTracker
                        WHERE strftime('{format}', EndDate) = '{value}' ORDER BY EndDate  {order}";

            var codingDurations = connection.Query<UserEntity>(sql).ToList();

            foreach (UserEntity userEntity in codingDurations)
            {
                Console.WriteLine($"{userEntity.Id}, {userEntity.StartDate}, {userEntity.EndDate}, {userEntity.Duration}");
            }
            Console.WriteLine("------------------------------------------------\n"); ;

        }

        public void Analyze(string format, string value)
        {
            string connectionString = @"Data Source = Coding_Tracker.db";
            using var connection = new SqliteConnection(connectionString);

            var sql = @$"SELECT *
                        FROM codingTracker
                        WHERE strftime('{format}', EndDate) = '{value}'";

            var codingDurations = connection.Query<UserEntity>(sql).ToList();

            TimeSpan duration;
            TimeSpan total = TimeSpan.Zero;


            Console.WriteLine("------------------------------------------------\n");
            foreach (UserEntity userEntity in codingDurations)
            {
                TimeSpan.TryParse(userEntity.Duration, out duration);
                total += duration;
            }

            string totalString = FormatTimeSpan(total);
            TimeSpan average = total / codingDurations.Count;
            string averageString = FormatTimeSpan(average);
            Console.WriteLine(@$"This month you sent a total of {totalString}
            and an Average of {averageString}
            ");
            Console.WriteLine("------------------------------------------------\n");
        }

        public string FormatTimeSpan(TimeSpan duration)
        {
            List<string> parts = new List<string>();
            if (duration.Days > 0)
            {
                parts.Add($"{duration.Days} {(duration.Days == 1 ? "day" : "days")}");
            }
            if (duration.Hours > 0)
            {
                parts.Add($"{duration.Hours} {(duration.Hours == 1 ? "hour" : "hours")}");
            }
            if (duration.Minutes > 0)
            {
                parts.Add($"{duration.Minutes} {(duration.Minutes == 1 ? "minute" : "minutes")}");
            }
            if (duration.Seconds > 0)
            {
                parts.Add($"{duration.Seconds} {(duration.Seconds == 1 ? "second" : "seconds")}");
            }
            return string.Join(", ", parts);
        }


        public async void Delete(int id)
        {
            string connectionString = @"Data Source = Coding_Tracker.db";
            using var connection = new SqliteConnection(connectionString);

            var sql = @"DELETE FROM codingTracker where Id = @Id";

            var affectedRows = await connection.ExecuteAsync(sql, new { Id = id });
            Console.WriteLine($"{affectedRows} row(s) inserted");
        }
    }
}