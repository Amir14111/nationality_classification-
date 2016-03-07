using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApplication5
{
    static class MatStat
    {
        static private double[] dvalues1;
        static private double[] dvalues2;
        public static TextBox tbData;

        static private void DisplayArray(double[] ivals)
        {
            for (int jc = 0; jc < ivals.Length; jc++)
                DisplayData((Math.Round(ivals[jc], 3).ToString() + " "), true);
            DisplayData(" ", false);
        }

        static private void DisplayData(string sData, bool bNoScroll)
        {

            int maxTextLength = 1000; // maximum text length in text box

            if ( tbData.TextLength > maxTextLength)
                tbData.Text = tbData.Text.Remove(0, tbData.TextLength - maxTextLength);

            //string str = Encoding.ASCII.GetString(e.Data);
            tbData.AppendText(sData);
            tbData.ScrollToCaret();
            if (bNoScroll == false)
                tbData.SelectedText = Environment.NewLine;

        }


        static private double[] CalcKSProbability(int[] ival, int iMin, int iMax)
        {

            //List<double> lvals = new List<double>(iMax-iMin+1);
            int len = ival.Length;
            int rezlen = iMax - iMin + 1;
            double prob = (double)1 / (double)len;
            double curprob = (double)0;
            double[] dval = new double[rezlen];
            int i;
            for (i = 0; i < rezlen; i++) { dval[i] = (double)-1; }
            for (int j = 0; j < len; j++)
            {
                curprob += prob;
                dval[ival[j] - iMin] = curprob;
            }
            curprob = 0;
            for (i = 0; i < rezlen; i++)
            {
                if (dval[i] < 0)
                    dval[i] = curprob;
                else
                    curprob = dval[i];
            }
            return dval;
        }


        /// <summary> Calculation of KS
        /// Calculation of KS
        /// </summary>
        /// <param name="ivals1"></param>
        /// <param name="ivals2"></param>
        static public void CalcKS(int[] ivals1, int[] ivals2)
        {


            int iLen1 = ivals1.Length;
            int iLen2 = ivals2.Length;
            int iMin = Math.Min(ivals1[0], ivals2[0]);
            int iMax = Math.Max(ivals1[iLen1 - 1], ivals2[iLen2 - 1]);
            int iRange = iMax - iMin + 1;
            dvalues1 = CalcKSProbability(ivals1, iMin, iMax);
            DisplayData("Min=" + iMin.ToString() + " Max=" + iMax.ToString(), false);
            double dSqrt = Math.Sqrt(1.0 / (double)iLen1 + 1.0 / (double)iLen2);  ///iLen1*iLen2
            //DisplayArray(dvalues1);
            dvalues2 = CalcKSProbability(ivals2, iMin, iMax);
            //DisplayData("Min=" + iMin.ToString() + " Max=" + iMax.ToString(), false);
            //DisplayArray(dvalues2);
            double dMax = 0;
            int jmax = 0;
            for (int j = 0; j < iRange; j++)
            {
                double CurMax = Math.Abs(dvalues1[j] - dvalues2[j]);
                if (CurMax > dMax)
                {
                    dMax = CurMax;
                    jmax = j;
                }
            }
            DisplayData("Максимум различия(" + jmax.ToString() + ")=" + Math.Round(dMax, 3).ToString(), false);
            DisplayData("Sqrt((n1+n2)/n1*n2=" + Math.Round(dSqrt, 3).ToString(), false);
            DisplayData("5%=" + Math.Round(dSqrt * 1.358, 3).ToString(), false);
            DisplayData("10%=" + Math.Round(dSqrt * 1.224, 3).ToString(), false);


        }



        /// <summary> Calculation of LemanRozenblat criteria
        /// Calculation of LR with amendment(correction) on central rangs.
        /// </summary>
        /// <param name="ivals1"></param>
        /// <param name="ivals2"></param>
        static public void CalcLR(int[] ivals1, int[] ivals2)
        {


            int iRangeCommon = 1;
            int iLen1 = ivals1.Length;
            int iLen2 = ivals2.Length;
            int iMin = Math.Min(iLen1, iLen2);
            double[] iRange1 = new double[iLen1];
            double[] iRange2 = new double[iLen2];
            int j1 = 0;
            int j2 = 0;
            int ir2 = 0;
            int counter = 0;
            int sum = 0, sum1 = 0;
            //формируем ранги
            for (int i = 0; i < iMin; i++)
            {
                if (--sum > 0)
                {
                    sum--;
                    continue;
                }
                if ((j1 >= iLen1) || (j2 >= iLen2))
                    break;

                while (ivals1[j1] > ivals2[j2])
                {
                    iRange2[ir2++] = iRangeCommon++;

                    j2++;
                    if ((j1 >= iLen1) || (j2 >= iLen2) || (ir2 >= iLen2))
                        break;

                }
                if ((j1 >= iLen1) || (j2 >= iLen2))
                    break;
                if (ivals1[j1] == ivals2[j2])
                {
                    sum = 0;
                    sum1 = 0;
                    counter = 0;
                    while (  (j2 + sum1) < ivals2.Length && (ivals1[j1] == ivals2[j2 + sum1]))
                    {
                        sum1++;
                        counter++;
                    }
                    while ( (j1 + sum) < ivals1.Length && (ivals1[j1 + sum] == ivals2[j2]) )
                    {
                        sum++;
                        counter++;
                    }
                    --counter; //т.к. учли дважды сравнение ivals1[j1 + 0] == ivals2[j2 + 0]
                    for (int j = 0; j < sum; j++)
                    {
                        iRange1[i + j] = iRangeCommon + counter / 2.0;
                        j1++;
                    }
                    for (int j = 0; j < sum1; j++)
                    {
                        iRange2[ir2++] = iRangeCommon + counter / 2.0;
                        j2++;
                    }
                    for (int j = 0; j < sum1 + sum; j++)
                    {
                        iRangeCommon++;
                    }
                }
                else
                {
                    iRange1[i] = iRangeCommon++;
                    j1++;
                }





            }

            //DisplayData("Ранжированные файлы " + "j1=" + j1 + " j2=" + j2, false);
            //DisplayArray(iRange1);
           // DisplayArray(iRange2);

            if (j1 < iLen1)
            {
                for (int jtail = j1; jtail < iLen1; jtail++)
                    iRange1[jtail] = iRangeCommon++;

            }

            //  else
            if (j2 < iLen2)
            {
                for (int jtail = j2; jtail < iLen2; jtail++)
                    iRange2[jtail] = iRangeCommon++;

            }
            //DisplayData("Ранжированные файлы " + "j1=" + j1 + " j2=" + j2, false);
            //DisplayArray(iRange1);
            //DisplayArray(iRange2);
            //DisplayData("Максимум различия(" + jmax.ToString() + ")=" + Math.Round(dMax, 3).ToString(), false);
            //DisplayData("Sqrt((n1+n2)/n1*n2=" + Math.Round(dSqrt, 3).ToString(), false);
            //DisplayData("5%=" + Math.Round(dSqrt * 1.358, 3).ToString(), false);
            //DisplayData("10%=" + Math.Round(dSqrt * 1.224, 3).ToString(), false);

            // считаем квадраты разностей
            double sqR1 = 0;
            double[] iDiff1 = new double[iLen1];
            for (int i = 0; i < iLen1; i++)
            {
                iDiff1[i] =  iRange1[i] - i;/////
                sqR1 += (double)(iRange1[i] - i - 1) * (double)(iRange1[i] - i - 1);

            }

            //DisplayData("iDiff1 = ", false);
            //DisplayArray(iDiff1);
            DisplayData("sqR1 = " + sqR1, false);
            double sqR2 = 0;
            for (int i = 0; i < iLen2; i++)
            {
                sqR2 += (double)(iRange2[i] - i - 1) * (double)(iRange2[i] - i - 1);

            }

            DisplayData("sqR2 = " + sqR2, false);



            //взвешиваем
            double dMid = iLen1 * sqR1 + iLen2 * sqR2;
            DisplayData("dMid = " + dMid, false);
            double nPm = iLen1 + iLen2;
            double nMm = iLen1 * iLen2;
            double dRez1 = 1 / (double)((nMm) * (nPm));
            DisplayData("dRez1 = " + dRez1, false);
            double dRez = dRez1 * dMid - ((4 * nMm - 1) / (6 * nPm));
            DisplayData("dRez = " + dRez, false);
        }

    }
}
