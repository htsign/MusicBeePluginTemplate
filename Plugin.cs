using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;

namespace MusicBeePlugin
{
    using Net;
    using Windows.Forms;

    public partial class Plugin
    {
        public const string ConfigName = "PluginTemplateSettings.xml";

        private MusicBeeApiInterface mbApiInterface;
        private PluginInfo about = new PluginInfo();
        private string configPath;

        public PluginInfo Initialise(IntPtr apiInterfacePtr)
        {
            Assembly asm = Assembly.GetExecutingAssembly();
            var attributes = asm.GetCustomAttributes(true).Cast<Attribute>();
            Version version = asm.GetName().Version;

            mbApiInterface = new MusicBeeApiInterface();
            mbApiInterface.Initialise(apiInterfacePtr);
            about.PluginInfoVersion = PluginInfoVersion;
            about.Name        = attributes.OfType<AssemblyTitleAttribute>()      .SingleOrDefault()?.Title;
            about.Description = attributes.OfType<AssemblyDescriptionAttribute>().SingleOrDefault()?.Description;
            about.Author      = attributes.OfType<AssemblyCompanyAttribute>()    .SingleOrDefault()?.Company;
            about.TargetApplication = "";   // current only applies to artwork, lyrics or instant messenger name that appears in the provider drop down selector or target Instant Messenger
            about.Type         = PluginType.LyricsRetrieval;
            about.VersionMajor = (short)version.Major;
            about.VersionMinor = (short)version.Minor;
            about.Revision     = (short)version.Revision;
            about.MinInterfaceVersion  = MinInterfaceVersion;
            about.MinApiRevision       = MinApiRevision;
            about.ReceiveNotifications = (ReceiveNotificationFlags.PlayerEvents | ReceiveNotificationFlags.TagEvents);
            about.ConfigurationPanelHeight = 0;   // MusicBeeの設定エリアに表示されるパネルの高さを定義します。セットすることで、空のPanelオブジェクトのハンドルがConfigureメソッドに渡されます。

            // config
            string dataPath = mbApiInterface.Setting_GetPersistentStoragePath();
            this.configPath = Path.Combine(dataPath, ConfigName);
            Config.Instance.Load(configPath);

            return about;
        }

        public bool Configure(IntPtr panelHandle)
        {
            Config.Instance.Load(configPath);

            // panelHandle は about.ConfigurationPanelHeight に0以外を代入した場合にセットされます。
            // パネルの幅は、ユーザーが選択したフォントに依存することに留意してください。
            // about.ConfigurationPanelHeight に0を代入するとポップアップウィンドウが表示されます。
            if (panelHandle != IntPtr.Zero)
            {
                // ここでは設定エリア用のコントロールをユーザーコントロールとして分離した後、
                // それを一括りにして追加しています。
                // 各コントロールの宣言や配置をここに直接記述すると非常に見通しが悪くなるためです。
                // Configインスタンスを参照渡しすることでデータの整合を図っていますが、
                // データバインディングを用いて編集画面を構築する場合は、
                // Config#Load をデータバインドしている行より後で呼び出す必要があります。
                // また、実際に表示される各々のコントロールの位置は
                // デザイナに表示される位置と異なることがあるのに注意が必要です。
                Panel configPanel = (Panel)Control.FromHandle(panelHandle);
                var configureControl = new ConfigureControl(configPath);
                configPanel.Controls.Add(configureControl);
            }
            return about.ConfigurationPanelHeight != 0;
        }
        
        // MusicBeeの設定画面で「適用」または「保存」をクリックするたびに呼ばれます。
        // 何かが変更されたか、そして更新の必要があるかどうかを把握するのはあなた次第です。
        public void SaveSettings()
        {
            // このコメントアウトを外すと設定ファイルが作られるようになります。
            // 設定ファイルが存在しなくても Config#Load メソッドで例外が出ることはありません。
            Config.Instance.Save(configPath);
        }

        // プラグインがユーザーの手によって無効化されるとき、またMusicBeeが終了するときに呼ばれます。
        public void Close(PluginCloseReason reason)
        {
        }

        // 「アンインストール」がクリックされると呼び出されます。
        // このプラグインが何かファイルを作成しているのであれば、ここで削除します。
        public void Uninstall()
        {
            File.Delete(configPath);
        }

        // MusicBeeからの通知イベントを受け取ります。
        // about.ReceiveNotificationFlags = PlayerEvents とすることですべての通知を受け取れます。
        // それ以外の場合は開始イベントだけです。
        public void ReceiveNotification(string sourceFileUrl, NotificationType type)
        {
            // 通知のタイプごとにそれぞれ独立したコードを実行してください。
            switch (type)
            {
                case NotificationType.PluginStartup:
                    // ここにプラグインの初期化処理を記述します。
                    switch (mbApiInterface.Player_GetPlayState())
                    {
                        case PlayState.Playing:
                        case PlayState.Paused:
                            // ...
                            break;
                    }
                    break;

                case NotificationType.TrackChanged:
                    // ...
                    break;
            }
        }

        // このプラグインがサポートする歌詞やアートワークの提供元（プロバイダ名）を配列で返してください。
        // ここで得られたプロバイダ名がMusicBeeの設定の「タグ(2)」に表示され、
        // そしてそこで選択されたものが一つ一つRetrieveLyrics/RetrieveArtworkメソッドに渡されます。
        public string[] GetProviders()
        {
            return LyricsFetcher.RegisteredProviders;
        }

        // provider に対して artist や title を元にリクエストして得られた歌詞を返してください。
        // このメソッドは PluginType = LyricsRetrieval のときのみ有効です。
        // 歌詞が見つからなかった場合は null を返してください。
        public string RetrieveLyrics(string sourceFileUrl, string artist, string trackTitle, string album, bool synchronisedPreferred, string provider)
        {
            if (about.Type != PluginType.LyricsRetrieval) return null;

            var fetcher = LyricsFetcher.GetFetcher(provider);
            return fetcher?.Fetch(trackTitle, artist);
        }

        // provider に対してリクエストして得られたアートワークのバイナリデータをBASE64エンコードして返してください。
        // このメソッドは PluginType = ArtworkRetrieval のときのみ有効です。
        // アートワークが見つからなかった場合は null を返してください。
        public string RetrieveArtwork(string sourceFileUrl, string albumArtist, string album, string provider)
        {
            //Return Convert.ToBase64String(artworkBinaryData)
            return null;
        }

        public IEnumerable<string> GetSongs(string query)
        {
            if (mbApiInterface.Library_QueryFiles(query))
            {
                while (true)
                {
                    string file = mbApiInterface.Library_QueryGetNextFile();
                    if (file == null) yield break;
                    yield return file;
                }
            }
        }
    }
}
