using System;
using System.Collections.Generic;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.IO;

namespace StreamAnalyzer
{
    /// <summary>
    /// MainWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class MainWindow : Window
    {
        #region Field
        private string[] FileName;
        public struct SCDFILEINFO
        {
            public string name;
            public long size;
            public int frame;
            public int BPF;
        } 
        private SCDFILEINFO myfile;

        private int CurrentDataVector_num { set; get; }

        List<string> DataVector;

        private Brush backgroundbrush;
        private BinaryReader reader;
        #endregion

        public MainWindow()
        {
            InitializeComponent();
            this.InitField();
            this.Init_Events();
        }

        private void InitField()
        {
            this.CurrentDataVector_num = 0;
            this.DataVector = new List<string>();
            this.TextBlock_Title.Text = "工程名：" + System.Diagnostics.Process.GetCurrentProcess().ProcessName;
        }

        /// <summary>
        /// イベントの登録
        /// </summary>
        private void Init_Events()
        {
            this.MouseLeftButtonDown += (sender, e) => this.DragMove();
            this.Closing += MainWindow_Closing;
        }

        /// <summary>
        /// ウィンドウ終了イベント
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MainWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (this.reader != null)
            {
                this.reader.Close();
            }
        }

        private void Button_Close_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }


        private void TextBlock_FilleDrop_MouseEnter(object sender, MouseEventArgs e)
        {
            this.backgroundbrush = this.TextBlock_FilleDrop.Background;
            this.TextBlock_FilleDrop.Background = Brushes.Black;
        }

        private void TextBlock_FilleDrop_MouseLeave(object sender, MouseEventArgs e)
        {
            this.TextBlock_FilleDrop.Background = this.backgroundbrush;
        }

        private void TextBlock_FilleDrop_Drop(object sender, DragEventArgs e)
        {
            try
            {
                this.FileName = e.Data.GetData(DataFormats.FileDrop) as string[];
                if (this.FileName != null)
                {
                    if (!this.FileName[0].Contains("scd"))
                    {
                        throw new Exception("\"scd\"ファイルのみ対応しています。");
                    }
                    this.myfile.name = this.FileName[0];
                    this.reader = new BinaryReader(File.OpenRead(this.myfile.name));
                    this.myfile.size = this.reader.BaseStream.Length;

                    if (this.myfile.size < 8)
                    {
                        throw new Exception("ファイルに正常な書き込みが行われていません。");
                    }
                    //First frame number
                    this.reader.ReadInt32();
                    this.reader.ReadInt64();
                    this.myfile.BPF = this.reader.ReadInt32();
                    this.myfile.frame = (int)this.myfile.size / (this.myfile.BPF + sizeof(int) * 2 + sizeof(long));

                    this.TextBlock_FileInfo1.Text = this.myfile.name;
                    this.TextBlock_FileInfo2.Text = this.myfile.size.ToString();
                    this.TextBlock_FileInfo3.Text = this.myfile.frame.ToString();
                    this.TextBlock_FileInfo4.Text = this.myfile.BPF.ToString();

                    this.ProgressBar_DataVector.Maximum = (double)this.myfile.BPF;
                    this.TextBlock_MaxDataLength.Text = this.myfile.BPF.ToString() + " byte";

                    this.TextBox_ExcelFileName.Text = this.myfile.name.Replace(".scd", ".csv");;

                    this.Update_DataVectorUI();
                    this.reader.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        #region DataVector add and remove UI events
        private void Button_AddInt_Click(object sender, RoutedEventArgs e)
        {
            this.AddInt();
        }

        private void AddInt()
        {
            this.DataVector.Add("int");
            this.ListBox_DataVector.Items.Add("4 byte int");
            this.CurrentDataVector_num += 4;
            this.Update_DataVectorUI();
        }

        private void Button_AddShort_Click(object sender, RoutedEventArgs e)
        {
            this.AddShort();
        }

        private void AddShort()
        {
            this.DataVector.Add("short");
            this.ListBox_DataVector.Items.Add("2 byte short");
            this.CurrentDataVector_num += 2;
            this.Update_DataVectorUI();
        }

        private void Button_AddLong_Click(object sender, RoutedEventArgs e)
        {
            this.AddLong();
        }

        private void AddLong()
        {
            this.DataVector.Add("long");
            this.ListBox_DataVector.Items.Add("8 byte long");
            this.CurrentDataVector_num += 8;
            this.Update_DataVectorUI();
        }

        private void Button_AddFloat_Click(object sender, RoutedEventArgs e)
        {
            this.AddFloat();
        }

        private void AddFloat()
        {
            this.DataVector.Add("float");
            this.ListBox_DataVector.Items.Add("4 byte float");
            this.CurrentDataVector_num += 4;
            this.Update_DataVectorUI();
        }

        private void Button_AddDouble_Click(object sender, RoutedEventArgs e)
        {
            this.AddDouble();
        }

        private void AddDouble()
        {
            this.DataVector.Add("double");
            this.ListBox_DataVector.Items.Add("8 byte double");
            this.CurrentDataVector_num += 8;
            this.Update_DataVectorUI();
        }

        private void Button_AddByte_Click(object sender, RoutedEventArgs e)
        {
            this.AddByte();
        }

        private void AddByte()
        {
            this.DataVector.Add("byte");
            this.ListBox_DataVector.Items.Add("1 byte byte");
            this.CurrentDataVector_num += 1;
            this.Update_DataVectorUI();
        }

        private void Update_DataVectorUI()
        {
            this.ProgressBar_DataVector.Value = this.CurrentDataVector_num;
            this.TextBlock_CurrentDataLength.Text = this.CurrentDataVector_num.ToString() + " byte";
            if(this.CurrentDataVector_num == this.myfile.BPF){
                this.TextBlock_StatusDataVector.Text = "データ配列の設定が完了しました。保存先ファイルの設定を行い、出力してください。";
            }
            else if (this.CurrentDataVector_num > this.myfile.BPF)
            {
                this.TextBlock_StatusDataVector.Text = "データ配列が多すぎます。リストを確認して削除してください。";
            }
            else if (this.CurrentDataVector_num < this.myfile.BPF)
            {
                this.TextBlock_StatusDataVector.Text = "データ配列が不足しています。";
            }

            if (this.myfile.BPF == 0)
            {
                this.TextBlock_StatusDataVector.Text = "ファイルをドラッグ＆ドロップしてデータをセットしてください。";
            }
        }

        private void Button_DeleteBack_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                this.ListBox_DataVector.Items.RemoveAt(this.DataVector.Count - 1);
                switch (this.DataVector[this.DataVector.Count - 1])
                {
                    case "int":
                        this.CurrentDataVector_num -= 4;
                        break;
                    case "short":
                        this.CurrentDataVector_num -= 2;
                        break;
                    case "long":
                        this.CurrentDataVector_num -= 8;
                        break;
                    case "float":
                        this.CurrentDataVector_num -= 4;
                        break;
                    case "double":
                        this.CurrentDataVector_num -= 8;
                        break;
                    case "byte":
                        this.CurrentDataVector_num -= 1;
                        break;
                    default:
                        break;
                }
                this.DataVector.RemoveAt(this.DataVector.Count - 1);

                this.Update_DataVectorUI();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void Button_DeleteAll_Click(object sender, RoutedEventArgs e)
        {
            this.DeleteAll();
        }

        private void DeleteAll()
        {
            try
            {
                this.ListBox_DataVector.Items.Clear();
                this.DataVector.Clear();
                this.CurrentDataVector_num = 0;
                this.Update_DataVectorUI();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        #endregion

        private void Button_OutputExcelFile_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (this.CurrentDataVector_num != this.myfile.BPF)
                {
                    throw new Exception("データ配列の設定が完了していません．BPFが合うように設定してください．");
                }
                FileStream fs;
                StreamWriter sw;
                try
                {
                    fs = new FileStream(this.TextBox_ExcelFileName.Text, FileMode.CreateNew);
                    sw = new StreamWriter(fs);
                }
                catch (System.IO.IOException ioex)
                {
                    myDialog mydialog = new myDialog("そのファイルはすでに存在します。上書きしますか？");
                    if (mydialog.ShowDialog() == true)
                    {
                        fs = new FileStream(this.TextBox_ExcelFileName.Text, FileMode.Create);
                        sw = new StreamWriter(fs);
                    }
                    else
                    {
                        throw new Exception("ファイル出力を中止します。");
                    }
                }
                this.reader = new BinaryReader(File.OpenRead(this.myfile.name));
                try
                    {
                    while(true){
                    
                        int frame = this.reader.ReadInt32();
                        long time = this.reader.ReadInt64();
                        int datalength = this.reader.ReadInt32();
                        string str = "";
                        str += frame.ToString() + ",";
                        str += time.ToString() + ",";
                        foreach (var data in this.DataVector)
                        {
                            switch (data)
                            {
                                case "int":
                                    {
                                        int i = this.reader.ReadInt32();
                                        str += i.ToString() + ",";
                                    }
                                    break;
                                case "short":
                                    {
                                        short i = this.reader.ReadInt16();
                                        str += i.ToString() + ",";
                                    }
                                    break;
                                case "long":
                                    {
                                        long i = this.reader.ReadInt64();
                                        str += i.ToString() + ",";
                                    }
                                    break;
                                case "float":
                                    {
                                        float i = this.reader.ReadSingle();
                                        str += i.ToString() + ",";
                                    }
                                    break;
                                case "double":
                                    {
                                        double i = this.reader.ReadDouble();
                                        str += i.ToString() + ",";
                                    }
                                    break;
                                case "byte":
                                    {
                                        byte i = this.reader.ReadByte();
                                        str += i.ToString() + ",";
                                    }
                                    break;
                                default:
                                    break;
                            }
                        }

                        str = str.TrimEnd(',');
                        sw.WriteLine(str);

                    }
                }
                catch (Exception ex)
                {
                    sw.Close();
                    fs.Close();
                    this.reader.Close();
                    throw new Exception("出力が完了しました。");
                }
            }
            catch(Exception ex)
            {
                myDialog mydialog = new myDialog(ex.Message);
                mydialog.ShowDialog();
            }
        }


        private void Button_SaveSettingFile_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
                dlg.FileName = "default.rsf"; // Default file name
                dlg.DefaultExt = ".rsf"; // Default file extension
                dlg.Filter = "ReaderSettingFile (.rsf)|*.rsf"; // Filter files by extension
                dlg.CheckFileExists = false;
                dlg.CheckPathExists = false;

                // Show open file dialog box
                Nullable<bool> result = dlg.ShowDialog();

                // Process open file dialog box results
                if (result == true)
                {
                    // Open document
                    this.TextBlock_SettingFileName.Text = dlg.FileName;
                }
                else
                {
                    throw new Exception("読込を中止しました．");
                }

                FileStream fs;
                StreamReader sr;
                try
                {
                    fs = new FileStream(this.TextBlock_SettingFileName.Text, FileMode.Open);
                    sr = new StreamReader(fs);
                }
                catch (System.IO.IOException ioex)
                {
                    myDialog mydialog = new myDialog("そのファイルはすでに存在します。上書きしますか？");
                    if (mydialog.ShowDialog() == true)
                    {
                        fs = new FileStream(this.TextBlock_SettingFileName.Text, FileMode.Create);
                        sr = new StreamReader(fs);
                    }
                    else
                    {
                        throw new Exception("ファイル出力を中止します。");
                    }
                }

                this.DeleteAll();
                
                string str = sr.ReadLine();
                int vectorlength = int.Parse(str);
                for (int i = 0; i < vectorlength; i++ )
                {
                    str = sr.ReadLine();
                    switch (str)
                    {
                        case "int":
                            this.AddInt();
                            break;
                        case "short":
                            this.AddShort();
                            break;
                        case "long":
                            this.AddLong();
                            break;
                        case "float":
                            this.AddFloat();
                            break;
                        case "double":
                            this.AddDouble();
                            break;
                        case "byte":
                            this.AddByte();
                            break;
                        default:
                            break;
                    }
                }
                sr.Close();
                fs.Close();
            }
            catch (Exception ex)
            {
                myDialog mydialog = new myDialog(ex.Message);
                mydialog.ShowDialog();
            }
        }

        private void Button_ReadSettingFile_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Microsoft.Win32.SaveFileDialog dlg = new Microsoft.Win32.SaveFileDialog();
                dlg.FileName = "default.rsf"; // Default file name
                dlg.DefaultExt = ".rsf"; // Default file extension
                dlg.Filter = "ReaderSettingFile (.rsf)|*.rsf"; // Filter files by extension
                dlg.CheckFileExists = false;
                dlg.CheckPathExists = false;

                // Show open file dialog box
                Nullable<bool> result = dlg.ShowDialog();

                // Process open file dialog box results
                if (result == true)
                {
                    // Open document
                    this.TextBlock_SettingFileName.Text = dlg.FileName;
                }
                else
                {
                    throw new Exception("保存を中止しました．");
                }

                FileStream fs;
                StreamWriter sw;
                try
                {
                    fs = new FileStream(this.TextBlock_SettingFileName.Text, FileMode.CreateNew);
                    sw = new StreamWriter(fs);
                }
                catch (System.IO.IOException ioex)
                {
                    myDialog mydialog = new myDialog("そのファイルはすでに存在します。上書きしますか？");
                    if (mydialog.ShowDialog() == true)
                    {
                        fs = new FileStream(this.TextBlock_SettingFileName.Text, FileMode.Create);
                        sw = new StreamWriter(fs);
                    }
                    else
                    {
                        throw new Exception("ファイル出力を中止します。");
                    }
                }

                sw.WriteLine(this.DataVector.Count);
                foreach (var item in this.DataVector)
                {
                    sw.WriteLine(item);
                }
                sw.Close();
                fs.Close();
                throw new Exception("設定を保存しました．"); 
            }
            catch (Exception ex)
            {
                myDialog mydialog = new myDialog(ex.Message);
                mydialog.ShowDialog();
            }
        }
    }
}
