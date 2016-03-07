using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace WindowsFormsApplication5
{
    static class Claster
    {

        /// <summary>
        /// кластеризация алгоритмом к средних для матстата
        /// </summary>
        /// <param name="descriptors"> значения среднеквадратичных отклонений для пикселей </param>
        /// <param name="bytes"> пиксели изображений </param>
        /// <returns> массив значений </returns>
        public static int[] K_Means_for_MatStat(double[] descriptors, ref Int32[,] bytes, int count_of_clasters)
        {

            int count_of_pixels = descriptors.GetLength(0);

            Int32[] num_of_claster = new Int32[count_of_pixels];  // номера кластеров для каждой фотки

            double max = 0, min = descriptors[0];

            for (int j = 0; j < count_of_pixels; j++)
            {
                if (min > descriptors[j]) min = descriptors[j];
                if (max < descriptors[j]) max = descriptors[j];
            }

            //double[] centroids = { min, (max + min) * 0.167, (max + min) * 0.333, (max + min) * 0.5, (max + min) * 0.667, (max + min) * 0.833, max };   // центроиды  //определяю начальные центроиды
            // double[] centroids = { min, min + (max - min) * 0.125, min + (max - min) * 0.25, min + (max - min) * 0.375, min + (max - min) * 0.5, min + (max - min) * 0.625, min + (max - min) * 0.75, min + (max - min) * 0.875, max };   // центроиды  //определяю начальные центроиды
            //TODO 
            //TODO 
            //TODO 

            int count_of_centroids = count_of_clasters;

            //TODO 
            //TODO //////////
            //TODO 


            double[] centroids = new double[count_of_centroids];
            centroids[0] = min;
            centroids[count_of_centroids - 1] = max;
            for (int j = 1; j < count_of_centroids - 1; j++)
            {
                centroids[j] = min + (max - min) * j / (count_of_centroids - 1);
            }

            Int32 count_of_itter = 20;

            //k-means step1
            for (int j = 0; j < count_of_pixels; j++)
            {
                num_of_claster[j] = NearestCentroid(centroids, descriptors[j]);
            }
            //k-means  step1

            //k-means
            for (int j = 0; j < count_of_itter; j++)
            {
                RecountingCentroids(ref centroids, descriptors, num_of_claster);

                for (int k = 0; k < count_of_pixels; k++)
                {
                    num_of_claster[k] = NearestCentroid(centroids, descriptors[k]);
                }
            }
            //k-means

            //формирую массив возвращаемых значений СКО
            
            int count_of_pixels_in_0_claster = 0;
             for (int k = 0; k < count_of_pixels; k++)
                {
                    if (num_of_claster[k] == 0)
                    {
                        count_of_pixels_in_0_claster++;
                    }
                }
            int[] returns_values = new int[count_of_pixels_in_0_claster];
            int counter = 0;
            for (int k = 0; k < count_of_pixels; k++)
            {
                if (num_of_claster[k] == 0)
                {
                    returns_values[counter++] = (int) descriptors[k];
                }
            }

            return  /*num_of_claster;*/  returns_values;
        }


        /// <summary>
        /// кластеризация алгоритмом к средних для фотографии областей
        /// </summary>
        /// <param name="descriptors"> значения среднеквадратичных отклонений для пикселей </param>
        /// <param name="bytes"> пиксели изображений </param>
       public static void K_Means(double[] descriptors, ref Int32[,] bytes,int count_of_clasters)
        {

            int count_of_pixels = descriptors.GetLength(0);

            Int32[] num_of_claster = new Int32[count_of_pixels];  // номера кластеров для каждой фотки

            double max = 0, min = descriptors[0];

            for (int j = 0; j < count_of_pixels; j++)
            {
                if (min > descriptors[j]) min = descriptors[j];
                if (max < descriptors[j]) max = descriptors[j];
            }

            //double[] centroids = { min, (max + min) * 0.167, (max + min) * 0.333, (max + min) * 0.5, (max + min) * 0.667, (max + min) * 0.833, max };   // центроиды  //определяю начальные центроиды
            // double[] centroids = { min, min + (max - min) * 0.125, min + (max - min) * 0.25, min + (max - min) * 0.375, min + (max - min) * 0.5, min + (max - min) * 0.625, min + (max - min) * 0.75, min + (max - min) * 0.875, max };   // центроиды  //определяю начальные центроиды
            //TODO 
            //TODO 
            //TODO 

            int count_of_centroids = count_of_clasters;

            //TODO 
            //TODO //////////
            //TODO 


            double[] centroids = new double[count_of_centroids];
            centroids[0] = min;
            centroids[count_of_centroids - 1] = max;
            for (int j = 1; j < count_of_centroids - 1; j++)
            {
                centroids[j] = min + (max - min) * j / (count_of_centroids - 1);
            }

            Int32 count_of_itter = 20;

            //k-means step1
            for (int j = 0; j < count_of_pixels; j++)
            {
                num_of_claster[j] = NearestCentroid(centroids, descriptors[j]);
            }
            //k-means  step1

            //k-means
            for (int j = 0; j < count_of_itter; j++)
            {
                RecountingCentroids(ref centroids, descriptors, num_of_claster);

                for (int k = 0; k < count_of_pixels; k++)
                {
                    num_of_claster[k] = NearestCentroid(centroids, descriptors[k]);
                }
            }
            //k-means


            
            Int32[] larger_claster = new Int32[count_of_centroids];
            larger_claster[0] = 0;
            larger_claster[count_of_centroids - 1] = 255;

            for (int j = 1; j < count_of_centroids - 1; j++)
            {
                larger_claster[j] = (byte)(j * 256 / (count_of_centroids - 1));
            }

            for (int i = 0; i < count_of_pixels; i++)
            {
                bytes[i, 0] = Color.FromArgb(larger_claster[num_of_claster[i]], larger_claster[num_of_claster[i]], larger_claster[num_of_claster[i]]).ToArgb();
            }
        }

        /// <summary>
        /// пересчет центроидов
        /// </summary>
        /// <param name="centroids">ссылка на массив центроидов</param>
        /// <param name="deviation_from_the_average"> массив среднеквадратичных отклонений от среднего </param>
        /// <param name="num_of_claster"> список кластеров </param>
        private static void RecountingCentroids(ref double[] centroids, double[] deviation_from_the_average, Int32[] num_of_claster)   // пересчет центроидов. просто присваиваю среднее
        {
            double[] sum = new double[centroids.GetLength(0)];
            int[] count = new int[centroids.GetLength(0)];

            for (int j = 0; j < centroids.GetLength(0); j++)
            {
                count[j] = 0;
                sum[j] = 0;
            }
            for (int i = 0; i < num_of_claster.GetLength(0); i++)
            {
                for (int j = 0; j < centroids.GetLength(0); j++)
                {
                    if (num_of_claster[i] == j)
                    {
                        count[j]++;
                        sum[j] += deviation_from_the_average[i];
                    }
                }
            }
            for (int j = 0; j < centroids.GetLength(0); j++)
            {
                centroids[j] = sum[j] / count[j];
            }

        }

        /// <summary>
        /// сравниваем расстояние объекта от центроидов и возвращаем номер центроида к которому он ближе
        /// </summary>
        /// <param name="centroids"> центроиды </param>
        /// <param name="x"> значение сравниваемое </param>
        /// <returns>номер ближайшего центроида</returns>
        private static int NearestCentroid(double[] centroids, double x)  // возвращает номер центроида расстояние до которого наименьшее.
        {
            /* хрень. тупанул. нужно чтобы многомерная была, иначе толку от метрики никакой!
            double lp_Metr = 1;
            byte i = 0;
            double min = Math.Pow(Math.Abs(Math.Pow(centroids[0], lp_Metr) - Math.Pow(x, lp_Metr)), 1 / (double)lp_Metr);
            for (byte j = 1; j < centroids.GetLength(0);  j++)
            {
                if (min > Math.Pow(Math.Abs(Math.Pow(centroids[j], lp_Metr) - Math.Pow(x, lp_Metr)), 1/(double) lp_Metr) )
                {
                    min = Math.Pow(Math.Abs(Math.Pow(centroids[j], lp_Metr) - Math.Pow(x, lp_Metr)), 1 / (double)lp_Metr);
                    //должно быть как ниже показано!
                    // min = Math.Pow(Math.Pow(centroids[j]-x, lp_Metr), 1 / (double)lp_Metr);
                    i = j;
                }
            }
            return i;
            */

            int i = 0;
            double min = Math.Abs(centroids[0] - x);
            for (int j = 1; j < centroids.GetLength(0); j++)
            {
                if (min > Math.Abs(centroids[j] - x))
                {
                    min = Math.Abs(centroids[j] - x);
                    i = j;
                }
            }
            return i;
        }



        //////////////////



        /// <summary>
        /// кластеризация минимальным деревом покрытия
        /// </summary>
        /// <param name="descriptors"> отклонения которые я буду разделять </param>
        /// <param name="bytes"> изображение </param>
        public static void Minimun_spanning_tree_clasterization(double[] descriptors, ref Int32[,] bytes, int count_of_clasters)
        {

            double[,] array_of_graph = new double[descriptors.GetLength(0), descriptors.GetLength(0)];

            for (int i = 0; i < descriptors.GetLength(0); i++)
            {
                for (int j = 0; j < descriptors.GetLength(0); j++)
                {
                    array_of_graph[i, j] = Math.Abs(descriptors[i] - descriptors[j]);
                }
            }

            double[] array_of_length = new double[(descriptors.GetLength(0) * descriptors.GetLength(0) - descriptors.GetLength(0)) / 2]; //то же самое что и аррей оф граф только в одну размерность и только над главной диагональю

            int[] array_of_numbers = new int[(descriptors.GetLength(0) * descriptors.GetLength(0) - descriptors.GetLength(0)) / 2];
            int count = 0;
            for (int i = 0; i < descriptors.GetLength(0) - 1; i++)
            {
                for (int j = i + 1; j < descriptors.GetLength(0); j++)
                {
                    array_of_length[count] = array_of_graph[i, j];
                    array_of_numbers[count] = i * descriptors.GetLength(0) + j;
                    count++;

                }
            }

            //sorting
            array_of_length = shellSort(array_of_length, ref array_of_numbers);
            
           
            //plotting antysycle graph
            int edgeCount = 0; //число ребер
            double[,] array_of_Minimum_spanning_graph = new double[descriptors.GetLength(0), descriptors.GetLength(0)];

            for (int i = 0; i < descriptors.GetLength(0); i++)
            {
                for (int j = 0; j < descriptors.GetLength(0); j++)
                {
                    array_of_Minimum_spanning_graph[i, j] = -1; //because 0 - is valid
                }
            }

            int includedCount = descriptors.GetLength(0); //число включ вершин

            int[] graph_nodes = new int[includedCount];

            for (int k = 0; k < array_of_numbers.GetLength(0); k++)
            {
                int x = array_of_numbers[k] / descriptors.GetLength(0);
                int y = array_of_numbers[k] - x * descriptors.GetLength(0);

                array_of_Minimum_spanning_graph[x, y] = array_of_length[k];
                array_of_Minimum_spanning_graph[y, x] = array_of_length[k];
                graph_nodes[x] = 1; graph_nodes[y] = 1;

                if (Checking_graph_on_antycycling(array_of_Minimum_spanning_graph, graph_nodes))
                {
                    array_of_Minimum_spanning_graph[x, y] = -1;
                    array_of_Minimum_spanning_graph[y, x] = -1;
                    graph_nodes[x] = 0; graph_nodes[y] = 0;
                }
                else
                {
                    edgeCount++;
                    if (edgeCount >= (descriptors.GetLength(0) - count_of_clasters)) break;
                }
                // checking graph on antycycle 
                // if(checking OK! ) edgeCount++; else array_of_Minimun_spanning clear

            }
            // }
            //end plotting
            //here a have a graph
            // i must to del max branches




            int u = 0;
            int numer = 0;
            List<int> stack = new List<int>();
            List<int> stack1 = new List<int>();
            int[] num_of_claster = new int[descriptors.GetLength(0)];

            for (int i = 0; i < graph_nodes.GetLength(0); i++)
            {
                if ( stack.IndexOf(i) < 0)
                {
                    stack.Add(i);
                    stack1.Add(-1);
                    u = stack.Count;
                    num_of_claster[i] = numer;
                    while (u <= stack.Count)
                    {
                        for (int j = 0; j < graph_nodes.GetLength(0); j++)
                        {
                            if (array_of_Minimum_spanning_graph[stack[u - 1], j] >= 0 && j != stack1[u - 1])
                            {
                                if (stack.IndexOf(j) < 0)
                                {
                                    stack.Add(j);
                                    stack1.Add(stack[u - 1]);
                                    num_of_claster[j] = numer;
                                }
                            }
                        }
                        u++;
                    }
                    numer++;
                }
            }

            Int32[] larger_claster = new Int32[count_of_clasters];
            //bool erp=Checking_graph_on_antycycling(array_of_Minimum_spanning_graph, graph_nodes);
            larger_claster[0] = 0;
            larger_claster[count_of_clasters - 1] = 255;
            for (int j = 1; j < count_of_clasters - 1; j++)
            {
                larger_claster[j] = (byte)(j * 256 / (count_of_clasters - 1));
            }
            int count_of_pixels = descriptors.GetLength(0);
            for (int i = 0; i < count_of_pixels; i++)
            {
                bytes[i, 0] = Color.FromArgb(larger_claster[num_of_claster[i]], larger_claster[num_of_claster[i]], larger_claster[num_of_claster[i]]).ToArgb();
            }


        }

        /// <summary>
        /// сортировка шелла
        /// </summary>
        /// <param name="vector"></param>
        /// <param name="nums"></param>
        public static double[] shellSort(double[] vector, ref int[] nums)
        {
            int step = vector.Length / 2;
            while (step > 0)
            {
                int i, j;
                for (i = step; i < vector.Length; i++)
                {
                    double value = vector[i];
                    int tmp = nums[i];
                    for (j = i - step; (j >= 0) && (vector[j] > value); j -= step)
                    {
                        vector[j + step] = vector[j];
                        nums[j + step] = nums[j];
                    }
                    nums[j + step] = tmp;
                    vector[j + step] = value;
                }
                step /= 2;
            }
            return vector;
        }


        //public only for test!

        /// <summary>
        /// проверка графа на ацикличность
        /// </summary>
        /// <param name="array_of_Minimum_spanning_graph"> матрица расстояний (смежности) </param>
        /// <param name="graph_nodes"> заполненные (посещенные вершины - для ускорения ) </param>
        /// <returns> тру если есть цикл фалс если нету цикла</returns>
        public static bool Checking_graph_on_antycycling(double[,] array_of_Minimum_spanning_graph, int[] graph_nodes) // true if graph has a cycle
        {
            // int[,] visited_nodes = new int[graph_nodes.GetLength(0),2];
            bool flag = false;
            int k = 0;
            // int[] stack = new int[graph_nodes.GetLength(0)];
            List<int> stack = new List<int>();
            List<int> stack1 = new List<int>();

            for (int i = 0; i < graph_nodes.GetLength(0); i++)
            {
                if (/*graph_nodes[i] == 1 &&*/ stack.IndexOf(i) < 0)
                {
                    stack.Add(i);
                    stack1.Add(-1);
                    k = stack.Count;
                    while (k <= stack.Count)
                    {
                        for (int j = 0; j < graph_nodes.GetLength(0); j++)
                        {
                            if (array_of_Minimum_spanning_graph[stack[k - 1], j] >= 0 && j != stack1[k - 1])
                            {
                                if (stack.IndexOf(j) < 0)
                                {
                                    stack.Add(j);
                                    stack1.Add(stack[k - 1]);
                                }
                                else { flag = true; }

                            }
                        }
                        k++;
                    }

                }
            }

            return flag;
        }



    }
}
