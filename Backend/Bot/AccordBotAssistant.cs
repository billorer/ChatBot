using Accord.MachineLearning.VectorMachines;
using Accord.MachineLearning.VectorMachines.Learning;

using Backend.Public;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;

namespace Backend.Bot
{
    public class AccordBotAssistant : IBotAssistant
    {
        /// <summary>
        /// Document vocabulary, containing each word's IDF value.
        /// </summary>
        private static Dictionary<string, double> _vocabularyIDF;

        private List<string> questions;
        private List<string> answers;
        private List<string> mainVocabulary;
        private List<List<string>> databaseQuestionsWithWords;

        private List<string> currentQuestion;
        private List<string> currentQuestionsVocabulary;
        private List<List<string>> currentQuestionsWords;

        /// <summary>
        /// This variables is used to check if the machine has to learn or it loaded the data.
        /// </summary>
        public bool machineLearned;
        private MulticlassSupportVectorMachine machine;

        private static AccordBotAssistant instance = null;

        public static AccordBotAssistant Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new AccordBotAssistant();
                }

                return instance;
            }
        }

        private AccordBotAssistant()
        {
            machineLearned = false;
            _vocabularyIDF = new Dictionary<string, double>();
        }

        public string Answer(string message)
        {
            PrepareTheCurrentQuestion(message);

            if(File.Exists(PublicFunctionsVariables.answersDataPath) && File.Exists(PublicFunctionsVariables.questionsDataPath) && machine == null)
            {
                double[][] databaseInputsTFIDF = GetNormalizedTFIDFInputsFromFiles(_vocabularyIDF);

                machine = GetSVM(databaseInputsTFIDF);

                SaveMandatoryDataIntoFiles();

                machineLearned = true;
            }

            double[][] currentQuestionInputsTFIDF = CreateNormalizedTFIDFInputs(_vocabularyIDF, currentQuestionsVocabulary, currentQuestionsWords);

            int[] result = machine.Decide(currentQuestionInputsTFIDF);

            return answers[result[0]];
        }

        /// <summary>
        /// This Function proceeds the current question
        /// </summary>
        /// <param name="message"></param>
        private void PrepareTheCurrentQuestion(string message)
        {
            currentQuestion = new List<string>();
            currentQuestion.Add(message);
            currentQuestionsVocabulary = GetVocabulary(currentQuestion);
            currentQuestionsWords = GetStemmedDocs(currentQuestion);
        }

        /// <summary>
        /// This function loads the adequate files 
        /// </summary>
        public string LoadMandatoryDataFromFiles()
        {
            if (File.Exists(PublicFunctionsVariables.machineDataPath) && File.Exists(PublicFunctionsVariables.answersDataPath) &&
                File.Exists(PublicFunctionsVariables.vocabularyIDFDataPath) && File.Exists(PublicFunctionsVariables.questionsDataPath))
            {
                machine = MulticlassSupportVectorMachine.Load(PublicFunctionsVariables.machineDataPath);

                answers = GetAnswersFromFile();
                questions = GetQuestionsFromFile();

                _vocabularyIDF = XElement.Parse(File.ReadAllText(PublicFunctionsVariables.vocabularyIDFDataPath))
                .Elements()
                .ToDictionary(k => k.Name.ToString(), v => Convert.ToDouble(v.Value.ToString()));

                machineLearned = true;

                return "ok";
            }
            else
            {
                return "The mandatory files do not exist or they cannot be found in the directory: ex. " + PublicFunctionsVariables.machineDataPath + " " +
                    PublicFunctionsVariables.answersDataPath + " " + PublicFunctionsVariables.vocabularyIDFDataPath + " " + PublicFunctionsVariables.questionsDataPath;
            }
        }

        /// <summary>
        /// This function saves the SVM object and the vocabulary with is TF*IDF values into the adequate files
        /// </summary>
        private void SaveMandatoryDataIntoFiles()
        {
            machine.Save(PublicFunctionsVariables.machineDataPath);
            new XElement("vocabulary", _vocabularyIDF.Select(kv => new XElement(kv.Key, kv.Value)))
            .Save(PublicFunctionsVariables.vocabularyIDFDataPath, SaveOptions.OmitDuplicateNamespaces);
        }

        /// <summary>
        /// Apply TF*IDF to the documents and get the resulting vectors.
        /// </summary>
        /// <param name="vocabulary"></param>
        /// <param name="stemmedDocs"></param>
        /// <returns></returns>
        private double[][] CreateNormalizedTFIDFInputs(Dictionary<string, double> _vocabularyIDF, List<string> vocabulary, List<List<string>> stemmedDocs)
        {
            double[][] inputs = Transform(_vocabularyIDF, vocabulary, stemmedDocs);
            return Normalize(inputs);
        }

        /// <summary>
        /// This function reads in the questions and answers
        /// It creates the vocabulary from them
        /// It creates a list of words from questions
        /// It creates the normalized TFIDF inputs and returns them
        /// </summary>
        /// <returns></returns>
        private double[][] GetNormalizedTFIDFInputsFromFiles(Dictionary<string, double> _vocabularyIDF)
        {
            questions = GetQuestionsFromFile();
            answers = GetAnswersFromFile();

            mainVocabulary = GetVocabulary(questions);
            databaseQuestionsWithWords = GetStemmedDocs(questions);

            return CreateNormalizedTFIDFInputs(_vocabularyIDF, mainVocabulary, databaseQuestionsWithWords);
        }

        /// <summary>
        /// This function reads the questions from the XML file and returns them as a list
        /// </summary>
        /// <returns></returns>
        private List<string> GetQuestionsFromFile()
        {
            return XDocument.Load(PublicFunctionsVariables.questionsDataPath).Root.Elements("question")
                                       .Select(element => element.Value)
                                       .ToList();
        }

        /// <summary>
        /// This function reads the answers from the XML file and returns them as a list
        /// </summary>
        /// <returns></returns>
        private List<string> GetAnswersFromFile()
        {
            return XDocument.Load(PublicFunctionsVariables.answersDataPath).Root.Elements("answer")
                           .Select(element => element.Value)
                           .ToList();
        }

        /// <summary>
        /// This function creates an SVM objects from the given inputs and outputs and returns the object
        /// It also sets the learning algorithm for the SVM object and also teaches the object
        /// </summary>
        /// <param name="inputs"></param>
        /// <returns></returns>
        private MulticlassSupportVectorMachine GetSVM(double[][] inputs)
        {
            MulticlassSupportVectorMachine machine = new MulticlassSupportVectorMachine(inputs[0].Length,
                new Accord.Statistics.Kernels.Linear(), inputs.Length);

            int[] outputs = new int[inputs.Length];
            for (int i = 0; i < inputs.Length; i++)
            {
                outputs[i] = i;
            }

            var teacher = new MulticlassSupportVectorLearning(machine, inputs, outputs);

            teacher.Algorithm = (svm, classInputs, classOutputs, i, j)
              => new SequentialMinimalOptimization(svm, classInputs, classOutputs);

            double error = teacher.Run();
            return machine;
        }

        /// <summary>
        /// This function normalizes the vectors it gets and returns them in a matrix
        /// </summary>
        /// <param name="vectors"></param>
        /// <returns></returns>
        public static double[][] Normalize(double[][] vectors)
        {
            // Normalize the vectors using L2-Norm.
            List<double[]> normalizedVectors = new List<double[]>();
            foreach (var vector in vectors)
            {
                var normalized = Normalize(vector);
                normalizedVectors.Add(normalized);
            }

            return normalizedVectors.ToArray();
        }

        /// <summary>
        /// This function normalizes the vector it gets
        /// It basically creates a sum of each elements square
        /// Then it sqrt-s it
        /// And returns the divided part of the value itself
        /// L2-norm: Xi = Xi / Sqrt(X0^2 + X1^2 + .. + Xn^2)
        /// </summary>
        /// <param name="vector"></param>
        /// <returns></returns>
        public static double[] Normalize(double[] vector)
        {
            List<double> result = new List<double>();

            double sumSquared = 0;
            foreach (var value in vector)
            {
                sumSquared += value * value;
            }

            double SqrtSumSquared = Math.Sqrt(sumSquared);

            foreach (var value in vector)
            {                
                result.Add(value / SqrtSumSquared);
            }

            return result.ToArray();
        }

        /// <summary>
        /// This function gets a list of the questions and splits them into words
        /// It creates a new list from these words and returns this list -> Vocabulary
        /// </summary>
        /// <param name="questions"></param>
        /// <returns></returns>
        private List<string> GetVocabulary(List<string> questions)
        {
            List<string> voc = new List<string>();
            foreach (string question in questions)
            {
                string[] words = question.Split(' ');
                for(int i = 0; i < words.Length; i++)
                {
                    string word = PublicFunctionsVariables.Tokenize(words[i]);
                    if (word != "")
                    {
                        voc.Add(word.ToLower());
                    }                  
                }
            }
            return voc;
        }

        /// <summary>
        /// This function creates a list similar to the Vocabulary but here each instance contains the words of its question
        /// </summary>
        /// <param name="questions"></param>
        /// <returns></returns>
        private List<List<string>> GetStemmedDocs(List<string> questions)
        {
            List<List<string>> stemmedDocs = new List<List<string>>();
            
            foreach (string question in questions)
            {
                List<string> stemmedDoc = new List<string>();
                string[] words = question.Split(' ');
                for (int i = 0; i < words.Length; i++)
                {
                    string word = PublicFunctionsVariables.Tokenize(words[i]);
                    stemmedDoc.Add(word.ToLower());
                }
                stemmedDocs.Add(stemmedDoc);
            }

            return stemmedDocs;
        }

        /// <summary>
        /// This function goes through the terms in the vocabulary and creates a list that contains the IDF value of each term
        /// Then returns the value of the TransformToTFIDFVectors function
        /// </summary>
        /// <param name="_vocabularyIDF">The words IDF value</param>
        /// <param name="vocabulary">The words used in the whole document</param>
        /// <param name="stemmedDocs">A list of questions, which contains the question as a list of words</param>
        /// <returns></returns>
        public static double[][] Transform(Dictionary<string, double> _vocabularyIDF, List<string> vocabulary, List<List<string>> stemmedDocs)
        {
            if (_vocabularyIDF.Count == 0)
            {
                // Calculate the IDF for each vocabulary term.
                foreach (var term in vocabulary)
                {
                    // Goes through all the stemmedDocs and if theres a list of string that has the current term, it counts
                    double numberOfDocsContainingTerm = stemmedDocs.Where(d => d.Contains(term)).Count();
                    // Calculating the IDF value for the term
                    _vocabularyIDF[term] = Math.Log((double)stemmedDocs.Count / ((double)1 + numberOfDocsContainingTerm));
                }
            }

            // Transform each document into a vector of tfidf values.
            return TransformToTFIDFVectors(stemmedDocs, _vocabularyIDF);
        }

        /// <summary>
        /// This function goes through all the documents(sentences) in the stemmedDocs list
        /// Then it goes through the values of IDF of each term from the vocabulary
        /// It counts the TF, how many times the term appears in the document(sentence)
        /// It counts the TFIDF value, saves it in a list and then returns the list as an array
        /// </summary>
        /// <param name="stemmedDocs"></param>
        /// <param name="vocabularyIDF"></param>
        /// <returns></returns>
        private static double[][] TransformToTFIDFVectors(List<List<string>> stemmedDocs, Dictionary<string, double> vocabularyIDF)
        {
            // Transform each document into a vector of tfidf values.
            List<List<double>> vectors = new List<List<double>>();
            foreach (var doc in stemmedDocs)
            {
                List<double> vector = new List<double>();

                foreach (var vocab in vocabularyIDF)
                {
                    // Term frequency = count how many times the term appears in this document.
                    double tf = doc.Where(d => d == vocab.Key).Count();
                    double tfidf = tf * vocab.Value;

                    vector.Add(tfidf);
                }
                vectors.Add(vector);
            }

            return vectors.Select(v => v.ToArray()).ToArray();
        }
    }
}