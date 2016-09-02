using Ionic.Zip;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Forms;

namespace Compression
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {


        string userCreated=string.Empty ;
        FolderBrowserDialog fbd = new FolderBrowserDialog();
        List<ResourceInfo> list = new List<ResourceInfo>();//资源信息
        FileSecurity.FileSecurity fs = new FileSecurity.FileSecurity();//加密解密

        public MainWindow()
        {
            Pb_context = "加密进度";
               ZipName = "请输入文件名";
            isActived = true;
            InitializeComponent();
          
        }

       
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            ResizeMode = ResizeMode.NoResize;
    
            fbd.RootFolder = Environment.SpecialFolder.MyComputer;

            cmb_encrypt_compress.Items.Add("加密并压缩");
            cmb_encrypt_compress.Items.Add("仅加密");
            cmb_encrypt_compress.SelectedIndex = 0;

            
        }


        /// <summary>
        /// 编码
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button_encription_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (list.Count() == 0)
                {
                    System.Windows.Forms.MessageBox.Show("请选择需要加密的文件（列表为空）");
                    return;
                }
                s_sourceDirectory = tb_ToDirectoryPath.Text;

                if (cmb_encrypt_compress.SelectedItem.ToString() == "加密并压缩")
                {
                    SaveFileDialog sfd = new SaveFileDialog();
                    sfd.Filter = "zip files(*.zip) | *.zip|selfExtraction (*.exe)|*.exe ";
                    sfd.RestoreDirectory = true;
                    if (sfd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                    {
                        ZipName = sfd.FileName;
                        var th = new Thread(encryption_compress);
                        IsActived = false;
                        th.Start();
                    }
                }
                else if (cmb_encrypt_compress.SelectedItem.ToString() == "仅加密")
                {

                    if (fbd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                    {
                        userCreated = fbd.SelectedPath;
                        var th = new Thread(encryption);
                        IsActived = false;
                        th.Start();
                    }
                       
                }
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show(ex.Message,ex.HResult.ToString());
            }
            finally{

                IsActived = true;
                percent = 0;
            }
           
        }
        private void encryption()
        {
            try
            {
                double i = list.Count();
                double j = 1;
                FileSecurity.FileSecurity fs1 = new FileSecurity.FileSecurity();

                foreach (ResourceInfo item in list)
                {
                    Percent = (int)((j / i) * 100);//百分比 binding
                    CurrentProgressFileName = Path.Combine(ToDirectoryPath, item.ResourceName);
                    fs1.EncryptFile("abc", Path.Combine(ToDirectoryPath, item.ResourceName), Path.Combine(userCreated, item.ResourceName));
                    j++;
                    if (Percent == 100)
                    {
                        System.Windows.Forms.MessageBox.Show("操作已经完成");
                        Percent= 0;
                        IsActived = true;
                    }
                }

            }
            catch (Exception ex)
            {

                System.Windows.Forms.MessageBox.Show(ex.Message, ex.HResult.ToString());
                return;
            }
            finally {
                Percent= 0;
                IsActived = true;
            }
        
        }



        
        public event PropertyChangedEventHandler PropertyChanged;
      
        protected void NotifyPropertyChange(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }




    

        bool b = true;
        /// <summary>
        /// 选择文件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button_chooseFile_Click(object sender, RoutedEventArgs e)
        {
            Percent = 0;
            tb_fileNum.Text = "0";
            listView.ItemsSource = null;
            list.Clear();
            b = true;
            var openFileDialog = new Microsoft.Win32.OpenFileDialog();
            openFileDialog.Multiselect = true;
            openFileDialog.Filter = "all files|*.*";
            if ((bool)openFileDialog.ShowDialog())
            {
                tb_ToDirectoryPath.Text = Path.GetDirectoryName(openFileDialog.FileName);
                ToDirectoryPath = Path.GetDirectoryName(openFileDialog.FileName);
            }
            string[] s = new string[] { };
            s = openFileDialog.FileNames;
            foreach (string str in s)
            {
                if (s.Count(sc => sc == str) > 1)
                {
                    System.Windows.Forms.MessageBox.Show("文件名有重复，请检查文件夹:{0}");
                    return;
                }
             
            }
            foreach (string item in openFileDialog.FileNames)
            {
                ResourceInfo ri = new ResourceInfo
                {
                    ResourceName = Path.GetFileName(item),
                    ResourceType = item,
                    ResourceSize = (int)(new FileInfo(item)).Length
                };
                list.Add(ri);
            }
            tb_fileNum.Text = list.Count().ToString() ;
            listView.ItemsSource = list;
        }
        /// <summary>
        /// 选择文件夹
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button_choosefolder_Click(object sender, RoutedEventArgs e)
        {

            Percent = 0;
            tb_fileNum.Text = "0";
            list.Clear();
            listView.ItemsSource = null;
            b = false;

            if (fbd.ShowDialog() == System.Windows.Forms.DialogResult.Cancel)
            {
                return;
            }
            
            DirectoryInfo di = new DirectoryInfo(fbd.SelectedPath);
            FileInfo[] fi = di.GetFiles();
            foreach (FileInfo item in fi)
            {
                if (fi.Count(sc=>sc.FullName== item.FullName)>1)
                {
                    System.Windows.Forms.MessageBox.Show("文件名有重复，请检查文件夹:{0}", item.FullName);
                    return;
                }
            
            }
            foreach (FileInfo item in di.GetFiles())
            {
                ResourceInfo ri = new ResourceInfo()
                {
                    ResourceName = item.Name,
                    ResourceType = item.Name,
                    ResourceSize = (int)item.Length

                };
                list.Add(ri);
            }


            tb_ToDirectoryPath.Text = fbd.SelectedPath;
            ToDirectoryPath = fbd.SelectedPath;
            tb_fileNum.Text = list.Count().ToString();
            listView.ItemsSource = list;
        }

        private void button_compress_Click(object sender, RoutedEventArgs e)
        {
            if (list.Count()<=0)
            {
                System.Windows.Forms.MessageBox.Show("列表为空");
                return;
            }
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Filter = "zip files(*.zip) | *.zip|selfExtraction (*.exe)|*.exe";
            sfd.RestoreDirectory = true;
            if (sfd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                ZipName = sfd.FileName;

                Thread th = new Thread(compression);
                th.Start();
            }
                // grid_compress_mode.Visibility = Visibility.Visible;


            }


        /// <summary>
        /// 压缩（线程）
        /// </summary>
        void compression()
        {
            try
            {
                Pb_context = "压缩进度";
                ZipFile zf = new ZipFile(Path.Combine(userCreated, ZipName), Encoding.UTF8);
                foreach (ResourceInfo item in list)
                {
                    CurrentProgressFileName = Path.Combine(ToDirectoryPath, item.ResourceName);
                    zf.AddFile(Path.Combine(ToDirectoryPath, item.ResourceName));
                }
                zf.SaveProgress += Zf_SaveProgress;
                if (ZipName.Contains(".zip"))
                {
                    zf.Save();
                }
                else if (ZipName.Contains(".exe"))
                {
                    zf.SaveSelfExtractor(ZipName, SelfExtractorFlavor.WinFormsApplication);
                }
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show(ex.Message, ex.HResult.ToString());
            }
            finally
            {
                IsActived = true;
                Percent = 0;
            }

        }

        

        
        /// <summary>
        /// 获得焦点清空输入栏
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tb_zipName_GotFocus(object sender, RoutedEventArgs e)
        {
            ZipName = "";
        }



 

       
        /// <summary>
        /// 关闭进程（注意多线程）
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_Closing(object sender, CancelEventArgs e)
        {
             Process p=  Process.GetCurrentProcess();
             p.Kill();
        }

         
    
        String s_sourceDirectory;
        private void encryption_compress()
        {
            try
            {
                
                if (!Directory.Exists(Path.Combine(Path.Combine(userCreated, "temp"))))
                {
                    Directory.CreateDirectory(Path.Combine(Path.Combine(userCreated, "temp")));
                }
                double i = list.Count();
                double j = 1;
                Pb_context = "加密进度";
                ZipFile zf = new ZipFile(Path.Combine(userCreated, ZipName), Encoding.UTF8);
                FileSecurity.FileSecurity fs1 = new FileSecurity.FileSecurity();
                
                foreach (ResourceInfo item in list)
                {
                    Percent = (int)((j / i) * 100);//百分比 binding
                    fs1.EncryptFile("abc", Path.Combine(ToDirectoryPath, item.ResourceName), Path.Combine(Path.Combine(userCreated,"temp"), item.ResourceName));
                    zf.AddFile(Path.Combine(Path.Combine(userCreated, "temp"), item.ResourceName), "");
                    CurrentProgressFileName = item.ResourceName;
                    j++;
                    if (Percent == 100)
                    {
                        
                        Percent = 0;
                    }
                }
                Pb_context = "压缩进度";
                zf.SaveProgress += Zf_SaveProgress;
                if (ZipName.Contains(".zip"))
                {

                    zf.Save();
                }
                else if (ZipName.Contains(".exe"))
                {

                    zf.SaveSelfExtractor(ZipName, SelfExtractorFlavor.WinFormsApplication);
                }

                
            }
            catch (Exception ex)
            {

                System.Windows.Forms.MessageBox.Show(ex.Message, ex.HResult.ToString());
            }
            finally
            {
                IsActived = true;
                Percent = 0;
            }
        }


        private void Zf_SaveProgress(object sender, SaveProgressEventArgs e)
        {
            if (e.EntriesTotal!=0)
            {
                double i = e.EntriesSaved;
                double j = e.EntriesTotal;
                Percent = (int)((i/j)*100) ;
                CurrentProgressFileName = e.CurrentEntry.FileName.ToString();
               
                if (i==j)
                {
                    System.Windows.Forms.MessageBox.Show("操作已经完成");
                    CurrentProgressFileName = "完成";
                    IsActived = true;
                    if (Directory.Exists(Path.Combine(userCreated,"temp")))
                    {
                        DirectoryInfo di = new DirectoryInfo(Path.Combine(userCreated, "temp"));
                        foreach (FileInfo item in di.GetFiles())
                        {
                           
                                File.Delete(item.FullName);
                           
                        }

                        Directory.Delete(Path.Combine(userCreated, "temp"));
                    }
                }
               
            }
           
        }

        private string pb_context;
        private string zipName;
        private string currentProgressFileName;
        private Boolean isActived;
        string _ToDirectoryPath = "";
        private int percent = 0;
        /// <summary>
        /// 进度条百分比
        /// </summary>
        public int Percent
        {
            get { return this.percent; }
            set
            {
                this.percent = value;
                NotifyPropertyChange("Percent");
            }
        }


        /// <summary>
        /// 输入路径（加密解密路径）
        /// </summary>
        public string ToDirectoryPath
        {
            get { return _ToDirectoryPath; }
            set
            {
                _ToDirectoryPath = value;
                NotifyPropertyChange("ToDirectoryPath");
            }
        }
        /// <summary>
        /// 用户输入的压缩文件名
        /// </summary>
        public string ZipName
        {
            get
            {
                return zipName;
            }

            set
            {
                zipName = value;
                NotifyPropertyChange("ZipName");
            }
        }

        /// <summary>
        /// 当前操作的文件名
        /// </summary>
        public string CurrentProgressFileName
        {
            get
            {
                return currentProgressFileName;
            }

            set
            {
                currentProgressFileName = value;
                NotifyPropertyChange("CurrentProgressFileName");
            }
        }

        /// <summary>
        /// 主窗体是否可交互
        /// </summary>
        public bool IsActived
        {
            get
            {
                return isActived;
            }

            set
            {
                isActived = value;
                NotifyPropertyChange("IsActived");
            }
        }

        public string Pb_context
        {
            get
            {
                return pb_context;
            }

            set
            {
                pb_context = value;
                NotifyPropertyChange("Pb_context");
            }
        }



        /// <summary>
        /// 解码
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button_decrypt_Click(object sender, RoutedEventArgs e)
        {
            if (fbd.ShowDialog()==System.Windows.Forms.DialogResult.OK)
            {
                userCreated = fbd.SelectedPath;
                Thread th = new Thread(decryption);
                isActived = false;
                th.Start();
            }
        }
        void decryption()
        {
            try
            {
                double sum = list.Count();
                double count = 1;
                foreach (ResourceInfo item in list)
                {
                    Percent = (int)((count / sum) * 100);
                    CurrentProgressFileName = Path.Combine(ToDirectoryPath, item.ResourceName);
                    fs.DecryptFile("abc", Path.Combine(ToDirectoryPath, item.ResourceName), Path.Combine(userCreated, item.ResourceName));
                    if (Percent == 100)
                    {
                        System.Windows.Forms.MessageBox.Show("操作已经完成");
                        Percent = 0;
                        IsActived = true;
                        CurrentProgressFileName = "完成";
                    }
                    count++;
                }
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show(ex.Message, ex.HResult.ToString());
            }
            finally
            {
                IsActived = true;
                Percent = 0;
            }



        }
    }
}
