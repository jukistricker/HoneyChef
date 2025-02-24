
using Newtonsoft.Json;
using static IOITCore.Constants.ApiConstants;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace IOITCore.Models.Common
{
    public class DefaultResponse
    {
        public Meta meta { get; set; }
        public object data { get; set; }
        public object metadata { get; set; }
        public DefaultResponse()
        {
            meta = new Meta();
            data = null;
            metadata = null;
        }

        public DefaultResponse Success(object data)
        {
            this.data = data;
            this.metadata = null;
            this.meta.error_code = StatusCode.Success200;
            this.meta.error_message = MessageResource.ACCTION_SUCCESS;

            return this;
        }

        public DefaultResponse Success(object data, int code)
        {
            this.data = data;
            this.metadata = null;
            this.meta.error_code = code;
            this.meta.error_message = MessageResource.ACCTION_SUCCESS;

            return this;
        }

        public DefaultResponse Success(object data, string msg)
        {
            this.data = data;
            this.metadata = null;
            this.meta.error_code = StatusCode.Success200;
            this.meta.error_message = msg;

            return this;
        }

        public DefaultResponse Success(object data, string msg, int code)
        {
            this.data = data;
            this.metadata = null;
            this.meta.error_code = code;
            this.meta.error_message = msg;

            return this;
        }

        public DefaultResponse Success(object data, int metadata, string msg, int code)
        {
            this.data = data;
            this.metadata = metadata;
            this.meta.error_code = code;
            this.meta.error_message = msg;

            return this;
        }

        public DefaultResponse Success(object data, string msg, int code, int count)
        {
            this.data = data;
            this.metadata = count;
            this.meta.error_code = code;
            this.meta.error_message = msg;

            return this;
        }

        public DefaultResponse Error(string msg)
        {
            meta.error_code = -500;
            meta.error_message = msg;
            data = null;

            return this;
        }

        public DefaultResponse Error(string msg, int code)
        {
            this.data = null;
            this.metadata = null;
            this.meta.error_code = code;
            this.meta.error_message = msg;

            return this;
        }
        public string ToJson()
        {
            return JsonConvert.SerializeObject(this);
        }
    }

    public class Meta
    {
        public int error_code { get; set; }
        public string error_message { get; set; }

        public Meta()
        {
            error_code = 200;
            error_message = string.Empty;
        }

        public Meta(int errorCode, string errorMessage)
        {
            error_code = errorCode;
            error_message = errorMessage;
        }
    }

    public class Metadata
    {
        public int item_count { get; set; }
        public decimal total { get; set; }
        public Metadata()
        { }

        public Metadata(int item_count)
        {
            this.item_count = item_count;
        }
        public Metadata(decimal total)
        {
            this.total = total;
        }

        public Metadata(int item_count, decimal total)
        {
            this.item_count = item_count;
            this.total = total;
        }

        
    }

    public class MetadataTotal
    {
        public int Count { get; set; }

        public MetadataTotal()
        { }

        public MetadataTotal(int Count)
        {
            this.Count = Count;
        }

    }

    public class Output
    {
        public Meta meta { get; set; }

        public int? metadata { get; set; }
    }

    public class Output<T> : Output
    {
        public T data { get; set; }
    }


}