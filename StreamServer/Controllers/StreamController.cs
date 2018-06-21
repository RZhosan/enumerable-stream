using System;
using System.Collections.Generic;
using System.Web.Http;

namespace StreamServer.Controllers
{
    public class StreamController : ApiController
    {
        public IHttpActionResult Get(int size)
        {
            var data = GetData(size);


            var stream = new JsonEnumerableStream<object>(data);
            const string fileName = "Sample stream.json";

            return new FileResult(fileName, stream);
        }

        private static IEnumerable<object> GetData(int count)
        {
            for (int current = 0; current < count; current++)
            {
                yield return new
                {
                    current,
                    count,
                    timeStamp = DateTime.UtcNow.ToString("G")
                };
            }
        }
    }
}
