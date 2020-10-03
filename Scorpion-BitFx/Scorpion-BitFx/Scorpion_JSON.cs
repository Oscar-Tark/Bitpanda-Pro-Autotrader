using System;
using RestSharp;
using RestSharp.Authenticators;
using System.Threading.Tasks;
using System.Threading;
using System.Json;
using System.Collections;
using Newtonsoft.Json.Linq;

namespace ScorpionBitFx
{
    public class ScorpionJSON
    {
        public ScorpionJSON()
        {
            return;
        }

        public string JSON_get(string URL)
        {
            //Console.WriteLine("JSON: " + URL);
            Task<string> ts = JSON_getAsync(URL);
            ts.Wait();
            //Console.WriteLine("Done JSON: " + URL);
            return ts.Result;
        }

        public string JSON_get_auth(string URL, string AUTH)
        {
            Console.WriteLine("auth JSON: " + URL);
            Task<string> ts = JSON_getAsync_auth(URL, AUTH);
            ts.Wait();
            Console.WriteLine("Done auth JSON: " + URL);
            return ts.Result;
        }

        public string JSON_post_auth(string URL, string AUTH, string[] names, string[] values, bool has_body, string body)
        {
            Console.WriteLine("auth JSON: " + URL);
            Task<string> ts = JSON_postAsync_auth(URL, AUTH, names, values, has_body, body);
            ts.Wait();
            Console.WriteLine("Done auth JSON: " + URL);
            return ts.Result;
        }

        private async Task<string> JSON_getAsync(string URL)
        {
            var client = new RestClient(URL);
            //client.Authenticator = new HttpBasicAuthenticator("username", "password");

            var request = new RestRequest("", DataFormat.None);
            var response = await client.GetAsync<string>(request, CancellationToken.None);

            return response;
        }

        private async Task<string> JSON_getAsync_auth(string URL, string AUTH)
        {
            var client = new RestClient(URL);
            client.AddDefaultHeader("Authorization", "Bearer " + AUTH);
            //client.Authenticator = new HttpBasicAuthenticator("username", "password");

            var request = new RestRequest("", DataFormat.None);
            var response = await client.GetAsync<string>(request, CancellationToken.None);

            return response;
        }

        private async Task<string> JSON_postAsync_auth(string URL, string AUTH, string[] name, string[] value, bool has_body, string body)
        {
            var client = new RestClient(URL);
            client.AddDefaultHeader("Content-Type", "application/json ");
            client.AddDefaultHeader("Accept", "application/json ");
            client.AddDefaultHeader("Authorization", "Bearer " + AUTH);
            var request = new RestRequest(Method.POST);

            for (int i = 0; i < name.Length; i++)
                request.AddParameter(name[i], value[i]);

            if(has_body && body != "")
                request.AddParameter("application/json", body, ParameterType.RequestBody);

            request.RequestFormat = DataFormat.Json;
            var response = await client.PostAsync<string>(request, CancellationToken.None);

            return response;
        }

        //Convert
        public JArray jsontoarray(ref string JSON)
        {
            return JArray.Parse(JSON);
        }

        public JObject jsontoobject(ref string JSON)
        {
            return JObject.Parse(JSON);
        }
    }
}
