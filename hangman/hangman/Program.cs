using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Newtonsoft.Json;
using System.Collections;

namespace hangman
{
    class Program
    {
        static void Main(string[] args)
        {
changCategory:
            string[] files = Directory.GetFiles("../../word/","*.json");
            if (files.Length < 1) { Console.WriteLine("No category found!"); return; };
            Console.WriteLine("Select Category : ");
            for (int i=0;i<files.Length;i++)
            {
                Console.WriteLine("\t[{0}] : {1}",i+1,Path.GetFileNameWithoutExtension(files[i]));
            }
            int selectCat;
    select: bool isInt = int.TryParse(Console.ReadLine(), out selectCat);
            if (isInt)
            {
                if (selectCat < 1 || selectCat > files.Length)
                {
                    Console.WriteLine("Enter only number 1-{0}", files.Length);
                    goto select;
                }
            }
            else
            {
                Console.WriteLine("Enter only integer!");
                goto select;
            }
            string json = File.ReadAllText(files[selectCat-1]);
            bool isCont = false;
            int remainWrong = 10;
            int totalScore = 0;
            int t;
            var alreadyRand = new List<int>();
            try
            {
nextWord:
                var dict = JsonConvert.DeserializeObject<Dictionary<string, string>>(json);
                int totalWord = dict.Count;
                Random rand = new Random();
                do {
                    t = rand.Next(0, totalWord);
                } while (alreadyRand.Contains(t));
                alreadyRand.Add(t);
TryAgain:
                int i = 0;
                foreach (var kv in dict)
                {
                    if (i==t)
                    {
                        String word = kv.Key;
                        String hint = kv.Value;
                        var guessed = new List<char>();
                        var wrongGuessed = new List<char>();
                        Console.WriteLine("\nHint : "+hint);
                        char[] ch = word.ToCharArray();
                        bool[] totalCorrect = new bool[ch.Length];
                        bool retry = true;
                        if(!isCont)remainWrong = 10;
                        if(!isCont)totalScore = 0;
                        while (retry&&remainWrong>0) {
                            int k=0;
                            foreach (char c in ch)
                            {
                                if ((!char.IsLower(c) && !char.IsUpper(c)) || guessed.Contains(c))
                                {
                                    Console.Write(c);
                                    totalCorrect[k] = true;
                                }
                                else
                                {
                                    Console.Write("_ ");
                                    totalCorrect[k] = false;
                                }
                                k++;
                            }
                            Console.Write('\t');
                            Console.Write("totalScore : {0},\tremaining wrong guess : {1}",totalScore,remainWrong);
                            if (wrongGuessed.Count > 0)
                            {
                                String wrgString=",\twrong guessed : ";
                                int j = 0;
                                foreach (var w in wrongGuessed)
                                {
                                    if (j > 0) wrgString += ",";
                                    wrgString += w;
                                    j++;
                                }
                                Console.Write(wrgString);
                            }
                            Console.WriteLine();
                            retry=!(totalCorrect.All(x => x));
                            if (retry)
                            {
                                char letter;
                                Console.Write("> ");
                inputChar :     bool isChar = char.TryParse(Console.ReadLine(),out letter);
                                if (!isChar)
                                {
                                    Console.WriteLine("Enter only character!");
                                    goto inputChar;
                                }
                                if (word.Contains<char>(letter))
                                {
                                    guessed.Add(letter);
                                    totalScore += 5;
                                }
                                else
                                {
                                    wrongGuessed.Add(letter);
                                    remainWrong--;
                                }
                            }
                        }
                        if (remainWrong == 0) Console.WriteLine("You lost!");
                    }
                    i++;
                }
                Console.WriteLine();
                if (remainWrong!=0&&alreadyRand.Count!=totalWord) {
                    Console.Write("[1] NextWord\n[2] TryAgain\n[3] ChangeCategory\nOther keys to Exit\n");
                    char getch = Console.ReadKey().KeyChar;
                    if (getch == '1')
                    {
                        isCont = true;
                        goto nextWord;
                    }
                    else if (getch == '2')
                    {
                        isCont = false;
                        goto TryAgain;
                    }
                    else if (getch == '3') goto changCategory;
                }
                else
                {
                    Console.Write("[1] TryAgain\n[2] ChangeCategory\nOther keys to Exit\n");
                    char getch = Console.ReadKey().KeyChar;
                    if (getch == '1')
                    {
                        isCont = false;
                        goto TryAgain;
                    }
                    else if (getch == '2') goto changCategory;
                }
            }
            catch
            {
                Console.WriteLine("Error in file : {0}",Path.GetFileNameWithoutExtension(files[selectCat-1]));
                if (json == "") Console.WriteLine("file has no content.");
            }
        }
    }
}
