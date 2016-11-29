using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Collections.Generic;
using System.Timers;

namespace HamiltonCycle
{   
    class Program
    {
        long value;
        
        public int nodesNumber;
        public int[,] matrix;
        public bool[,] matrix1;
        public int[,] solution;
        public int[] minWeightSums;
        public const int maxThreadNumber = 8;
        public int mutex = 1;
        const int maxWeightsSum = 1000000000;
        public static long memoryUsed = 0;

        public void readGraph(string file)
        {
            TextReader textReader = File.OpenText(file);

            nodesNumber = int.Parse(textReader.ReadLine());
            matrix = new int[nodesNumber, nodesNumber];
            matrix1 = new bool[nodesNumber, nodesNumber];

            for (int i = 0; i < nodesNumber; i++)
            {
                for (int j = 0; j < nodesNumber; j++)
                {
                    matrix1[i, j] = false;
                }
            }

            int edgesNumberToRead = int.Parse(textReader.ReadLine());

            while (edgesNumberToRead > 0)
            {
                string line = textReader.ReadLine();
                string[] edges = line.Split(' ');
                int edge1 = int.Parse(edges[0]);
                int edge2 = int.Parse(edges[1]);
                int weight = int.Parse(edges[2]);
                matrix[edge1, edge2] = weight;
                matrix[edge2, edge1] = weight;
                matrix1[edge1, edge2] = true;
                matrix1[edge2, edge1] = true;
                --edgesNumberToRead;
            }
        }

        public void compute()
        {
            Thread current = Thread.CurrentThread;
            current.Priority = ThreadPriority.Highest;
            int secondNode;
            Thread.BeginCriticalRegion();
            secondNode = mutex;
            mutex++;
            Thread.EndCriticalRegion();
            solve(secondNode);
        }

        public void solve(int secondNode)
        {
            while (secondNode < nodesNumber)
            {
                Console.WriteLine("Computing for node " + secondNode);

                if (matrix1[0, secondNode])
                {
                    int[] vector = new int[nodesNumber];
                    vector[1] = secondNode;
                    int idx = 2;

                    for (int i = 1; i < nodesNumber; i++)
                    {
                        if (i != secondNode)
                        {
                            vector[idx] = i;
                            idx++;
                        }
                    }

                    do
                    {
                        bool isSolution = true;
                        int weightSum = 0;

                        for (int i = 0; i < nodesNumber - 1; i++)
                        {
                            if (!matrix1[vector[i], vector[i + 1]])
                            {
                                isSolution = false;
                                break;
                            }

                            weightSum += matrix[vector[i], vector[i + 1]];
                        }

                        if (!matrix1[vector[nodesNumber - 1], vector[0]])
                        {
                            isSolution = false;
                        }
                        else
                        {
                            weightSum += matrix[vector[nodesNumber - 1], vector[0]];
                        }

                        if (isSolution && weightSum < minWeightSums[secondNode])
                        {
                            minWeightSums[secondNode] = weightSum;

                            for (int i = 0; i < nodesNumber; i++)
                            {
                                solution[secondNode, i] = vector[i];


                                //zliczam uzywany 
                                //value += ((long)perfCntr.NextValue() / 1024) / 1024;

                            }
                        }

                    } while (nextPermutation(vector));
                }

                secondNode += maxThreadNumber;
            }
            //Console.WriteLine(string.Format("\nMemomry used by application: {0} MB", value.ToString()));
        }

        public bool nextPermutation(int[] vector)
        {
            
            int i = nodesNumber - 1;

            while (i > 2 && vector[i - 1] >= vector[i])
                i--;
            
            if (i <= 2)
                return false;

          
            int j = nodesNumber - 1;
            while (vector[j] <= vector[i - 1])
                j--;
            
            int temp = vector[i - 1];
            vector[i - 1] = vector[j];
            vector[j] = temp;

            j = nodesNumber - 1;
            while (i < j)
            {
                temp = vector[i];
                vector[i] = vector[j];
                vector[j] = temp;
                i++;
                j--;
            }

            return true;
        }

        static void Main( string[] args )
        { 
            System.DateTime startTime = DateTime.Now;

            string file1 = "graph13.txt"; // args[ 0 ];  
            Program p = new Program();

            p.readGraph( file1 );
            p.solution = new int[ p.nodesNumber, p.nodesNumber ];
            p.minWeightSums = new int[ p.nodesNumber ];

            for( int i = 0; i < p.nodesNumber; i++ )
            {
                p.minWeightSums[ i ] = maxWeightsSum;
            }

            if( p.nodesNumber < maxThreadNumber ) 
            {
                Thread[] thread = new Thread[ p.nodesNumber ];

               
                for ( int i = 0; i < p.nodesNumber - 1; i++ ) 
                {
                    thread[ i ] = new Thread( new ThreadStart( p.compute ) );
                    thread[ i ].Start();
                }
                for( int i = 0; i < p.nodesNumber - 1; i++ ) 
                {
                       thread[ i ].Join();
                }
            }
            else
            {
                Thread[] thread = new Thread[ maxThreadNumber ];
                

                for ( int j = 0; j < maxThreadNumber; j++ ) 
                {   
                    thread[ j ] = new Thread( new ThreadStart( p.compute ) );
                    thread[ j ].Start();
                }
                for( int i = 0; i < maxThreadNumber; i++ ) 
                {
                    thread[ i ].Join();
                }
            }
            
            int position = 0;
            int minWeightSum = maxWeightsSum;

            for( int i = 0; i < p.nodesNumber; i++ ) 
            {
                if( p.minWeightSums[ i ] < minWeightSum )
                {
                    position = i;
                    minWeightSum = p.minWeightSums[ i ];
                }
            }

            StreamWriter streamWriter = new StreamWriter("solution.txt");

            if ( minWeightSum < maxWeightsSum ) 
            {
                for( int i = 0; i < p.nodesNumber; i++ ) 
                {
                    streamWriter.WriteLine( p.solution[ position, i ] );      
                }
                streamWriter.WriteLine( "Weight sum: " + minWeightSum );
            }
            else
            {
                Console.WriteLine( "There is no solution for this case" );
            }

            System.DateTime endTime = DateTime.Now;
            TimeSpan timeSpan = new TimeSpan();
            timeSpan = endTime.Subtract( startTime );
            Action<object> write = Console.WriteLine;
            Console.Write( "\nThe time is: " );
            Console.WriteLine( timeSpan );
            streamWriter.Write("The time is: ");
            streamWriter.WriteLine(timeSpan);
            streamWriter.Close();
            write("\n---------------------------------------\n");
            write("Press any key to exit");
            Console.ReadLine();

        }
    }
}
