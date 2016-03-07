#pragma warning disable 219
#pragma warning disable 162
using System;
public class MemoryLeaksTest : System.Runtime.ConstrainedExecution.CriticalFinalizerObject
{
    public int dummy;
    public MemoryLeaksTest()
    {
        dummy = 0;
    }
    ~MemoryLeaksTest()
    {
        long cnt = alglib.alloc_counter();
        System.Console.WriteLine("Allocation counter checked... "+(cnt==0 ? "OK" : "FAILED"));
        if( cnt!=0 )
            System.Environment.ExitCode = 1;
    }
}
public class XTest
{
    public static void Main(string[] args)
    {
        bool _TotalResult = true;
        bool _TestResult;
        int _spoil_scenario;
        System.Console.WriteLine("x-tests. Please wait...");
        alglib.alloc_counter_activate();
        System.Console.WriteLine("Allocation counter activated...");
        try
        {
            const int max1d = 70;
            const int max2d = 40;
            
            System.Console.WriteLine("Basic tests:");
            {
                // deallocateimmediately()
                alglib.minlbfgsstate s;
                double[] x = new double[100];
                long cnt0, cnt1;
                cnt0 = alglib.alloc_counter();
                alglib.minlbfgscreate(x.Length, 10, x, out s);
                alglib.deallocateimmediately(ref s);
                cnt1 = alglib.alloc_counter();
                _TestResult = cnt1<=cnt0;
                System.Console.WriteLine("* deallocateimmediately()    "+(_TestResult ? " OK" : " FAILED"));
                _TotalResult = _TotalResult && _TestResult;
            }
            {
                // boolean 1D arrays (this test checks both interface and ref/out conventions used by ALGLIB)
                int n, i, cnt;
                _TestResult = true;
                for(n=0; n<=max1d; n++)
                {
                    bool[] arr0 = new bool[n];
                    bool[] arr1 = new bool[n];
                    bool[] arr2 = new bool[n];
                    bool[] arr3 = null;
                    cnt = 0;
                    for(i=0; i<n; i++)
                    {
                        arr0[i] = alglib.math.randomreal()>0.5;
                        arr1[i] = arr0[i];
                        arr2[i] = arr0[i];
                        if( arr0[i] )
                            cnt++;
                    }
                    _TestResult = _TestResult && (alglib.xdebugb1count(arr0)==cnt);
                    alglib.xdebugb1not(ref arr1);
                    if( alglib.ap.len(arr1)==n )
                    {
                        for(i=0; i<n; i++)
                            _TestResult = _TestResult && (arr1[i]==!arr0[i]);
                    }
                    else
                        _TestResult = false;
                    alglib.xdebugb1appendcopy(ref arr2);
                    if( alglib.ap.len(arr2)==2*n )
                    {
                        for(i=0; i<2*n; i++)
                            _TestResult = _TestResult && (arr2[i]==arr0[i%n]);
                    }
                    else
                        _TestResult = false;
                    alglib.xdebugb1outeven(n, out arr3);
                    if( alglib.ap.len(arr3)==n )
                    {
                        for(i=0; i<n; i++)
                            _TestResult = _TestResult && (arr3[i]==(i%2==0));
                    }
                    else
                        _TestResult = false;
                }
                System.Console.WriteLine("* boolean 1D arrays          "+(_TestResult ? " OK" : " FAILED"));
                _TotalResult = _TotalResult && _TestResult;
            }
            {
                // integer 1D arrays (this test checks both interface and ref/out conventions used by ALGLIB)
                int n, i, sum;
                _TestResult = true;
                for(n=0; n<=max1d; n++)
                {
                    int[] arr0 = new int[n];
                    int[] arr1 = new int[n];
                    int[] arr2 = new int[n];
                    int[] arr3 = null;
                    sum = 0;
                    for(i=0; i<n; i++)
                    {
                        arr0[i] = alglib.math.randominteger(10);
                        arr1[i] = arr0[i];
                        arr2[i] = arr0[i];
                        sum+=arr0[i];
                    }
                    _TestResult = _TestResult && (alglib.xdebugi1sum(arr0)==sum);
                    alglib.xdebugi1neg(ref arr1);
                    if( alglib.ap.len(arr1)==n )
                    {
                        for(i=0; i<n; i++)
                            _TestResult = _TestResult && (arr1[i]==-arr0[i]);
                    }
                    else
                        _TestResult = false;
                    alglib.xdebugi1appendcopy(ref arr2);
                    if( alglib.ap.len(arr2)==2*n )
                    {
                        for(i=0; i<2*n; i++)
                            _TestResult = _TestResult && (arr2[i]==arr0[i%n]);
                    }
                    else
                        _TestResult = false;
                    alglib.xdebugi1outeven(n,out arr3);
                    if( alglib.ap.len(arr3)==n )
                    {
                        for(i=0; i<n; i++)
                            if( i%2==0 )
                                _TestResult = _TestResult && (arr3[i]==i);
                            else
                                _TestResult = _TestResult && (arr3[i]==0);
                    }
                    else
                        _TestResult = false;
                }
                System.Console.WriteLine("* integer 1D arrays          "+(_TestResult ? " OK" : " FAILED"));
                _TotalResult = _TotalResult && _TestResult;
            }
            {
                // real 1D arrays (this test checks both interface and ref/out conventions used by ALGLIB)
                int n, i;
                double sum;
                _TestResult = true;
                for(n=0; n<=max1d; n++)
                {
                    double[] arr0 = new double[n];
                    double[] arr1 = new double[n];
                    double[] arr2 = new double[n];
                    double[] arr3 = null;
                    sum = 0;
                    for(i=0; i<n; i++)
                    {
                        arr0[i] = alglib.math.randomreal()-0.5;
                        arr1[i] = arr0[i];
                        arr2[i] = arr0[i];
                        sum+=arr0[i];
                    }
                    _TestResult = _TestResult && (Math.Abs(alglib.xdebugr1sum(arr0)-sum)<1.0E-10);
                    alglib.xdebugr1neg(ref arr1);
                    if( alglib.ap.len(arr1)==n )
                    {
                        for(i=0; i<n; i++)
                            _TestResult = _TestResult && (Math.Abs(arr1[i]+arr0[i])<1.0E-10);
                    }
                    else
                        _TestResult = false;
                    alglib.xdebugr1appendcopy(ref arr2);
                    if( alglib.ap.len(arr2)==2*n )
                    {
                        for(i=0; i<2*n; i++)
                            _TestResult = _TestResult && (arr2[i]==arr0[i%n]);
                    }
                    else
                        _TestResult = false;
                    alglib.xdebugr1outeven(n,out arr3);
                    if( alglib.ap.len(arr3)==n )
                    {
                        for(i=0; i<n; i++)
                            if( i%2==0 )
                                _TestResult = _TestResult && (arr3[i]==i*0.25);
                            else
                                _TestResult = _TestResult && (arr3[i]==0);
                    }
                    else
                        _TestResult = false;
                }
                System.Console.WriteLine("* real 1D arrays             "+(_TestResult ? " OK" : " FAILED"));
                _TotalResult = _TotalResult && _TestResult;
            }
            {
                // complex 1D arrays (this test checks both interface and ref/out conventions used by ALGLIB)
                int n, i;
                alglib.complex sum;
                _TestResult = true;
                for(n=0; n<=max1d; n++)
                {
                    alglib.complex[] arr0 = new alglib.complex[n];
                    alglib.complex[] arr1 = new alglib.complex[n];
                    alglib.complex[] arr2 = new alglib.complex[n];
                    alglib.complex[] arr3 = null;
                    sum = 0;
                    for(i=0; i<n; i++)
                    {
                        arr0[i].x = alglib.math.randomreal()-0.5;
                        arr0[i].y = alglib.math.randomreal()-0.5;
                        arr1[i] = arr0[i];
                        arr2[i] = arr0[i];
                        sum+=arr0[i];
                    }
                    _TestResult = _TestResult && (alglib.math.abscomplex(alglib.xdebugc1sum(arr0)-sum)<1.0E-10);
                    alglib.xdebugc1neg(ref arr1);
                    if( alglib.ap.len(arr1)==n )
                    {
                        for(i=0; i<n; i++)
                            _TestResult = _TestResult && (alglib.math.abscomplex(arr1[i]+arr0[i])<1.0E-10);
                    }
                    else
                        _TestResult = false;
                    alglib.xdebugc1appendcopy(ref arr2);
                    if( alglib.ap.len(arr2)==2*n )
                    {
                        for(i=0; i<2*n; i++)
                            _TestResult = _TestResult && (arr2[i]==arr0[i%n]);
                    }
                    else
                        _TestResult = false;
                    alglib.xdebugc1outeven(n,out arr3);
                    if( alglib.ap.len(arr3)==n )
                    {
                        for(i=0; i<n; i++)
                            if( i%2==0 )
                            {
                                _TestResult = _TestResult && (arr3[i].x==i*0.250);
                                _TestResult = _TestResult && (arr3[i].y==i*0.125);
                            }
                            else
                                _TestResult = _TestResult && (arr3[i]==0);
                    }
                    else
                        _TestResult = false;
                }
                System.Console.WriteLine("* complex 1D arrays          "+(_TestResult ? " OK" : " FAILED"));
                _TotalResult = _TotalResult && _TestResult;
            }
            {
                // boolean 2D arrays (this test checks both interface and ref/out conventions used by ALGLIB)
                int m, n, i, j, cnt;
                _TestResult = true;
                for(n=0; n<=max2d; n++)
                    for(m=0; m<=max2d; m++)
                    {
                        // skip situations when n*m==0, but n!=0 or m!=0
                        if( n*m==0 && (n!=0 || m!=0) )
                            continue;
                        
                        // proceed to testing
                        bool[,] arr0 = new bool[m,n];
                        bool[,] arr1 = new bool[m,n];
                        bool[,] arr2 = new bool[m,n];
                        bool[,] arr3 = null;
                        cnt = 0;
                        for(i=0; i<m; i++)
                            for(j=0; j<n; j++)
                            {
                                arr0[i,j] = alglib.math.randomreal()>0.5;
                                arr1[i,j] = arr0[i,j];
                                arr2[i,j] = arr0[i,j];
                                if( arr0[i,j] )
                                    cnt++;
                            }
                        _TestResult = _TestResult && (alglib.xdebugb2count(arr0)==cnt);
                        alglib.xdebugb2not(ref arr1);
                        if( alglib.ap.rows(arr1)==m && alglib.ap.cols(arr1)==n )
                        {
                            for(i=0; i<m; i++)
                                for(j=0; j<n; j++)
                                    _TestResult = _TestResult && (arr1[i,j]==!arr0[i,j]);
                        }
                        else
                            _TestResult = false;
                        alglib.xdebugb2transpose(ref arr2);
                        if( alglib.ap.rows(arr2)==n && alglib.ap.cols(arr2)==m )
                        {
                            for(i=0; i<m; i++)
                                for(j=0; j<n; j++)
                                    _TestResult = _TestResult && (arr2[j,i]==arr0[i,j]);
                        }
                        else
                            _TestResult = false;
                        alglib.xdebugb2outsin(m, n, out arr3);
                        if( alglib.ap.rows(arr3)==m && alglib.ap.cols(arr3)==n )
                        {
                            for(i=0; i<m; i++)
                                for(j=0; j<n; j++)
                                    _TestResult = _TestResult && (arr3[i,j]==(Math.Sin(3*i+5*j)>0));
                        }
                        else
                            _TestResult = false;
                    }
                System.Console.WriteLine("* boolean 2D arrays          "+(_TestResult ? " OK" : " FAILED"));
                _TotalResult = _TotalResult && _TestResult;
            }
            {
                // integer 2D arrays (this test checks both interface and ref/out conventions used by ALGLIB)
                int m, n, i, j;
                int sum;
                _TestResult = true;
                for(n=0; n<=max2d; n++)
                    for(m=0; m<=max2d; m++)
                    {
                        // skip situations when n*m==0, but n!=0 or m!=0
                        if( n*m==0 && (n!=0 || m!=0) )
                            continue;
                        
                        // proceed to testing
                        int[,] arr0 = new int[m,n];
                        int[,] arr1 = new int[m,n];
                        int[,] arr2 = new int[m,n];
                        int[,] arr3 = null;
                        sum = 0;
                        for(i=0; i<m; i++)
                            for(j=0; j<n; j++)
                            {
                                arr0[i,j] = alglib.math.randominteger(10);
                                arr1[i,j] = arr0[i,j];
                                arr2[i,j] = arr0[i,j];
                                sum += arr0[i,j];
                            }
                        _TestResult = _TestResult && (alglib.xdebugi2sum(arr0)==sum);
                        alglib.xdebugi2neg(ref arr1);
                        if( alglib.ap.rows(arr1)==m && alglib.ap.cols(arr1)==n )
                        {
                            for(i=0; i<m; i++)
                                for(j=0; j<n; j++)
                                    _TestResult = _TestResult && (arr1[i,j]==-arr0[i,j]);
                        }
                        else
                            _TestResult = false;
                        alglib.xdebugi2transpose(ref arr2);
                        if( alglib.ap.rows(arr2)==n && alglib.ap.cols(arr2)==m )
                        {
                            for(i=0; i<m; i++)
                                for(j=0; j<n; j++)
                                    _TestResult = _TestResult && (arr2[j,i]==arr0[i,j]);
                        }
                        else
                            _TestResult = false;
                        alglib.xdebugi2outsin(m, n, out arr3);
                        if( alglib.ap.rows(arr3)==m && alglib.ap.cols(arr3)==n )
                        {
                            for(i=0; i<m; i++)
                                for(j=0; j<n; j++)
                                    _TestResult = _TestResult && (arr3[i,j]==System.Math.Sign(Math.Sin(3*i+5*j)));
                        }
                        else
                            _TestResult = false;
                    }
                System.Console.WriteLine("* integer 2D arrays          "+(_TestResult ? " OK" : " FAILED"));
                _TotalResult = _TotalResult && _TestResult;
            }
            {
                // real 2D arrays (this test checks both interface and ref/out conventions used by ALGLIB)
                int m, n, i, j;
                double sum;
                _TestResult = true;
                for(n=0; n<=max2d; n++)
                    for(m=0; m<=max2d; m++)
                    {
                        // skip situations when n*m==0, but n!=0 or m!=0
                        if( n*m==0 && (n!=0 || m!=0) )
                            continue;
                        
                        // proceed to testing
                        double[,] arr0 = new double[m,n];
                        double[,] arr1 = new double[m,n];
                        double[,] arr2 = new double[m,n];
                        double[,] arr3 = null;
                        sum = 0;
                        for(i=0; i<m; i++)
                            for(j=0; j<n; j++)
                            {
                                arr0[i,j] = alglib.math.randomreal()-0.5;
                                arr1[i,j] = arr0[i,j];
                                arr2[i,j] = arr0[i,j];
                                sum += arr0[i,j];
                            }
                        _TestResult = _TestResult && (System.Math.Abs(alglib.xdebugr2sum(arr0)-sum)<1.0E-10);
                        alglib.xdebugr2neg(ref arr1);
                        if( alglib.ap.rows(arr1)==m && alglib.ap.cols(arr1)==n )
                        {
                            for(i=0; i<m; i++)
                                for(j=0; j<n; j++)
                                    _TestResult = _TestResult && (arr1[i,j]==-arr0[i,j]);
                        }
                        else
                            _TestResult = false;
                        alglib.xdebugr2transpose(ref arr2);
                        if( alglib.ap.rows(arr2)==n && alglib.ap.cols(arr2)==m )
                        {
                            for(i=0; i<m; i++)
                                for(j=0; j<n; j++)
                                    _TestResult = _TestResult && (arr2[j,i]==arr0[i,j]);
                        }
                        else
                            _TestResult = false;
                        alglib.xdebugr2outsin(m, n, out arr3);
                        if( alglib.ap.rows(arr3)==m && alglib.ap.cols(arr3)==n )
                        {
                            for(i=0; i<m; i++)
                                for(j=0; j<n; j++)
                                    _TestResult = _TestResult && (System.Math.Abs(arr3[i,j]-Math.Sin(3*i+5*j))<1E-10);
                        }
                        else
                            _TestResult = false;
                    }
                System.Console.WriteLine("* real 2D arrays             "+(_TestResult ? " OK" : " FAILED"));
                _TotalResult = _TotalResult && _TestResult;
            }
            {
                // real 2D arrays (this test checks both interface and ref/out conventions used by ALGLIB)
                int m, n, i, j;
                alglib.complex sum;
                _TestResult = true;
                for(n=0; n<=max2d; n++)
                    for(m=0; m<=max2d; m++)
                    {
                        // skip situations when n*m==0, but n!=0 or m!=0
                        if( n*m==0 && (n!=0 || m!=0) )
                            continue;
                        
                        // proceed to testing
                        alglib.complex[,] arr0 = new alglib.complex[m,n];
                        alglib.complex[,] arr1 = new alglib.complex[m,n];
                        alglib.complex[,] arr2 = new alglib.complex[m,n];
                        alglib.complex[,] arr3 = null;
                        sum = 0;
                        for(i=0; i<m; i++)
                            for(j=0; j<n; j++)
                            {
                                arr0[i,j].x = alglib.math.randomreal()-0.5;
                                arr0[i,j].y = alglib.math.randomreal()-0.5;
                                arr1[i,j] = arr0[i,j];
                                arr2[i,j] = arr0[i,j];
                                sum += arr0[i,j];
                            }
                        _TestResult = _TestResult && (alglib.math.abscomplex(alglib.xdebugc2sum(arr0)-sum)<1.0E-10);
                        alglib.xdebugc2neg(ref arr1);
                        if( alglib.ap.rows(arr1)==m && alglib.ap.cols(arr1)==n )
                        {
                            for(i=0; i<m; i++)
                                for(j=0; j<n; j++)
                                    _TestResult = _TestResult && (arr1[i,j]==-arr0[i,j]);
                        }
                        else
                            _TestResult = false;
                        alglib.xdebugc2transpose(ref arr2);
                        if( alglib.ap.rows(arr2)==n && alglib.ap.cols(arr2)==m )
                        {
                            for(i=0; i<m; i++)
                                for(j=0; j<n; j++)
                                    _TestResult = _TestResult && (arr2[j,i]==arr0[i,j]);
                        }
                        else
                            _TestResult = false;
                        alglib.xdebugc2outsincos(m, n, out arr3);
                        if( alglib.ap.rows(arr3)==m && alglib.ap.cols(arr3)==n )
                        {
                            for(i=0; i<m; i++)
                                for(j=0; j<n; j++)
                                {
                                    _TestResult = _TestResult && (System.Math.Abs(arr3[i,j].x-Math.Sin(3*i+5*j))<1E-10);
                                    _TestResult = _TestResult && (System.Math.Abs(arr3[i,j].y-Math.Cos(3*i+5*j))<1E-10);
                                }
                        }
                        else
                            _TestResult = false;
                    }
                System.Console.WriteLine("* complex 2D arrays          "+(_TestResult ? " OK" : " FAILED"));
                _TotalResult = _TotalResult && _TestResult;
            }
            {
                // "biased product / sum" test
                int m, n, i, j;
                double sum;
                _TestResult = true;
                for(n=1; n<=max2d; n++)
                    for(m=1; m<=max2d; m++)
                    {
                        // proceed to testing
                        double[,] a = new double[m,n];
                        double[,] b = new double[m,n];
                        bool[,]   c = new bool[m,n];
                        sum = 0;
                        for(i=0; i<m; i++)
                            for(j=0; j<n; j++)
                            {
                                a[i,j] = alglib.math.randomreal()-0.5;
                                b[i,j] = alglib.math.randomreal()-0.5;
                                c[i,j] = alglib.math.randomreal()>0.5;
                                if( c[i,j] )
                                    sum += a[i,j]*(1+b[i,j]);
                            }
                        _TestResult = _TestResult && (Math.Abs(alglib.xdebugmaskedbiasedproductsum(m,n,a,b,c)-sum)<1.0E-10);
                    }
                System.Console.WriteLine("* multiple arrays            "+(_TestResult ? " OK" : " FAILED"));
                _TotalResult = _TotalResult && _TestResult;
            }
            
            //////////////////////////////////
            // Test issues from Mantis
            //////
            System.Console.WriteLine("Testing issies from Mantis:");
                
            
            //
            // Task #594 (http://bugs.alglib.net/view.php?id=594) - additional
            // test for correctness of copying of objects. When we copy ALGLIB
            // object, indenendent new copy is created.
            //
            {
                //
                // First, test copying of alglib.multilayerperceptron, which
                // is an "opaque object".
                //
                // Test copy constructors:
                // * copy object with make_copy()
                // * process vector with original network
                // * randomize original network
                // * process vector with copied networks and compare
                //
                alglib.multilayerperceptron net0, net1;
                double[] x  = new double[]{1,2};
                double[] y0 = new double[]{0,0};
                double[] y1 = new double[]{0,0};
                double[] y2 = new double[]{0,0};
                _TestResult = true;
                alglib.mlpcreate0(2, 2, out net0);
                alglib.mlpprocess(net0, x, ref y0);
                net1 = (alglib.multilayerperceptron)net0.make_copy();
                alglib.mlprandomize(net0);
                alglib.mlpprocess(net1, x, ref y1);
                _TestResult = _TestResult && (Math.Abs(y0[0]-y1[0])<1.0E-9) && (Math.Abs(y0[1]-y1[1])<1.0E-9);
                
                //
                // Then, test correctness of copying "records", i.e.
                // objects with publicly visible fields.
                //
                alglib.xdebugrecord1 r0, r1;
                alglib.xdebuginitrecord1(out r0);
                r1 = (alglib.xdebugrecord1)r0.make_copy();
                _TestResult = _TestResult && (r1.i==r0.i);
                _TestResult = _TestResult && (r1.c==r0.c);
                
                _TestResult = _TestResult && (r1.a.Length==2);
                _TestResult = _TestResult && (r0.a.Length==2);
                _TestResult = _TestResult && (r1.a!=r0.a);
                _TestResult = _TestResult && (r1.a[0]==r0.a[0]);
                _TestResult = _TestResult && (r1.a[1]==r0.a[1]);
                
                //
                // Test result
                //
                System.Console.WriteLine("* issue 594                  "+(_TestResult ? " OK" : " FAILED"));
                _TotalResult = _TotalResult && _TestResult;
            }
            
        }
        catch
        {
            System.Console.WriteLine("Unhandled exception was raised!");
            System.Environment.ExitCode = 1;
            return;
        }
        
        //
        // Test below creates instance of MemoryLeaksTest object.
        //
        // This object is descendant of CriticalFinalizerObject class,
        // which guarantees that it will be finalized AFTER all other
        // ALGLIB objects which hold pointers to unmanaged memory.
        //
        // Tests for memory leaks are done within object's destructor.
        //
        MemoryLeaksTest _test_object = new MemoryLeaksTest();
        if( !_TotalResult )
            System.Environment.ExitCode = 1;
    }
}
