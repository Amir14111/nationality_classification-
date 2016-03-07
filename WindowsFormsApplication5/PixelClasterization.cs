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
using Accord.Statistics.Analysis;

namespace WindowsFormsApplication5
{
    static class PixelClasterization
    {
        private static int coeff_compression = 15;
        private static int count_of_clasters = 0;

        /// <summary>
        /// метод запускающий выделение кластеров по среднеквадртичному отклонению.
        /// </summary>
        /// <param name="coeff_compression1">коэффициент сжатия фотографий</param>
        /// <param name="count_of_clasters1">число кластеров</param>
            public static Bitmap GetClasters(int coeff_compression1, int count_of_clasters1)
             {
                coeff_compression = coeff_compression1;
                count_of_clasters = count_of_clasters1;

                Color col;
                Int32[,] bytes = null;
               // double[,] images_data = null;
                Int32[] newImageData = null;
                double[,] result = null;
                int i = 0, w = 0, h = 0;
                string mainPath = "C:\\Users\\Amigo's\\Desktop\\pic\\wrap";
                //string mainPath = "C:\\Users\\Amigo's\\Desktop\\h";

                string[] paths = Directory.GetFiles(mainPath);
                int count = 0;
                
                foreach (string path in paths)
                {
                    Bitmap bmp = new Bitmap(path);
                    
                    bmp = bmp.MakeBitmapSmaller();

                    w = bmp.Width;
                    h = bmp.Height;
                    
                    if (bytes == null)
                        bytes = new Int32[w * h, paths.Length];
                    //bytes - массив, первый индекс - номер пикселя, второй индекс - номер фотки, значение массива - цвет

                   // images_data = new double[w * h, paths.Length];

            int width = bmp.Width;
            int height = bmp.Height;
            if (result == null) result = new double[ paths.Length , w * h];
                    
            for (int y = 0; y < h; ++y)
            {
                for (int x = 0; x < w; ++x)
                {
                    Color color = bmp.GetPixel(x, y);
                    result[ count , y * w + x] = ((color.R + color.G + color.B) / 3.0);
                }
            }
            count++;
            //----------------test-------------------//
 ////////////////////////////////////////test
                   // testing graph on antycycling
                   // double[,] array_of_Minimum_spanning_graph={{-1, 0.1 ,-1, 0.1},{ 0.1,-1, 8 ,0.3},{-1,8,-1,-1},{0.1,0.3,-1,-1}};
                   // int[] graph_nodes= {1,1,1,1};
                   // bool kok = Checking_graph_on_antycycling(array_of_Minimum_spanning_graph, graph_nodes); //true - if cycle^ false if not
                   // int g = 0;
                   // test
////////////////////////////////////////

                    //////////////////////////////////////////// test
                    //testing Minimun_spanning_tree_clasterization
                    // double[] testdescriptors = { 1, 2, 6, 8, 5, 10, 16, 14, 0.3, 2, 0.6, 15, 30  };
                    // Claster.Minimun_spanning_tree_clasterization(testdescriptors, ref bytes, 3);
                    // testing Minimun_spanning_tree_clasterization
                    //////////////////////////////////////////////test
            //----------------test-------------------//  
                    
                    Int32[] tmpimageData = bmp.ToImageData();

                    for (uint y = 0; y < h; ++y)
                    {
                        for (uint x = 0; x < w; ++x)
                        {
                            col = Color.FromArgb(tmpimageData[(int)y * w + (int)x]);

                            #region  для  компонент цвета Y (YUV),(R+G+B)/3 (RGB), V (HSV)
                            //bytes[(int)y * w + (int)x, i] = (byte)((col.R + col.G + col.B) / 3);                // RGB/3
                            bytes[(int)y * w + (int)x, i] = (byte) (0.299*col.R +0.587* col.G + 0.114* col.B);  // Y
                            //bytes[(int)y * w + (int)x, i] = (byte)(Math.Max(col.R, Math.Max(col.G, col.B)));    // V
                            #endregion
                        }
                    }
                    i++;
                    GC.Collect();
                }


                //----------------test Вычисление главных компонент-------------------//
                ////////////////////////
               /*
                var pca = new PrincipalComponentAnalysis(result, AnalysisMethod.Standardize);
                pca.Compute();

                Color cl;
                var res = pca.Eigenvalues;
                var eee = pca.Result;
                var res1 = pca.ComponentMatrix;
                double[,] actual = pca.Transform(result, 1);
               // alglib.rmatrixgemm(actual.GetLength(0), result.GetLength(1), actual.GetLength(1), 1, actual, 0, 0, 0, result, 0, 0, 0, 0, ref actual, 0, 0);
                


                newImageData = new Int32[actual.GetLength(0) *10];

                for (uint y = 0; y < actual.GetLength(0); y++)
                {
                    for (uint x = 0; x < 10; x++)
                    {
                        cl = Color.FromArgb((byte)actual[y, x], (byte)actual[y, x], (byte)actual[y, x]);
                        newImageData[(int)y * 1 + (int)x] = cl.ToArgb();
                    }
                }

                Bitmap resultBmp1 = newImageData.ToBitmap(actual.GetLength(0), 10);
                Bitmap resultBmp2 = ChangeBitmapSize(resultBmp1, 160, 190);
                return resultBmp2;
                //////////////////
                */

                //----------------test  Вычисление главных компонент-------------------//


                // кластеризация для Y(YUV), (R+G+B)/3, V (HSV)
                double[] descriptors = GetDescriptorsFor_Component(bytes);  //кластеризация для Y,V,rgb/3 
                // кластеризация

                Claster.K_Means(descriptors, ref bytes, count_of_clasters);
                //Claster.Minimun_spanning_tree_clasterization(descriptors, ref bytes, count_of_clasters);
 
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

                Bitmap resultBmp = newImageData.ToBitmap(w, h);
                //SuperSave(resultBmp, "C:\\Users\\Амир\\Desktop\\resultBmp.jpg");
                resultBmp = ChangeBitmapSize(newImageData.ToBitmap(w, h), 160, 190);
                GC.Collect();
                //SuperSave(resultBmp, "C:\\Users\\Амир\\Desktop\\resultBmp.jpg");
                //SuperSave(resultBmp, mainPath + "\\resultBmp.jpg");
                return resultBmp;
             }




            /// <summary>
            /// метод запускающий выделение кластеров по среднеквадртичному отклонению.
            /// </summary>
            /// <param name="coeff_compression1">коэффициент сжатия фотографий</param>
            /// <param name="count_of_clasters1">число кластеров</param>
            public static int[] GetClastersForMatStat(int coeff_compression1, int count_of_clasters1, string pathOfPhotos)
            {
                coeff_compression = coeff_compression1;
                count_of_clasters = count_of_clasters1;

                Color col;
                Int32[,] bytes = null;
                // double[,] images_data = null;
                Int32[] newImageData = null;
                double[,] result = null;
                int i = 0, w = 0, h = 0;
                string mainPath = pathOfPhotos; // "C:\\Users\\Amigo's\\Desktop\\pic\\wrap";
                //string mainPath = "C:\\Users\\Amigo's\\Desktop\\h";

                string[] paths = Directory.GetFiles(mainPath);
                int count = 0;

                foreach (string path in paths)
                {
                    Bitmap bmp = new Bitmap(path);

                    bmp = bmp.MakeBitmapSmaller();

                    w = bmp.Width;
                    h = bmp.Height;

                    if (bytes == null)
                        bytes = new Int32[w * h, paths.Length];
                    //bytes - массив, первый индекс - номер пикселя, второй индекс - номер фотки, значение массива - цвет

                    // images_data = new double[w * h, paths.Length];

                    int width = bmp.Width;
                    int height = bmp.Height;
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
                            //bytes[(int)y * w + (int)x, i] = (byte)((col.R + col.G + col.B) / 3);                 // RGB/3
                            bytes[(int)y * w + (int)x, i] = (byte)(0.299 * col.R + 0.587 * col.G + 0.114 * col.B); // Y
                            //bytes[(int)y * w + (int)x, i] = (byte)(Math.Max(col.R, Math.Max(col.G, col.B)));     // V
                            #endregion
                        }
                    }
                    i++;
                    GC.Collect();
                }
                  
                // кластеризация для Y(YUV), (R+G+B)/3, V (HSV)
                double[] descriptors = GetDescriptorsFor_Component(bytes);  //кластеризация для Y,V,rgb/3 
                // кластеризация

                int[] returnValues = Claster.K_Means_for_MatStat(descriptors, ref bytes, count_of_clasters);
                 /*
                // если returnValues - номера кластеров то!
                int count_of_pixels_in_0_claster = 0;
                for (int k = 0; k < returnValues.GetLength(0); k++)
                {
                    if (returnValues[k] == 0)
                    {
                        count_of_pixels_in_0_claster++;
                    }
                }
                int[] returns_values = new int[count_of_pixels_in_0_claster];
                int counter = 0;
                for (int k = 0; k < returnValues.GetLength(0); k++)
                {
                    if (returnValues[k] == 0)
                    {
                        returns_values[counter++] =  (int)middle_Of_Pixels[k];
                    }
                }
                // если returnValues - номера кластеров то! возвращаем средние значения
                */
                return returnValues; //returns_values;
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
            /// Преобразование Bitmap - сжатие
            /// </summary>
            /// <param name="bmp"></param>
            /// <returns></returns>
            private static Bitmap MakeBitmapSmaller(this Bitmap bmp)
            {   

                //TODO 
                //TODO 
                int x = coeff_compression;
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
            /// изменение размера bitmap
            /// </summary>
            /// <param name="bmp"></param>
            /// <param name="w"></param>
            /// <param name="h"></param>
            /// <returns></returns>
            private static Bitmap ChangeBitmapSize( Bitmap bmp , int w, int h)
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
            /// Преобразование массива чисел в Bitmap
            /// </summary>
            /// <param name="imageData"></param>
            /// <param name="w"></param>
            /// <param name="h"></param>
            /// <returns></returns>
            private static Bitmap ToBitmap(this Int32[] imageData, int w, int h)
            {
               // Bitmap ff=new Bitmap(1,4);
                Bitmap bmp = new Bitmap(w, h);
                BitmapData lockData = bmp.LockBits(new Rectangle(0, 0, w, h), System.Drawing.Imaging.ImageLockMode.ReadWrite, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
                Marshal.Copy(imageData, 0, lockData.Scan0, imageData.Length);
                bmp.UnlockBits(lockData);

                imageData = null;
                lockData = null;
                return bmp;
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
            /// Сохраняет указанный Bitmap по указанному пути используя JPEG кодек при наивысшем качестве.
            /// </summary>
            /// <param name="bmp"></param>
            /// <param name="put"></param>
            public static void SuperSave(Bitmap bmp, String put)
            {
                ImageCodecInfo myImageCodecInfo;
                System.Drawing.Imaging.Encoder myEncoder;
                EncoderParameter myEncoderParameter;
                EncoderParameters myEncoderParameters;
                // Get an ImageCodecInfo object that represents the JPEG codec.
                myImageCodecInfo = GetEncoderInfo("image/jpeg");
                // Create an Encoder object based on the GUID
                // for the Quality parameter category.
                myEncoder = System.Drawing.Imaging.Encoder.Quality;
                // Create an EncoderParameters object.
                // An EncoderParameters object has an array of EncoderParameter
                // objects. In this case, there is only one
                // EncoderParameter object in the array.
                myEncoderParameters = new EncoderParameters(1);
                // Save the bitmap as a JPEG file with quality level 25.
                myEncoderParameter = new EncoderParameter(myEncoder, 100L);
                myEncoderParameters.Param[0] = myEncoderParameter;
                bmp.Save(put, myImageCodecInfo, myEncoderParameters);
            }

            /// <summary>
            /// Получение информации о кодеке
            /// </summary>
            /// <param name="mimeType"></param>
            /// <returns></returns>
            private static ImageCodecInfo GetEncoderInfo(String mimeType)
            {
                int j;
                ImageCodecInfo[] encoders;
                encoders = ImageCodecInfo.GetImageEncoders();
                for (j = 0; j < encoders.Length; ++j)
                {
                    if (encoders[j].MimeType == mimeType)
                        return encoders[j];
                }
                return null;
            }
    }
}
