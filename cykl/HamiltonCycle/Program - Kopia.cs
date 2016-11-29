
using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Threading;

namespace HamiltonCycle
{
    class Program
    {
        public struct Nodes
        {
            public int number;
            public int[,] matrix;
        }

        public Nodes nodes;

        public int[] st, st1;
        public int k;
        public int current = 0;
        public bool isSolution = false;
        public int mutex = 0;
        public bool sol = false;

        public int[,] solution;

        static void Main( string[] args )
        {
            //Thread.Sleep( 10000 );
            System.DateTime startTime = DateTime.Now;

            string file1 = args[ 0 ];

            Program p = new Program();

            p.readGraph( file1 );

            p.st = new int[ p.nodes.number ];
            p.st1 = new int[ p.nodes.number ];
            p.st[ 0 ] = p.current;
            p.k = 1;
            p.st[ p.k ] = p.current;

            p.back();

            p.st1[ 0 ] = p.st[ 0 ];
            int poz = p.nodes.number - 1;
            for( int i = 1; i < p.nodes.number; i++ )
            {
                p.st1[ i ] = p.st[ poz ];
                poz--;
            }

            if ( p.isSolution ) //if there is a solution then create threads 
            {
                p.solution = new int[ p.nodes.number * 2, p.nodes.number ];

                for( int i = 0; i < p.nodes.number * 2; i++ )
                {
                    for( int j = 0; j < p.nodes.number; j++ )
                    {
                        p.solution[ i, j ] = 65536;
                    }
                }

                if( p.nodes.number < 8 ) 
                {
                    p.mutex = 1;
                    Thread[] trd = new Thread[ p.nodes.number ];
                    for( int i = 0; i < p.nodes.number - 1; i++ ) 
                    {
                        trd[ i ] = new Thread( new ThreadStart( p.compute ) );
                        trd[ i ].Start();
                    }
                    p.solve( 0 );
                    for( int i = 0; i < p.nodes.number - 1; i++ ) 
                    {
                        trd[ i ].Join();
                    }
                }
                else
                {
                    Thread[] trd = new Thread[ 8 ];
                    for( int j = 0; j < 8; j++ ) 
                    {
                        trd[ j ] = new Thread( new ThreadStart( p.compute ) );
                        trd[ j ].Start();
                        trd[ j ].Join();
                    }
                    for( int i = 0; i < 8; i++ ) 
                    {
                        trd[ i ] = new Thread( new ThreadStart( p.compute ) );
                        trd[ i ].Start();
                        trd[ i ].Join();
                    }
                }

                int min = p.nodes.number, position = 0;
                for( int i = 0; i < p.nodes.number; i++ ) 
                {
                    for( int j = 0; j < p.nodes.number; j++ ) 
                    {
                        if( ( p.solution[ i, j ] != 65536 ) && ( p.solution[ i, j ] < min ) ) 
                        {
                            position = i;
                            min = j;
                            i = p.nodes.number;
                            break;
                        }
                    }
                }

                if( min < p.nodes.number ) 
                {
                    p.sol = true;
                    StreamWriter sw = new StreamWriter( "solution.txt" );
                    for( int i = min; i < min + p.nodes.number; i++ ) 
                    {
                        sw.WriteLine( p.solution[ position, i ] );      
                    }
                    sw.Close();
                }
            }

            if( p.sol == false ) 
            {
                Console.WriteLine( "There is no solution for this case" );
            }

            System.DateTime endTime = DateTime.Now;
            TimeSpan timeSpan = new TimeSpan();
            timeSpan = endTime.Subtract( startTime );

            Console.Write( "The time is: " );
            Console.WriteLine( timeSpan );
            Console.ReadLine();
        }

        public void readGraph( string file )
        {
            TextReader textReader = File.OpenText( file );

            nodes.number = int.Parse( textReader.ReadLine() );
            nodes.matrix = new int[ nodes.number, nodes.number ];

            int edgesNumberToRead = int.Parse( textReader.ReadLine() );

            while( edgesNumberToRead > 0 ) 
            {
                string line = textReader.ReadLine();
                string[] edges = line.Split( ' ' );
                int edge1 = int.Parse( edges[ 0 ] );
                int edge2 = int.Parse( edges[ 1 ] );
                int weight = int.Parse( edges[ 2 ] );
                nodes.matrix[ edge1, edge2 ] = weight;
                nodes.matrix[ edge2, edge1 ] = weight;
                --edgesNumberToRead;
            }
        }

        public void back()
        {
            bool S, V;
            k = 1;
            st[ k ] = current;
            while( k > 0 ) 
            {
                if( st[ k ] < ( nodes.number - 1 ) ) 
                {
                    st[ k ]++;
                    S = true;
                }
                else
                {
                    S = false;
                }

                V = true;

                if( nodes.matrix[ st[ k - 1 ], st[ k ] ] == 0 ) 
                {
                    V = false;
                }
                else
                {
                    for( int i = 0; i <= k - 1; i++ ) 
                    {
                        if( st[ i ] == st[ k ] )
                        {
                            V = false;
                            break;
                        }
                    }
                }

                while( ( S ) && ( V == false ) ) 
                {
                    if( st[ k ] < ( nodes.number - 1 ) ) 
                    {
                        st[ k ]++;
                        S = true;
                    }
                    else
                    {
                        S = false;
                    }

                    V = true;
                    if( nodes.matrix[ st[ k - 1 ], st[ k ] ] == 0 )
                    {
                        V = false;
                    }
                    else
                    {
                        for( int i = 0; i <= k - 1; i++ ) 
                        {
                            if( st[ i ] == st[ k ] ) 
                            {
                                V = false;
                                break;
                            }
                        }
                    }
                }
                if( S ) 
                {
                    if( k == ( nodes.number - 1 ) && ( nodes.matrix[ st[ 0 ], st[ k ] ] != 0 ) )
                    {
                        //the solution is found
                        k = 0;
                        isSolution = true;
                    }
                    else
                    {
                        if( k >= ( nodes.number - 1 ) )
                        {
                            k = -1;
                            Console.WriteLine( "There is no solution for this case" );
                        }
                        else
                        {
                            k++;
                            st[ k ] = current;
                        }
                    }
                }
                else
                {
                    k--;
                }
            }
        }

        public void compute()
        {
            Thread current = Thread.CurrentThread;
            current.Priority = ThreadPriority.Highest;
            int position;
            Thread.BeginCriticalRegion();
            position = mutex; mutex++;
            Thread.EndCriticalRegion();
            solve( position );
        }

        public void solve( int position )
        {
            while( position < nodes.number )
            {
                int[] vector = new int[ nodes.number ];
                //right to left
                for( int i = 0; i < nodes.number; i++ )
                {
                    vector[ i ] = st[ i ];
                }
                if( position > 0 ) 
                { 
                    //rearrange positions  
                    int j = 0;
                    for( int i = position; i < nodes.number; i++ )
                    {
                        vector[ j ] = st[ i ];
                        j++;
                    }
                    for( int i = 0; i < position; i++ )
                    {
                        vector[ j ] = st[ i ];
                        j++;
                    }
                }

                bool ok = false;
                int jj = 0;
                int ct = 0;
                int initial = 0;

                for( int i = 0; i < nodes.number; i++ )
                {
                    jj++;
                    ct++;

                    if( ct == nodes.number )
                    {
                        ok = true;
                        break;
                    }
                }
                if( ok )
                {
                    int j = 0;
                    for( int i = initial; i < initial + nodes.number; i++ )
                    {
                        solution[ position, i ] = vector[ j ];
                        j++;
                    }
                }
                ////////////////////////////////
                //left to right
                for( int i = 0; i < nodes.number; i++ )
                {
                    vector[ i ] = st1[ i ];
                }
                if( position > 0 ) 
                {
                    //rearrange positions  
                    int j = 0;
                    for( int i = position; i < nodes.number; i++ )
                    {
                        vector[ j ] = st1[ i ];
                        j++;
                    }
                    for( int i = 0; i < position; i++ )
                    {
                        vector[ j ] = st1[ i ];
                        j++;
                    }
                }

                ok = false;
                jj = 0;
                ct = 0;
                initial = 0;
                for( int i = 0; i < nodes.number; i++ )
                {
                    jj++;
                    ct++;

                    if( ct == nodes.number ) 
                    {
                        ok = true;
                        break;
                    }
                }

                if( ok )
                {
                    int j = 0;
                    for( int i = initial; i < initial + nodes.number; i++ ) 
                    {
                        solution[ position, i ] = vector[ j ];
                        j++;
                    }
                }

                position += 8;
            }
        }
    }
}
