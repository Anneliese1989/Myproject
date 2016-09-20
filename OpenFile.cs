     private void OpenScreenKeyboard ( )
        {

            try
            {
                string path = @"C:\Program Files\Common Files\microsoft shared\ink\tabtip.exe";
                if (File.Exists ( path ))
                {
                    Process.Start ( path );
                }
                else
                {
                    OpenFileDialog openFileDialog = new OpenFileDialog ( );
                    openFileDialog.InitialDirectory = "c:\\"; 
                    openFileDialog.Filter = "平板键盘|tabtip.exe|软键盘|osk.exe|可执行文件|*.exe|所有文件|*.*";
                    openFileDialog.RestoreDirectory = true;
                    openFileDialog.FilterIndex = 1;
                    if (openFileDialog.ShowDialog ( ) == System.Windows.Forms.DialogResult.OK)
                    {
                        path = openFileDialog.FileName;
                        Process.Start ( path );
                    }
                }
            }
            catch (Exception ex)
            {

                XMessageBox.Show  ( ex.Message ,"应用程序错误。");
            }
         
        }
