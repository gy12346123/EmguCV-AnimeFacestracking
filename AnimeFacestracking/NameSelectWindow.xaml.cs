using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace AnimeFacestracking
{
    /// <summary>
    /// NameSelectWindow.xaml 的交互逻辑
    /// </summary>
    public partial class NameSelectWindow : Window
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public NameSelectWindow()
        {
            InitializeComponent();
        }

        public int result = -1;

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            scrollViewer.DataContext = this;
            if (totalCount > 7 || totalCount < 1)
            {
                MessageBox.Show("未传入总人数数据！或数据数不符合标准！");
                return;
            }else
            {
                textBlock_NowNumber.Text = "现在角色：" + personCount;
                switch (totalCount)
                {
                    case 1:
                        radioButton_Name_2.Visibility = Visibility.Hidden;
                        radioButton_Name_3.Visibility = Visibility.Hidden;
                        radioButton_Name_4.Visibility = Visibility.Hidden;
                        radioButton_Name_5.Visibility = Visibility.Hidden;
                        radioButton_Name_6.Visibility = Visibility.Hidden;
                        radioButton_Name_7.Visibility = Visibility.Hidden;
                        radioButton_Name_1.Content = name_1;
                        break;
                    case 2:
                        radioButton_Name_3.Visibility = Visibility.Hidden;
                        radioButton_Name_4.Visibility = Visibility.Hidden;
                        radioButton_Name_5.Visibility = Visibility.Hidden;
                        radioButton_Name_6.Visibility = Visibility.Hidden;
                        radioButton_Name_7.Visibility = Visibility.Hidden;
                        radioButton_Name_1.Content = name_1;
                        radioButton_Name_2.Content = name_2;
                        break;
                    case 3:
                        radioButton_Name_4.Visibility = Visibility.Hidden;
                        radioButton_Name_5.Visibility = Visibility.Hidden;
                        radioButton_Name_6.Visibility = Visibility.Hidden;
                        radioButton_Name_7.Visibility = Visibility.Hidden;
                        radioButton_Name_1.Content = name_1;
                        radioButton_Name_2.Content = name_2;
                        radioButton_Name_3.Content = name_3;
                        break;
                    case 4:
                        radioButton_Name_5.Visibility = Visibility.Hidden;
                        radioButton_Name_6.Visibility = Visibility.Hidden;
                        radioButton_Name_7.Visibility = Visibility.Hidden;
                        radioButton_Name_1.Content = name_1;
                        radioButton_Name_2.Content = name_2;
                        radioButton_Name_3.Content = name_3;
                        radioButton_Name_4.Content = name_4;
                        break;
                    case 5:
                        radioButton_Name_6.Visibility = Visibility.Hidden;
                        radioButton_Name_7.Visibility = Visibility.Hidden;
                        radioButton_Name_1.Content = name_1;
                        radioButton_Name_2.Content = name_2;
                        radioButton_Name_3.Content = name_3;
                        radioButton_Name_4.Content = name_4;
                        radioButton_Name_5.Content = name_5;
                        break;
                    case 6:
                        radioButton_Name_7.Visibility = Visibility.Hidden;
                        radioButton_Name_1.Content = name_1;
                        radioButton_Name_2.Content = name_2;
                        radioButton_Name_3.Content = name_3;
                        radioButton_Name_4.Content = name_4;
                        radioButton_Name_5.Content = name_5;
                        radioButton_Name_6.Content = name_6;
                        break;
                    case 7:
                        radioButton_Name_1.Content = name_1;
                        radioButton_Name_2.Content = name_2;
                        radioButton_Name_3.Content = name_3;
                        radioButton_Name_4.Content = name_4;
                        radioButton_Name_5.Content = name_5;
                        radioButton_Name_6.Content = name_6;
                        radioButton_Name_7.Content = name_7;
                        break;
                }
            }
        }

        private int _personCount = 0;
        public int personCount
        {
            get { return _personCount; }
            set
            {
                if (_personCount != value)
                {
                    _personCount = value;
                    if (PropertyChanged != null)
                        PropertyChanged(this, new PropertyChangedEventArgs("personCount"));
                }
            }
        }

        private int _totalCount = 0;
        public int totalCount
        {
            get { return _totalCount; }
            set
            {
                if (_totalCount != value)
                {
                    _totalCount = value;
                    if (PropertyChanged != null)
                        PropertyChanged(this, new PropertyChangedEventArgs("totalCount"));
                }
            }
        }

        private string _name_1 = "";
        public string name_1
        {
            get { return _name_1; }
            set
            {
                if (_name_1 != value)
                {
                    _name_1 = value;
                    if (PropertyChanged != null)
                        PropertyChanged(this,new PropertyChangedEventArgs("name_1"));
                }
            }
        }

        private string _name_2 = "";
        public string name_2
        {
            get { return _name_2; }
            set
            {
                if (_name_2 != value)
                {
                    _name_2 = value;
                    if (PropertyChanged != null)
                        PropertyChanged(this, new PropertyChangedEventArgs("name_2"));
                }
            }
        }

        private string _name_3 = "";
        public string name_3
        {
            get { return _name_3; }
            set
            {
                if (_name_3 != value)
                {
                    _name_3 = value;
                    if (PropertyChanged != null)
                        PropertyChanged(this, new PropertyChangedEventArgs("name_3"));
                }
            }
        }

        private string _name_4 = "";
        public string name_4
        {
            get { return _name_4; }
            set
            {
                if (_name_4 != value)
                {
                    _name_4 = value;
                    if (PropertyChanged != null)
                        PropertyChanged(this, new PropertyChangedEventArgs("name_4"));
                }
            }
        }

        private string _name_5 = "";
        public string name_5
        {
            get { return _name_5; }
            set
            {
                if (_name_5 != value)
                {
                    _name_5 = value;
                    if (PropertyChanged != null)
                        PropertyChanged(this, new PropertyChangedEventArgs("name_5"));
                }
            }
        }

        private string _name_6 = "";
        public string name_6
        {
            get { return _name_6; }
            set
            {
                if (_name_6 != value)
                {
                    _name_6 = value;
                    if (PropertyChanged != null)
                        PropertyChanged(this, new PropertyChangedEventArgs("name_6"));
                }
            }
        }

        private string _name_7 = "";
        public string name_7
        {
            get { return _name_7; }
            set
            {
                if (_name_7 != value)
                {
                    _name_7 = value;
                    if (PropertyChanged != null)
                        PropertyChanged(this, new PropertyChangedEventArgs("name_7"));
                }
            }
        }

        private void radioButton_Name_1_Click(object sender, RoutedEventArgs e)
        {
            result = 1;
            DialogResult = true;
        }

        private void radioButton_Name_2_Click(object sender, RoutedEventArgs e)
        {
            result = 2;
            DialogResult = true;
        }

        private void radioButton_Name_3_Click(object sender, RoutedEventArgs e)
        {
            result = 3;
            DialogResult = true;
        }

        private void radioButton_Name_4_Click(object sender, RoutedEventArgs e)
        {
            result = 4;
            DialogResult = true;
        }

        private void radioButton_Name_5_Click(object sender, RoutedEventArgs e)
        {
            result = 5;
            DialogResult = true;
        }

        private void radioButton_Name_6_Click(object sender, RoutedEventArgs e)
        {
            result = 6;
            DialogResult = true;
        }

        private void radioButton_Name_7_Click(object sender, RoutedEventArgs e)
        {
            result = 7;
            DialogResult = true;
        }

        private void Window_Closing(object sender, CancelEventArgs e)
        {
            //DialogResult = false;
        }
    }
}
