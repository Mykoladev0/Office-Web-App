using System;
using System.Collections.Generic;
using System.Text;

namespace CoreDAL.Models
{
    public class DataCollisionException<T> : Exception
    {
        public DataCollisionException(string msg) : base(msg) { }
        public T OriginalData {get;set;}
        public T IncomingData { get; set; }
        
    }
}
