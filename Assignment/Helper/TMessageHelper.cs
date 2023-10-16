using System.Runtime.Serialization;

namespace Assignment.Helper
{
    public class TMessageHelper<T>
    {
        [DataMember]
        public string Message { get; set; }
        public int statuscode { get; set; }
        public int Key { get; set; }
        public T Value { get; set; }
    }
}
