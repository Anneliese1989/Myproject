using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Threading;

namespace FileSecurity
{
  /// <summary>
  /// 加密与解密类
  /// </summary>
  public class FileSecurity
  {
    /// <summary>
    ///
    /// 解密文件 二进制长度
    ///
    /// </summary>
    private static int destLen = 8 * 1024;

    /// <summary>
    /// 当操作进度发生变化时，触发事件
    /// </summary>
    public event EventHandler<SecurityProcessArgs> SecurityProcessChangedEvent;

    /// <summary>
    /// 解锁Base64安全码
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    private byte[] ToKey(string key)
    {
        
      byte[] aeskey = Convert.FromBase64String(key);
      return aeskey;
    }

        /// <summary>
        /// 获得安全码
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
    private string GetSecurityKey(string key)
    {
      //实例化MD5加密类
      MD5 md5 = new MD5CryptoServiceProvider();
      //计算安全码的hash
      byte[] s = md5.ComputeHash(UnicodeEncoding.UTF8.GetBytes(key));
      //高级加密算法的
      AesCryptoServiceProvider aes = new AesCryptoServiceProvider();
            //获取或者设置加密解密的对称密钥
      aes.Key = s;
      //返回BASE64的对称密钥
      return Convert.ToBase64String(aes.Key);
    }
        /// <summary>
        /// 加密
        /// </summary>
        /// <param name="data"></param>
        /// <param name="pwd">安全码</param>
        /// <returns></returns>
    public byte[] Encrypt(byte[] data, string pwd)
    {
      byte[] key = ToKey(GetSecurityKey(pwd));
            //实例化算法
      RijndaelManaged rDel = new RijndaelManaged();
            //获取或者设置加密解密的对称key
      rDel.Key = key;
      rDel.Mode = CipherMode.ECB;
      rDel.Padding = PaddingMode.PKCS7;
      ICryptoTransform cTransform = rDel.CreateEncryptor();
      byte[] resultArray = cTransform.TransformFinalBlock(data, 0, data.Length);
       //返回已经被加密的数据流
      return resultArray;
    }
        /// <summary>
        /// 解密
        /// </summary>
        /// <param name="data"></param>
        /// <param name="pwd"></param>
        /// <returns></returns>
    public byte[] Decrypt(byte[] data, string pwd)
    {
      byte[] key = ToKey(GetSecurityKey(pwd));
      RijndaelManaged rDel = new RijndaelManaged();
      rDel.Key = key;
      rDel.Mode = CipherMode.ECB;
      rDel.Padding = PaddingMode.PKCS7;
      ICryptoTransform cTransform = rDel.CreateDecryptor();
      byte[] result = null;
      try
      {
        result = cTransform.TransformFinalBlock(data, 0, data.Length);
      }
      catch (Exception ex)
      {
        Console.Write(ex);
      }

      return result;
    }
          
    /// <summary>
    /// 加密文件
    /// </summary>
    /// <param name="pwd">文件安全码</param>
    /// <param name="sourceFile">源文件</param>
    /// <param name="destFile">加密过的文件</param>
    public void EncryptFile(string pwd, string sourceFile, string destFile)
    {
      using (FileStream sourcefs = File.Open(destFile, FileMode.OpenOrCreate))
      using (FileStream destfs = File.Open(sourceFile, FileMode.Open))
      {
        byte[] destData = new byte[destLen];
        int length = 0;
        byte[] key = ToKey(GetSecurityKey(pwd));

        //实例化托管的算法
        RijndaelManaged rDel = new RijndaelManaged();
        rDel.Key = key;
        rDel.Mode = CipherMode.ECB;
        rDel.Padding = PaddingMode.PKCS7;

        ICryptoTransform cTransform = rDel.CreateEncryptor();
                //开始加密
        CryptoStream cs = new CryptoStream(destfs, cTransform, CryptoStreamMode.Read);

        long totalPoint = sourcefs.Length;
                SecurityProcessArgs oldArgs = new SecurityProcessArgs(0, 0);


        while ((length = cs.Read(destData, 0, destLen)) > 0)
        {
          var newArgs = new SecurityProcessArgs(destfs.Position, totalPoint);
          if (newArgs.ProcessPercent != oldArgs.ProcessPercent)
          {
            OnSecurityProcessChanged(newArgs);
            oldArgs = newArgs;
          }

          if (newArgs.NeedCancel)
          {
            break;
          }

          //输出
          sourcefs.Write(destData, 0, length);
        }
      }
    }

    /// <summary>
    /// 解密文件
    /// </summary>
    /// <param name="pwd"></param>
    /// <param name="destFile"></param>
    /// <param name="sourceFile"></param>
    public void DecryptFile(string pwd, string destFile, string sourceFile)
    {
      using (FileStream destfs = File.Open(destFile, FileMode.Open))
      using (FileStream sourcefs = File.Open(sourceFile, FileMode.OpenOrCreate))
      {
        byte[] destData = new byte[destLen];
        int length = 0;
        byte[] key = ToKey(GetSecurityKey(pwd));
        RijndaelManaged rDel = new RijndaelManaged();
        rDel.Key = key;
        rDel.Mode = CipherMode.ECB;
        rDel.Padding = PaddingMode.PKCS7;
        ICryptoTransform cTransform = rDel.CreateDecryptor();
        CryptoStream cs = new CryptoStream(destfs, cTransform, CryptoStreamMode.Read);

        long totalPoint = destfs.Length;
        SecurityProcessArgs oldArgs = new SecurityProcessArgs(0, 0);
        while ((length = cs.Read(destData, 0, destLen)) > 0)
        {
          var newArgs = new SecurityProcessArgs(destfs.Position, totalPoint);
          if (newArgs.ProcessPercent != oldArgs.ProcessPercent)
          {
            OnSecurityProcessChanged(newArgs);
            oldArgs = newArgs;
          }

          if (newArgs.NeedCancel)
          {
            break;
          }

          sourcefs.Write(destData, 0, length);
        }
      }
    }

    protected virtual void OnSecurityProcessChanged(SecurityProcessArgs args)
    {
      var handler = SecurityProcessChangedEvent;
      if (null != handler)
      {
        handler.Invoke(this, args);
      }
    }
  }

  public class SecurityProcessArgs : EventArgs
  {
    public SecurityProcessArgs(long currentPoint, double totalPoint)
        : base()
    {
      this.currentPoint = currentPoint;
      this.totalPoint = totalPoint;
    }

    private long currentPoint;
    private double totalPoint;

    /// <summary>
    /// 进度的百分比
    /// </summary>
    public int ProcessPercent
    {
      get
      {
        if (totalPoint == 0d)
        {
          return 0;
        }
        else
        {
          return (int)((currentPoint / totalPoint) * 100);
        }
      }
    }

    /// <summary>
    /// 如果进度需要停止，则传递此值通知，注意本次停止不会操作文件，比如删除冗余文件
    /// </summary>
    public bool NeedCancel { get; set; }
  }
}