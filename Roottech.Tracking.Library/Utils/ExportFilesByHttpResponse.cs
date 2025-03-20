using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Roottech.Tracking.RTM.UI.Web.Infrastructure
{
    public class ExportFilesByHttpResponse
    {
        public static void ExportToExcel<T>(IQueryable<T> queryable, Func<T, object> fields, HttpResponseBase response, string fileName)
        {
            ExportToExcel(queryable, fields, response, fileName, null);
        }

        public static void ExportToExcel<T>(IQueryable<T> queryable, Func<T, object> fields, HttpResponseBase response, string fileName, string[,] colNamesToReplace)
        {
            ProcessExport(queryable.Select(fields), response, fileName, colNamesToReplace);
        }

        public static void ExportToExcel(IList<object> list, string tt, HttpResponseBase response, string fileName, string[,] colNamesToReplace)
        {
            ProcessExport(list, response, fileName, colNamesToReplace);
        }

        private static void ProcessExport(object list, HttpResponseBase response, string fileName, string[,] colNamesToReplace)
        {
            var gridView = new GridView {DataSource = list};
            gridView.DataBind();
            var request =
                WebRequest.Create(string.Format("http://{0}/ExcelFiles/",
                                                HttpContext.Current.Request.ServerVariables["HTTP_HOST"]));
            request.Method = "POST";

            HttpContext context = HttpContext.Current;
            //context.Response.Write(Environment.NewLine);
            //context.Response.Write(Environment.NewLine);
            //context.Response.Write(Environment.NewLine);

            context.Response.ClearContent();
            context.Response.AddHeader("location", "/ExcelFiles/");
            context.Response.AddHeader("content-location", "/ExcelFiles/");
            context.Response.AddHeader("content-disposition", string.Format("attachment; filename={0}.xls", fileName.Replace(" ", "")));
            context.Response.ContentType = "application/octet-stream"; //"application/excel";
            var sw = new StringWriter();
            gridView.RenderControl(new HtmlTextWriter(sw));
            var stringToWrite = sw.ToString();
            if (colNamesToReplace != null)
                for (int i = 0; i < colNamesToReplace.Length/2; i++)
                    stringToWrite = stringToWrite.Replace(
                        string.Format("<th scope=\"col\">{0}</th>", colNamesToReplace[i, 0]),
                        string.Format("<th scope=\"col\">{0}</th>", colNamesToReplace[i, 1]));

            context.Response.Write(stringToWrite);
            context.Response.End();
        }

        public static void ExportExcelToServer<T>(IQueryable<T> queryable, Func<T, object> fields, string fileName, string[,] colNamesToReplace, string address)
        {
            var gridView = new GridView { DataSource = queryable.Select(fields) };
            gridView.DataBind();
            var sw = new StringWriter();
            gridView.RenderControl(new HtmlTextWriter(sw));
            var stringToWrite = sw.ToString();
            if (colNamesToReplace != null)
                for (int i = 0; i < colNamesToReplace.Length / 2; i++)
                    stringToWrite = stringToWrite.Replace(
                        string.Format("<th scope=\"col\">{0}</th>", colNamesToReplace[i, 0]),
                        string.Format("<th scope=\"col\">{0}</th>", colNamesToReplace[i, 1]));
            
            using (var stream1 = new StreamReader(new MemoryStream(Encoding.ASCII.GetBytes(stringToWrite))).BaseStream)
            {
                var files = new[] { new UploadFile {
                                    Name = "file",
                                    Filename = string.Format("{0}-{1}.xls", HttpContext.Current.Session["FolderNameForImages"], fileName.Replace(" ", "")),
                                    ContentType = "application/excel",
                                    Stream = stream1}};
                var values = new NameValueCollection { { "urlToSaveFile", address } };
                byte[] result = UploadFiles(string.Format("http://{0}/upload.aspx", HttpContext.Current.Request.ServerVariables["HTTP_HOST"]), files, values); //"http://localhost:1234/upload"
            }
        }

        public static void HttpUploadFile(string url, Stream fileStream, string file, string paramName, string contentType, NameValueCollection nvc)
        {
            string boundary = "---------------------------" + DateTime.Now.Ticks.ToString("x");
            byte[] boundarybytes = Encoding.ASCII.GetBytes("\r\n--" + boundary + "\r\n");

            var wr = (HttpWebRequest)WebRequest.Create(url);
            wr.ContentType = "multipart/form-data; boundary=" + boundary;
            wr.Method = "POST";
            wr.KeepAlive = true;
            wr.Credentials = CredentialCache.DefaultCredentials;

            Stream rs = wr.GetRequestStream();

            string formdataTemplate = "Content-Disposition: form-data; name=\"{0}\"\r\n\r\n{1}";
            foreach (string key in nvc.Keys)
            {
                rs.Write(boundarybytes, 0, boundarybytes.Length);
                string formitem = string.Format(formdataTemplate, key, nvc[key]);
                byte[] formitembytes = System.Text.Encoding.UTF8.GetBytes(formitem);
                rs.Write(formitembytes, 0, formitembytes.Length);
            }
            rs.Write(boundarybytes, 0, boundarybytes.Length);

            string headerTemplate = "Content-Disposition: form-data; name=\"{0}\"; filename=\"{1}\"\r\nContent-Type: {2}\r\n\r\n";
            string header = string.Format(headerTemplate, paramName, file, contentType);
            byte[] headerbytes = Encoding.UTF8.GetBytes(header);
            rs.Write(headerbytes, 0, headerbytes.Length);

            //FileStream fileStream = new FileStream(file, FileMode.Open, FileAccess.Read);
            byte[] buffer = new byte[4096];
            int bytesRead = 0;
            while ((bytesRead = fileStream.Read(buffer, 0, buffer.Length)) != 0)
            {
                rs.Write(buffer, 0, bytesRead);
            }
            fileStream.Close();

            byte[] trailer = Encoding.ASCII.GetBytes("\r\n--" + boundary + "--\r\n");
            rs.Write(trailer, 0, trailer.Length);
            rs.Close();

            WebResponse wresp = null;
            try
            {
                wresp = wr.GetResponse();
                var stream2 = wresp.GetResponseStream();
                var reader2 = new StreamReader(stream2);
            }
            catch (Exception)
            {
                if (wresp != null)
                {
                    wresp.Close();
                    wresp = null;
                }
            }
            finally
            {
                wr = null;
            }
        }

        public static byte[] UploadFiles(string address, IEnumerable<UploadFile> files, NameValueCollection values)
        {
            var request = (HttpWebRequest)WebRequest.Create(address);
            request.Method = "POST";
            var boundary = "---------------------------" + DateTime.Now.Ticks.ToString("x", NumberFormatInfo.InvariantInfo);
            request.ContentType = "multipart/form-data; boundary=" + boundary;
            request.KeepAlive = true;
            request.Credentials = CredentialCache.DefaultCredentials;
            boundary = "--" + boundary;

            using (var requestStream = request.GetRequestStream())
            {
                // Write the values
                foreach (string name in values.Keys)
                {
                    var buffer = Encoding.ASCII.GetBytes(boundary + Environment.NewLine);
                    requestStream.Write(buffer, 0, buffer.Length);
                    buffer = Encoding.ASCII.GetBytes(string.Format("Content-Disposition: form-data; name=\"{0}\"{1}{1}", name, Environment.NewLine));
                    requestStream.Write(buffer, 0, buffer.Length);
                    buffer = Encoding.UTF8.GetBytes(values[name] + Environment.NewLine);
                    requestStream.Write(buffer, 0, buffer.Length);
                }

                // Write the files
                foreach (var file in files)
                {
                    var buffer = Encoding.ASCII.GetBytes(boundary + Environment.NewLine);
                    requestStream.Write(buffer, 0, buffer.Length);
                    buffer = Encoding.UTF8.GetBytes(string.Format("Content-Disposition: form-data; name=\"{0}\"; filename=\"{1}\"{2}", file.Name, file.Filename, Environment.NewLine));
                    requestStream.Write(buffer, 0, buffer.Length);
                    buffer = Encoding.ASCII.GetBytes(string.Format("Content-Type: {0}{1}{1}", file.ContentType, Environment.NewLine));
                    requestStream.Write(buffer, 0, buffer.Length);
                    file.Stream.CopyTo(requestStream);
                    buffer = Encoding.ASCII.GetBytes(Environment.NewLine);
                    requestStream.Write(buffer, 0, buffer.Length);
                }

                var boundaryBuffer = Encoding.ASCII.GetBytes(boundary + "--");
                requestStream.Write(boundaryBuffer, 0, boundaryBuffer.Length);
            }

            using (var response = request.GetResponse())
            using (var responseStream = response.GetResponseStream())
            using (var stream = new MemoryStream())
            {
                responseStream.CopyTo(stream);
                return stream.ToArray();
            }
        }
    }

    public class UploadFile
    {
        public UploadFile()
        {
            ContentType = "application/octet-stream";
        }
        public string Name { get; set; }
        public string Filename { get; set; }
        public string ContentType { get; set; }
        public Stream Stream { get; set; }
    }
}