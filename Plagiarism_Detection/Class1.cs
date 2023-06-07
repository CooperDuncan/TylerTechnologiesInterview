

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

    class Class1
    {
        //Globals
        private static string[] text_doc;
        private static string[] control_doc;
        //variable for hashing, d = number of characters in input alphabet
        public readonly static int d = 256;
        public readonly static int index = 0;
        private static int counter;
        private static EnglishStemmer stemmer = new EnglishStemmer();

        //Dictionary for displaying 


        //path for test docs
        private static string test = @"C:\Temp\source\Test.txt";
        //path for control docs
        private static string[] control = { @"C:\Temp\source\Test.txt", @"C:\Temp\control\Chinese Discourse on 9 Dashed Line.txt", @"C:\Temp\control\NYT South China Sea.txt",
                @"C:\Temp\control\rankin 2014 tc.txt", @"C:\Temp\control\rankin 2016 intro (trimmed).txt" ,
                @"C:\Temp\control\Rocks that Cannot Sustain Human Habitation.txt", @"C:\Temp\control\WApost south china sea.txt"};

        //an array of some common words we can exclude when comparing files
        private static string[] common_words = {"do", "she", "they", "we", "are", "is", "a", "an", "in", "as", "but", "were",
                                                "of", "at", "as", "an", "had", "but", "the", "be", "to", "that", "has", "by", "or",
                                                "have", "I", "it", "for", "not", "on", "with", "he", "and", "its", "been", "all"};

        //an array of common punctuation 
        private static char[] punctuation = { ' ', '\n', '\r', ',', ';', '.', '!', '?', '–', '-', ' ', '"', '\'', '“', '”', '=',
                                                '^', '(', ')', '1', '2', '3', '4', '5', '6', '7', '8', '9', '0', ':', '’', '/'};



        //method to read Files into array
        private static string[] load_file(string path1)
        {
            string[] loaded_array = File.ReadAllLines(path1);
            return loaded_array;
        }



        //method to hash text into integers for RabinKarp
        private static void RabinKarp(string text, string pattern, int q)
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
                //compare hash values of current window
                //we compare one by one only if values match
                if (p == t)
                {
                    //then check characters one by one
                    for (j = 0; j < M; j++)
                    {
                        if (text[i + j] != pattern[j])
                            break;
                    }


                    //if hashes match
                    if (j == M)
                    {
                        counter++;
                        Console.WriteLine($"Pattern that was matched is: {pattern}");
                        Console.WriteLine($"Pattern was found at character index: {i}");
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

        //some strings are poorly broke apart and should be remove, ex: U.S. becomes u s 
        private static string[] remove_singlechars(string[] text)
        {

            string[] res_arr = new string[text.Length];
            int c = 0;

            foreach (string s in text)
            {
                List<string> res = new List<string>();
                string[] t = s.Split();


                foreach (string ss in t)
                {
                    //if string has length of more than 1 AND there is more than one string in the array
                    //this takes care of cases where a word is isolated in an array slot
                    //this can cause unessecary matching

                    if (ss.Length != 1 && t.Length != 1)
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







        //main
        public static void Main(String[] args)
        {
            //just a file compare
            if (FileEquals(test, control[index]))
            {
                Console.WriteLine("Files are an exact match.");
            }
            else
            {
                Console.WriteLine("Files are not an exact match.");
            }

            //prep text doc and control docs
            text_doc = load_file(test);
            text_doc = prep_text(text_doc);

            control_doc = load_file(control[index]);
            control_doc = prep_text(control_doc);

            //for rabinKarp, q must be a prime number
            int q = 101;
            counter = 0;

            //result variables
            float SH = 0;
            float THB = 0;
            float THA = 0;

            //RabinKarp Driver
            //takes the original text and pattern as strings
            //along with our prime number, q
            /*
            for (int i = 0; i < control_doc.Length; i++)
                {
                    string[] splt = control_doc[i].Split();
                    splt = remove_emptystring(splt);
                    THB += splt.Length;

                    for(int j = 0; j < splt.Length; j++)
                    {
                        int indx = RabinKarp(string.Join("", text_doc), splt[j], q);

                        if(indx != -1)
                            Console.WriteLine($"Match was found at character index: {indx}");
                    }
                    
                }

            SH = counter * 2;
      
            for(int i = 0; i < text_doc.Length; i++)
            {
                string[] txt_splt = text_doc[i].Split();
                txt_splt = remove_emptystring(txt_splt);
                THA += txt_splt.Length;
            }
            
            //post test output
            Console.WriteLine("There were " + counter + " pattern matches.");
            float rate_plagiarism = ((SH) / (THA + THB)) * 100;
            Console.WriteLine("The calculated plagiarism rate is " + rate_plagiarism);

    */
            SH = 0;
            THB = 0;
            THA = 0;
            counter = 0;

            test for whole length strings
            for(int i = 0; i < control_doc.Length; i++)
            {
                RabinKarp(string.Join("", text_doc), control_doc[i], q);
            }

            

            SH = counter * 2;
            THB = control_doc.Length;
            THA = text_doc.Length;

            //post test output
            Console.WriteLine("There were " + counter + " pattern matches.");
            float rate_plagiarism = ((SH) / (THA + THB)) * 100;
            Console.WriteLine("The calculated plagiarism rate is " + rate_plagiarism);

        }
    }



}




