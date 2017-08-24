using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace Backend.Public
{
    public class PublicFunctionsVariables
    {
        public static string machineDataPath = @"c:\machineData.dat";
        public static string answersDataPath = @"c:\answers.xml";
        public static string questionsDataPath = @"c:\questions.xml";
        public static string vocabularyIDFDataPath = @"c:\vocabularyIDF.xml";
        public static string wordDocumentImagesFilePath = @"c:\WordDocImages\img_";

        /// <summary>
        /// This function read a file given as parameter and returns the file's content as a list
        /// </summary>
        /// <param name="filename"></param>
        /// <returns></returns>
        public static List<string> ReadFile(string filename)
        {
            var list = new List<string>();
            var fileStream = new FileStream(filename, FileMode.Open, FileAccess.Read);
            using (var streamReader = new StreamReader(fileStream, Encoding.UTF8))
            {
                string line;
                while ((line = streamReader.ReadLine()) != null)
                {
                    if (line != "")
                        list.Add(line);
                }
            }
            return list;
        }

        /// <summary>
        /// This function gets a string and removes the inproper characters
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public static string Tokenize(string text)
        {
            // Strip numbers.
            text = Regex.Replace(text, "[0-9]+", "number");

            // Tokenize and also get rid of any punctuation
            return text.Split(" @$/#.-:&*+=[]?!(){},''\">_<;%\\".ToCharArray())[0];
        }
    }
}