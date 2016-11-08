using Emgu.CV;
using Emgu.CV.Face;
using Emgu.CV.Structure;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Xml;

namespace AnimeFacestracking
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        private Capture capture = null;
        Mat mat_ShowImage;
        List<System.Drawing.Rectangle> autoFacesTrack;
        DataUpdate dataUpdate;
        List<string> animePathList;
        private string saveFacesTrackResultPath = "";
        static int renameCount = 0;
        static int renameNegCount = 0;
        //Task task_Track;
        string nowLoadedFile = "";
        private List<string> autoAnimeFacesTrackListResult;
        private List<string> saveAnimeFacesTrackListResult;
        bool flag_RunCheckImage = false;
        bool flag_CheckInfo = true;
        string nowLoaded;
        private int sleepTime = 10;
        object _lock;
        Thread thread_Track;
        int saveNegFlagCount = 0;
        int saveFlagCount = 0;
        bool useCuda = false;
        FaceDetect faceDetect;
        public string[] name;
        bool needInputName = false;
        Image<Rgb, byte> image_usedToNamed;
        XmlDocument docu;
        const int labelDim = 14;

        public MainWindow()
        {
            InitializeComponent();
            dataUpdate = new DataUpdate();
            animePathList = new List<string>();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            image_Show.DataContext = this;
            textBlock_ReadAnimeCount.DataContext = dataUpdate;
            textBlock_FinishedCount.DataContext = dataUpdate;
            textBlock_SuccessedCount.DataContext = dataUpdate;
            textBlock_ErrorCount.DataContext = dataUpdate;
            name = new string[7];
            name[0] = "一条萤";
            name[1] = "宫内莲华";
            name[2] = "越谷小鞠";
            name[3] = "越谷夏海";
            name[4] = "宫内光华";
            name[5] = "富士宫木实";
            name[6] = "越谷卓";
        }

        private void button_Start_Click(object sender, RoutedEventArgs e)
        {
            if (saveFacesTrackResultPath.Equals(""))
            {
                MessageBox.Show("请设置存储路径！","提示");
                dataUpdate.errorCount++;
                return;
            }
            if (animePathList.Count() == 0)
            {
                MessageBox.Show("请设置读取路径！","提示");
                dataUpdate.errorCount++;
                return;
            }
            if (!Directory.Exists(saveFacesTrackResultPath + @"\neg"))
            {
                Directory.CreateDirectory(saveFacesTrackResultPath + @"\neg");
            }
            flag_RunCheckImage = false;
            sleepTime = Convert.ToInt32(textBox_Time.Text);
            if ((bool)checkBox_UseCuda.IsChecked)
            {
                useCuda = true;
            }else
            {
                useCuda = false;
            }
            faceDetect = new FaceDetect("face_default.xml",false);
            button_Stop.Visibility = Visibility.Visible;
            button_Start.Visibility = Visibility.Hidden;
            _lock = new object();
            thread_Track = new Thread(new ThreadStart(startAnimeThread));
            thread_Track.IsBackground = true;
            thread_Track.Start();
            
            //foreach (string path in animePathList)
            //{
            //    nowLoadedFile = path;
            //    try
            //    {
            //        capture = new Capture(path);
            //    }catch(Exception)
            //    {
            //        dataUpdate.errorCount++;
            //        MessageBox.Show("加载动画时报错了！","提示");
            //    }
            //    task_Track = Task.Factory.StartNew(() => {
            //        frameThread();
            //    });
            //    task_Track.ContinueWith(t => { dataUpdate.animeCount--; task_Track.Dispose(); t.Dispose(); });
            //}

            //System.Windows.Forms.Timer myTimer = new System.Windows.Forms.Timer();
            //myTimer.Interval = 1000 / Convert.ToInt32(capture.GetCaptureProperty(Emgu.CV.CvEnum.CapProp.Fps));
            //myTimer.Tick += new EventHandler(MyTimer_Tick);
            //myTimer.Start();
        }

        private void startAnimeThread()
        {
            while(true)
            {
                if (animePathList.Count == 0)
                {
                    MessageBox.Show("已无可用动画！","提示");
                    break;
                }
                string path;
                lock (_lock)
                {
                    path = animePathList.First();
                    animePathList.RemoveAt(0);
                }
                nowLoadedFile = path;
                try
                {
                    capture = new Capture(path);
                    frameThread();
                    dataUpdate.animeCount--;
                    if (capture != null)
                        capture.Dispose();
                }
                catch (Exception)
                {
                    dataUpdate.errorCount++;
                    MessageBox.Show("加载动画时报错了！", "提示");
                }
            }

        }

        private void MyTimer_Tick(object sender, EventArgs e)
        {
            mat_ShowImage = capture.QuerySmallFrame();

            long checkTime = 0;
            autoFacesTrack = new List<System.Drawing.Rectangle>();
            DetectFace.Detect(mat_ShowImage, "face_default.xml",autoFacesTrack, useCuda, out checkTime);
            foreach (System.Drawing.Rectangle face in autoFacesTrack)
            {
                CvInvoke.Rectangle(mat_ShowImage, face, new Bgr(System.Drawing.Color.Red).MCvScalar, 2);
                dataUpdate.faceCheckSuccessed++;
            }
            autoFacesTrack.Clear();
            autoFacesTrack = null;
            callDisplayImage_Delegate();
        }
        
        private void frameThread()
        {
            while(true)
            {
                try
                {
                    //bool saveFlag = false;
                    if (capture.Grab())
                    {
                        mat_ShowImage = capture.QuerySmallFrame();
                    }
                    else
                    {
                        break;
                    }
                    if (mat_ShowImage == null) break;
                    //long checkTime = 0;
                    autoFacesTrack = new List<System.Drawing.Rectangle>();
                    //DetectFace.Detect(mat_ShowImage, "face_default.xml",autoFacesTrack, useCuda, out checkTime);
                    faceDetect.detect(mat_ShowImage, ref autoFacesTrack);

                    //foreach (System.Drawing.Rectangle face in autoFacesTrack)
                    //{
                    //    saveFlag = true;
                    //    CvInvoke.Rectangle(mat_ShowImage, face, new Bgr(System.Drawing.Color.Red).MCvScalar, 2);
                    //    dataUpdate.faceCheckSuccessed++;
                    //}

                    if (autoFacesTrack.Count() > 0 && countFlag())
                    {
                        if (needInputName)
                        {
                            writeImageAndName();
                        }
                        else
                        {
                            writeImage();
                        }
                    }
                    if (countNegFlag() && autoFacesTrack.Count() == 0)
                    {
                        writeNegImage();
                    }

                    autoFacesTrack.Clear();
                    autoFacesTrack = null;
                    if (mat_ShowImage != null)
                    {
                        mat_ShowImage.Dispose();
                        mat_ShowImage = null;
                    }
                    dataUpdate.finishedCount++;
                    Thread.Sleep(sleepTime);
                }
                catch (Exception)
                {
                    dataUpdate.errorCount++;
                }

            }
            //capture.Dispose();
        }

        private bool countFlag()
        {
            if ((dataUpdate.finishedCount - saveFlagCount) < 100)
            {
                return false;
            }else
            {
                saveFlagCount = dataUpdate.finishedCount;
                return true;
            }
        }
        
        private bool countNegFlag()
        {
            if ((dataUpdate.finishedCount - saveNegFlagCount) < 100)
            {
                return false;
            }
            else
            {
                saveNegFlagCount = dataUpdate.finishedCount;
                return true;
            }
        }

        private void writeImage()
        {
            try
            {
                string time = GetTimeStamp();
                string name = saveFacesTrackResultPath + @"\" + time + "_" + renameCount + ".jpg";
                renameCount++;
                mat_ShowImage.Save(name);

                //show the image
                foreach (System.Drawing.Rectangle face in autoFacesTrack)
                {
                    CvInvoke.Rectangle(mat_ShowImage, face, new Bgr(System.Drawing.Color.Red).MCvScalar, 2);
                }
                callDisplayImage_Delegate();

                StringBuilder SB = new StringBuilder();
                SB.Append(name + " " + autoFacesTrack.Count + " ");
                foreach (System.Drawing.Rectangle face in autoFacesTrack)
                {
                    SB.Append(face.X + " " + face.Y + " " + face.Width + " " + face.Height + " ");
                }
                SB.Remove(SB.Length - 1, 1);
                using (StreamWriter SW = new StreamWriter(new FileStream("info.dat", FileMode.Append)))
                {
                    SW.WriteLine(SB.ToString());
                }
                dataUpdate.faceCheckSuccessed++;
            }
            catch(Exception)
            {
                dataUpdate.errorCount++;
            }
        }

        private void writeImageAndName()
        {
            int personCount = 1;
            autoFacesTrack.Sort();
            // used the image not has been draw the bounding box.
            image_usedToNamed = mat_ShowImage.ToImage<Rgb, byte>();
            foreach (System.Drawing.Rectangle face in autoFacesTrack)
            {
                CvInvoke.Rectangle(mat_ShowImage, face, new Bgr(System.Drawing.Color.Red).MCvScalar, 2);
                CvInvoke.PutText(mat_ShowImage,personCount.ToString(),new System.Drawing.Point(face.X,face.Y - 10),Emgu.CV.CvEnum.FontFace.HersheyPlain,2 ,new Bgr(System.Drawing.Color.Green).MCvScalar);
            }
            callDisplayImage_Delegate();
            callDisplayWindow_Delegate();
        }

        private void writeNegImage()
        {
            try
            {
                Image<Bgr, byte> image = mat_ShowImage.ToImage<Bgr, byte>();
                string time = GetTimeStamp();
                string name = saveFacesTrackResultPath + @"\neg\" + time + "_" + renameNegCount + ".jpg";
                renameNegCount++;
                //mat_ShowImage.Save(name);
                int width = mat_ShowImage.Width;
                int height = mat_ShowImage.Height;
                int newWidth = width / 2;
                int newHeight = height / 2;
                for (float i = 0; i < 2; i++)
                {
                    float startHeight = height * (i / 2);
                    for (float j = 0; j < 2; j++)
                    {
                        float startWidth = width * (j / 2);

                        image.ROI = new System.Drawing.Rectangle(Convert.ToInt32(startWidth), Convert.ToInt32(startHeight), newWidth, newHeight);
                        image.Save(name);
                        CvInvoke.cvResetImageROI(image);

                        //image.ROI = System.Drawing.Rectangle.Empty;
                        using (StreamWriter SW = new StreamWriter(new FileStream("neg.txt", FileMode.Append)))
                        {
                            SW.WriteLine(name);
                        }
                        name = saveFacesTrackResultPath + @"\neg\" + time + "_" + renameNegCount + ".jpg";
                        renameNegCount++;
                    }
                }
                image.Dispose();
                image = null;
            }
            catch (Exception)
            {
                dataUpdate.errorCount++;
            }
        }

        [System.Runtime.InteropServices.DllImport("gdi32.dll")]
        public static extern bool DeleteObject(IntPtr hObject);

        public static BitmapSource CreateBitmapSourceFromBitmap(ref Bitmap bitmap)
        {
            IntPtr ptr = bitmap.GetHbitmap();
            BitmapSource result =
                System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(
                    ptr, IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
            //release resource  
            DeleteObject(ptr);

            return result;
        }

        private void displayImage()
        {
            Bitmap tempBitmap = mat_ShowImage.Bitmap;
            bitmapSource = CreateBitmapSourceFromBitmap(ref tempBitmap);
            tempBitmap.Dispose();
            tempBitmap = null;
            if (mat_ShowImage != null)
            {
                mat_ShowImage.Dispose();
                mat_ShowImage = null;
            }
        }

        private delegate void displayImage_Delegate();

        private void callDisplayImage_Delegate()
        {
            this.Dispatcher.Invoke(new displayImage_Delegate(displayImage));
        }

        private void displayWindow()
        {
            for (int i = 0; i < autoFacesTrack.Count(); i++)
            {
                try
                {
                    System.Drawing.Rectangle face = autoFacesTrack.First();
                    autoFacesTrack.RemoveAt(0);
                    int selectedName = -1;
                    NameSelectWindow NSW = new NameSelectWindow();
                    NSW.name_1 = name[0];
                    NSW.name_2 = name[1];
                    NSW.name_3 = name[2];
                    NSW.name_4 = name[3];
                    NSW.name_5 = name[4];
                    NSW.name_6 = name[5];
                    NSW.name_7 = name[6];
                    NSW.totalCount = 7;
                    NSW.personCount = i + 1;
                    NSW.Title = "Select the character name:" + (i + 1).ToString();
                    NSW.Owner = this;
                    NSW.WindowStartupLocation = WindowStartupLocation.CenterScreen;
                    NSW.ShowDialog();
                    if (NSW.DialogResult == true)
                    {
                        // save image
                        selectedName = NSW.result;
                        if (selectedName != -1)
                        {
                            string time = GetTimeStamp();
                            string fileName = saveFacesTrackResultPath + @"\" + time + "_" + renameCount + ".jpg";
                            renameCount++;
                            Image<Rgb, byte> subImage = image_usedToNamed.GetSubRect(face).Resize(32, 32, Emgu.CV.CvEnum.Inter.Cubic);
                            //subImage._EqualizeHist();
                            subImage.Save(fileName);

                            List<float> rawImage = subImage.Bitmap.ParallelExtractCHW();

                            save_training_data(selectedName + 6, ref rawImage);
                            save_convert_data(selectedName + 6, fileName);
                            subImage.Dispose();
                            subImage = null;
                            rawImage = null;
                            // save xml

                            //switch(selectedName)
                            //{
                            //    case 1:
                            //        save_training_data(fileName, 1.ToString());
                            //        break;
                            //    case 2:
                            //        save_training_data(fileName, 2.ToString());
                            //        break;
                            //    case 3:
                            //        save_training_data(fileName, 3.ToString());
                            //        break;
                            //    case 4:
                            //        save_training_data(fileName, 4.ToString());
                            //        break;
                            //    case 5:
                            //        save_training_data(fileName, 5.ToString());
                            //        break;
                            //    case 6:
                            //        save_training_data(fileName, 6.ToString());
                            //        break;
                            //    case 7:
                            //        save_training_data(fileName, 7.ToString());
                            //        break;
                            //}

                        }
                    }
                }catch (Exception)
                {

                }finally
                {
                    if (image_usedToNamed != null)
                    {
                        image_usedToNamed.Dispose();
                        image_usedToNamed = null;
                    }
                }

            }
        }

        private bool save_training_data(int personIndex, ref List<float> rawImage)
        {
            int[] person = new int[labelDim];
            for (int i = 0; i < labelDim; i++)
            {
                person[i] = 0;
            }
            person[personIndex] = 1;
            using (StreamWriter SW = new StreamWriter(new FileStream(saveFacesTrackResultPath + @"\data.txt",FileMode.Append)))
            {
                SW.Write("|labels ");
                for(int i = 0; i < labelDim; i++)
                {
                    SW.Write(person[i] + " ");
                }
                SW.Write("|features");
                foreach(float f in rawImage)
                {
                    SW.Write(" " + f);
                }
                SW.WriteLine();
            }
            return true;
        }

        private bool save_convert_data(int personIndex,string filePath)
        {
            using (StreamWriter SW = new StreamWriter(new FileStream(saveFacesTrackResultPath + @"\path.txt",FileMode.Append)))
            {
                SW.Write(filePath + " ");
                SW.Write(personIndex.ToString());
                SW.WriteLine();
            }
            return true;
        }

        private bool save_training_data(string fileName, string personName)
        {
            try
            {
                string trainedLabelsPath = saveFacesTrackResultPath + @"\TrainedLabels.xml";

                if (File.Exists(trainedLabelsPath))
                {
                    //File.AppendAllText(Application.StartupPath + "/TrainedFaces/TrainedLabels.txt", NAME_PERSON.Text + "\n\r");
                    bool loading = true;
                    while (loading)
                    {
                        try
                        {
                            docu.Load(trainedLabelsPath);
                            loading = false;
                        }
                        catch
                        {
                            docu = null;
                            docu = new XmlDocument();
                            Thread.Sleep(10);
                        }
                    }

                    //Get the root element
                    XmlElement root = docu.DocumentElement;

                    XmlElement face_D = docu.CreateElement("FACE");
                    XmlElement name_D = docu.CreateElement("NAME");
                    XmlElement file_D = docu.CreateElement("FILE");

                    //Add the values for each nodes
                    //name.Value = textBoxName.Text;
                    //age.InnerText = textBoxAge.Text;
                    //gender.InnerText = textBoxGender.Text;
                    name_D.InnerText = personName;
                    file_D.InnerText = fileName;

                    //Construct the Person element
                    //person.Attributes.Append(name);
                    face_D.AppendChild(name_D);
                    face_D.AppendChild(file_D);

                    //Add the New person element to the end of the root element
                    root.AppendChild(face_D);

                    //Save the document
                    docu.Save(trainedLabelsPath);
                    //XmlElement child_element = docu.CreateElement("FACE");
                    //docu.AppendChild(child_element);
                    //docu.Save("TrainedLabels.xml");
                }
                else
                {
                    FileStream FS_Face = File.OpenWrite(trainedLabelsPath);
                    using (XmlWriter writer = XmlWriter.Create(FS_Face))
                    {
                        writer.WriteStartDocument();
                        writer.WriteStartElement("Faces_For_Training");

                        writer.WriteStartElement("FACE");
                        writer.WriteElementString("NAME", personName);
                        writer.WriteElementString("FILE", fileName);
                        writer.WriteEndElement();

                        writer.WriteEndElement();
                        writer.WriteEndDocument();
                    }
                    FS_Face.Close();
                }

                return true;
            }
            catch (Exception ex)
            {
                return false;
            }

        }

        private delegate void displayWindow_Delegate();

        private void callDisplayWindow_Delegate()
        {
            this.Dispatcher.Invoke(new displayWindow_Delegate(displayWindow));
        }


        public string GetTimeStamp()
        {
            TimeSpan ts = DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, 0);
            return Convert.ToInt64(ts.TotalSeconds).ToString();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        BitmapSource _bitmapSource;
        public BitmapSource bitmapSource
        {
            get { return _bitmapSource; }

            set
            {
                if (_bitmapSource != value)
                {
                    _bitmapSource = null;
                    _bitmapSource = value;
                    if (PropertyChanged != null)
                        PropertyChanged(this, new PropertyChangedEventArgs("bitmapSource"));
                }
            }
        }

        private void getAnimePath(string dir, ref List<string> pathList)
        {
            try
            {
                DirectoryInfo dirInfo = new DirectoryInfo(dir);
                foreach (FileInfo fileInfo in dirInfo.GetFiles("*.mp4"))
                {
                    pathList.Add(fileInfo.FullName);
                }
                foreach (FileInfo fileInfo in dirInfo.GetFiles("*.mkv"))
                {
                    pathList.Add(fileInfo.FullName);
                }
                foreach (FileInfo fileInfo in dirInfo.GetFiles("*.rmvb"))
                {
                    pathList.Add(fileInfo.FullName);
                }
                pathList.Sort();
                dataUpdate.animeCount = pathList.Count();
            }
            catch (Exception E)
            {
                dataUpdate.errorCount++;
                MessageBox.Show("获取视频文件出错，可能是目录有误！","提示");
                return;
            }

        }

        private void button_SaveFile_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.FolderBrowserDialog FBD = new System.Windows.Forms.FolderBrowserDialog();
            System.Windows.Forms.DialogResult DR = FBD.ShowDialog();
            if (DR == System.Windows.Forms.DialogResult.Cancel)
                return;
            saveFacesTrackResultPath = FBD.SelectedPath;
        }

        private void button_ReadFile_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.FolderBrowserDialog FBD = new System.Windows.Forms.FolderBrowserDialog();
            System.Windows.Forms.DialogResult DR = FBD.ShowDialog();
            if (DR == System.Windows.Forms.DialogResult.Cancel)
                return;
            string getPath = FBD.SelectedPath;

            Task T = Task.Factory.StartNew(() => {
                getAnimePath(getPath,ref animePathList);
            });
        }

        private void button_Stop_Click(object sender, RoutedEventArgs e)
        {
            if (thread_Track != null)
            {
                thread_Track.Abort();
                thread_Track = null;
                //task_Track.Dispose();
            }
            button_Stop.Visibility = Visibility.Hidden;
            button_Start.Visibility = Visibility.Visible;
            MessageBox.Show("剩余：" + dataUpdate.animeCount + @"\n" + "捕捉到：" + dataUpdate.faceCheckSuccessed + @"\n" + "目前访问文件：" + nowLoadedFile, "结果");
        }

        private void button_ReadInfoData_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.OpenFileDialog OFD = new System.Windows.Forms.OpenFileDialog();
            OFD.Filter = "Info.dat|*.dat|neg.txt|*.txt";
            if (OFD.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                //MessageBox.Show(OFD.FileName);
                autoAnimeFacesTrackListResult = new List<string>();
                saveAnimeFacesTrackListResult = new List<string>();
                try
                {
                    using(StreamReader SR = new StreamReader(new FileStream(OFD.FileName, FileMode.Open)))
                    {
                        while (!SR.EndOfStream)
                        {
                            //MessageBox.Show(SR.ReadLine());
                            autoAnimeFacesTrackListResult.Add(SR.ReadLine());
                        }
                    }
                }
                catch(Exception)
                {
                    MessageBox.Show("读取Info文件时报错！","提示");
                    return;
                }
                
            }
        }
        
        private void button_ReadImage_Click(object sender, RoutedEventArgs e)
        {
            if (autoAnimeFacesTrackListResult == null)
            {
                MessageBox.Show("未读取Info文件！","提示");
                return;
            }

            if (autoAnimeFacesTrackListResult.Count == 0)
            {
                textBlcok_Result.Text = "";
                if (flag_CheckInfo)
                {
                    using (StreamWriter SW = new StreamWriter(new FileStream("InfoNew.dat", FileMode.Create)))
                    {
                        foreach (string str in saveAnimeFacesTrackListResult)
                        {
                            SW.WriteLine(str);
                        }
                    }
                }else {
                    using (StreamWriter SW = new StreamWriter(new FileStream("NegNew.dat", FileMode.Create)))
                    {
                        foreach (string str in saveAnimeFacesTrackListResult)
                        {
                            SW.WriteLine(str);
                        }
                    }
                }

                flag_RunCheckImage = false;
                MessageBox.Show("已完成全部审查！","提示");
                return;
            }else
            {
                flag_RunCheckImage = true;
                textBlcok_Result.Text = "";
                nowLoaded = autoAnimeFacesTrackListResult.First();
                autoAnimeFacesTrackListResult.RemoveAt(0);

                string[] nowLoadedData = nowLoaded.Split(' ');
                if (nowLoadedData.Count() > 1)
                {
                    flag_CheckInfo = true;
                    string imagePath = nowLoadedData[0];
                    int boundingBox = Convert.ToInt32(nowLoadedData[1]);

                    List<System.Drawing.Rectangle> boundingBoxList = new List<System.Drawing.Rectangle>();
                    for (int i = 0; i < boundingBox; i++)
                    {
                        boundingBoxList.Add(new System.Drawing.Rectangle(Convert.ToInt32(nowLoadedData[i * 4 + 2]), Convert.ToInt32(nowLoadedData[i * 4 + 3]), Convert.ToInt32(nowLoadedData[i * 4 + 4]), Convert.ToInt32(nowLoadedData[i * 4 + 5])));
                    }

                    Image<Rgb, byte> image = new Image<Rgb, byte>(imagePath);
                    foreach (System.Drawing.Rectangle R in boundingBoxList)
                    {
                        CvInvoke.Rectangle(image, R, new Bgr(System.Drawing.Color.Red).MCvScalar, 2);
                    }
                    Bitmap tempBitmap = image.Bitmap;
                    bitmapSource = CreateBitmapSourceFromBitmap(ref tempBitmap);
                    tempBitmap.Dispose();
                    tempBitmap = null;
                    image.Dispose();
                    image = null;
                }else
                {
                    flag_CheckInfo = false;
                    string imagePath = nowLoaded;
                    Image<Rgb, byte> image = new Image<Rgb, byte>(imagePath);
                    Bitmap tempBitmap = image.Bitmap;
                    bitmapSource = CreateBitmapSourceFromBitmap(ref tempBitmap);
                    tempBitmap.Dispose();
                    tempBitmap = null;
                    image.Dispose();
                    image = null;
                }

            }

        }

        private void image_Show_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (flag_RunCheckImage && !saveAnimeFacesTrackListResult.Contains(nowLoaded))
            {
                string path = nowLoaded.Split(' ')[0];
                if (File.Exists(path))
                {
                    saveAnimeFacesTrackListResult.Add(nowLoaded);
                    textBlcok_Result.Text = "保留";
                }
            }
        }

        private void image_Show_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (flag_RunCheckImage)
            {
                if (flag_CheckInfo)
                {
                    string path = nowLoaded.Split(' ')[0];
                    if (saveAnimeFacesTrackListResult.Contains(path))
                    {
                        saveAnimeFacesTrackListResult.Remove(path);
                    }
                    if (File.Exists(path))
                    {
                        File.Delete(path);
                    }
                }else
                {
                    if (saveAnimeFacesTrackListResult.Contains(nowLoaded))
                    {
                        saveAnimeFacesTrackListResult.Remove(nowLoaded);
                    }
                    if (File.Exists(nowLoaded))
                    {
                        File.Delete(nowLoaded);
                    }
                }
                textBlcok_Result.Text = "删除";
            }
        }

        private void button_CheckInfoData_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.OpenFileDialog OFD = new System.Windows.Forms.OpenFileDialog();
            OFD.Filter = "Info.dat|*.dat|neg.txt|*.txt";
            if (OFD.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                List<string> checkList = new List<string>();
                List<string> removeList = new List<string>();
                try
                {
                    using(StreamReader SR = new StreamReader(new FileStream(OFD.FileName, FileMode.Open)))
                    {
                        while (!SR.EndOfStream)
                        {
                            //MessageBox.Show(SR.ReadLine());
                            checkList.Add(SR.ReadLine());
                        }
                    }

                    foreach(string str in checkList)
                    {
                        string path = str.Split(' ')[0];
                        if (!File.Exists(path))
                        {
                            removeList.Add(str);
                        }
                    }

                    if (removeList.Count() != 0)
                    {
                        foreach (string str in removeList)
                        {
                            checkList.Remove(str);
                        }
                    }

                    using(StreamWriter SW = new StreamWriter(new FileStream("Checked.dat",FileMode.Create)))
                    {
                        foreach(string str in checkList)
                        {
                            SW.WriteLine(str);
                        }
                    }
                    MessageBox.Show("复查完毕！","提示");
                }
                catch (Exception)
                {
                    MessageBox.Show("复查Info文件时报错！", "提示");
                    return;
                }

            }
        }

        private void button_SaveInfodata_Click(object sender, RoutedEventArgs e)
        {
            if (flag_CheckInfo)
            {
                try
                {
                    using (StreamWriter SW = new StreamWriter(new FileStream("InfoNew.dat", FileMode.CreateNew)))
                    {
                        foreach (string str in saveAnimeFacesTrackListResult)
                        {
                            SW.WriteLine(str);
                        }
                    }
                }catch (IOException)
                {
                    MessageBox.Show("已存在 infoNew.dat 文件，请先将其改名！","警告");
                }

            }
            else
            {
                try
                {
                    using (StreamWriter SW = new StreamWriter(new FileStream("NegNew.dat", FileMode.CreateNew)))
                    {
                        foreach (string str in saveAnimeFacesTrackListResult)
                        {
                            SW.WriteLine(str);
                        }
                    }
                }catch (IOException)
                {
                    MessageBox.Show("已存在 infoNew.dat 文件，请先将其改名！", "警告");
                }

            }
        }

        private void button_SaveFileReadData_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.FolderBrowserDialog FBD = new System.Windows.Forms.FolderBrowserDialog();
            System.Windows.Forms.DialogResult DR = FBD.ShowDialog();
            if (DR == System.Windows.Forms.DialogResult.Cancel)
                return;
            string path = FBD.SelectedPath;
            List<string> pathList = new List<string>();
            try
            {
                DirectoryInfo dirInfo = new DirectoryInfo(path);
                foreach (FileInfo fileInfo in dirInfo.GetFiles("*.jpg"))
                {
                    pathList.Add(fileInfo.FullName);
                }
                foreach (FileInfo fileInfo in dirInfo.GetFiles("*.png"))
                {
                    pathList.Add(fileInfo.FullName);
                }
                
            }
            catch (Exception E)
            {
                dataUpdate.errorCount++;
                MessageBox.Show("获取图片文件出错，可能是目录有误！", "提示");
                return;
            }
            try
            {
                using (StreamWriter SW = new StreamWriter(new FileStream("List.dat", FileMode.CreateNew)))
                {
                    foreach(string str in pathList)
                    {
                        SW.WriteLine(str);
                    }
                }
            }catch(Exception)
            {
                MessageBox.Show("已存在 List.dat 文件，请先将其改名！", "警告");
                return;
            }

            MessageBox.Show("List化完成！","提示");
        }

        private void checkBox_InputName_Click(object sender, RoutedEventArgs e)
        {
            if ((bool)checkBox_InputName.IsChecked)
            {
                needInputName = true;
            }else
            {
                needInputName = false;
            }
        }
    }
}
