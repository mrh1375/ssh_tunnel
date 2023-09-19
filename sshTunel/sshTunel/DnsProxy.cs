using Newtonsoft.Json.Linq;
using System.Net;

namespace DnsServerExample
{
    public class DnsProxy
    {
        private readonly HttpClient _httpClient;
        private readonly Dictionary<string, IPAddress> _cache;

        public DnsProxy(string server, int port)
        {
            var handler = new HttpClientHandler
            {
                Proxy = new WebProxy
                {
                    Address = new Uri($"socks5://{server}:{port}")
                }
            };
            _httpClient = new HttpClient(handler);
            _httpClient.DefaultRequestHeaders.Accept.Clear();
            _httpClient.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/dns-json"));


            _cache = new Dictionary<string, IPAddress>();
        }

        public async Task<IPAddress> ResolveAsync(string domain, string DNSAddress)
        {
            if (_cache.ContainsKey(domain))
            {
                Console.WriteLine($"Found {domain} in cache");
                return _cache[domain];
            }

            Console.WriteLine($"Resolving {domain} via DNS {DNSAddress}");
            IPAddress ipAddress = new IPAddress(0);

            var response = await _httpClient.GetAsync($"https://{DNSAddress}/dns-query?name={domain}");

            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadAsStringAsync();
                var resJsonObj = JObject.Parse(result);


                if (IPAddress.TryParse(resJsonObj["Answer"][0]["data"].ToString(), out ipAddress))
                {
                    _cache[domain] = ipAddress;
                }

                Console.WriteLine($"Resolved {domain} to {ipAddress}");
            }

            return ipAddress;
        }
    }
}
