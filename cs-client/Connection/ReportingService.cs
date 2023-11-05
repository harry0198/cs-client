using cs_client.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace cs_client.Connection
{
    public class ReportingService
    {
        private readonly HttpClient _httpClient;

        public ReportingService(HttpClient httpClient)
        {
            this._httpClient = httpClient;
        }



    }
}
