

namespace Plagiarism_Detection
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Data;
    using System.Drawing;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Windows.Forms;
    using System.IO;
    using System.Collections;
    using Iveonik.Stemmers;

    class Character_Based
    {
        //Globals
        private static string[] text_doc;
        private static string[] control_doc;
        //variable for hashing, d = number of characters in input alphabet
        public readonly static int d = 256;
        public readonly static int index = 1;
        private static int current_index = 0;
        private static int counter;
        private static EnglishStemmer stemmer = new EnglishStemmer();

        private static Dictionary<string, List<int>> dict = new Dictionary<string, List<int>>();
        private static Dictionary<string, List<int>> matches = new Dictionary<string, List<int>>();
        private static List<Result> results = new List<Result>();
        public class Result{
            private string pattern;
            private int userdoc_match_start;
            private int controldoc_match_start;
            private int matchLength;

            public Result(string s, int ui, int ci, int ml)
            {
                pattern = s;
                userdoc_match_start = ui;
                controldoc_match_start = ci;
                matchLength = ml;
            }

            public string getPattern()
            {
                return pattern;
            }

            public int getUserIndex()
            {
                return userdoc_match_start;
            }

            public int getControlIndex()
            {
                return controldoc_match_start;
            }

            public int getMatchLength()
            {
                return matchLength;
            }
        }

        public static List<Result> compareDocuments(string user, string control)
        {
            string[] userDoc = load_array(user);
            string[] controlDoc = load_array(control);
            userDoc = prep_text(userDoc);
            controlDoc = prep_text(controlDoc);

            //run it through the algorthim
            runTest(userDoc, controlDoc);

            return results;
        }

        public static List<Result> comparePaths(string pathuser, string pathcontrol)
        {
            text_doc = new string[0];
            text_doc = load_file(pathuser);
            control_doc = load_file(pathcontrol);

            text_doc = prep_text(text_doc);
            control_doc = prep_text(control_doc);

            runTest(text_doc, control_doc);
            return results;
        }


        //path for test docs
        private static string test = @"C:\Temp\source\Test.txt";
        //path for control docs
        private static string[] control = { @"C:\Temp\source\Test.txt", @"C:\Temp\control\control1.txt", @"C:\Temp\control\NYT South China Sea.txt",
                @"C:\Temp\control\rankin 2014 tc.txt", @"C:\Temp\control\rankin 2016 intro (trimmed).txt" ,
                @"C:\Temp\control\Rocks that Cannot Sustain Human Habitation.txt", @"C:\Temp\control\WApost south china sea.txt"};

        //an array of some common words we can exclude when comparing files
        private static string[] common_words = {"do", "she", "they", "we", "are", "is", "a", "an", "in", "as", "but", "were",
                                                "of", "at", "as", "an", "had", "but", "the", "be", "to", "that", "has", "by", "or",
                                                "have", "I", "it", "for", "not", "on", "with", "he", "and", "its", "been", "all"};

        //an array of common punctuation 
        private static char[] punctuation = { ' ', '\n', '\r', ',', ';', '.', '!', '?', '–', '-', ' ', '"', '\'', '“', '”', '=',
                                                '^', '(', ')', '1', '2', '3', '4', '5', '6', '7', '8', '9', '0', ':', '’', '/'};


        private static string t1 = "I had seen little of Holmes lately. My marriage\n" +
                            "had drifted us away from each other.My own\n" +
                            "complete happiness, and the home-centred interests which rise up around the man who first finds\n" +
                            "himself master of his own establishment, were sufficient to absorb all my attention, while Holmes,\n" +
                            "who loathed every form of society with his whole\n" +
                            "That, doting on his own obsequious bondage,\n" +
                            "Wears out his time, much like his master's ass,\n" +
                            "For nought but provender, and when he's old, cashier'd\n" +
                            "Hence, you see, my double deduction\n" +
                            "that you had been out in vile weather, and that you\n" +
                            "had a particularly malignant boot-slitting specimen\n" +
                            "of the London slavey\n" +
                            "And, throwing but shows of service on their lords,\n" +
                            "Do well thrive by them and when they have lined\n" +
                            "their coats\n";

        private static  string c1 = "I follow him to serve my turn upon him:\n" +
                           "We cannot all be masters, nor all masters\n" +
                           "Cannot be truly follow'd. You shall mark\n" +
                           "Many a duteous and knee-crooking knave,\n" +
                           "That, doting on his own obsequious bondage,\n" +
                           "Wears out his time, much like his master's ass,\n" +
                           "For nought but provender, and when he's old, cashier'd\n" +
                           "Whip me such honest knaves. Others there are\n" +
                           "Who, trimm'd in forms and visages of duty,\n" +
                           "Keep yet their hearts attending on themselves,\n" +
                           "And, throwing but shows of service on their lords,\n" +
                           "Do well thrive by them and when they have lined\n" +
                           "their coats\n" +
                           "Do themselves homage: these fellows have some soul;\n" +
                           "And such a one do I profess myself. For, sir,\n" +
                           "It is as sure as you are Roderigo,\n" +
                           "Were I the Moor, I would not be Iago:\n" +
                           "In following him, I follow but myself;\n" +
                           "Heaven is my judge, not I for love and duty,\n";


        //method to read Files into array
        private static string[] load_file(string path1)
        {
            string[] loaded_array = File.ReadAllLines(path1);
            return loaded_array;
        }

        private static string[] load_array(string doc)
        {
            string[] loaded_doc = doc.Split('\n');
            return loaded_doc;
        }



        //method to hash text into integers for RabinKarp
        private static int RabinKarp(string text, string pattern, int q)
        {
            int M = pattern.Length;
            int N = text.Length;
            int i, j;
            int p = 0;  //hash value for pattern
            int t = 0;  //hash value for text
            int h = 1;

            //value of h is "pow(d, M-1)%q"
            for (i = 0; i < M - 1; i++)
            {
                h = (h * d) % q;
            }

            //calc hash value of pattern and first window of text
            for (i = 0; i < M; i++)
            {
                p = (d * p + pattern[i]) % q;
                t = (d * t + text[i]) % q;
            }

            //slide pattern over by one
            for (i = 0; i <= N - M; i++)
            {
                current_index++;
                //compare hash values of current window
                //we compare one by one only if values match
                if (p == t)
                {
                    //then check characters one by one
                    for (j = 0; j < M; j++)
                    {
                        
                        if (text[i + j] != pattern[j])
                        {
                            break;
                        }

                    }


                    //if hashes match
                    if (j == M)
                    {
                        counter++;
                        int temp = current_index;
                        current_index = 0;
                        return temp - 1;
                    }
                }

                //calc hash value for next window of text
                //remove leading digit and add trailing digit
                if (i < N - M)
                {
                    t = (d * (t - text[i] * h) + text[i + M]) % q;

                    //t could be negative, make sure it is positive
                    if (t < 0)
                        t = (t + q);
                }
            }

          
            current_index = 0;
            return -1;

        }


        //file compare
        private static bool FileEquals(string path1, string path2)
        {
            byte[] file1 = File.ReadAllBytes(path1);
            byte[] file2 = File.ReadAllBytes(path2);

            //if files are not same length, no need to continue
            if (file1.Length == file2.Length)
            {
                for (int i = 0; i < file1.Length; i++)
                {
                    if (file1[i] != file2[i])
                    {
                        //files are not exact match, return false
                        return false;
                    }

                }
                return true;
            }

            //satisfy condition
            return false;
        }



        //removes stop words to make more accurate comparisons
        private static string[] remove_common_words(string[] text)
        {
            //convert all strings to lowercase
            text = Array.ConvertAll(text, d => d.ToLower());

            //removes all common words and punctuation
            for (int i = 0; i < text.Length; i++)
            {
                text[i] = string.Join(" ", text[i].Split(punctuation).Except(common_words)).Trim();
            }

            return text;
        }



        //removes all empty strings from array
        private static string[] remove_emptystring(string[] text)
        {
            text = text.Where(x => !string.IsNullOrEmpty(x)).ToArray();
            return text;
        }

        //some strings are poorly broke apart and should be removed, ex: U.S. becomes u s 
        private static string[] remove_singlechars(string[] text)
        {
            
            string[] res_arr = new string[text.Length];
            int c = 0;

            foreach(string s in text)
            {
                List<string> res = new List<string>();
                string[] t = s.Split();

                
                foreach(string ss in t)
                {
                    //if string has length of more than 1 AND there is more than one string in the array
                    //this takes care of cases where a word is isolated in an array slot
                    //this can cause unessecary matching

                    if(ss.Length != 1 && t.Length != 1)
                    {
                      res.Add(ss);
                    }
                }

                res_arr[c] = string.Join(" ", res);
                c++;
                
            }

            return res_arr;
        }

        private static string[] stemstring(string[] text)
        {
            //stem all strings
            int stem_counter = 0;
            foreach (string s in text)
            {
                string[] stemmed = s.Split();
                List<string> list = new List<string>();

                foreach (string ss in stemmed)
                {
                    list.Add(stemmer.Stem(ss));
                }

                text[stem_counter] = string.Join(" ", list);
                stem_counter++;
            }

            return text;
        }

        private static string[] prep_text(string[] text)
        {
            text = remove_common_words(text);
            text = remove_singlechars(text);
            text = remove_emptystring(text);
            text = stemstring(text);

            return text;
        }

        private static int get_index(string pattern)
        {
            List<int> temp = matches[pattern];
            return temp.ElementAt(0);
        }

        private static void runTest(string[] user, string[] control)
        {
            //for rabinKarp, q must be a prime number
            int q = 101;
            counter = 0;

            //result variables
            float SH = 0;
            float THB = 0;
            float THA = 0;

            counter = 0;

            //test for whole length strings
            for (int i = 0; i < user.Length; i++)
            {
                int res = RabinKarp(string.Join("", control),user[i], q);

                if (res != -1)
                {
                    int user_index = 1;
                    foreach(string s in user)
                    {
                        if (s.CompareTo(user[i]) == 0)
                            break;
                        foreach (char c in s)
                            user_index++;
                    }
                    Result r = new Result(user[i], user_index, res, user[i].Length);
                    results.Add(r);
  
                }
            }

            SH = counter * 2;
            THB = control.Length;
            THA = user.Length;

            //post test output
            Console.WriteLine("There were " + counter + " pattern matches.");

            float rate_plagiarism = ((SH) / (THA + THB)) * 100;
            Console.WriteLine("The calculated plagiarism rate is " + rate_plagiarism);
        }

        private static List<Result> joinMatches(List<Result> r)
        {
            int next_match_index = 0;
            int prev_match_index = 0;
            int control_index = 0;
            int offset = 0;
            String pattern = "";
            List<Result> newRes = new List<Result>();

            foreach(Result res in r)
            {
                //for first res
                if(next_match_index == 0)
                {
                    next_match_index += res.getMatchLength() + res.getUserIndex();
                    pattern += " " + res.getPattern();
                    prev_match_index = res.getUserIndex();
                    control_index = res.getControlIndex();
                    offset++;
                }
                //for every res after      
                else
                {
                    //if is continuos match, extend match pos and append pattern
                    if(next_match_index == res.getUserIndex())
                    {
                        //then pattern is a consecutive match, add pattern to previous pattern
                        pattern += " " + res.getPattern();
                        next_match_index += res.getMatchLength();
                        offset++;
                    }

                    //next pattern is not consecutive with previous pattern, 
                    else
                    {
                        Result newR = new Result(pattern, prev_match_index, control_index, pattern.Length - offset);
                        newRes.Add(newR);
                        next_match_index = res.getUserIndex() + res.getMatchLength();
                        prev_match_index = res.getUserIndex();
                        control_index = res.getControlIndex();
                        pattern = res.getPattern();
                        offset = 0;
                    }
                }
            }

            Result leftover = new Result(pattern, prev_match_index, control_index, pattern.Length - offset);
            newRes.Add(leftover);

            return newRes;
        }

        //main
        public static void Main(String[] args)
        {

            //just a file compare
           /* if (FileEquals(test, control[index]))
            {
                Console.WriteLine("Files are an exact match.");
            }
            else
            {
                Console.WriteLine("Files are not an exact match.");
            }
            */
            compareDocuments(t1, c1);

            Console.WriteLine("Initial Results: ");
            foreach(Result r in results)
            {
                Console.WriteLine("Pattern: " + r.getPattern() + " {user_doc index: " + r.getUserIndex() + ", length: " + r.getMatchLength() + "}, was matched at control index {" + r.getControlIndex() + "}");
            }

            results = joinMatches(results);

            Console.WriteLine();
            Console.WriteLine("Joined matches: " + results.Count);
            foreach(Result r in results)
            {
                Console.WriteLine("Pattern: " + r.getPattern() + "\n {user_doc index: " + r.getUserIndex() + ", length: " + r.getMatchLength() + "}, was matched at control index {" + r.getControlIndex() + "}");
            }

            Console.WriteLine();
            results = new List<Result>();
            //comparePaths(test, control[0]);

            Console.WriteLine("Initial Results: ");
            foreach (Result r in results)
            {
                Console.WriteLine("Pattern: " + r.getPattern() + " {user_doc index: " + r.getUserIndex() + ", length: " + r.getMatchLength() + "}, was matched at control index {" + r.getControlIndex() + "}");
            }

            results = joinMatches(results);

            Console.WriteLine();

           /* foreach (Result r in results)
            {
                Console.WriteLine("Pattern: " + r.getPattern() + " {user_doc index: " + r.getUserIndex() + ", length: " + r.getMatchLength() + "}, was matched at control index {" + r.getControlIndex() + "}");
            }
            */
        }


        }



    }



    
