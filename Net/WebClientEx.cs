using System;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Net;
using System.Threading.Tasks;

namespace MusicBeePlugin.Net
{
    public class WebClientEx : WebClient
    {
        public string Referer = null;
        public CookieContainer CookieContainer { get; set; } = new CookieContainer();

        protected override WebRequest GetWebRequest(Uri address)
        {
            var req = base.GetWebRequest(address);

            if (req is HttpWebRequest)
            {
                (req as HttpWebRequest).CookieContainer = CookieContainer;
            }
            return req;
        }

        protected override WebResponse GetWebResponse(WebRequest request)
        {
            var res = base.GetWebResponse(request);

            if (res is HttpWebResponse)
            {
                Referer = (res as HttpWebResponse).ResponseUri.AbsoluteUri;
            }
            return res;
        }
        
        public Task<string> DownloadStringTaskAsync(string address)
        {
            var tcs = new TaskCompletionSource<string>(address);

            DownloadStringCompletedEventHandler handler = null;
            handler = (sender, e) => HandleCompletion(tcs, e, args => args.Result, handler,
                (webClient, completion) => webClient.DownloadStringCompleted -= completion);
            DownloadStringCompleted += handler;
            
            try { DownloadStringAsync(new Uri(address), tcs); }
            catch
            {
                DownloadStringCompleted -= handler;
                throw;
            }
            
            return tcs.Task;
        }

        public Task<byte[]> DownloadDataTaskAsync(string address)
        {
            var tcs = new TaskCompletionSource<byte[]>(address);
            
            DownloadDataCompletedEventHandler handler = null;
            handler = (sender, e) => HandleCompletion(tcs, e, (args) => args.Result, handler,
                (webClient, completion) => webClient.DownloadDataCompleted -= completion);
            DownloadDataCompleted += handler;
            
            try { DownloadDataAsync(new Uri(address), tcs); }
            catch
            {
                DownloadDataCompleted -= handler;
                throw;
            }
            
            return tcs.Task;
        }

        public Task<byte[]> UploadValuesTaskAsync(string uri, NameValueCollection data)
        {
            var tcs = new TaskCompletionSource<byte[]>();

            UploadValuesCompletedEventHandler handler = null;
            handler = (sender, e) => HandleCompletion(tcs, e, args => args.Result, handler,
                (webClient, completion) => webClient.UploadValuesCompleted -= completion);
            UploadValuesCompleted += handler;

            try { UploadValuesAsync(new Uri(uri), null, data, tcs); }
            catch
            {
                UploadValuesCompleted -= handler;
                throw;
            }
            
            return tcs.Task;
        }

        private void HandleCompletion<TAsyncCompletedEventArgs, TCompletionDelegate, T>(
            TaskCompletionSource<T> tcs,
            TAsyncCompletedEventArgs e,
            Func<TAsyncCompletedEventArgs, T> getResult,
            TCompletionDelegate handler,
            Action<WebClientEx, TCompletionDelegate> unregisterHandler)
            where TAsyncCompletedEventArgs : AsyncCompletedEventArgs
        {
            if (e.UserState == tcs)
            {
                try { unregisterHandler(this, handler); }
                finally
                {
                    if (e.Error != null) tcs.TrySetException(e.Error);
                    else if (e.Cancelled) tcs.TrySetCanceled();
                    else tcs.TrySetResult(getResult(e));
                }
            }
        }
    }
}
