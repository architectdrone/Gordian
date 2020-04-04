/*
 * GORDIAN
 * by OWEN MELLEMA
 * 
 * ROUGH INTRODUCTION:
 * GORIDAN is a tool for thinking about the connections between events. It has two main concepts: KNOTS and THREADS.
 * A KNOT is like a single event. A THREAD connects several knots. The KNOT describes the THREAD, and the THREAD describes the KNOT.
 * 
 * EXAMPLE:
 * For example, there might be two threads representing two people, Joe and Sally. Both threads could be connected at a single knot, when they meet for the first time. 
 * Joe might then go to the store, creating a knot in Joe's thread. Sally then might visit her parents, likewise creating a knot in Sally's thread.
 * Joe's thread would have him meeting Sally, then going to the store, but NOT Sally visiting her parents. Vice Versa for Sally.
 * However, each knot in each thread may have an effect on the threads with which it is entangled. For example, Perhaps Sally had to leave Joe quickly to meet her parents.
 * 
 * THREADS:
 * A thread could be:
 * -A person
 * -A place
 * -A concept
 * In short, a thread is anything that exists across time. A NARRATIVE.
 * The idea is that you want the Thread to describe itself as little as possible. Instead, you want the thread to be described by the knots in it.
 * 
 * KNOTS:
 * A knot could be:
 * -An event
 * -A relationship
 * In short, a knot is something that describes a thread, in a linear time interpretation.
*/
using System;
using System.Collections.Generic;

namespace Gordian
{
    class Program
    {
        static void Main(string[] args)
        {
            GordianMatrix gordianMatrix = new GordianMatrix();
            while (true)
            {
                Console.WriteLine("1. New Thread");
                Console.WriteLine("2. New Knot");
                Console.WriteLine("3. View ALL Threads");
                Console.WriteLine("4. View ALL Knots");
                Console.WriteLine("5. Exit");
                string title;
                string description;
                switch (getInt("> "))
                {
                    case 1:
                        title = getString("Title > ");
                        gordianMatrix.newThread(title);
                        break;

                    case 2:
                        title = getString("Title > ");
                        description = getString("Description > ");
                        if (getBool("Add threads now? >"))
                        {
                            List<int> thread_ids = new List<int>();
                            while (true)
                            {
                                gordianMatrix.printAllThreads();
                                thread_ids.Add(getInt("> "));
                                if (!getBool("Add more? > "))
                                {
                                    break;
                                }
                            }
                            gordianMatrix.newKnot(title, description, thread_ids);
                        }
                        else
                        {
                            gordianMatrix.newKnot(title, description, null);
                        }
                        break;

                    case 3:
                        gordianMatrix.printAllThreads();
                        break;

                    case 4:
                        gordianMatrix.printAllKnots();
                        break;

                    case 5:
                        return;

                    default:
                        Console.WriteLine("Hm?");
                        break;
                }
            }
        }

        static int getInt(string prompt)
        {
            Console.Write(prompt);
            return Convert.ToInt32(Console.ReadLine());

        }

        static string getString(string prompt)
        {
            Console.Write(prompt);
            return Console.ReadLine();
        }

        static bool getBool(string prompt)
        {
            Console.Write("(y/n) {0}", prompt);
            string result = Console.ReadLine();
            if (result == "y")
            {
                return true;
            }
            else if (result == "n")
            {
                return false;
            }
            else
            {
                return getBool(prompt);
            }
        }
    }

    class Knot
    {
        public int id;
        public string title;
        public string description;

        public Knot(int _id, string _title, string _description)
        {
            id = _id;
            title = _title;
            description = _description;
        }
    }

    class Thread
    {
        public int id;
        public string title;
        public List<int> knots = new List<int>();

        public Thread(int _id, string _title)
        {
            id = _id;
            title = _title;
        }
    }

    class GordianMatrix
    {
        private List<Knot> all_knots = new List<Knot>(); 
        //NOTE: 
        //1. For all indexes, i1 and i2, i1 < i2, all_knots[i1].id < all_knots[i2].id
        //2. For all indexes, i, i <= all_knots[i].id
        private List<Thread> all_threads = new List<Thread>();

        private int next_knot_id = 0;
        private int next_thread_id = 0;

        public void printAllThreads()
        {
            printThreads(all_threads);
        }

        public void printAllKnots()
        {
            printKnots(all_knots);
        }

        public void newThread(string title)
        {
            Thread new_thread = new Thread(next_thread_id, title);
            all_threads.Add(new_thread);
            next_thread_id++;
        }

        public void newKnot(string title, string description, List<int>? thread_ids)
        {
            Knot new_knot = new Knot(next_knot_id, title, description);
            all_knots.Add(new_knot);
            next_knot_id++;

            if (thread_ids != null)
            {
                foreach (int thread_id in thread_ids)
                {
                    foreach (Thread thread in all_threads)
                    {
                        if (thread.id == thread_id)
                        {
                            thread.knots.Add(new_knot.id);
                        }
                    }
                }
            }
        }

        private List<Knot> getKnotsInThread(Thread thread)
        {
            /*
             * TODO: Improve runtime complexity using iterators. Apparently this is very complicated in C#, or something.
             * We can get the average case complexity down to around O(n) if we do that. Right now it is something like O(n^3). Ouch.
             * IEnumerators?
             */
            List<Knot> to_return = new List<Knot>();

            foreach (int id in thread.knots)
            {
                foreach (Knot knot in all_knots)
                {
                    if (knot.id == id)
                    {
                        to_return.Add(knot);
                        break;
                    }
                }
            }

            //God, it hurts.
            return to_return;
        }

        private List<Thread> getThreadsInKnot(Knot knot)
        {
            List<Thread> to_return = new List<Thread>();
            //TODO: Improve this runtime complexity, somewhat.
            foreach (Thread thread in all_threads)
            {
                foreach (int knot_id in thread.knots)
                {
                    to_return.Add(thread);
                }
            }
            return to_return;
        }

        private Knot? getKnot(int knot_id)
        {
            foreach (Knot knot in all_knots)
            {
                if (knot.id == knot_id)
                {
                    return knot;
                }
            }
            return null;
        }

        private Thread? getThread(int thread_id)
        {
            foreach (Thread thread in all_threads)
            {
                if (thread.id == thread_id)
                {
                    return thread;
                }
            }
            return null;
        }

        private void printThreads(List<Thread> to_print)
        {
            foreach (Thread thread in to_print)
            {
                Console.WriteLine("({0}) {1}", thread.id, thread.title);
            }
        }

        private void printKnots(List<Knot> to_print)
        {
            foreach (Knot knot in to_print)
            {
                Console.WriteLine("({0}) {1}", knot.id, knot.title);
            }
        }
    }
}
