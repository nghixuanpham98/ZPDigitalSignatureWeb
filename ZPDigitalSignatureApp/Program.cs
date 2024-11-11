using System;
using System.IO;
using System.Data;
using System.Linq;
using System.Drawing;
using System.Runtime;
using System.Net;
using System.Net.Sockets;
using System.Net.Security;
using System.Text;
using System.Text.RegularExpressions;
using System.Configuration;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using RestSharp;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using iTextSharp.text;
using iTextSharp.text.pdf;
using iTextSharp.text.pdf.security;
using Org.BouncyCastle.Crmf;
using Org.BouncyCastle.Pkcs;
using Org.BouncyCastle.X509;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Asn1.Ocsp;
using WebSocketSharp;
using WebSocketSharp.Server;
using ErrorEventArgs = WebSocketSharp.ErrorEventArgs;
using System.Runtime.InteropServices;
using static System.Runtime.InteropServices.JavaScript.JSType;
using Microsoft.Win32;
using System.Reflection;

namespace ZPDigitalSignatureApp
{
    class Program
    {
        #region -- Configuration --

        public class Configuration
        {
            public static float posX = 0;
            public static float posY = 0;
            public static int pageSign = 1;
            public static int isCompleted = 0;
            public static float widthPlace = 180;
            public static float heightPlace = 160;
            public static string dn = string.Empty;
            public static string reason = string.Empty;
            public static string contact = string.Empty;
            public static string location = string.Empty;
            public static string certName = string.Empty;
            public static string signID = string.Empty;
            public static string fileSignID = string.Empty;
            public static string fileImageID = string.Empty;
            public static string apiUrl = ConfigurationManager.AppSettings["ApiUrl"];
            public static string serverIP = ConfigurationManager.AppSettings["ServerIP"];
            public static int port = Int32.Parse(ConfigurationManager.AppSettings["Port"]);
            public static int widthImage = Int32.Parse(ConfigurationManager.AppSettings["WidthImage"]);
            public static int heightImage = Int32.Parse(ConfigurationManager.AppSettings["HeightImage"]);
            public static string pdfFilePath = AppDomain.CurrentDomain.BaseDirectory + "/Files/pdf/";
            public static string imageFilePath = AppDomain.CurrentDomain.BaseDirectory + "/Files/icons/";
        }

        #endregion

        #region -- Main --

        static void Main(string[] args)
        {
            [DllImport("kernel32.dll")]
            static extern IntPtr GetConsoleWindow();

            [DllImport("user32.dll")]
            static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

            const int SW_HIDE = 0;
            //const int SW_SHOW = 5;
            var handle = GetConsoleWindow();

            // Hide
            ShowWindow(handle, SW_HIDE);

            //// Show
            //ShowWindow(handle, SW_SHOW);

            Program run = new Program();
            run.RunOnStartup();
            run.CreateFolderFile();
            run.CreateWebSocketServer();
        }

        #endregion

        #region -- Create and Handle WebSocket Server --

        private void CreateWebSocketServer()
        {
            try
            {
                WebSocketServer wssv = new WebSocketServer(
                    "ws://"
                    + Configuration.serverIP
                    + ":"
                    + Configuration.port);

                wssv.AddWebSocketService<Echo>("/echo");
                wssv.Start();

                Log($"[Program]: Server has started, Waiting for a connection: "
                        + Configuration.serverIP
                        + ":"
                        + Configuration.port
                        + "/echo");

                Console.ReadKey();
                wssv.Stop();

                Log($"[Program]: Server has stopped.");
            }
            catch (Exception ex)
            {
                Log("[Program]: Error in function CreateWebSocketServer - Message: " + ex.Message);
            }
        }

        public class Echo : WebSocketBehavior
        {
            Program run = new Program();

            #region -- Events listener --

            protected override void OnMessage(MessageEventArgs e)
            {
                if (!string.IsNullOrEmpty(e.Data))
                {
                    run.Log($"[Client]: Received message from Client - Message: {e.Data}");
                    ProcessingMsg(e.Data);
                }
            }

            protected override void OnOpen()
            {
                run.Log($"[Program]: A client connected.");
            }

            protected override void OnClose(CloseEventArgs e)
            {
                run.Log($"[Program]: A client disconnected - Message: ({e.Code}) {e.Reason}");
            }

            protected override void OnError(ErrorEventArgs e)
            {
                run.Log($"[Program]: Websocket Error!!! - Message: {e.Message}");
            }

            #endregion

            #region -- Processing messages from Client --

            private void ProcessingMsg(string msg)
            {
                try
                {
                    JObject json = JObject.Parse(msg);

                    var checkType = json["type"];
                    var checkData = json["data"];

                    if (checkType != null && checkData != null)
                    {
                        var type = json["type"].Value<string>();

                        switch (type)
                        {
                            case "sign":
                                var checkFileImageID = json["data"]["FileImageID"];
                                if (checkFileImageID != null)
                                {
                                    Configuration.fileImageID = json["data"]["FileImageID"].Value<string>();
                                    if (!string.IsNullOrEmpty(Configuration.fileImageID)) { 
                                        GetFileImage(Configuration.fileImageID);
                                    }
                                }

                                var checkSignID = json["data"]["SignID"];
                                if (checkSignID != null)
                                {
                                    Configuration.signID = json["data"]["SignID"].Value<string>();
                                }

                                var checkIsCompleted = json["data"]["IsCompleted"];
                                if (checkIsCompleted != null)
                                {
                                    Configuration.isCompleted = Int32.Parse(json["data"]["IsCompleted"].Value<string>());
                                }

                                var checkPageSign = json["data"]["PageSign"];
                                if (checkPageSign != null)
                                {
                                    Configuration.pageSign = Int32.Parse(json["data"]["PageSign"].Value<string>());
                                }

                                var checkPosX = json["data"]["PosX"];
                                if (checkPosX != null)
                                {
                                    Configuration.posX = float.Parse(json["data"]["PosX"].Value<string>());
                                }

                                var checkPosY = json["data"]["PosY"];
                                if (checkPosY != null)
                                {
                                    Configuration.posY = float.Parse(json["data"]["PosY"].Value<string>());
                                }

                                var checkWidthPlace = json["data"]["WidthPlace"];
                                if (checkWidthPlace != null)
                                {
                                    Configuration.widthPlace = float.Parse(json["data"]["WidthPlace"].Value<string>());
                                }

                                var checkHeightPlace = json["data"]["HeightPlace"];
                                if (checkHeightPlace != null)
                                {
                                    Configuration.heightPlace = float.Parse(json["data"]["HeightPlace"].Value<string>());
                                }

                                var checkReason = json["data"]["Reason"];
                                if (checkReason != null)
                                {
                                    Configuration.reason = json["data"]["Reason"].Value<string>();
                                }

                                var checkContact = json["data"]["Contact"];
                                if (checkContact != null)
                                {
                                    Configuration.contact = json["data"]["Contact"].Value<string>();
                                }

                                var checkLocation = json["data"]["Location"];
                                if (checkLocation != null)
                                {
                                    Configuration.location = json["data"]["Location"].Value<string>();
                                }

                                var checkFileSignID = json["data"]["FileSignID"];
                                if (checkFileSignID != null)
                                {
                                    Configuration.fileSignID = json["data"]["FileSignID"].Value<string>();
                                    GetFilePdf(Configuration.fileSignID);
                                }

                                break;
                        }
                    }
                    else
                    {
                        var rs = new
                        {
                            code = 500,
                            mess = "Đã có lỗi xảy ra. Vui lòng thử lại sau hoặc liên hệ quản trị viên."
                        };

                        Send(JsonConvert.SerializeObject(rs));
                    }
                }
                catch (Exception ex)
                {
                    run.Log("[Program]: Error in function ProcessingMsg - Message: " + ex.Message);
                }
            }

            #endregion

            #region -- Get file to Sign --

            private void GetFilePdf(string fileSignID)
            {
                try
                {
                    string fileBase64 = "";
                    ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls
                        | SecurityProtocolType.Tls11
                        | SecurityProtocolType.Tls12
                        | SecurityProtocolType.Tls13;

                    ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };

                    var options = new RestClientOptions(Configuration.apiUrl)
                    {
                        MaxTimeout = -1,
                    };
                    var client = new RestClient(options);
                    var request = new RestRequest("file/" + fileSignID, Method.Get);

                    RestResponse response = client.Execute(request);

                    if (response.IsSuccessful == true)
                    {
                        var responseConvert = (JObject)JsonConvert.DeserializeObject(response.Content);

                        var checkCode = responseConvert["code"];
                        if (checkCode != null)
                        {
                            var code = responseConvert["code"].Value<string>();
                            if (code == "200")
                            {
                                fileBase64 = responseConvert["data"]["FileBase64"].Value<string>();

                                DecodeBase64(Configuration.pdfFilePath, fileBase64, fileSignID, ".pdf");

                                SelectCertAndSign();
                            }
                            else
                            {
                                var error = responseConvert["messVN"];

                                Send(JsonConvert.SerializeObject(responseConvert));

                                run.Log("[Program]: Error in function GetFilePdf - ["
                                    + "Code: "
                                    + checkCode
                                    + ", Message: "
                                    + error
                                    + "]");
                            }
                        }
                    }
                    else
                    {
                        var rs = new
                        {
                            code = response.StatusCode,
                            mess = response.ErrorMessage
                        };

                        Send(JsonConvert.SerializeObject(rs));

                        run.Log("[Program]: Error in function GetFilePdf - Message: " + response.ErrorMessage);
                    }
                }
                catch (Exception ex)
                {
                    run.Log("[Program]: Error in function GetFilePdf - Message: " + ex.Message);
                }
            }

            private void GetFileImage(string fileImageID)
            {
                try
                {
                    string fileBase64 = "";
                    ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls
                        | SecurityProtocolType.Tls11
                        | SecurityProtocolType.Tls12
                        | SecurityProtocolType.Tls13;

                    ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };

                    var options = new RestClientOptions(Configuration.apiUrl)
                    {
                        MaxTimeout = -1,
                    };
                    var client = new RestClient(options);
                    var request = new RestRequest("file/" + fileImageID, Method.Get);

                    RestResponse response = client.Execute(request);

                    if (response.IsSuccessful == true)
                    {
                        var responseConvert = (JObject)JsonConvert.DeserializeObject(response.Content);

                        var checkCode = responseConvert["code"];
                        if (checkCode != null)
                        {
                            var code = responseConvert["code"].Value<string>();
                            if (code == "200")
                            {
                                fileBase64 = responseConvert["data"]["FileBase64"].Value<string>();

                                DecodeBase64(Configuration.imageFilePath, fileBase64, fileImageID, ".png");
                            }
                            else
                            {
                                var error = responseConvert["messVN"];

                                Send(JsonConvert.SerializeObject(responseConvert));

                                run.Log("[Program]: Error in function GetFileImage - ["
                                     + "Code: "
                                     + checkCode
                                     + ", Message: "
                                     + error
                                     + "]");
                            }
                        }
                    }
                    else
                    {
                        var rs = new
                        {
                            code = response.StatusCode,
                            mess = response.ErrorMessage
                        };

                        Send(JsonConvert.SerializeObject(rs));

                        run.Log("[Program]: Error in function GetFileImage - Message: " + response.ErrorMessage);
                    }
                }
                catch (Exception ex)
                {
                    run.Log("[Program]: Error in function GetFileImage - Message: " + ex.Message);
                }
            }

            #endregion

            #region -- Select Certificate and Sign --

            private void SelectCertAndSign()
            {
                try
                {
                    X509Store store = new X509Store(StoreLocation.CurrentUser);
                    store.Open(OpenFlags.ReadOnly);

                    X509Certificate2Collection sel = X509Certificate2UI
                        .SelectFromCollection(
                        store.Certificates
                        , null
                        , null
                        , X509SelectionFlag.SingleSelection
                        );

                    if (sel.Count > 0)
                    {
                        X509Certificate2 cert = sel[0];

                        Org.BouncyCastle.X509.X509CertificateParser cp = new Org.BouncyCastle.X509
                            .X509CertificateParser();

                        Org.BouncyCastle.X509.X509Certificate[] chain = new Org.BouncyCastle.X509
                            .X509Certificate[] {
                    cp.ReadCertificate(cert.RawData)
                        };

                        IExternalSignature externalSignature = new X509Certificate2Signature(cert, "SHA1");

                        PdfReader pdfReader = new PdfReader(Configuration.pdfFilePath
                            + Configuration.fileSignID
                            + ".pdf");

                        var signedPdf = new FileStream(Configuration.pdfFilePath
                            + Configuration.fileSignID
                            + "_signed.pdf",
                            FileMode.Create);

                        var pdfStamper = PdfStamper
                            .CreateSignature(pdfReader, signedPdf, '\0', null, true);

                        PdfSignatureAppearance signatureAppearance = pdfStamper
                            .SignatureAppearance;

                        signatureAppearance.CertificationLevel = 0;
                        signatureAppearance.Reason = Configuration.reason;
                        signatureAppearance.Contact = Configuration.contact;
                        signatureAppearance.Location = Configuration.location;
                        signatureAppearance.SignatureCreator = "IVG VN Application";

                        // Set the signature appearance location (in points)
                        signatureAppearance.Acro6Layers = true;
                        signatureAppearance.Layer2Font = FontFactory
                            .GetFont(BaseFont.TIMES_ROMAN, BaseFont.CP1257, 8.6f);

                        Configuration.dn = cert.Issuer;
                        Configuration.certName = cert.GetNameInfo(X509NameType.SimpleName, true);

                        signatureAppearance.Layer2Text = $"Digitally signed by {Configuration.certName}" +
                            $"\nDN: {Configuration.dn}" +
                            $"\nReason: {signatureAppearance.Reason}" +
                            $"\nLocation: {signatureAppearance.Location}" +
                            $"\nDate: {signatureAppearance.SignDate.ToString("dd-MM-yyyy (HH:mm:ss)")}";

                        if (!string.IsNullOrEmpty(Configuration.fileImageID))
                        {
                            signatureAppearance.SignatureGraphic = iTextSharp.text.Image
                                .GetInstance(Configuration.imageFilePath + Configuration.fileImageID + ".png");

                            signatureAppearance.SignatureRenderingMode = PdfSignatureAppearance
                                .RenderingMode.GRAPHIC_AND_DESCRIPTION;

                            //// Load signature image
                            //iTextSharp.text.Image sigImg = iTextSharp.text.Image
                            //    .GetInstance(Configuration.imageFilePath + Configuration.fileImageID + ".png");

                            //// Scale image to fit
                            //sigImg.ScaleToFit(Configuration.widthImage, Configuration.heightImage);

                            //// Set signature position on page
                            //sigImg.SetAbsolutePosition(Configuration.posX + (Configuration.widthPlace - Configuration.widthImage)
                            //    , Configuration.posY);

                            //// Add signatures to desired page
                            //PdfContentByte over = pdfStamper.GetOverContent(Configuration.pageSign);
                            //over.AddImage(sigImg);
                        }

                        signatureAppearance.SetVisibleSignature(
                            new iTextSharp.text.Rectangle(Configuration.posX
                            , Configuration.posY
                            , Configuration.posX + Configuration.widthPlace
                            , Configuration.posY + Configuration.heightPlace)
                            , Configuration.pageSign
                            , null);

                        MakeSignature.SignDetached(signatureAppearance
                            , externalSignature
                            , chain
                            , null
                            , null
                            , null
                            , 0
                            , CryptoStandard.CMS);

                        run.Log("[Program]: PDF signed successfully!");

                        UpdateFile();
                    }
                    else
                    {
                        run.Log("[Program]: User cancel select Certificate");
                    }
                }
                catch (Exception ex)
                {
                    run.Log("[Program]: Error in function SelectCertAndSign - Message: " + ex.Message);
                }
            }

            #endregion

            #region -- Update file after signed to database --

            private void UpdateFile()
            {
                try
                {
                    var options = new RestClientOptions(Configuration.apiUrl)
                    {
                        MaxTimeout = -1,
                    };
                    var client = new RestClient(options);
                    var request = new RestRequest("sign-contract", Method.Post);
                    request.AddHeader("Content-Type", "application/json");

                    var obj = new
                    {
                        ID = Configuration.signID,
                        FileSignID = Configuration.fileSignID,
                        FileImageID = Configuration.fileImageID,
                        Status = Configuration.isCompleted,
                        PosX = Configuration.posX,
                        PosY = Configuration.posY,
                        WidthPlace = Configuration.widthPlace,
                        HeightPlace = Configuration.heightPlace,
                        PageSign = Configuration.pageSign,
                        Reason = Configuration.reason,
                        Contact = Configuration.contact,
                        Location = Configuration.location,
                        FileBase64_Signed = EncodeBase64()
                    };

                    request.AddJsonBody(obj);

                    RestResponse response = client.Execute(request);

                    if (response.IsSuccessful == true)
                    {
                        var responseConvert = (JObject)JsonConvert.DeserializeObject(response.Content);

                        var checkCode = responseConvert["code"];
                        if (checkCode != null)
                        {
                            var code = responseConvert["code"].Value<string>();
                            var message = responseConvert["messVN"].Value<string>();

                            if (code == "200")
                            {
                                run.Log("[Program]: UpdateFile Success - ["
                                    + "Code: "
                                    + checkCode
                                    + ", Message: "
                                    + message
                                    + "]");
                            }
                            else
                            {
                                run.Log("[Program]: Error in function UpdateFile - ["
                                     + "Code: "
                                     + checkCode
                                     + ", Message: "
                                     + message
                                     + "]");
                            }

                            Send(JsonConvert.SerializeObject(responseConvert));
                        }
                    }
                    else
                    {
                        var rs = new
                        {
                            code = response.StatusCode,
                            mess = response.ErrorMessage
                        };

                        Send(JsonConvert.SerializeObject(rs));

                        run.Log("[Program]: Error in function UpdateFile - Message: " + response.ErrorMessage);
                    }
                }
                catch (Exception ex)
                {
                    run.Log("[Program]: Error in function UpdateFile - Message: " + ex.Message);
                }
            }

            #endregion

            #region -- Encode physical file to file base64 --

            private string EncodeBase64()
            {
                try
                {
                    Byte[] bytes = File.ReadAllBytes(Configuration.pdfFilePath + Configuration.fileSignID + "_signed.pdf");
                    string fileBase64 = Convert.ToBase64String(bytes);

                    return fileBase64;
                }
                catch (Exception ex)
                {
                    run.Log("[Program]: Error in function EncodeBase64 - Message: " + ex.Message);

                    return string.Empty;
                }
            }

            #endregion

            #region -- Decode file base64 to physical file --

            private void DecodeBase64(string filePath, string fileBase64, string fileID, string fileType)
            {
                try
                {
                    byte[] bytes = Convert.FromBase64String(fileBase64);
                    File.WriteAllBytes(filePath + fileID + fileType, bytes);
                }
                catch (Exception ex)
                {
                    run.Log("[Program]: Error in function DecodeBase64 - Message: " + ex.Message);
                }
            }

            #endregion
        }

        #endregion

        #region -- Create Folder --

        public void CreateFolderFile()
        {
            string path1 = AppDomain.CurrentDomain.BaseDirectory + "\\Files";
            if (!Directory.Exists(path1))
            {
                Directory.CreateDirectory(path1);
            }

            string path2 = AppDomain.CurrentDomain.BaseDirectory + "\\Files\\pdf";
            if (!Directory.Exists(path2))
            {
                Directory.CreateDirectory(path2);
            }

            string path3 = AppDomain.CurrentDomain.BaseDirectory + "\\Files\\icons";
            if (!Directory.Exists(path3))
            {
                Directory.CreateDirectory(path3);
            }
        }

        #endregion

        #region -- Run on startup --

        public void RunOnStartup() {
            var path = AppDomain.CurrentDomain.BaseDirectory + "ZPDigitalSignatureApp.exe";
            RegistryKey rkApp = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);
            rkApp.SetValue("ZPDigitalSignatureApp.exe", path);
        }

        #endregion

        #region -- Write Log --

        public void Log(string msg)
        {
            Console.WriteLine(DateTime.Now + " - " + msg);

            string path = AppDomain.CurrentDomain.BaseDirectory + "\\Logs";
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            string filepath = AppDomain.CurrentDomain.BaseDirectory + "\\Logs\\DigitalSignature_" + DateTime.Now.Date.ToShortDateString().Replace('/', '_') + ".txt";
            if (!File.Exists(filepath))
            {
                // Create a file to write to.   
                using (StreamWriter sw = File.CreateText(filepath))
                {
                    sw.WriteLine(DateTime.Now + " - " + msg);
                }
            }
            else
            {
                using (StreamWriter sw = File.AppendText(filepath))
                {
                    sw.WriteLine(DateTime.Now + " - " + msg);
                }
            }
        }

        #endregion
    }
}