﻿using Newtonsoft.Json;
using PAYNLSDK.Exceptions;
using PAYNLSDK.Utilities;

namespace PAYNLSDK.API.Validate.BankAccountNumber
{
    public class Request : RequestBase
    {
        [JsonProperty("bankAccountNumber")]
        public string BankAccountNumber { get; set; }

        public override bool RequiresApiToken
        {
            get
            {
                return false;
            }
        }

        public override int Version
        {
            get { return 1; }
        }

        public override string Controller
        {
            get { return "Validate"; }
        }

        public override string Method
        {
            get { return "BankAccountNumber"; }
        }

        public override string Querystring
        {
            get { return ""; }
        }

        public override System.Collections.Specialized.NameValueCollection GetParameters()
        {
            var nvc = base.GetParameters();

            ParameterValidator.IsNotEmpty(BankAccountNumber, "bankAccountNumber");
            nvc.Add("bankAccountNumber", BankAccountNumber);

            return nvc;
        }

        public Response Response { get { return (Response)response; } }

        public override void SetResponse()
        {
            if (ParameterValidator.IsEmpty(rawResponse))
            {
                throw new ErrorException("rawResponse is empty!");
            }
            response = JsonConvert.DeserializeObject<Response>(RawResponse);
        }
    }
}
