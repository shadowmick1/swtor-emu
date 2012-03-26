/*
* Copyright (C) 2008-2012 Emulator Nexus <http://emulatornexus.com//>
*
* This program is free software; you can redistribute it and/or modify it
* under the terms of the GNU General Public License as published by the
* Free Software Foundation; either version 3 of the License, or (at your
* option) any later version.
*
* This program is distributed in the hope that it will be useful, but WITHOUT
* ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or
* FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License for
* more details.
*
* You should have received a copy of the GNU General Public License along
* with this program. If not, see <http://www.gnu.org/licenses/>.
*/

using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Security;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;
using System.Xml.Serialization;
using Commons.Objects.Shard;
using RuntimeHelpers = System.Runtime.CompilerServices.RuntimeHelpers;
using SecureString = System.Security.SecureString;

namespace ShardListServer
{
    [XmlRoot("Shards")]
    public class ShardList
    {
        [XmlElement("Shard")]
        public List<ShardListEntry> Shards;
        [XmlAttribute("ProxyIP")]
        public string ProxyIP;
        [XmlAttribute("ProxyPort")]
        public int ProxyPort;

        [XmlIgnore]
        public int ShardCount
        {
            get
            {
                return Shards.Count;
            }
        }

        public static ShardList Load()
        {
            XmlSerializer ser = new XmlSerializer(typeof(ShardList));
            ShardList sl = ser.Deserialize(new StreamReader("Shards.xml")) as ShardList;
            return sl;
        }
    }

    class Program
    {
        static TcpListener listener;

        static HttpListener httpListener;

        static ShardList shardList;

        static void Main(string[] args)
        {
            Console.Title = "Shard List Server";
            Console.WriteLine("Shard List Server");
            Console.WriteLine("");

            shardList = ShardList.Load();

            Console.WriteLine("Loaded " + shardList.ShardCount + " shards.");

            httpListener = new HttpListener();
            httpListener.Prefixes.Add("https://*:4444/");
            httpListener.Start();

            Console.WriteLine("Ready!");

            while (true)
            {
                HttpListenerContext ctx = httpListener.GetContext();
                HttpListenerRequest req = ctx.Request;
                HttpListenerResponse resp = ctx.Response;
                string response = "";
                Console.WriteLine("Request = " + req.Url.AbsolutePath);
                switch (req.Url.AbsolutePath)
                {
                    case "/gamepad/shardlist":
                        List<string> shardEntries = new List<string>();
                        foreach (ShardListEntry shardEntry in shardList.Shards)
                        {
                            string s = "{\"region\":" + (byte)shardEntry.Region + ",\"isup\":" + (shardEntry.Online ? "true" : "false");
                            s += ",\"timezone\":" + shardEntry.Timezone + ",\"weight\":100,\"host\":\"" + shardList.ProxyIP + ":" + shardList.ProxyPort + ":" + shardEntry.ServerID + "\"";
                            s += ",\"focusid\":0,\"name\":\"" + shardEntry.Name + "\",\"queuewait\":" + shardEntry.QueueWait + ",\"loadlevel\":" + (byte)shardEntry.LoadLevel + ",\"language\":\"" + shardEntry.Language + "\"}";
                            shardEntries.Add(s);
                        }
                        response = "{\"shards\":[";
                        bool first = true;
                        foreach (string shard in shardEntries)
                        {
                            if (!first) response += ",";
                            response += shard;
                            first = false;
                        }
                        response += "],\"environmentDisabled\":0, \"accountRegion\":1}";
                        break;
                    case "/gamepad/lastshard":
                        response = "{\"region\":2,\"isup\":true,\"timezone\":-600,\"weight\":100,\"host\":\"127.0.0.1:7979:castlehilltest\",\"focusid\":0,\"name\":\"Sylar Testing Galaxy\",\"queuewait\":0,\"loadlevel\":1,\"language\":\"en\"}";
                        break;
                }
                byte[] buffer = Encoding.UTF8.GetBytes(response);
                resp.ContentLength64 = buffer.Length;
                resp.OutputStream.Write(buffer, 0, buffer.Length);
                resp.OutputStream.Close();
            }
        }

        static void BeginAcceptClient()
        {
            listener.BeginAcceptTcpClient(new AsyncCallback(EndAcceptClient), null);
        }

        static void EndAcceptClient(IAsyncResult ar)
        {
            TcpClient client = listener.EndAcceptTcpClient(ar);

            new Thread(new ThreadStart(() =>
                {
                    ProcessTcpClient(client);
                })).Start();

            BeginAcceptClient();
        }

        static void ProcessTcpClient(TcpClient client)
        {
            try
            {
                SslStream stream = new SslStream(client.GetStream());
                X509Certificate cert = new X509Certificate2(Certificate.CreateSelfSignCertificatePfx(
                    "CN=localhost", //host name
                    DateTime.Parse("2000-01-01"), //not valid before
                    DateTime.Parse("2099-01-01"), //not valid after
                    "mypassword"), "mypassword"); //password to encrypt key file)
                stream.AuthenticateAsServer(cert);

                byte[] requestBuffer = new byte[8192];
                int read = stream.Read(requestBuffer, 0, 8192);
                Array.Resize<byte>(ref requestBuffer, read);

                string request = Encoding.UTF8.GetString(requestBuffer);
                string requestedPath = request.Split('?')[0].Replace("GET ", "");

                Console.WriteLine(client.GetHashCode() + " => " + requestedPath);

                string response = "";

                if (requestedPath == "/gamepad/shardlist")
                {
                }
                else if (requestedPath == "/gamepad/lastshard")
                {
                }

                string header = "HTTP/1.1 200 OK\r\nDate: Fri, 10 Feb 2012 09:19:00 GMT\r\nServer: Apache/2.2.17 (Win32) mod_ssl/2.2.17 OpenSSL/0.9.8o\r\nContent-Length: " + response.Length + "\r\nContent-Type: text/html\r\n\r\n";

                byte[] headerBytes = Encoding.UTF8.GetBytes(header);
                byte[] responseBytes = Encoding.UTF8.GetBytes(response);
                stream.Write(headerBytes);
                stream.Flush();
                stream.Write(responseBytes);
                stream.Flush();
            }
            catch (Exception e)
            {
                Console.WriteLine("Exception occured!\nMessage: " + e.Message + "\n" + e.StackTrace);
            }
        }
    }

    internal class Certificate
    {
        public static byte[] CreateSelfSignCertificatePfx(
            string x500,
            DateTime startTime,
            DateTime endTime)
        {
            byte[] pfxData = CreateSelfSignCertificatePfx(
                x500,
                startTime,
                endTime,
                (SecureString)null);
            return pfxData;
        }

        public static byte[] CreateSelfSignCertificatePfx(
            string x500,
            DateTime startTime,
            DateTime endTime,
            string insecurePassword)
        {
            byte[] pfxData;
            SecureString password = null;

            try
            {
                if (!string.IsNullOrEmpty(insecurePassword))
                {
                    password = new SecureString();
                    foreach (char ch in insecurePassword)
                    {
                        password.AppendChar(ch);
                    }

                    password.MakeReadOnly();
                }

                pfxData = CreateSelfSignCertificatePfx(
                    x500,
                    startTime,
                    endTime,
                    password);
            }
            finally
            {
                if (password != null)
                {
                    password.Dispose();
                }
            }

            return pfxData;
        }

        public static byte[] CreateSelfSignCertificatePfx(
            string x500,
            DateTime startTime,
            DateTime endTime,
            SecureString password)
        {
            byte[] pfxData;

            if (x500 == null)
            {
                x500 = "";
            }

            SystemTime startSystemTime = ToSystemTime(startTime);
            SystemTime endSystemTime = ToSystemTime(endTime);
            string containerName = Guid.NewGuid().ToString();

            GCHandle dataHandle = new GCHandle();
            IntPtr providerContext = IntPtr.Zero;
            IntPtr cryptKey = IntPtr.Zero;
            IntPtr certContext = IntPtr.Zero;
            IntPtr certStore = IntPtr.Zero;
            IntPtr storeCertContext = IntPtr.Zero;
            IntPtr passwordPtr = IntPtr.Zero;
            RuntimeHelpers.PrepareConstrainedRegions();
            try
            {
                Check(NativeMethods.CryptAcquireContextW(
                    out providerContext,
                    containerName,
                    null,
                    1, // PROV_RSA_FULL
                    8)); // CRYPT_NEWKEYSET

                Check(NativeMethods.CryptGenKey(
                    providerContext,
                    1, // AT_KEYEXCHANGE
                    1, // CRYPT_EXPORTABLE
                    out cryptKey));

                IntPtr errorStringPtr;
                int nameDataLength = 0;
                byte[] nameData;

                // errorStringPtr gets a pointer into the middle of the x500 string,
                // so x500 needs to be pinned until after we've copied the value
                // of errorStringPtr.
                dataHandle = GCHandle.Alloc(x500, GCHandleType.Pinned);

                if (!NativeMethods.CertStrToNameW(
                    0x00010001, // X509_ASN_ENCODING | PKCS_7_ASN_ENCODING
                    dataHandle.AddrOfPinnedObject(),
                    3, // CERT_X500_NAME_STR = 3
                    IntPtr.Zero,
                    null,
                    ref nameDataLength,
                    out errorStringPtr))
                {
                    string error = Marshal.PtrToStringUni(errorStringPtr);
                    throw new ArgumentException(error);
                }

                nameData = new byte[nameDataLength];

                if (!NativeMethods.CertStrToNameW(
                    0x00010001, // X509_ASN_ENCODING | PKCS_7_ASN_ENCODING
                    dataHandle.AddrOfPinnedObject(),
                    3, // CERT_X500_NAME_STR = 3
                    IntPtr.Zero,
                    nameData,
                    ref nameDataLength,
                    out errorStringPtr))
                {
                    string error = Marshal.PtrToStringUni(errorStringPtr);
                    throw new ArgumentException(error);
                }

                dataHandle.Free();

                dataHandle = GCHandle.Alloc(nameData, GCHandleType.Pinned);
                CryptoApiBlob nameBlob = new CryptoApiBlob(
                    nameData.Length,
                    dataHandle.AddrOfPinnedObject());

                CryptKeyProviderInformation kpi = new CryptKeyProviderInformation();
                kpi.ContainerName = containerName;
                kpi.ProviderType = 1; // PROV_RSA_FULL
                kpi.KeySpec = 1; // AT_KEYEXCHANGE

                certContext = NativeMethods.CertCreateSelfSignCertificate(
                    providerContext,
                    ref nameBlob,
                    0,
                    ref kpi,
                    IntPtr.Zero, // default = SHA1RSA
                    ref startSystemTime,
                    ref endSystemTime,
                    IntPtr.Zero);
                Check(certContext != IntPtr.Zero);
                dataHandle.Free();

                certStore = NativeMethods.CertOpenStore(
                    "Memory", // sz_CERT_STORE_PROV_MEMORY
                    0,
                    IntPtr.Zero,
                    0x2000, // CERT_STORE_CREATE_NEW_FLAG
                    IntPtr.Zero);
                Check(certStore != IntPtr.Zero);

                Check(NativeMethods.CertAddCertificateContextToStore(
                    certStore,
                    certContext,
                    1, // CERT_STORE_ADD_NEW
                    out storeCertContext));

                NativeMethods.CertSetCertificateContextProperty(
                    storeCertContext,
                    2, // CERT_KEY_PROV_INFO_PROP_ID
                    0,
                    ref kpi);

                if (password != null)
                {
                    passwordPtr = Marshal.SecureStringToCoTaskMemUnicode(password);
                }

                CryptoApiBlob pfxBlob = new CryptoApiBlob();
                Check(NativeMethods.PFXExportCertStoreEx(
                    certStore,
                    ref pfxBlob,
                    passwordPtr,
                    IntPtr.Zero,
                    7)); // EXPORT_PRIVATE_KEYS | REPORT_NO_PRIVATE_KEY | REPORT_NOT_ABLE_TO_EXPORT_PRIVATE_KEY

                pfxData = new byte[pfxBlob.DataLength];
                dataHandle = GCHandle.Alloc(pfxData, GCHandleType.Pinned);
                pfxBlob.Data = dataHandle.AddrOfPinnedObject();
                Check(NativeMethods.PFXExportCertStoreEx(
                    certStore,
                    ref pfxBlob,
                    passwordPtr,
                    IntPtr.Zero,
                    7)); // EXPORT_PRIVATE_KEYS | REPORT_NO_PRIVATE_KEY | REPORT_NOT_ABLE_TO_EXPORT_PRIVATE_KEY
                dataHandle.Free();
            }
            finally
            {
                if (passwordPtr != IntPtr.Zero)
                {
                    Marshal.ZeroFreeCoTaskMemUnicode(passwordPtr);
                }

                if (dataHandle.IsAllocated)
                {
                    dataHandle.Free();
                }

                if (certContext != IntPtr.Zero)
                {
                    NativeMethods.CertFreeCertificateContext(certContext);
                }

                if (storeCertContext != IntPtr.Zero)
                {
                    NativeMethods.CertFreeCertificateContext(storeCertContext);
                }

                if (certStore != IntPtr.Zero)
                {
                    NativeMethods.CertCloseStore(certStore, 0);
                }

                if (cryptKey != IntPtr.Zero)
                {
                    NativeMethods.CryptDestroyKey(cryptKey);
                }

                if (providerContext != IntPtr.Zero)
                {
                    NativeMethods.CryptReleaseContext(providerContext, 0);
                    NativeMethods.CryptAcquireContextW(
                        out providerContext,
                        containerName,
                        null,
                        1, // PROV_RSA_FULL
                        0x10); // CRYPT_DELETEKEYSET
                }
            }

            return pfxData;
        }

        private static SystemTime ToSystemTime(DateTime dateTime)
        {
            long fileTime = dateTime.ToFileTime();
            SystemTime systemTime;
            Check(NativeMethods.FileTimeToSystemTime(ref fileTime, out systemTime));
            return systemTime;
        }

        private static void Check(bool nativeCallSucceeded)
        {
            if (!nativeCallSucceeded)
            {
                int error = Marshal.GetHRForLastWin32Error();
                Marshal.ThrowExceptionForHR(error);
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct SystemTime
        {
            public short Year;
            public short Month;
            public short DayOfWeek;
            public short Day;
            public short Hour;
            public short Minute;
            public short Second;
            public short Milliseconds;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct CryptoApiBlob
        {
            public int DataLength;
            public IntPtr Data;

            public CryptoApiBlob(int dataLength, IntPtr data)
            {
                this.DataLength = dataLength;
                this.Data = data;
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct CryptKeyProviderInformation
        {
            [MarshalAs(UnmanagedType.LPWStr)]
            public string ContainerName;
            [MarshalAs(UnmanagedType.LPWStr)]
            public string ProviderName;
            public int ProviderType;
            public int Flags;
            public int ProviderParameterCount;
            public IntPtr ProviderParameters; // PCRYPT_KEY_PROV_PARAM
            public int KeySpec;
        }

        private static class NativeMethods
        {
            [DllImport("kernel32.dll", SetLastError = true, ExactSpelling = true)]
            [return: MarshalAs(UnmanagedType.Bool)]
            public static extern bool FileTimeToSystemTime(
                [In] ref long fileTime,
                out SystemTime systemTime);

            [DllImport("AdvApi32.dll", SetLastError = true, ExactSpelling = true)]
            [return: MarshalAs(UnmanagedType.Bool)]
            public static extern bool CryptAcquireContextW(
                out IntPtr providerContext,
                [MarshalAs(UnmanagedType.LPWStr)] string container,
                [MarshalAs(UnmanagedType.LPWStr)] string provider,
                int providerType,
                int flags);

            [DllImport("AdvApi32.dll", SetLastError = true, ExactSpelling = true)]
            [return: MarshalAs(UnmanagedType.Bool)]
            public static extern bool CryptReleaseContext(
                IntPtr providerContext,
                int flags);

            [DllImport("AdvApi32.dll", SetLastError = true, ExactSpelling = true)]
            [return: MarshalAs(UnmanagedType.Bool)]
            public static extern bool CryptGenKey(
                IntPtr providerContext,
                int algorithmId,
                int flags,
                out IntPtr cryptKeyHandle);

            [DllImport("AdvApi32.dll", SetLastError = true, ExactSpelling = true)]
            [return: MarshalAs(UnmanagedType.Bool)]
            public static extern bool CryptDestroyKey(
                IntPtr cryptKeyHandle);

            [DllImport("Crypt32.dll", SetLastError = true, ExactSpelling = true)]
            [return: MarshalAs(UnmanagedType.Bool)]
            public static extern bool CertStrToNameW(
                int certificateEncodingType,
                IntPtr x500,
                int strType,
                IntPtr reserved,
                [MarshalAs(UnmanagedType.LPArray)] [Out] byte[] encoded,
                ref int encodedLength,
                out IntPtr errorString);

            [DllImport("Crypt32.dll", SetLastError = true, ExactSpelling = true)]
            public static extern IntPtr CertCreateSelfSignCertificate(
                IntPtr providerHandle,
                [In] ref CryptoApiBlob subjectIssuerBlob,
                int flags,
                [In] ref CryptKeyProviderInformation keyProviderInformation,
                IntPtr signatureAlgorithm,
                [In] ref SystemTime startTime,
                [In] ref SystemTime endTime,
                IntPtr extensions);

            [DllImport("Crypt32.dll", SetLastError = true, ExactSpelling = true)]
            [return: MarshalAs(UnmanagedType.Bool)]
            public static extern bool CertFreeCertificateContext(
                IntPtr certificateContext);

            [DllImport("Crypt32.dll", SetLastError = true, ExactSpelling = true)]
            public static extern IntPtr CertOpenStore(
                [MarshalAs(UnmanagedType.LPStr)] string storeProvider,
                int messageAndCertificateEncodingType,
                IntPtr cryptProvHandle,
                int flags,
                IntPtr parameters);

            [DllImport("Crypt32.dll", SetLastError = true, ExactSpelling = true)]
            [return: MarshalAs(UnmanagedType.Bool)]
            public static extern bool CertCloseStore(
                IntPtr certificateStoreHandle,
                int flags);

            [DllImport("Crypt32.dll", SetLastError = true, ExactSpelling = true)]
            [return: MarshalAs(UnmanagedType.Bool)]
            public static extern bool CertAddCertificateContextToStore(
                IntPtr certificateStoreHandle,
                IntPtr certificateContext,
                int addDisposition,
                out IntPtr storeContextPtr);

            [DllImport("Crypt32.dll", SetLastError = true, ExactSpelling = true)]
            [return: MarshalAs(UnmanagedType.Bool)]
            public static extern bool CertSetCertificateContextProperty(
                IntPtr certificateContext,
                int propertyId,
                int flags,
                [In] ref CryptKeyProviderInformation data);

            [DllImport("Crypt32.dll", SetLastError = true, ExactSpelling = true)]
            [return: MarshalAs(UnmanagedType.Bool)]
            public static extern bool PFXExportCertStoreEx(
                IntPtr certificateStoreHandle,
                ref CryptoApiBlob pfxBlob,
                IntPtr password,
                IntPtr reserved,
                int flags);
        }
    }
}