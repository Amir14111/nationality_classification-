using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace WindowsFormsApplication5
{
    static class RGB
    {
         

        /// <summary>
        /// метод запускающий выделение кластеров по среднеквадртичному отклонению.
        /// </summary>
        /// <param name="coeff_compression1">коэффициент сжатия фотографий</param>
        /// <param name="count_of_clasters1">число кластеров</param>
        public static Bitmap GetClasters(string[] paths, int width, int height, int count_of_clasters)
        { 

            Color col;
            Int32[,] bytes = null;
            // double[,] images_data = null;
            Int32[] newImageData = null;
            double[,] result = null;
            int i = 0, w = width, h = height;
            int count = 0;

            foreach (string path in paths)
            {
                Bitmap bmp = new Bitmap(path);

                //bmp = bmp.MakeBitmapSmaller();
                bmp = ChangeBitmapSize(bmp, w, h);

                w = bmp.Width;
                h = bmp.Height;

                if (bytes == null)
                    bytes = new Int32[w * h, paths.Length];
                //bytes - массив, первый индекс - номер пикселя, второй индекс - номер фотки, значение массива - цвет
                 
                if (result == null) result = new double[paths.Length, w * h];

                for (int y = 0; y < h; ++y)
                {
                    for (int x = 0; x < w; ++x)
                    {
                        Color color = bmp.GetPixel(x, y);
                        result[count, y * w + x] = ((color.R + color.G + color.B) / 3.0);
                    }
                }
                count++;

                Int32[] tmpimageData = bmp.ToImageData();

                for (uint y = 0; y < h; ++y)
                {
                    for (uint x = 0; x < w; ++x)
                    {
                        col = Color.FromArgb(tmpimageData[(int)y * w + (int)x]);

                        #region  для  компонент цвета Y (YUV),(R+G+B)/3 (RGB), V (HSV)
                        //bytes[(int)y * w + (int)x, i] = (byte)((col.R + col.G + col.B) / 3);                // RGB/3
                        bytes[(int)y * w + (int)x, i] = (byte)(0.299 * col.R + 0.587 * col.G + 0.114 * col.B);  // Y
                        //bytes[(int)y * w + (int)x, i] = (byte)(Math.Max(col.R, Math.Max(col.G, col.B)));    // V
                        #endregion
                    }
                }
                i++;
                GC.Collect();
            }

            // кластеризация для Y(YUV), (R+G+B)/3, V (HSV)
            double[] descriptors = GetDescriptorsFor_Component(bytes);  //кластеризация для Y,V,rgb/3 
            // кластеризация

            K_Means(descriptors, ref bytes, count_of_clasters);

            //после выоплнения этого метода в байтс на месте первой фотки будет нужное нам изображение
            //потом обратно в imagedata и в битмап

            newImageData = new Int32[w * h];

            for (uint y = 0; y < h; ++y)
            {
                for (uint x = 0; x < w; ++x)
                {
                    newImageData[(int)y * w + (int)x] = bytes[(int)y * w + (int)x, 0];
                }
            }

            Bitmap resultBmp = newImageData.ToBitmap(width, height);
            GC.Collect();
            return resultBmp;
        }

        static private double[] middle_Of_Pixels;

        /// <summary>
        /// получаем набор дескрипторов - среднеквадратических отклонений
        /// </summary>
        /// <param name="bytes"> пиксели изображений </param>
        /// <returns> массив - в ячейке значение сигмы для этого пикселя </returns>
        private static double[] GetDescriptorsFor_Component(int[,] bytes)
        {
            int count_of_pixels = bytes.GetLength(0);   //количество пикселей
            int count_of_photos = bytes.GetLength(1);   //количество картинок

            double[] descriptors = new double[count_of_pixels]; // дескрипторы - среднеквадратичное отклонение от (отклонения от среднего) 
            //для каждого пикселя (считай от количества фоток откл от среднего смотрим и для всего в 1 число загоняем)
            middle_Of_Pixels = new double[count_of_pixels];
            for (int i = 0; i < count_of_pixels; i++)
            {
                Int32[] massiv = new Int32[count_of_photos];
                for (int j = 0; j < count_of_photos; j++)
                    massiv[j] = bytes[i, j];  //bytes - массив, первый индекс - номер пикселя, второй индекс - номер фотки, значение массива - цвет

                //теперь в массиве massiv есть все значения i-того пикселя для всех j-фоток

                //считаю среднее для i-того пикселя. считаю разности от среднего для каждой фотографии. 3 кластера. (max, 1/2 (max+min) , min)
                //кластеризация к-средних. 

                double middle = 0;
                double sigma_sqr = 0;
                for (int j = 0; j < count_of_photos; j++)
                {
                    middle += massiv[j];
                }
                middle /= count_of_photos; //среднее
                middle_Of_Pixels[i] = middle;
                for (int j = 0; j < count_of_photos; j++)
                {
                    sigma_sqr += Math.Pow(Math.Abs(massiv[j] - middle), 2);
                }
                sigma_sqr /= count_of_photos;    /////////////////// при кластеризации не дает никаких изменений!
                descriptors[i] = Math.Sqrt(sigma_sqr);  //среднеквадратическое отклонение

            }

            return descriptors;

        }

        /// <summary>
        /// Преобразование массива чисел в Bitmap
        /// </summary>
        /// <param name="imageData"></param>
        /// <param name="w"></param>
        /// <param name="h"></param>
        /// <returns></returns>
        private static Bitmap ToBitmap(this Int32[] imageData, int w, int h)
        {
            Bitmap bmp = new Bitmap(w, h);
            BitmapData lockData = bmp.LockBits(new Rectangle(0, 0, w, h), System.Drawing.Imaging.ImageLockMode.ReadWrite, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            Marshal.Copy(imageData, 0, lockData.Scan0, imageData.Length);
            bmp.UnlockBits(lockData);

            imageData = null;
            lockData = null;
            return bmp;
        }

        /// <summary>
        /// изменение размера bitmap
        /// </summary>
        /// <param name="bmp"></param>
        /// <param name="w"></param>
        /// <param name="h"></param>
        /// <returns></returns>
        private static Bitmap ChangeBitmapSize(Bitmap bmp, int w, int h)
        {
            Bitmap newBitmap = new Bitmap(w, h);  //пусть будут пока такие
            Graphics graphic = Graphics.FromImage(newBitmap);
            graphic.InterpolationMode = InterpolationMode.HighQualityBicubic;
            graphic.DrawImage(bmp, 0, 0, w, h);

            graphic.Dispose();
            GC.Collect();
            return newBitmap;
        }


        /// <summary>
        /// Преобразование Bitmap - сжатие
        /// </summary>
        /// <param name="bmp"></param>
        /// <returns></returns>
        private static Bitmap MakeBitmapSmaller(this Bitmap bmp)
        {

            //TODO 
            //TODO 
            int x = 4;
            //TODO 
            //TODO 

            Bitmap newBitmap = new Bitmap(3 * x, 4 * x);  //пусть будут пока такие
            Graphics graphic = Graphics.FromImage(newBitmap);
            graphic.InterpolationMode = InterpolationMode.HighQualityBicubic;
            graphic.DrawImage(bmp, 0, 0, 3 * x, 4 * x);

            graphic.Dispose();
            GC.Collect();
            return newBitmap;
        }

        /// <summary>
        /// Преобразование Bitmap в массив чисел
        /// </summary>
        /// <param name="bmp"></param>
        /// <returns></returns>
        private static Int32[] ToImageData(this Bitmap bmp)//, int w, int h)
        {
            int w = bmp.Width;
            int h = bmp.Height;
            BitmapData lockData = bmp.LockBits(new Rectangle(0, 0, w, h), System.Drawing.Imaging.ImageLockMode.ReadWrite, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            Int32[] imageData = new Int32[w * h];
            Marshal.Copy(lockData.Scan0, imageData, 0, imageData.Length);
            bmp.UnlockBits(lockData);
            lockData = null;

            GC.Collect();
            return imageData;
        }

        /// <summary>
        /// кластеризация алгоритмом к средних для фотографии областей
        /// </summary>
        /// <param name="descriptors"> значения среднеквадратичных отклонений для пикселей </param>
        /// <param name="bytes"> пиксели изображений </param>
        public static void K_Means(double[] descriptors, ref Int32[,] bytes, int count_of_clasters)
        {

            int count_of_pixels = descriptors.GetLength(0);

            Int32[] num_of_claster = new Int32[count_of_pixels];  // номера кластеров для каждой фотки

            double max = 0, min = descriptors[0];

            for (int j = 0; j < count_of_pixels; j++)
            {
                if (min > descriptors[j]) min = descriptors[j];
                if (max < descriptors[j]) max = descriptors[j];
            }

            int count_of_centroids = count_of_clasters;

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


    }
}
