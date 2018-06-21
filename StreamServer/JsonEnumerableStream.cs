using System.Collections.Generic;
using Newtonsoft.Json;

namespace StreamServer
{
    public class JsonEnumerableStream<T> : EnumerableStream<T>
    {
        public JsonEnumerableStream(IEnumerable<T> enumerable) : base(enumerable)
        {
        }

        public JsonEnumerableStream(IEnumerable<T> enumerable, int itemsBufferSize) : base(enumerable, itemsBufferSize)
        {
        }

        protected override string SerializeItem(T item)
        {
            return JsonConvert.SerializeObject(item);
        }

        protected override string DelimiterToken => ",";

        protected override string FirstRow => "[";

        protected override string LastRow => "]";
    }
}