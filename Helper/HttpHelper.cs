using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Mime;
using System.Text;
using System.Threading.Tasks;

namespace Suflow.Common.Utils {

    /// <summary>
    /// http://www.codeproject.com/Tips/1065769/Support-Desk-Day-of-n
    /// http://stackoverflow.com/questions/10304863/how-to-use-system-net-httpclient-to-post-a-complex-type
    /// http://stackoverflow.com/questions/30120456/receive-a-postasync-in-frombody-as-param
    /// http://stackoverflow.com/questions/19395669/fiddler-testing-api-post-passing-a-frombody-class
    /// </summary>
    public class HttpHelper {

        /// <summary>
        /// http://geekflare.com/online-scan-website-security-vulnerabilities/
        /// https://www.scanmyserver.com/
        /// </summary>
        /// <param name="address"></param>
        public static void CheckForCommonVulnerabilities(string address) {
            //TODO
        }

        #region Get

        public static async Task<string> GetXml(string address, bool throwException) {
            return await Get(address, null, "application/xml", throwException);
        }

        public static async Task<string> GetJson(string address, bool throwException) {
            return await Get(address, null, "application/json", throwException);
        }

        /// <summary>
        /// Send GET request as async
        /// How to get object out of string? Use NewtecJson (It is out of scope of Suflow.Common.Utils)
        /// </summary>
        /// <param name="baseAddress">eg: http://localhost:57975/</param>
        /// <param name="requestUri">eg: api/Values/</param>
        /// <param name="mediaType">eg: application/json</param>
        /// <returns></returns>
        public static async Task<string> Get(string baseAddress, string requestUri, string mediaType, bool throwException) {
            try {
                using (var client = new HttpClient()) {
                    //Prepare
                    client.BaseAddress = new Uri(baseAddress);
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(mediaType));

                    //Actual call
                    var response = client.GetAsync(requestUri).Result;

                    //Response
                    var responseString = "<unknown>";
                    if (response.IsSuccessStatusCode) {
                        responseString = await response.Content.ReadAsStringAsync();
                    }
                    else {
                        responseString = "ERROR: " + response.StatusCode;
                        if (throwException)
                            throw new Exception(responseString);
                    }

                    return responseString;
                }
            }
            catch(Exception ex)
            {
                if (throwException)
                    throw;
                return ex.FullMessage();
            }
        }

        #endregion

        #region Post

        public static async Task<string> PostJson(string baseAddress, string requestUri, string content, bool throwException) {
            var contentToPost = new StringContent(content);
            return await Post(baseAddress, requestUri, contentToPost, "application/json", throwException);
        }

        public static async Task<string> PostJson(string baseAddress, string requestUri, IEnumerable<KeyValuePair<string, string>> content, bool throwException) {
            var contentToPost = new FormUrlEncodedContent(content);
            return await Post(baseAddress, requestUri, contentToPost, "application/json", throwException);
        }

        public static async Task<string> Post(string baseAddress, string requestUri, HttpContent contentToPost, string mediaType, bool throwException) {
            try {
                using (var client = new HttpClient()) {
                    //Prepare
                    client.BaseAddress = new Uri(baseAddress);
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(mediaType));

                    //Actual call
                    var response = client.PostAsync(requestUri, contentToPost).Result;

                    //Response
                    var responseString = "<unknown>";
                    if (response.IsSuccessStatusCode) {
                        responseString = await response.Content.ReadAsStringAsync();
                    }
                    else {
                        responseString = "ERROR: " + response.StatusCode;
                        if (throwException)
                            throw new Exception(responseString);
                    }

                    return responseString;
                }
            }
            catch (Exception ex) {
                if (throwException)
                    throw;
                return ex.FullMessage();
            }
        }

        #endregion

    }
}
