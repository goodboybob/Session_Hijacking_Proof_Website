using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Data.SqlClient;
using System.Web.UI.WebControls;
using System.Text;
using System.Security.Cryptography;


public partial class home : System.Web.UI.Page
{
    private SqlCommand sqlCmd;
    private String uid;
    private String pas;
    private String uname;
    private String uage;
    private String Ipaddress;
    private String useragent;
    private Int32 unum;
    private String udata;
    private String correctpass;
    private SqlConnection hookUp;
    private SqlDataReader reader;
    private String passphrase = "secret";
    private String localkey;
   // private String dbconn = "workstation id=logindb.mssql.somee.com;packet size=4096;user id=boys;pwd=secret123;data source=logindb.mssql.somee.com;persist security info=False;initial catalog=logindb";
    // private String dbconn = "Server=localhost\\SqlExpress;Database=login;Integrated Security=True";
    
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


    protected void Page_Load(object sender, EventArgs e)
    {

        if ((Request.Cookies.Get("userid") != null))
        {

            HttpCookie IPadd = Request.Cookies.Get("IP");
            Ipaddress = DecryptString((IPadd.Value.ToString()), passphrase);
            String realIP = Request.UserHostAddress.ToString();

            if ((String.Compare(realIP, Ipaddress)) == 0)
            {
                HttpCookie uagent = Request.Cookies.Get("UserAgent");
                useragent = DecryptString((uagent.Value.ToString()), passphrase);
                String realUagent = Request.UserAgent.ToString();

                if ((String.Compare(useragent, realUagent)) == 0)
                {
                    ClientScript.GetPostBackEventReference(this, string.Empty);
                    if (this.IsPostBack)
                    {

                        string eventTarget = (this.Request["__EVENTTARGET"] == null) ?
                         string.Empty : this.Request["__EVENTTARGET"];
                        string eventArgument = (this.Request["__EVENTARGUMENT"] == null) ?
                         string.Empty : this.Request["__EVENTARGUMENT"];

                        if (eventTarget == "callServersideMethod")
                        {
                          //==========================================================================================
                            HttpCookie uidd = Request.Cookies.Get("userid");
                            uid = DecryptString((uidd.Value.ToString()), passphrase);

                            hookUp = new SqlConnection(dbconn);
                            sqlCmd = new SqlCommand("SELECT email FROM login WHERE (userid = N'" + uid + "')", hookUp);
                            hookUp.Open();
                            reader = sqlCmd.ExecuteReader();
                            while (reader.Read())
                            {
                                localkey = Convert.ToString(reader["email"]);
                            }
                            reader.Close();
                            //========================================================================================
                            String recievedkey=(DecryptString(eventArgument, passphrase));
                         //   String recievedkey = eventArgument;
                            if  ((String.Compare(recievedkey, localkey)) == 0)
                            {
                                display(uagent);
                            }
                            else
                            {
                                HttpCookie cookie1 = new HttpCookie("userid");
                                cookie1.Value = EncryptString("0", passphrase);
                                Response.Cookies.Add(cookie1);

                                HttpCookie cookie2 = new HttpCookie("pass");
                                cookie2.Value = EncryptString("0", passphrase);
                                Response.Cookies.Add(cookie2);

                                HttpCookie cookie3 = new HttpCookie("SecurityLevel");
                                cookie3.Value = EncryptString("Three", passphrase);
                                Response.Cookies.Add(cookie3);

                                Response.Redirect("hijacking.aspx");
                            }

                        }
                        else
                        {
                        }
                    }
                    else
                    {
                    }
                }
                else
                {
                    HttpCookie cookie1 = new HttpCookie("userid");
                    cookie1.Value = EncryptString("0", passphrase);
                    Response.Cookies.Add(cookie1);

                    HttpCookie cookie2 = new HttpCookie("pass");
                    cookie2.Value = EncryptString("0", passphrase);
                    Response.Cookies.Add(cookie2);

                    HttpCookie cookie3 = new HttpCookie("SecurityLevel");
                    cookie3.Value = EncryptString("Two", passphrase);
                    Response.Cookies.Add(cookie3);

                    Response.Redirect("hijacking.aspx");
                }
                }
                    else
                    {
                        HttpCookie cookie1 = new HttpCookie("userid");
                        cookie1.Value = EncryptString("0", passphrase);
                        Response.Cookies.Add(cookie1);

                        HttpCookie cookie2 = new HttpCookie("pass");
                        cookie2.Value = EncryptString("0", passphrase);
                        Response.Cookies.Add(cookie2);

                        HttpCookie cookie3 = new HttpCookie("SecurityLevel");
                        cookie3.Value = EncryptString("One", passphrase);
                        Response.Cookies.Add(cookie3);

                        Response.Redirect("hijacking.aspx");

                    }
                }

             else
             {
                    Response.Redirect("Default.aspx");
             }          
        
    }
    protected void display(HttpCookie uagent)
    {

        HttpCookie uidd = Request.Cookies.Get("userid");
        HttpCookie passs = Request.Cookies.Get("pass");

        uid = DecryptString((uidd.Value.ToString()), passphrase);
        pas = DecryptString((passs.Value.ToString()), passphrase);

        hookUp = new SqlConnection(dbconn);
        sqlCmd = new SqlCommand("SELECT password FROM login WHERE (userid = N'" + uid + "')", hookUp);
        hookUp.Open();
        reader = sqlCmd.ExecuteReader();
        while (reader.Read())
        {
            correctpass = Convert.ToString(reader["password"]);
        }
        reader.Close();
        if ((String.Compare(correctpass, pas)) == 0)
        {
            //********************************************************************************

            sqlCmd = new SqlCommand("SELECT name FROM login WHERE (userid = N'" + uid + "')", hookUp);
            reader = sqlCmd.ExecuteReader();
            while (reader.Read())
            {
                uname = Convert.ToString(reader["name"]);
            }
            reader.Close();
            //********************************************************************************
            Label1.Text = uname;
            //********************************************************************************

            sqlCmd = new SqlCommand("SELECT age FROM login WHERE (userid = N'" + uid + "')", hookUp);
            reader = sqlCmd.ExecuteReader();
            while (reader.Read())
            {
                uage = Convert.ToString(reader["age"]);
            }
            reader.Close();
            //********************************************************************************
            Label2.Text = uage;
            Label3.Text = Request.UserHostAddress.ToString();
            String agent;
            agent = DecryptString((uagent.Value.ToString()), passphrase);
            Label4.Text = agent;

            IServiceProvider provider = (IServiceProvider)HttpContext.Current;
            HttpRequest util = (HttpRequest)provider.GetService(typeof(HttpRequest));
            // Get the worker
            HttpWorkerRequest wr = (HttpWorkerRequest)provider.GetService(typeof(HttpWorkerRequest));
            // Get the NIC address
            // string addr = wr.GetLocalAddress(); 
            int remotePort = wr.GetRemotePort();
            Label5.Text = remotePort.ToString();
            Label6.Text = Request.Browser.ToString();

            //********************************************************************************

            sqlCmd = new SqlCommand("SELECT cuser FROM login WHERE (userid = N'" + uid + "')", hookUp);
            reader = sqlCmd.ExecuteReader();
            while (reader.Read())
            {
                unum = Convert.ToInt32(reader["cuser"]);
            }
            reader.Close();
            //********************************************************************************
            Image1.ImageUrl = ("users/" + unum + "/pic.jpg");


            //###################################################################################
            // Now we will read the file using c#
            // but be sure to add "using system.io" in the start of the code file
            //###################################################################################

            //StreamReader myFile = new StreamReader("c:\\Users\\winay143\\Desktop\\loginsite\\users\\" + unum + "\\data.txt");
            // StreamReader myFile = new StreamReader("d:\\DZHosts\\LocalUser\\mitproject\\www.mitproject.somee.com\\users\\" + unum + "\\data.txt");
             StreamReader myFile = new StreamReader("d:\\DZHosts\\LocalUser\\backupmitproject\\www.backupmitproject.somee.com\\users\\" + unum + "\\data.txt");
            udata = myFile.ReadToEnd();
            myFile.Close();

            //##############################################################################
            TextBox1.Text = udata;
        }
        else
        {
            Response.Redirect("Default.aspx");
        }
        hookUp.Close();
    }
    protected void Button1_Click(object sender, EventArgs e)
    {
        
        HttpCookie cookie1 = new HttpCookie("userid");
        cookie1.Value = EncryptString("0",passphrase);
        Response.Cookies.Add(cookie1);

        HttpCookie cookie2 = new HttpCookie("pass");
        cookie2.Value = EncryptString("0",passphrase);
        Response.Cookies.Add(cookie2);
        Response.Redirect("Default.aspx");
    }
}