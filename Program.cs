using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

namespace LogSearchApp
{
    class Program
    {
        //change to local file path
        private const string PATH_TO_LOG_FILE = "C:/LogFile/log20201104.txt";

        private const int NUM_OF_WHITESPACES_BETWEEN_DATABLOCKS = 1;
        private const int NUM_OF_CHARACTERS_TO_SKIP = 1;
        private static Stopwatch _stopwatch;

        static void Main(string[] args)
        {
            WelcomeMessage();
            var logFileLinesList = LoadAndReadLogFile();

            while(true)
            {
                SearchTypeMessage();
                var userTypeInput = GetTypeInput();

                SearchParameterMessage();
                var userSearchParameterInput = GetSearchParameterInput();

                var resultsList = SearchLogFile(userTypeInput, userSearchParameterInput, logFileLinesList);
                DisplayResults(resultsList, logFileLinesList);
            }
        }

        /// <summary>
        /// Writes the application welcome message to the console.
        /// </summary>
        private static void WelcomeMessage()
        {
            Console.WriteLine("Welcome to the Log search application!");
        }

        /// <summary>
        /// Loads the log file from the file system and reads it from start to end.
        /// </summary>
        /// <returns> File contents in a form of a list of strings, each entry representing one line in the file. </returns>
        private static List<string> LoadAndReadLogFile()
        {
            List<string> logFileLines = new List<string>();

            StreamReader logFileStreamReader = File.OpenText(PATH_TO_LOG_FILE);
            while(!logFileStreamReader.EndOfStream)
            {
                logFileLines.Add(logFileStreamReader.ReadLine());
            }

            return logFileLines;
        }

        /// <summary>
        /// Writes the search type message to the console.
        /// </summary>
        private static void SearchTypeMessage()
        {
            Console.WriteLine("\nInput search type. Possible options: DATE, TYPE, ID, MODULE");
        }

        /// <summary>
        /// Writes the search parameter message to the console.
        /// </summary>
        private static void SearchParameterMessage()
        {
            Console.WriteLine("\nInput search parameter (case sensitive). This can be anything you are searchhing for in a certain format.");
        }

        /// <summary>
        /// Gets the type input.
        /// </summary>
        /// <returns> Type input in a string. </returns>
        private static string GetTypeInput()
        {
            while(true)
            {
                var searchType = Console.ReadLine();

                if (!(searchType.ToLower().Equals("date")
                    || searchType.ToLower().Equals("type")
                    || searchType.ToLower().Equals("id")
                    || searchType.ToLower().Equals("module")))
                {
                    Console.WriteLine("\nWrong search type. Please input: DATE, TYPE, ID or MODULE.");
                }
                else
                {
                    return searchType;
                }
            }
        }

        /// <summary>
        /// Gets the search parameter input.
        /// </summary>
        /// <returns> Search parameter in a string. </returns>
        private static string GetSearchParameterInput()
        {
            var searchParameter = Console.ReadLine();
            return searchParameter;
        }

        /// <summary>
        /// Displays the search results.
        /// </summary>
        /// <param name="resultsList"> List of the search results that is going to be displayed. </param>
        /// <param name="logFileLinesList"> List of log file lines. </param>
        private static void DisplayResults(List<string> resultsList, List<string> logFileLinesList)
        {
            Console.WriteLine("\n\nSearch results:");
            foreach(var result in resultsList)
            {
                Console.WriteLine(result);
            }

            Console.WriteLine("\n---------------------------------------------------------------");
            Console.WriteLine($"Number of matches found: {resultsList.Count} out of {logFileLinesList.Count}");
            Console.WriteLine($"Time elapsed: {_stopwatch.Elapsed}");
            Console.WriteLine("---------------------------------------------------------------");
        }

        /// <summary>
        /// Function that searches the loaded log file line by line.
        /// </summary>
        /// <param name="userTypeInput"> Type of search. </param>
        /// <param name="userSearchParameterInput"> Search parameter, string that is being compared to provide search results. </param>
        /// <param name="logFileLinesList"> Log file lines list, each entry is a line in the log file. </param>
        /// <returns> Search results list, 1 result = 1 line in the log file. </returns>
        private static List<string> SearchLogFile(string userTypeInput, string userSearchParameterInput, List<string> logFileLinesList)
        {
            _stopwatch = new Stopwatch();
            _stopwatch.Start();

            List<string> resultsList = new List<string>();
            string resultString = "";
            string[] lineSplit;

            foreach(var line in logFileLinesList)
            {
                switch (userTypeInput.ToLower())
                {
                    case "date":
                        resultString = line.Substring(0, line.IndexOf("[") - NUM_OF_WHITESPACES_BETWEEN_DATABLOCKS);
                        break;
                    case "type":
                        lineSplit = line.Split(']');
                        resultString = lineSplit[0].Substring(line.IndexOf("[") + NUM_OF_CHARACTERS_TO_SKIP);
                        break;
                    case "id":
                        lineSplit = line.Split('}');
                        resultString = lineSplit[0].Substring(line.IndexOf("{") + NUM_OF_CHARACTERS_TO_SKIP);
                        break;
                    case "module":
                        lineSplit = line.Split(']');
                        resultString = lineSplit[1].Substring(line.IndexOf("[") + NUM_OF_CHARACTERS_TO_SKIP);
                        break;
                }

                if(resultString.Contains(userSearchParameterInput))
                {
                    resultsList.Add(line);
                }
            }            

            _stopwatch.Stop();

            return resultsList;
        }
    }
}
