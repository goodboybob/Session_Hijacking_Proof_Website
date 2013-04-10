using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data.SqlClient;
using System.Net.NetworkInformation;
using System.Text;
using System.Security.Cryptography;

public partial class _Default : System.Web.UI.Page
{
    private SqlCommand sqlCmd;
    private String usid;
    private String pass;
    private String uuid;
    private String pas;
    public String passphrase="secret";
    public String localkey; 
    private String correctpass;
    private SqlConnection hookUp;
    private SqlDataReader reader;
   // private String dbconn = "workstation id=logindb.mssql.somee.com;packet size=4096;user id=boys;pwd=secret123;data source=logindb.mssql.somee.com;persist security info=False;initial catalog=logindb";
    // private String dbconn = "Server=localhost\\SqlExpress;Database=login;" + "Integrated Security=True";
    private String dbconn = "workstation id=backuplogindb.mssql.somee.com;packet size=4096;user id=boyz;pwd=secret123;data source=backuplogindb.mssql.somee.com;persist security info=False;initial catalog=backuplogindb";
    public string EncryptString(string strToEncrypt, string strKey)
    {
        try
        {
            TripleDESCryptoServiceProvider objDESCrypto =
                new TripleDESCryptoServiceProvider();
            MD5CryptoServiceProvider objHashMD5 = new MD5CryptoServiceProvider();
            byte[] byteHash, byteBuff;
            string strTempKey = strKey;
            byteHash = objHashMD5.ComputeHash(ASCIIEncoding.ASCII.GetBytes(strTempKey));
            objHashMD5 = null;
            objDESCrypto.Key = byteHash;
            objDESCrypto.Mode = CipherMode.ECB; //CBC, CFB
            byteBuff = ASCIIEncoding.ASCII.GetBytes(strToEncrypt);
            return Convert.ToBase64String(objDESCrypto.CreateEncryptor().
                TransformFinalBlock(byteBuff, 0, byteBuff.Length));
        }
        catch (Exception ex)
        {
            return "Wrong Input. " + ex.Message;
        }
    }


    public string DecryptString(string strEncrypted, string strKey)
    {
        try
        {
            TripleDESCryptoServiceProvider objDESCrypto =
                new TripleDESCryptoServiceProvider();
            MD5CryptoServiceProvider objHashMD5 = new MD5CryptoServiceProvider();
            byte[] byteHash, byteBuff;
            string strTempKey = strKey;
            byteHash = objHashMD5.ComputeHash(ASCIIEncoding.ASCII.GetBytes(strTempKey));
            objHashMD5 = null;
            objDESCrypto.Key = byteHash;
            objDESCrypto.Mode = CipherMode.ECB; //CBC, CFB
            byteBuff = Convert.FromBase64String(strEncrypted);
            string strDecrypted = ASCIIEncoding.ASCII.GetString
            (objDESCrypto.CreateDecryptor().TransformFinalBlock
            (byteBuff, 0, byteBuff.Length));
            objDESCrypto = null;
            return strDecrypted;
        }
        catch (Exception ex)
        {
            return "Wrong Input. " + ex.Message;
        }
    }

    
    protected void authenticate(object sender, EventArgs e)
    {
        usid = uid.Text;
        pass = Password1.Value;

        hookUp = new SqlConnection(dbconn);
        //=====================================================================================================
        sqlCmd = new SqlCommand("SELECT password FROM login WHERE (userid = N'" + usid + "')", hookUp);
        hookUp.Open();
        reader = sqlCmd.ExecuteReader();
        while (reader.Read())
        {
            correctpass = Convert.ToString(reader["password"]);
        }
        reader.Close();
        //=========================================================================================================
        sqlCmd = new SqlCommand("SELECT email FROM login WHERE (userid = N'" + usid + "')", hookUp);
        reader = sqlCmd.ExecuteReader();
        while (reader.Read())
        {
            localkey = Convert.ToString(reader["email"]);
        }
        reader.Close();
        //=========================================================================================================
        if ((String.Compare(correctpass, pass)) == 0)
        {
            HttpCookie cookie1 = new HttpCookie("userid");
            cookie1.Value = EncryptString(usid.ToString(),passphrase);
            cookie1.HttpOnly = true;
            Response.Cookies.Add(cookie1);
        

            HttpCookie cookie2 = new HttpCookie("pass");
            cookie2.Value = EncryptString(pass.ToString(), passphrase);
            cookie2.HttpOnly = true;
            Response.Cookies.Add(cookie2);


            HttpCookie cookie3 = new HttpCookie("IP");
            cookie3.Value = EncryptString(Request.UserHostAddress.ToString(), passphrase);
            cookie3.HttpOnly = true;
            Response.Cookies.Add(cookie3);

            HttpCookie cookie4 = new HttpCookie("UserAgent");
            String agent;
            agent = EncryptString(Request.UserAgent.ToString(),passphrase);
            cookie4.Value =agent;
            cookie4.HttpOnly = true;
            Response.Cookies.Add(cookie4);

            HttpCookie cookie5 = new HttpCookie("Url.Port");
            IServiceProvider provider = (IServiceProvider)HttpContext.Current;
            HttpRequest util = (HttpRequest)provider.GetService(typeof(HttpRequest));
            // Get the worker
            HttpWorkerRequest wr = (HttpWorkerRequest)provider.GetService(typeof(HttpWorkerRequest)); 
            int remotePort = wr.GetRemotePort();
            cookie5.Value = remotePort.ToString();
            cookie5.HttpOnly = true;
            Response.Cookies.Add(cookie5);

            HttpCookie cookie6 = new HttpCookie("key");
            cookie6.Value = EncryptString(localkey,passphrase);
            Response.Cookies.Add(cookie6);  

            HttpCookie cookie7 = new HttpCookie("SecurityLevel");
            cookie7.Value = EncryptString("0", passphrase);
            cookie7.HttpOnly = true;
            Response.Cookies.Add(cookie7);

            
            hookUp.Close();

            Response.Redirect("home.aspx");


        }
        else
        {
            Label1.Text = "Wrong user Id or Password";
            hookUp.Close();
        }
    }


    protected void Page_Load(object sender, EventArgs e)
    {
        if (((Request.Cookies.Get("userid")) != null) && ((Request.Cookies.Get("pass")) != null))
        {
            HttpCookie uidd = Request.Cookies.Get("userid");
            HttpCookie passs = Request.Cookies.Get("pass");
        

            uuid = DecryptString((uidd.Value.ToString()),passphrase);
            pas = DecryptString((passs.Value.ToString()),passphrase);


            if ((uuid != "0") && (pas !="0"))
            {

                           
                hookUp = new SqlConnection(dbconn);
                sqlCmd = new SqlCommand("SELECT password FROM login WHERE (userid = N'" + uuid + "')", hookUp);
                hookUp.Open();
                reader = sqlCmd.ExecuteReader();
                while (reader.Read())
                {
                    correctpass = Convert.ToString(reader["password"]);
                }
                reader.Close();
             
                if ((String.Compare(correctpass, pas)) == 0)
                {
                    reader.Close();
                    hookUp.Close();
                  
                    Response.Redirect("home.aspx");
                }
            }
        }
    }
}