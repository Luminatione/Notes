using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    public class Response<T>
    {
        public T Value { get; set; }
        public bool IsSuccess { get; set; }
        public string Message { get; set; }
    }

    public class Response
    {
        public bool IsSuccess { get; set; }
        public string Message { get; set; }
    }
}
