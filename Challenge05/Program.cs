using System;
using System.IO;

using System.Net;
using System.Net.Http;
using System.Net.Sockets;
using System.Net.Http.Headers;

using System.Text;
using System.Threading.Tasks;

namespace Challenge05
{
    class Program
    {
        static HttpClient Client
        {
            get;
            set;
        }
        
        static void Main(string[] args)
        {
            Task.Run(new GhostBuster().Initialize).Wait();

            Console.ReadKey();
        }
    }

    class GhostBuster
    {
        const byte   ResponseChunkLength = 13;
        const short  ResponseTotalLength = 3445;
        const string GhostHttpServiceUrl = "https://52.49.91.111:8443/";

        private HttpClient Client
        {
            get;
        }

        private HttpClientHandler Handler
        {
            get;
        }

        public GhostBuster()
        {
            Handler = new HttpClientHandler();
            Client  = new HttpClient(Handler);
        }

        public async Task Initialize()
        {
            SetupHttpClient();

            await FindGhostInHttpService();
        }

        private void SetupHttpClient()
        {
            Client.BaseAddress = new Uri(GhostHttpServiceUrl);

            Handler.ServerCertificateCustomValidationCallback += (c, e, r, t) =>
            {
                return true;
            };
        }

        /// <summary>
        /// What?? a response with a content-length of 3445 bytes
        /// but only returing the first 13 bytes??? Is it sorcery?
        /// Jokes aside.
        /// The service is returning a partial response,
        /// each response fragment is formed by 13 bytes.
        /// 
        /// The final response must be a base64 string with a length of 3445.
        /// </summary>
        private async Task FindGhostInHttpService()
        {
            var buffer = new byte[ResponseTotalLength];

            for (var offset = 0; offset < buffer.Length; offset += ResponseChunkLength)
            {
                Client.DefaultRequestHeaders.Range = new RangeHeaderValue(
                    offset, ResponseChunkLength + offset
                );

                var response = await Client.GetStreamAsync("/ghost");
                var restream = await response.ReadAsync(buffer, offset, ResponseChunkLength);
            }

            Console.WriteLine($"Response:\r\n{Encoding.UTF8.GetString(buffer).Trim()}");
        }

		/// <summary>
		/// So... The previous method returned an image with the
		/// Message: 4017-8120. The game is about ranges so... 
		/// I assume is the range continuation of the http request.
		/// 
		/// If I try to retrieve the token with HTTP v1.1 on the 
		/// 4017-8120 range, the service complains with error 505
		/// (HTTP Version Not Supported) So I try with http v2 and...
		/// Tada!! Another error: "There is nothing sadder than a client without a push"
		/// 
		/// Once requesting the service in range 4017-8120 with PUSH
		/// The token is finally puked out.
		/// </summary>
		private async Task RetrieveTokenFromPushService()
		{
			// .NET Core Doesn't implements Http v2.
			// So I sent the request with nghttp.
			//
			// ~Tylerian$ nghttp -H":method: PUSH" -H"Range: bytes=4017-8120" https://52.49.91.111:8443/ghost
			//
			// YourEffortToRemainWhatYouAreIsWhatLimitsYou
			// You found me.Pushing my token, did you get it?


			/*
			var length = 8120 - 4017;
            var buffer = new byte[length];

            Client.DefaultRequestHeaders.Range = new RangeHeaderValue(4017, 8120);

            var response = await Client.GetStreamAsync("/ghost");

            var restream = await response.ReadAsync(buffer, 0, length);

            Console.WriteLine($"Response:\r\n{Encoding.UTF8.GetString(buffer).Trim()}");
            
			*/
		}
    }
}