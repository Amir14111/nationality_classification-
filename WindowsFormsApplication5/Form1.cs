using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Accord.Statistics.Analysis;


namespace WindowsFormsApplication5
{
    public partial class Form1 : Form
    {
       
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        { 
        }

        private void button3_Click(object sender, EventArgs e)
        {

            //TODO проверка на корректность !
            //TODO смотреть только измененные !!! чтобы не делать лишних вычислений
            
                    Bitmap btm1 = PixelClasterization.GetClasters(Int32.Parse(textBox1.Text), Int32.Parse(textBox2.Text));
                    progressBar1.Value = 33;
                    this.Refresh();
                    Bitmap btm2 = PixelClasterization.GetClasters(Int32.Parse(textBox3.Text), Int32.Parse(textBox4.Text));
                    progressBar1.Value = 67;
                    this.Refresh();
                    Bitmap btm3 = PixelClasterization.GetClasters(Int32.Parse(textBox5.Text), Int32.Parse(textBox6.Text));
                    progressBar1.Value = 100;
                    this.Refresh();
                   
                    pictureBox1.Width = btm1.Width;
                    pictureBox1.Height = btm1.Height;

                    pictureBox2.Width = btm1.Width;
                    pictureBox2.Height = btm1.Height;

                    pictureBox3.Width = btm1.Width;
                    pictureBox3.Height = btm1.Height;

                    pictureBox1.Image = btm1;
                    pictureBox2.Image = btm2;
                    pictureBox3.Image = btm3;
             
        }

        private void button4_Click(object sender, EventArgs e)
        {
            textBox1.Text = (Int32.Parse(textBox1.Text)+1).ToString();
        }

        private void button5_Click(object sender, EventArgs e)
        {
            textBox1.Text = (Int32.Parse(textBox1.Text) - 1).ToString();
        }

        private void button7_Click(object sender, EventArgs e)
        {
            textBox2.Text = (Int32.Parse(textBox2.Text) + 1).ToString();
        }

        private void button6_Click(object sender, EventArgs e)
        {
            textBox2.Text = (Int32.Parse(textBox2.Text) - 1).ToString();
        }

        private void button11_Click(object sender, EventArgs e)
        {
            textBox3.Text = (Int32.Parse(textBox3.Text) + 1).ToString();
        }

        private void button9_Click(object sender, EventArgs e)
        {
            textBox4.Text = (Int32.Parse(textBox4.Text) + 1).ToString();
        }

        private void button10_Click(object sender, EventArgs e)
        {
            textBox3.Text = (Int32.Parse(textBox3.Text) - 1).ToString();
        }

        private void button8_Click(object sender, EventArgs e)
        {
            textBox4.Text = (Int32.Parse(textBox4.Text) - 1).ToString();
        }

        private void button15_Click(object sender, EventArgs e)
        {
            textBox5.Text = (Int32.Parse(textBox5.Text) + 1).ToString();
        }

        private void button14_Click(object sender, EventArgs e)
        {
            textBox5.Text = (Int32.Parse(textBox5.Text) - 1).ToString();
        }

        private void button13_Click(object sender, EventArgs e)
        {
            textBox6.Text = (Int32.Parse(textBox6.Text) + 1).ToString();
        }

        private void button12_Click(object sender, EventArgs e)
        {
            textBox6.Text = (Int32.Parse(textBox6.Text) - 1).ToString();
        }

        private Bitmap[] eigenfaces = null;

        private void button1_Click(object sender, EventArgs e)
        {

            string mainPath =
            //"C:\\Users\\Amigo's\\Desktop\\pic\\PCA_test\\compiled\\wrap1"; //TODO для сравнения между выборками
            "C:\\Users\\Amigo's\\Desktop\\recogn\\training_images";   //TODO для распознавания
            //"C:\\Users\\Amigo's\\Desktop\\pic\\wrap"; //TODO набор монгол

            string[] paths = Directory.GetFiles(mainPath);

            eigenfaces = PCA_for_images.Pca(PCA_for_images.BitmapToByte(paths));
            //TODO сделать проверку на равенство пикселей
            //TODO СВ можно взять левым вектором. тогда немного св будут четче.. но вроде проекции такие же 

            pictureBox1.Width = 160;
            pictureBox1.Height = 190;
            PixelClasterization.SuperSave(PCA_for_images.aver_img_btm[0], "C:\\Users\\Amigo's\\Desktop\\eigenfaces\\resultAverangeImg.jpg");
            pictureBox1.Image = PCA_for_images.ChangeBitmapSize(PCA_for_images.aver_img_btm[0], 160, 190);

            #region pca_testing
            /*
            double[,] x =
                {
                    {1.2, 2.3, 7, 10},
                    {2 , 5, 4.2, 1.4},
                    {3, 6, 9, 12}
                };
            

            #region standarize of x (входная переменная)

            double[] k = new double[nVars];
            double[] sigm = new double[nVars];
            for (int i = 0; i < nVars; i++)
            {
                k[i] = 0;
                sigm[i] = 0;
            }

            for (int i = 0; i < nPoints; i++)
            {
                for (int j = 0; j < nVars; j++)
                {
                    k[j] += x[i, j]/nPoints;
                }
            }//среднее

            
            for (int i = 0; i < nPoints; i++)
            {
                for (int j = 0; j < nVars; j++)
                {
                    sigm[j] += Math.Pow(x[i, j] - k[j], 2)/nPoints;
                }
                
            }

            for (int i = 0; i < nVars; i++)
            {
                sigm[i] = Math.Sqrt(sigm[i] * nPoints / (nPoints - 1));
            }
            

            

            for (int i = 0; i < nPoints; i++)
            {
                for (int j = 0; j < nVars; j++)
                {
                   x[i, j] = (x[i, j] - k[j]) / sigm[j];
                }
                
            }
             
            

            double[] eigen_values = new double[nVars];
            double[,] eigenvectors_matrix = new double[nVars, nVars]; //матрица собственных векторов, не понял почему на -1 умножено
            alglib.pca.pcabuildbasis(x, nPoints, nVars, ref info, ref eigen_values, ref eigenvectors_matrix);

            var pca = new PrincipalComponentAnalysis(x, AnalysisMethod.Standardize);
            pca.Compute();

            var res = pca.Eigenvalues;
            var res1 = pca.ComponentMatrix;
            double[,] actual = pca.Transform(x,1);
            
            int k3 = 9;
            */
            #endregion  
        }

        private void button16_Click(object sender, EventArgs e) // сохраняю собственные лица в виде картинок
        {
            //вначале посчитать СВ
            for (int i = 0; i < eigenfaces.GetLength(0); i++)
            {
                 PixelClasterization.SuperSave(eigenfaces[PCA_for_images.nums[i]], "C:\\Users\\Amigo's\\Desktop\\eigenfaces\\resultBmp" + i.ToString() + ".jpg");
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            //сначала построить СВ потом нажимать сюда
            PCA_for_images.Reconstruct_Projected_Image(0, 32, false); //номер изображения, число компонент, false - по возрастанию
             
        }

        private void button17_Click(object sender, EventArgs e)
        {
            //внутри метод построения СВ. можно сразу нажимать сюда
            PCA_for_images.Recognizion(4);
        }

        private void button18_Click(object sender, EventArgs e)
        {
            ////----------test ------------
            double[] eigenvalues = null;
            double[] tmp1 = null;
            double[,] tmp2 = null;
            double[,] right_eigenvectors = null;

            double[,] testCov ={{4,-3,-3},
                { 1,2, 1},
                {1,1,2} };
            if (alglib.rmatrixevd(testCov, testCov.GetLength(0), 3, out eigenvalues, out tmp1, out tmp2, out right_eigenvectors))
            {
                System.Windows.Forms.MessageBox.Show("");
            }
            ////----------test ------------
        }

        private void button19_Click(object sender, EventArgs e)
        {
            //можно сразу нажимать сюда
            int[] arr1 = PixelClasterization.GetClastersForMatStat(9, 11, @"C:\\Users\\Amigo's\\Desktop\\pic\\wrap");
            int[] arr2 = PixelClasterization.GetClastersForMatStat(9, 11, @"C:\\Users\\Amigo's\\Desktop\\pic\\wrap2");
            Array.Sort(arr1, 0, arr1.Length);
            Array.Sort(arr2, 0, arr2.Length); 
            MatStat.tbData = textBox7;
            MatStat.CalcLR(arr1,arr2);
        }

        private void button20_Click(object sender, EventArgs e)
        {
            //можно нажимать сюда сразу
            int[] arr1 = PixelClasterization.GetClastersForMatStat(9, 11, @"C:\\Users\\Amigo's\\Desktop\\pic\\wrap");
            int[] arr2 = PixelClasterization.GetClastersForMatStat(9, 11, @"C:\\Users\\Amigo's\\Desktop\\pic\\wrap2");
            Array.Sort(arr1, 0, arr1.Length);
            Array.Sort(arr2, 0, arr2.Length); 
            MatStat.tbData = textBox7;
            MatStat.CalcKS(arr1, arr2); 
        }

        private void button21_Click(object sender, EventArgs e) //проецируем на компоненты чтобы посмотреть scagnostic
        {
            // сначала посчитать СВ потом нажать сюда
            // вклад ГК
            // "C:\\Users\\Amigo's\\Desktop\\recogn\\training_images";   //TODO для распознавания
            //double[,] images = PCA_for_images.BitmapToByte(  Directory.GetFiles( "C:\\Users\\Amigo's\\Desktop\\pic\\wrap") );
            double[,] images = PCA_for_images.BitmapToByte(Directory.GetFiles("C:\\Users\\Amigo's\\Desktop\\pic\\PCA_test\\compiled\\общ"));//"C:\\Users\\Amigo's\\Desktop\\recogn\\testimage"));
            int m = images.GetLength(0); //число фоток
            int n = images.GetLength(1); //одно фото колич пикселей 

            // PCA_for_images.eigenfaces
            //[ размерность фотки , число фоток]

            int num_of_component = 10;

            double [] y = new double[m];

            for (int i = 0; i < m; i++)
            {
                for (int j = 0; j < n; j++)
                {
                    y[i] += images[i, j] * PCA_for_images.eigenfaces[j, num_of_component];
                }
            }
             
            Bitmap  btm = new Bitmap(900 , 800);
            Graphics graphics = Graphics.FromImage(btm);
            Font drawFont = new Font("Arial", 12, FontStyle.Regular);
            SolidBrush drawBrush = new SolidBrush(Color.LemonChiffon);

            double min = y.Min();
            double max = y.Max();
            for (int i = 0; i < m; i++)
            {
                Point drawPoint = new Point( (int) (860 * (y[i] - min)/(max-min)) , (int) (800 * 0.7 - i * 10 ) );
                graphics.DrawString(i.ToString(), drawFont, drawBrush, drawPoint);
                graphics.DrawLine(Pens.IndianRed, (float)(900 * (y[i] - min) / (max - min)), (float)(800 * 0.7 - i * 10),
                    (float)(900 * (y[i] - min) / (max - min)), (float)(800 * 0.7));
            }

            PixelClasterization.SuperSave(btm, "C:\\Users\\Amigo's\\Desktop\\projection_result_for" + num_of_component.ToString() + "component.jpg");
            // вклад ГК
            GC.Collect();
        }


        private int coefficient = 3;
        private int num_of_PC =0;

        private void button22_Click(object sender, EventArgs e)
        {
            //сперва посчитать СВ
        //static public double[] averange_img = null;
        //public static double[,] eigenfaces = null; //[ размерность фотки , число фоток]
            coefficient += 5;
            int counter1 = PCA_for_images.averange_img.GetLength(0);
            int counter2 = PCA_for_images.eigenfaces.GetLength(1);

            double[,] result_img = new double[counter1,1];

            for (int i = 0; i < counter1; i++)
            {
                result_img[i,0] = PCA_for_images.averange_img[i] + coefficient*PCA_for_images.eigenfaces[i, num_of_PC];
            }
            Bitmap[] btm = PCA_for_images.ByteToBitmap(result_img);
            pictureBox2.Image = PCA_for_images.ChangeBitmapSize(btm[0], 160, 190);
        }

        private void button23_Click(object sender, EventArgs e)
        {
            //сперва посчитать СВ
            int counter1 = PCA_for_images.averange_img.GetLength(0);
            int counter2 = PCA_for_images.eigenfaces.GetLength(1);
            double[,] result_img = new double[counter1, 1];
            double sum;
            for (int i = 0; i < counter1; i++)
            {
                sum = 0;
                for (int j = 0; j < counter2; j++)
                {
                    sum += Math.Abs(PCA_for_images.eigenfaces[i, j]);
                }
                result_img[i, 0] = sum < 127.5*counter2 ? 255 : 0;  // из-за растяжения у меня ноль на 127,5
            }
            Bitmap[] btm = PCA_for_images.ByteToBitmap(result_img);
            pictureBox3.Image = PCA_for_images.ChangeBitmapSize(btm[0], 160, 190);

        }

        private void button24_Click(object sender, EventArgs e)
        {

            // номер изобр из textbox8 вывод в textbox7

            //среднее изображение 1 выборки беру.
            //среднее второй выборки.

            //считаю проекцию каждой в собств простр своей выборки.
            //считаю проекцию тестового изображения в простр двух выборок

            //считаю расстояние.

            textBox7.Text =
                PCA_for_images.CompareProjections("C:\\Users\\Amigo's\\Desktop\\pic\\PCA_test\\compiled\\test",
                                                  "C:\\Users\\Amigo's\\Desktop\\pic\\PCA_test\\compiled\\wrap1",
                                                  "C:\\Users\\Amigo's\\Desktop\\pic\\PCA_test\\compiled\\wrap2");
        }



        

    }//закрывашка класса формы
}