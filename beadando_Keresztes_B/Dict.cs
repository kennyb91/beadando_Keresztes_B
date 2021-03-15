using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace beadando_Keresztes_B
{
    public class Dict
    {

        static Dictionary<string, int> dictionary = new Dictionary<string, int>();
        static ConcurrentDictionary<string, int> concdictionary = new ConcurrentDictionary<string, int>();
        private static readonly IEnumerable<object> keysArray;

        static void Main(string[] args)
        {

            Thread thread1 = new Thread(new ThreadStart(Iterate));
            Thread thread2 = new Thread(new ThreadStart(Iterate));

            thread1.Start();
            thread2.Start();
            thread1.Join();
            thread2.Join();

            Console.WriteLine("Summ: {0}", dictionary.Values.Sum() + "\n");

            thread1 = new Thread(new ThreadStart(Iterate2));
            thread2 = new Thread(new ThreadStart(Iterate2));

            thread1.Start();
            thread2.Start();
            thread1.Join();
            thread2.Join();

            Console.WriteLine("Summ: {0}", concdictionary.Values.Sum() + "\n");

            //elem hozzáadás
            concdictionary.TryAdd("21", 21);

            //elem értékének kiírása

            int added;
            concdictionary.TryGetValue("21", out added);
            Console.WriteLine("A hozzáadott elem értéke: " + added + "\n");

            //foreach iteráció a Key - Value párok kiírására 
            Console.WriteLine("Az eddig rögzített Key-Value Párok: \n");
            foreach (var item in concdictionary)
            {
                Console.WriteLine(item.Key + "-" + item.Value + "\n");
            }

            //TryRemove egy elem törlésére

            concdictionary.TryAdd("torlesre", 22);
            int removedItem;
            concdictionary.TryRemove("torlesre", out removedItem);
            Console.WriteLine("A törölt Value :" + removedItem + "\n");
            Console.WriteLine("ConcurentDictionary elemei: \n");
            foreach (var item in concdictionary)
            {
                Console.WriteLine(item.Key + " - " + item.Value);
            }

            //Dictionary másolása egyéb gyűjtőbe (Array)
            ArrayGenerator();

            //Egy Timer task, amely első Thread lefutásakor új elemeket tölt fel, a másik task pedig kiírja a Key-Value párokat, Sleep Timer-t használva. 
            Console.WriteLine("Time kiíratás : \n");
            Timer();

            //TryUpdate metódus az adatok módosítására adott kulcs alapján
            concdictionary.TryAdd("modosit", 124);
            int newItem;
            bool returnTrue = concdictionary.TryUpdate("modosit", 123, 124);
            concdictionary.TryGetValue("modosit", out newItem);
            Console.WriteLine("A módosítás után érték: " + newItem + "\n");

            Eredmenyek.Kiir();
            Program.Form();

            Program.FileWrite wr = new Program.FileWrite();
            wr.WriteData();

            Program.FileWrite rd = new Program.FileWrite();
            rd.ReadData();
            Console.WriteLine("Sleep timer 5000 millisec indítva.");
            Thread.Sleep(5000);

        }


        public static void Iterate()
        {

            for (int i = 0; i < 21; i++)
            {
                try
                {
                    dictionary.Add(i.ToString(), i);
                }
                catch (ArgumentException)
                {
                    Console.WriteLine("{0}. key már szerepel a dictionaryben. ", i + "\n");
                }
            }
        }
        public static void Iterate2()
        {
            for (int i = 0; i < 21; i++)
            {
                concdictionary.TryAdd(i.ToString(), i);
            }
        }
        static void Timer()
        {
            Task t1 = Task.Factory.StartNew(() =>
            {
                for (int i = 0; i < 10; ++i)
                {
                    dictionary.TryAdd(i.ToString(), i);
                    Thread.Sleep(100);
                }
            });
            Task t2 = Task.Factory.StartNew(() =>
            {
                Thread.Sleep(300);
                foreach (var item in dictionary)
                {
                    Console.WriteLine(item.Key + "-" + item.Value);
                    Thread.Sleep(150);
                }
            });
            try
            {
                Task.WaitAll(t1, t2);
            }
            catch (AggregateException ex)
            {
                Console.WriteLine(ex.Flatten().Message);
            }


        }


        public static void ArrayGenerator()
        {
            string[] keysArray = concdictionary.Keys.ToArray();
            Eredmenyek.eredmenyekArray = keysArray;
            Console.WriteLine("A tömb elemei: \n");
            foreach (var item in keysArray)
            {
                Console.WriteLine(item.ToString() + "\n");
            }


        }
    }
    public static class Eredmenyek
    {
        public static string[] eredmenyekArray;
        public static void Kiir()
        {
            Console.WriteLine("eredmenyekArray elemei: \n");
            foreach (var item in eredmenyekArray)
            {
                Console.WriteLine(item.ToString() + "\n");
            }
        }
    }
    public static class Program
    {
        static string[] keyElements = Eredmenyek.eredmenyekArray;


        public static void Form()
        {
            System.IO.File.WriteAllLines(@"C:\\Users\\keres\\Desktop\\beadando\\form.txt", keyElements);

        }
        public class FileWrite
        {
            public void WriteData()
            {
                FileStream fs = new FileStream(@"C:\\Users\\keres\\Desktop\\beadando\\readwrite.txt", FileMode.Append, FileAccess.Write);
                StreamWriter sw = new StreamWriter(fs);
                Console.WriteLine("Irja be a fájlba a szöveget");
                string str = Console.ReadLine();
                sw.WriteLine(str);
                sw.Flush();
                sw.Close();
                fs.Close();
            }
            public void ReadData()
            {
                string[] lines = File.ReadAllLines(@"C:\\Users\\keres\\Desktop\\beadando\\readwrite.txt");
                foreach (string line in lines)
                {
                    Console.WriteLine(line);
                }


            }
        }
    }


}



