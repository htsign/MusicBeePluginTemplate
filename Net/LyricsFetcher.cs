using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;
using Sgml;

namespace MusicBeePlugin.Net
{
    public abstract class LyricsFetcher
    {
        #region private fields
        private static Dictionary<string, LyricsFetcher> registeredProviders = new Dictionary<string, LyricsFetcher>();
        #endregion

        #region public properties
        /// <summary>
        /// LyricsFetcher に登録済の歌詞提供事業者名リスト
        /// </summary>
        public static string[] RegisteredProviders => registeredProviders.Keys.ToArray();

        /// <summary>
        /// 一意に識別可能な歌詞提供事業者名
        /// </summary>
        public abstract string Provider { get; }
        #endregion

        static LyricsFetcher()
        {
            // RegisterProvider<JLyricLyricsFetcher>();
        }

        #region public methods
        /// <summary>
        /// 歌詞取得クラスを登録します。
        /// </summary>
        /// <typeparam name="TLyricsFetcher"></typeparam>
        /// <returns></returns>
        public static bool RegisterProvider<TLyricsFetcher>()
            where TLyricsFetcher : LyricsFetcher, new()
        {
            var fetcher = new TLyricsFetcher();
            string provider = fetcher.Provider;

            if (registeredProviders.ContainsKey(provider)) return false;
            registeredProviders[provider] = fetcher;
            return true;
        }

        /// <summary>
        /// 登録済みの歌詞取得クラスを取得します。
        /// </summary>
        /// <param name="providerName"></param>
        /// <returns></returns>
        public static LyricsFetcher GetFetcher(string providerName) => registeredProviders[providerName];

        /// <summary>
        /// タグを元に歌詞を取得します。
        /// </summary>
        /// <param name="title">タイトル</param>
        /// <param name="artist">アーティスト</param>
        /// <param name="album">アルバム</param>
        /// <returns></returns>
        public abstract string Fetch(string title, string artist, string album = null);
        #endregion

        #region private methods
        /// <summary>
        /// リクエスト先のHTMLをXMLドキュメントとして非同期に取得します。
        /// </summary>
        /// <param name="url">リクエストURL</param>
        /// <returns></returns>
        protected Task<XDocument> DownloadPageAsXmlAsync(string url) =>
            Task.Factory.StartNew(() => new SgmlReader { Href = url, IgnoreDtd = true })
            .ContinueWith(t => XDocument.Load(t.Result));
        #endregion
    }
}
