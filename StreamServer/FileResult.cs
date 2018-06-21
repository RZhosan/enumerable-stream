using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;

namespace StreamServer
{
    public class FileResult : IHttpActionResult
    {
        private readonly string fileName;
        private readonly Stream stream;

        public FileResult(string fileName, Stream stream)
        {
            this.fileName = fileName ?? throw new ArgumentNullException(nameof(fileName));
            this.stream = stream;
        }

        public Task<HttpResponseMessage> ExecuteAsync(CancellationToken cancellationToken)
        {
            var response = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StreamContent(stream)
            };

            string contentType = MimeMapping.GetMimeMapping(Path.GetExtension(fileName));
            response.Content.Headers.ContentType = new MediaTypeHeaderValue(contentType);
            response.Content.Headers.Add("Content-Disposition", $"attachment;filename=\"{fileName}\"");

            return Task.FromResult(response);
        }
    }
}