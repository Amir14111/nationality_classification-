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
    static class PCA_for_images
    {
        //необходимо чтобы фотографии были одного размера

        static private int width;
        static public double[] averange_img = null;
        public static Bitmap[] aver_img_btm = null;
        private static double[,] reconstructed_image;
        static public int[] nums = null;
        public static double[,] eigenfaces = null; //[ размерность фотки , число фоток]

        /// <summary>
        /// Преобразование Bitmap - сжатие
        /// </summary>
        /// <param name="bmp"></param>
        /// <returns></returns>
        /// TODO !! эта функция дублируется в PixelClasterization!!!!
        private static Bitmap CompressBitmap(this Bitmap bmp)
        {

            //TODO 
             int x = 8;
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
        /// получаем набор дескрипторов - среднеквадратических отклонений
        /// </summary>
        /// <param name="bytes"> пиксели изображений </param>
        /// <returns> массив - в ячейке значение сигмы для этого пикселя </returns>
        /// TODO !! эта функция дублируется в PixelClasterization!!!!
        private static double[] GetDescriptorsFor_Component(double[,] bytes)    
        {
            int count_of_pixels = bytes.GetLength(0);   //количество пикселей
            int count_of_photos = bytes.GetLength(1);   //количество картинок

            double[] descriptors = new double[count_of_pixels]; // дескрипторы - среднеквадратичное отклонение от (отклонения от среднего) 
            //для каждого пикселя (считай от количества фоток откл от среднего смотрим и для всего в 1 число загоняем)

            for (int i = 0; i < count_of_pixels; i++)
            {
                double[] massiv = new double[count_of_photos];
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
        /// считываение изображений
        /// </summary>
        /// <param name="paths"></param>
        /// <returns>массив изображений [колич фоток, колич пикселей]</returns>
        ///  TODO !! эта функция дублируется в PixelClasterization!!!!
        public static double[,] BitmapToByte(string[] paths)
        {
            int n = 0;
            double[,] res = null;
            int counter = 0;
            foreach (string path in paths)
            {
                Bitmap bmp = new Bitmap(path);

              //  bmp = bmp.MakeBitmapSmaller();   // убрать сжатие???
                width = bmp.Width;
                

                int height = bmp.Height;
                if (n == 0) n = width*height;
                if (res == null) 
                {
                    res = new double[paths.GetLength(0),height * width];
                }

                BitmapData lockData = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height), System.Drawing.Imaging.ImageLockMode.ReadWrite, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
                Int32[] imageData = new Int32[bmp.Width * bmp.Height];
                Marshal.Copy(lockData.Scan0, imageData, 0, imageData.Length);

                for (int y = 0; y < bmp.Height; ++y)
                {
                    for (int x = 0; x < bmp.Width; ++x)
                    {
                        Color col = Color.FromArgb(imageData[y * bmp.Width + x]);
                        res[counter, y * bmp.Width + x] = (col.R + col.G + col.B) / 3.0 ;
                    }
                }

                counter++; //номер фотографии
                Marshal.Copy(lockData.Scan0, imageData, 0, imageData.Length);
                bmp.UnlockBits(lockData);

                lockData = null;
                imageData = null;

                GC.Collect();
            }
            /*
            var res1 = new double[res.GetLength(1), res.GetLength(0)];
            for (int i = 0; i < res.GetLength(1); i++)
            {
                for (int j = 0; j < res.GetLength(0); j++)
                {
                    res1[i,j] = res[j,i];
                }
            }
            double[] mean_square_deviation = GetDescriptorsFor_Component(res1);  //среднекв отклонения по пикселям

            for (int i = 0; i < mean_square_deviation.GetLength(0); i++)
            {
                for (int j = 0; j < res.GetLength(0); j++)
                {
                    res[j, i] /= mean_square_deviation[i];
                }
            }*/
            
            return res;
        }

        /// <summary>
        /// перевод массива в изображение Bitmap
        /// </summary>
        /// <param name="rgb">массив изображений</param>
        /// <returns>массив Bitmap</returns>
        ///  TODO !! эта функция дублируется в PixelClasterization!!!!
        public static Bitmap[] ByteToBitmap(double[,] rgb)
        {
           
            int count_of_images = rgb.GetLength(1),
                length = rgb.GetLength(0);
            int height = length/width;
            Bitmap[] arr_result= new Bitmap[count_of_images];

            //begin пропорционально представляю результаты, чтобы были красивые изображения
            for (int i = 0; i < rgb.GetLength(1); i++)
            {
                double max = 0.0, min = double.MaxValue;

                for (int k = 0; k < rgb.GetLength(0); k++)
                {
                    if (rgb[k, i] > max) max = rgb[k, i];
                    if (rgb[k, i] < min) min = rgb[k, i];
                }

                for (int k = 0; k < rgb.GetLength(0); k++)
                {
                    rgb[k, i] = 255 * (rgb[k, i] - min) / (max - min);
                }
            } 
            // end пропорционально представляю результаты, чтобы были красивые изображения

            for (int i = 0; i < count_of_images; i++)
            {
               Bitmap result = new Bitmap(width, height, PixelFormat.Format24bppRgb);
               for (int y = 0; y < height; ++y)
               {
                  for (int x = 0; x < width; ++x)
                  {
                      result.SetPixel
                              (
                                  x, y, Color.FromArgb
                                                  (
                                                  (byte)(rgb[x + y * width, i] ),
                                                  (byte)(rgb[x + y * width, i]),
                                                  (byte)(rgb[x + y * width, i])
                                                  )
                              );
                    }
                }
               arr_result[i] = result.ChangeBitmapSize(width, height);
            }


            return arr_result;
        }

        /// <summary>
        /// изменение размера bitmap
        /// </summary>
        /// <param name="bmp"></param>
        /// <param name="w"></param>
        /// <param name="h"></param>
        /// <returns></returns>
        ///  TODO !! эта функция дублируется в PixelClasterization!!!!
        public static Bitmap ChangeBitmapSize(this Bitmap bmp, int w, int h)
        {
            Bitmap newBitmap = new Bitmap(w, h);  //пусть будут пока такие
            Graphics graphic = Graphics.FromImage(newBitmap);
            graphic.InterpolationMode = InterpolationMode.HighQualityBicubic;
            graphic.DrawImage(bmp, 0, 0, w, h);

            graphic.Dispose();
            GC.Collect();
            return newBitmap;
        }


        public static Bitmap[] Pca (double[,] images)
        {

            int m = images.GetLength(0); //число фоток
            int n = images.GetLength(1); //одно фото колич пикселей 
            
            // вычесть среднее
            averange_img = new double[n];
            for (int i = 0; i < n; i++)
            {
                averange_img[i] = 0;
            }
            double[,] av_img = new double[n,1];

            for (int i = 0; i < m; i++)
            {
                for (int j = 0; j <n; j++)
                {
                    averange_img[j] += images[i, j]/m;
                }
            }

            for (int j = 0; j < n; j++)
            {
                av_img[j, 0] = averange_img[j];
            }

            aver_img_btm = ByteToBitmap(av_img);

            for (int i = 0; i < m; i++)
            {
                for (int j = 0; j < n; j++)
                {
                    images[i, j] -= averange_img[j];
                }
            }

            #region  делаю единичной дисперсию  - стандартизую
            var images1 = new double[images.GetLength(1),images.GetLength(0)];

            for (int i = 0; i < images.GetLength(0); i++)
            {
                for (int j = 0; j < images.GetLength(1); j++)
                {
                    images1[j, i] = images[i, j];
                }
            }

            double[] mean_square_deviation = GetDescriptorsFor_Component(images1);  //среднекв отклонения по пикселям
            
            GC.Collect();
            for (int i = 0; i < images.GetLength(0); i++)
            {
                for (int j = 0; j < images.GetLength(1); j++)
                {
                    if (mean_square_deviation[j]>0) images[i, j] /= mean_square_deviation[j];
                }
            }
            //TODO 
            #endregion

            double[,] cov = Matrix_Multiple(images, Trancpose_Matrix(images));  //  ков матрица на самом деле A'A. Но у меня как на хабре в статье AA'
          
            /**/
            
            double[] eigenvalues = null;
            double[] tmp1 = null;
            double[,] tmp2 = null;
            double[,] right_eigenvectors = null;
              
            if (alglib.rmatrixevd(cov, cov.GetLength(0), 1, out eigenvalues, out tmp1, out tmp2, out right_eigenvectors))
            {
                // right_eigvectors[значения внутри вектора, номер вектора]
                nums = new int[eigenvalues.GetLength(0)];
                for (int i = 0; i < eigenvalues.GetLength(0); i++)
                {
                    nums[i] = eigenvalues.GetLength(0)-i-1;
                }
                eigenvalues=Claster.shellSort(eigenvalues, ref nums);

                #region test проверка на правильность нахождения сз
                //----------------test-------------------//
                /* проверка на правильность нахождения сз
                double sum_of_diagonal_values = 0;
                for (int i = 0; i < cov.GetLength(0); i++)
                {
                    sum_of_diagonal_values += cov[i, i];
                }

                double sum_of_eigen_values = 0;
                for (int i = 0; i < eigenvalues.GetLength(0); i++)
                {
                    sum_of_eigen_values += eigenvalues[i];
                }
                
                if (sum_of_diagonal_values == sum_of_eigen_values) var r = 2;
                */
                //проверка на правильность нахождения сз


                // проверка на соотв сз своему св
                /*
                double[,] eigvect = new double[right_eigenvectors.GetLength(1), 1];
                for (int i = 0; i < right_eigenvectors.GetLength(1); i++)
                {
                    for (int j = 0; j < right_eigenvectors.GetLength(1); j++)
                    {
                        eigvect[i, j] = right_eigenvectors[i, nums[2]];
                    }
                    
                }
                var tm = Matrix_Multiple(cov, eigvect);
                for (int i = 0; i < right_eigenvectors.GetLength(1); i++)
                {
                    eigvect[i,0] *= eigenvalues[2];
                }
                int EV_EV = 3;
                */
                //сравнивал tm и eigenvect 
                //проверка на соотв сз своему св
                //----------------test-------------------//
                #endregion test

                //собственный вектор исходной матрицы AA' нахожу так:
                tmp2 = Matrix_Multiple(Trancpose_Matrix(images),  right_eigenvectors); //eigvect); //транспонировал потому что у меня размерности наоборот с тем что пишут в статьях
               // насчет транспонирования. в статье с хабра которую я копировал про МГК тоже транспонирование, так что с реализацией не ошибся вроде)
                eigenfaces = tmp2;
            }

            return ByteToBitmap(tmp2);
        }

        /// <summary>
        /// Транспонирование матрицы
        /// </summary>
        /// <param name="a1">матрица</param>
        /// <returns>транспонированную матрицу</returns>
        /// TODO !! ее можно в общий класс математики
        public static double[,] Trancpose_Matrix(double[,] a1)
        {
            int n1 = a1.GetLength(0);
            int n2 = a1.GetLength(1);
            double[,] res = new double[n2,n1];
            for (int i = 0; i < n1; i++)
            {
                for (int j = 0; j < n2; j++)
                {
                    res[j, i] = a1[i, j];
                }
            }
            return res;
        } 
         
        /// <summary>
        /// умножение матриц (а1(1) == а2(0), строка равна столбцу)
        /// </summary>
        /// <param name="a1"></param>
        /// <param name="a2"></param>
        /// <returns></returns>
        /// TODO !! ее можно в общий класс математики
        public static double[,] Matrix_Multiple(double[,] a1, double[,] a2  )
        {
            
            int n1 = a1.GetLength(0);
            int n2 = a1.GetLength(1);
            
            int k1 = a2.GetLength(0);
            int k2 = a2.GetLength(1);

            double[,] res = new double[n1, k2];  // проверку  k1==n2 сделать по хорошему надо бы... но у меня матрицы там норм стоят если че)

            for (int i = 0; i < n1; i++)
            {
                for (int j = 0; j < k2; j++)
                {
                    double mult = 0;
                    for (int k = 0; k < n2; k++)
                    {
                        mult += a1[i, k] * a2[k, j];
                    }
                    res[i, j] = mult;
                }
            }
            return res;
        } 
         
        /// <summary>
        /// Построение изображения спроецированного в пространство главных компонент.
        /// </summary>
        /// <param name="num_of_test_image">номер изображения которое будем проецировать по порядку из массива paths</param>
        /// <param name="number_of_subdeminsion">число ГК, размерность подпространства</param>
        /// <param name="sorting_eigenvalues_on_decrease"> собственные вектора сортированы по уменьшению собственного зничения (if true) </param>
        public static void Reconstruct_Projected_Image(byte num_of_test_image,  int number_of_subdeminsion, bool sorting_eigenvalues_on_decrease)
        {
            //byte num_of_test_image = 0; //номер изображения которое будем проецировать по порядку из массива paths
            try
            {
                if (eigenfaces == null) // проверка на то, что собственные лица уже построены!
                {
                    return;
                }
                if (number_of_subdeminsion > eigenfaces.GetLength(1)) number_of_subdeminsion = eigenfaces.GetLength(1);


                string mainPath = "C:\\Users\\Amigo's\\Desktop\\pic\\PCA_test\\test_for_projection";
                //string mainPath = "C:\\Users\\Amigo's\\Desktop\\recogn\\testimage";
                string[] paths = Directory.GetFiles(mainPath);


                //TODO размерность тестового изображения равна среднему 

                var testFace = BitmapToByte(paths);
                //вычли среднее
                for (int j = 0; j < paths.GetLength(0); j++)
                {
                    for (int i = 0; i < testFace.GetLength(1); i++)
                    {
                        testFace[j, i] -= averange_img[i];
                    }
                }
                //вычли среднее

                var weight_coefficients = Matrix_Multiple(Trancpose_Matrix(eigenfaces), Trancpose_Matrix(testFace)); // weight_coefficients_of_eigenfaces
                // weight_coefficients [число собственных векторов, колич тестовых фотографий]
                //var weight_coefficients = Matrix_Multiple( (testFace), (eigenfaces));


                 reconstructed_image = new double[eigenfaces.GetLength(0), 1];

                double r = 0;
                for (int i = 0; i < weight_coefficients.GetLength(0); i++)
                {
                    r += weight_coefficients[i, 0]; //смотрю суммы чтобы можно было оценить в какую сторону для этого лица отклонение весов СВ
                }

                int k = 2;
                for (int i = 0; i < reconstructed_image.GetLength(0); i++)
                {
                    for (int j = 0; j < reconstructed_image.GetLength(1); j++)
                    {
                        reconstructed_image[i, j] = 0;
                    }
                }

                //построение проекции
                
                if (sorting_eigenvalues_on_decrease)//построение проекции по убыванию дисперсии
                {
                    for (int i = 0; i < eigenfaces.GetLength(0); i++) //по каждому пикселю
                    {
                        for (int j = eigenfaces.GetLength(1) - 1; j >= (eigenfaces.GetLength(1) - number_of_subdeminsion); j--) //число собственных лиц (базис) номер! TODO number_of_subdeminsion
                        {
                            reconstructed_image[i, 0] += eigenfaces[i, j] * weight_coefficients[j, num_of_test_image]; // weight_coefficients[j, номер изображения в тестовой выборке] 
                            //reconstructed_image[i, 0] += eigenfaces[i, j] * weight_coefficients[ num_of_test_image, j];
                        }

                    }
                }
                else //по возрастанию дисперсии
                {
                    for (int i = 0; i < eigenfaces.GetLength(0); i++) //по каждому пикселю
                    {
                        for (int j = 0; j < number_of_subdeminsion; j++) //число собственных лиц (базис) номер! TODO number_of_subdeminsion
                        {
                            reconstructed_image[i, 0] += eigenfaces[i, j] * weight_coefficients[j, num_of_test_image]; // weight_coefficients[j, номер изображения в тестовой выборке]
                        }
                    }
                }

               /* ////////////////// ----------------------считаем разность между проек в n послед компо и проекц в перв n компонен--------------------------------------------
                double[,] reconstructed_image1 = new double[eigenfaces.GetLength(0), 1];
                for (int i = 0; i < reconstructed_image.GetLength(0); i++)
                {
                    for (int j = 0; j < reconstructed_image.GetLength(1); j++)
                    {
                        reconstructed_image1[i, j] = 0;
                    }
                }

                    for (int i = 0; i < eigenfaces.GetLength(0); i++) //по каждому пикселю
                    {
                        for (int j = eigenfaces.GetLength(1) - 1; j >= (eigenfaces.GetLength(1) - number_of_subdeminsion); j--) //число собственных лиц (базис) номер! TODO number_of_subdeminsion
                        {
                            reconstructed_image[i, 0] += eigenfaces[i, j] * weight_coefficients[j, num_of_test_image]; //первые компоненты
                        }

                    }
                
                    for (int i = 0; i < eigenfaces.GetLength(0); i++) //по каждому пикселю
                    {
                        for (int j = 0; j < number_of_subdeminsion; j++) //число собственных лиц (базис) номер! TODO number_of_subdeminsion
                        {
                            reconstructed_image1[i, 0] += eigenfaces[i, j] * weight_coefficients[j, num_of_test_image]; //последние компоненты
                        }
                    }

                ///
                    double max = 0.0, min = double.MaxValue;

                    for (int i = 0; i < reconstructed_image.GetLength(0); i++)
                    {
                        if (reconstructed_image[i, 0] > max) max = reconstructed_image[i, 0];
                        if (reconstructed_image[i, 0] < min) min = reconstructed_image[i, 0];
                    }

                    for (int i = 0; i < reconstructed_image.GetLength(0); i++)
                    {
                        reconstructed_image[i, 0] = 255 * (reconstructed_image[i, 0] - min) / (max - min);
                    }
                max = 0.0;
                min = double.MaxValue;

                    for (int i = 0; i < reconstructed_image1.GetLength(0); i++)
                    {
                        if (reconstructed_image1[i, 0] > max) max = reconstructed_image1[i, 0];
                        if (reconstructed_image1[i, 0] < min) min = reconstructed_image1[i, 0];
                    }

                    for (int i = 0; i < reconstructed_image1.GetLength(0); i++)
                    {
                        reconstructed_image1[i, 0] = 255 * (reconstructed_image1[i, 0] - min) / (max - min);
                    }
                ///


                    for (int i = 0; i < reconstructed_image.GetLength(0); i++)
                    {
                        for (int j = 0; j < reconstructed_image.GetLength(1); j++)
                        {
                            reconstructed_image[i, j] = ((reconstructed_image1[i, j] - reconstructed_image[i, j]) > 0 && (reconstructed_image1[i, j] - reconstructed_image[i, j]) < 255)
                                                            ? (reconstructed_image1[i, j] - reconstructed_image[i, j])
                                                            : 0;
                            //Math.Abs????
                        }
                    }
                    ////////////////// ----------------------------------------------------------------------*/

                for (int i = 0; i < averange_img.GetLength(0); i++)
                {
                    reconstructed_image[i, 0] += averange_img[i];
                }

                /* пропорционально сжимаю значения... чтобы красивая фотография получилась ---- перегнал в метод преобразования в bitmap
                double max = 0.0, min = double.MaxValue;

                for (int i = 0; i < reconstructed_image.GetLength(0); i++)
                {
                    if (reconstructed_image[i, 0] > max) max = reconstructed_image[i, 0];
                    if (reconstructed_image[i, 0] < min) min = reconstructed_image[i, 0];
                }

                for (int i = 0; i < reconstructed_image.GetLength(0); i++)
                {
                    reconstructed_image[i, 0] = 255 * (reconstructed_image[i, 0] - min) / (max - min);
                }
                */

                Bitmap[] proj_image = ByteToBitmap(reconstructed_image);
                PixelClasterization.SuperSave(proj_image[0], "C:\\Users\\Amigo's\\Desktop\\projectionIntoEigenSpace.jpg");
            }
            catch (Exception e1)
            {
                System.Windows.Forms.MessageBox.Show(e1.ToString());
            }
            

            /*
            //считаем евклидово расстояние между весами
            int k = 1;
            int kk = 4;
            double sum = 0;
            for (int i = 0; i < weight_coefficients.GetLength(1); i++)
            {
                sum += Math.Pow((byte)(weight_coefficients[i, k] - weight_coefficients[i, kk]), 2);
            }
            sum = Math.Sqrt(sum);
            */


        }

        public static void Recognizion(int is_in_testNum)
        {
            try
            {
                if (eigenfaces == null) // проверка на то, что собственные лица уже построены!
                {
                    string mainPath12 = "C:\\Users\\Amigo's\\Desktop\\recogn\\training_images"; 
                    string[] paths12 = Directory.GetFiles(mainPath12);
                    Pca(PCA_for_images.BitmapToByte(paths12));
                }
                string mainPath = "C:\\Users\\Amigo's\\Desktop\\recogn\\testimage";
                string[] paths = Directory.GetFiles(mainPath);
                var testFace = BitmapToByte(paths);
                string mainPath1 = "C:\\Users\\Amigo's\\Desktop\\recogn\\training_images";
                string[] paths1 = Directory.GetFiles(mainPath1);
                var classesFace = BitmapToByte(paths1);

                //вычли среднее
                for (int j = 0; j < paths.GetLength(0); j++)
                {
                    for (int i = 0; i < testFace.GetLength(1); i++)
                    {
                        testFace[j, i] -= averange_img[i];
                    }
                }
                //вычли среднее
                for (int j = 0; j < paths1.GetLength(0); j++)
                {
                    for (int i = 0; i < classesFace.GetLength(1); i++)
                    {
                        classesFace[j, i] -= averange_img[i];
                    }
                }
                //вычли среднее

                var weight_coefficients = Matrix_Multiple(Trancpose_Matrix(eigenfaces), Trancpose_Matrix(testFace));
                var weight_coefficients_of_classes = Matrix_Multiple(Trancpose_Matrix(eigenfaces), Trancpose_Matrix(classesFace));
                // weight_coefficients[j, номер изображения в тестовой выборке]

                double[,] distanse = new double[weight_coefficients.GetLength(1), weight_coefficients_of_classes.GetLength(1)];
                //[номер теста - расст от первого класса, расст от второго класса...]

                for (int i = 0; i < weight_coefficients.GetLength(1); i++) //для кажд изображения из теста
                {
                    for (int k = 0; k < weight_coefficients_of_classes.GetLength(1); k++) //для каждого класса изображения 
                    {
                        double sum = 0;
                        for (int j = 0; j < weight_coefficients.GetLength(0); j++)
                        {
                            sum += Math.Pow(weight_coefficients[j, i] - weight_coefficients_of_classes[k, j], 2);
                        }
                        distanse[i, k] = Math.Sqrt(sum);
                    }
                }
                double threshold = 0;
                for (int i = 0; i < distanse.GetLength(0); i++)
                {
                    for (int j = 0; j < distanse.GetLength(1); j++)
                    {
                        if (distanse[i, j] > threshold) threshold = distanse[i, j];
                    }
                }
                threshold = 0.5*threshold;

                //----------------------------------------------------// расстояние между проекц в гк и самим изображением.

                double[,] reconstructed_image = new double[ eigenfaces.GetLength(0), paths.GetLength(0)];  
                for (int i = 0; i < reconstructed_image.GetLength(0); i++)
                {
                    for (int j = 0; j < reconstructed_image.GetLength(1); j++)
                    {
                        reconstructed_image[i, j] = 0;
                    }
                }

                //построение проекции
                for (int num_Img = 0; num_Img < reconstructed_image.GetLength(1); num_Img++)
                {
                    for (int i = 0; i < eigenfaces.GetLength(0); i++) //по каждому пикселю
                    {
                        for (int j = 0; j < eigenfaces.GetLength(1); j++)
                            //число собственных лиц (базис) номер! TODO number_of_subdeminsion
                        {
                            reconstructed_image[i, num_Img] += eigenfaces[i, j] * weight_coefficients[j, num_Img];
                                // weight_coefficients[j, номер изображения в тестовой выборке]
                        }
                    }
                }
                for (int num_Img = 0; num_Img < reconstructed_image.GetLength(1); num_Img++)
                {
                    for (int i = 0; i < averange_img.GetLength(0); i++)
                    {
                        reconstructed_image[i, num_Img] += averange_img[i];
                    }

                    double max = 0.0, min = double.MaxValue;

                    for (int i = 0; i < reconstructed_image.GetLength(0); i++)
                    {
                        if (reconstructed_image[i, num_Img] > max) max = reconstructed_image[i, num_Img];
                        if (reconstructed_image[i, num_Img] < min) min = reconstructed_image[i, num_Img];
                    }

                    for (int i = 0; i < reconstructed_image.GetLength(0); i++)
                    {
                        reconstructed_image[i, num_Img] = 255 * (reconstructed_image[i, num_Img] - min) / (max - min);
                    }
                }
                //построили реконструкцию и сжали
                
                double[] dist_between_iamge_and_proj=new double[testFace.GetLength(0)];

                for (int i = 0; i < dist_between_iamge_and_proj.GetLength(0); i++)
                {
                    dist_between_iamge_and_proj[i] = 0;
                }
                
                for (int i = 0; i < eigenfaces.GetLength(0); i++) //count pixels
                {
                    for (int j = 0; j < reconstructed_image.GetLength(1); j++) //count test images
                    {
                        dist_between_iamge_and_proj[j] += Math.Pow(reconstructed_image[i, j] - testFace[j,i], 2);
                    }
                }

                for (int i = 0; i < dist_between_iamge_and_proj.GetLength(0); i++)
                {
                    dist_between_iamge_and_proj[i] = Math.Sqrt(dist_between_iamge_and_proj[i]);
                }

               // int is_in_testNum = 1;
                int num_of_test_class = 0;
                //перв половина того чтониже за то что лицо типа или не лицо. вторая за известность бд лица этого.
                if (dist_between_iamge_and_proj[is_in_testNum] < threshold && 
                    distanse[is_in_testNum, num_of_test_class] < threshold)
                {// distance[num_test,num_class]
                    string output = "вы есть в базе данных, вы {0}";
                    output = String.Format(output, num_of_test_class);
                    System.Windows.Forms.MessageBox.Show(output);
                    
                }
                else
                {
                    System.Windows.Forms.MessageBox.Show("Вы неизвестный"); 
                }
                //--------------------------------------------------- /

                int ewqe = 10;
            }
            catch (Exception e1)
            {
                System.Windows.Forms.MessageBox.Show(e1.ToString());
            }

        }
         
        /// <summary>
        /// Преобразование Bitmap - сжатие
        /// </summary>
        /// <param name="bmp"></param>
        /// <returns></returns>
        ///  TODO !! эта функция дублируется в PixelClasterization!!!!
        private static Bitmap MakeBitmapSmaller(this Bitmap bmp)
        {

            //TODO 
            //TODO 
            int x = 30;
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



        public static string CompareProjections(string path_of_test_image, string path_of_first_training,
                                     string path_of_second_training)
        {
            double[,] test = PCA_for_images.BitmapToByte(Directory.GetFiles(path_of_test_image));
            double[,] images1 = PCA_for_images.BitmapToByte(Directory.GetFiles(path_of_first_training));
            double[,] images2 = PCA_for_images.BitmapToByte(Directory.GetFiles(path_of_second_training));

            test = Trancpose_Matrix(test);
            
            double[,] averange_img1=new double[images1.GetLength(1),1];
            double[,] averange_img2=new double[images2.GetLength(1),1];

            ///
            Pca(images1);
            /*for (int i = 0; i < averange_img.GetLength(0); i++)
                {
                    averange_img1[i,0] = averange_img[i];
                }*/
            images1 = null; 
            GC.Collect();
            //Reconstruct_Projected_Image(averange_img1, 100, true);
            //double[,] proj_averange_img1 = reconstructed_image;
            double[,] proj_test1 = Reconstruct_Projected_Image(test, 100, true); 
            ///
            Pca(images2);
          /*  for (int i = 0; i < averange_img.GetLength(0); i++)
                {
                    averange_img2[i, 0] = averange_img[i];
                } */
            images2 = null;
            GC.Collect();
            //Reconstruct_Projected_Image(averange_img2, 100, true);
            //double[,] proj_averange_img2 = reconstructed_image;
            double[,] proj_test2 = Reconstruct_Projected_Image(test, 100, true); 
            ///
            //test= null;
           // GC.Collect();


            double dist1 = Dist( test, proj_test1 );
            double dist2 = Dist( test, proj_test2 );

            return String.Format("dist to монгол {0}"+ "" +
                                 " " +" dist to башкир {1}",dist1,dist2);

        }


        /// <summary>
        /// Построение изображения спроецированного в пространство главных компонент.
        /// </summary>
        public static double[,] Reconstruct_Projected_Image(double[,] image, int number_of_subdeminsion, bool sorting_eigenvalues_on_decrease, int num_of_image = 0)
        {
            //byte num_of_test_image = 0; //номер изображения которое будем проецировать по порядку из массива paths
            try
            {
                if (eigenfaces == null) // проверка на то, что собственные лица уже построены!
                {
                    return null;
                }
                if (number_of_subdeminsion > eigenfaces.GetLength(1)) number_of_subdeminsion = eigenfaces.GetLength(1);


                //string mainPath = "C:\\Users\\Amigo's\\Desktop\\pic\\PCA_test\\test_for_projection";
                //string mainPath = "C:\\Users\\Amigo's\\Desktop\\recogn\\testimage";
                //string[] paths = Directory.GetFiles(mainPath);


                //TODO размерность тестового изображения равна среднему 

                double[,] testFace = image;
                 

                var weight_coefficients = Matrix_Multiple(Trancpose_Matrix(eigenfaces), (testFace)); // weight_coefficients_of_eigenfaces
                // weight_coefficients [число собственных векторов, колич тестовых фотографий]
                //var weight_coefficients = Matrix_Multiple( (testFace), (eigenfaces));


                reconstructed_image = new double[eigenfaces.GetLength(0), 1];

                double r = 0;
                for (int i = 0; i < weight_coefficients.GetLength(0); i++)
                {
                    r += weight_coefficients[i, 0]; //смотрю суммы чтобы можно было оценить в какую сторону для этого лица отклонение весов СВ
                }

                int k = 2;
                for (int i = 0; i < reconstructed_image.GetLength(0); i++)
                {
                    for (int j = 0; j < reconstructed_image.GetLength(1); j++)
                    {
                        reconstructed_image[i, j] = 0;
                    }
                }

                //построение проекции

                if (sorting_eigenvalues_on_decrease)//построение проекции по убыванию дисперсии
                {
                    for (int i = 0; i < eigenfaces.GetLength(0); i++) //по каждому пикселю
                    {
                        for (int j = eigenfaces.GetLength(1) - 1; j >= (eigenfaces.GetLength(1) - number_of_subdeminsion); j--) //число собственных лиц (базис) номер! TODO number_of_subdeminsion
                        {
                            reconstructed_image[i, 0] += eigenfaces[i, j] * weight_coefficients[j, 0]; // weight_coefficients[j, номер изображения в тестовой выборке] 
                            //reconstructed_image[i, 0] += eigenfaces[i, j] * weight_coefficients[ num_of_test_image, j];
                        }

                    }
                }
                else //по возрастанию дисперсии
                {
                    for (int i = 0; i < eigenfaces.GetLength(0); i++) //по каждому пикселю
                    {
                        for (int j = 0; j < number_of_subdeminsion; j++) //число собственных лиц (базис) номер! TODO number_of_subdeminsion
                        {
                            reconstructed_image[i, 0] += eigenfaces[i, j] * weight_coefficients[j, 0]; // weight_coefficients[j, номер изображения в тестовой выборке]
                        }
                    }
                }
                  
                for (int i = 0; i < averange_img.GetLength(0); i++)
                {
                    reconstructed_image[i, 0] += averange_img[i];
                }

               

                Bitmap[] proj_image = ByteToBitmap(reconstructed_image);
                PixelClasterization.SuperSave(proj_image[0], "C:\\Users\\Amigo's\\Desktop\\projectionIntoEigenSpace.jpg");
            }
            catch (Exception e1)
            {
                System.Windows.Forms.MessageBox.Show(e1.ToString());
            }


            return reconstructed_image;

        }

        public static double Dist(double[,] x, double[,] y)
        {
            double sum = 0;
            for (int i = 0; i < x.GetLength(0); i++)
            {
                sum += Math.Pow(x[i, 0] - y[i, 0], 2);
            }
            return Math.Sqrt(sum);
        }

    } //закрывашка от класса PCA_
}
