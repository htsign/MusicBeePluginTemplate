using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using static MusicBeePlugin.Plugin;

namespace MusicBeePlugin
{
    public class SongFile : IDisposable
    {
        private MusicBeeApiInterface mbApiInterface;
        private Dictionary<string, PropertyInfo> propertyNamesCache = new Dictionary<string, PropertyInfo>();

        #region constructor
        public SongFile(MusicBeeApiInterface api, string filePath)
        {
            mbApiInterface = api;
            FullPath = filePath;
        }
        #endregion

        #region public properties
		public bool AutoCommit { get; set; } = true;

        public string FullPath { get; }
        
        private string fileName = null;
        public string FileName => fileName ?? (fileName = Path.GetFileName(FullPath));
		
        public string this[string name]
        {
            get
            {
                PropertyInfo pi = null;

                if (!propertyNamesCache.TryGetValue(name, out pi))
                {
                    pi = typeof(SongFile).GetProperty(name);
                    propertyNamesCache[name] = pi;
                }
                return pi?.GetValue(this, null) as string;
            }
            set
            {
                PropertyInfo pi = null;

                if (!propertyNamesCache.TryGetValue(name, out pi))
                {
                    pi = typeof(SongFile).GetProperty(name);
                    propertyNamesCache[name] = pi;
                }
                pi?.SetValue(this, value, null);
            }
        }

        public string this[MetaDataType type]
        {
            get { return this[type.ToString()]; }
            set { this[type.ToString()] = value; }
        }
        
        private string artist = null;
        public string Artist
        {
            get { return artist ?? (artist = mbApiInterface.Library_GetFileTag(FullPath, MetaDataType.Artist)); }
            set { mbApiInterface.Library_SetFileTag(FullPath, MetaDataType.Artist, artist = value); }
        }
        
        private string albumArtist = null;
        public string AlbumArtist
        {
            get { return albumArtist ?? (albumArtist = mbApiInterface.Library_GetFileTag(FullPath, MetaDataType.AlbumArtist)); }
            set { mbApiInterface.Library_SetFileTag(FullPath, MetaDataType.AlbumArtist, albumArtist = value); }
        }
        
        private string trackTitle = null;
        public string TrackTitle
        {
            get { return trackTitle ?? (trackTitle = mbApiInterface.Library_GetFileTag(FullPath, MetaDataType.TrackTitle)); }
            set { mbApiInterface.Library_SetFileTag(FullPath, MetaDataType.TrackTitle, trackTitle = value); }
        }
        
        private string album = null;
        public string Album
        {
            get { return album ?? (album = mbApiInterface.Library_GetFileTag(FullPath, MetaDataType.Album)); }
            set { mbApiInterface.Library_SetFileTag(FullPath, MetaDataType.Album, album = value); }
        }
        
        private string composer = null;
        public string Composer
        {
            get { return composer ?? (composer = mbApiInterface.Library_GetFileTag(FullPath, MetaDataType.Composer)); }
            set { mbApiInterface.Library_SetFileTag(FullPath, MetaDataType.Composer, composer = value); }
        }
        
        private string custom1 = null;
        public string Custom1
        {
            get { return custom1 ?? (custom1 = mbApiInterface.Library_GetFileTag(FullPath, MetaDataType.Custom1)); }
            set { mbApiInterface.Library_SetFileTag(FullPath, MetaDataType.Custom1, custom1 = value); }
        }
        
        private string custom2 = null;
        public string Custom2
        {
            get { return custom2 ?? (custom2 = mbApiInterface.Library_GetFileTag(FullPath, MetaDataType.Custom2)); }
            set { mbApiInterface.Library_SetFileTag(FullPath, MetaDataType.Custom2, custom2 = value); }
        }
        
        private string custom3 = null;
        public string Custom3
        {
            get { return custom3 ?? (custom3 = mbApiInterface.Library_GetFileTag(FullPath, MetaDataType.Custom3)); }
            set { mbApiInterface.Library_SetFileTag(FullPath, MetaDataType.Custom3, custom3 = value); }
        }
        
        private string custom4 = null;
        public string Custom4
        {
            get { return custom4 ?? (custom4 = mbApiInterface.Library_GetFileTag(FullPath, MetaDataType.Custom4)); }
            set { mbApiInterface.Library_SetFileTag(FullPath, MetaDataType.Custom4, custom4 = value); }
        }
        
        private string custom5 = null;
        public string Custom5
        {
            get { return custom5 ?? (custom5 = mbApiInterface.Library_GetFileTag(FullPath, MetaDataType.Custom5)); }
            set { mbApiInterface.Library_SetFileTag(FullPath, MetaDataType.Custom5, custom5 = value); }
        }
        
        private string custom6 = null;
        public string Custom6
        {
            get { return custom6 ?? (custom6 = mbApiInterface.Library_GetFileTag(FullPath, MetaDataType.Custom6)); }
            set { mbApiInterface.Library_SetFileTag(FullPath, MetaDataType.Custom6, custom6 = value); }
        }
        
        private string custom7 = null;
        public string Custom7
        {
            get { return custom7 ?? (custom7 = mbApiInterface.Library_GetFileTag(FullPath, MetaDataType.Custom7)); }
            set { mbApiInterface.Library_SetFileTag(FullPath, MetaDataType.Custom7, custom7 = value); }
        }
        
        private string custom8 = null;
        public string Custom8
        {
            get { return custom8 ?? (custom8 = mbApiInterface.Library_GetFileTag(FullPath, MetaDataType.Custom8)); }
            set { mbApiInterface.Library_SetFileTag(FullPath, MetaDataType.Custom8, custom8 = value); }
        }
        
        private string custom9 = null;
        public string Custom9
        {
            get { return custom9 ?? (custom9 = mbApiInterface.Library_GetFileTag(FullPath, MetaDataType.Custom9)); }
            set { mbApiInterface.Library_SetFileTag(FullPath, MetaDataType.Custom9, custom9 = value); }
        }
        
        private string custom10 = null;
        public string Custom10
        {
            get { return custom10 ?? (custom10 = mbApiInterface.Library_GetFileTag(FullPath, MetaDataType.Custom10)); }
            set { mbApiInterface.Library_SetFileTag(FullPath, MetaDataType.Custom10, custom10 = value); }
        }
        
        private string custom11 = null;
        public string Custom11
        {
            get { return custom11 ?? (custom11 = mbApiInterface.Library_GetFileTag(FullPath, MetaDataType.Custom11)); }
            set { mbApiInterface.Library_SetFileTag(FullPath, MetaDataType.Custom11, custom11 = value); }
        }
        
        private string custom12 = null;
        public string Custom12
        {
            get { return custom12 ?? (custom12 = mbApiInterface.Library_GetFileTag(FullPath, MetaDataType.Custom12)); }
            set { mbApiInterface.Library_SetFileTag(FullPath, MetaDataType.Custom12, custom12 = value); }
        }
        
        private string custom13 = null;
        public string Custom13
        {
            get { return custom13 ?? (custom13 = mbApiInterface.Library_GetFileTag(FullPath, MetaDataType.Custom13)); }
            set { mbApiInterface.Library_SetFileTag(FullPath, MetaDataType.Custom13, custom13 = value); }
        }
        
        private string custom14 = null;
        public string Custom14
        {
            get { return custom14 ?? (custom14 = mbApiInterface.Library_GetFileTag(FullPath, MetaDataType.Custom14)); }
            set { mbApiInterface.Library_SetFileTag(FullPath, MetaDataType.Custom14, custom14 = value); }
        }
        
        private string custom15 = null;
        public string Custom15
        {
            get { return custom15 ?? (custom15 = mbApiInterface.Library_GetFileTag(FullPath, MetaDataType.Custom15)); }
            set { mbApiInterface.Library_SetFileTag(FullPath, MetaDataType.Custom15, custom15 = value); }
        }
        
        private string custom16 = null;
        public string Custom16
        {
            get { return custom16 ?? (custom16 = mbApiInterface.Library_GetFileTag(FullPath, MetaDataType.Custom16)); }
            set { mbApiInterface.Library_SetFileTag(FullPath, MetaDataType.Custom16, custom16 = value); }
        }
        #endregion

        #region public methods
        public bool Commit() => mbApiInterface.Library_CommitTagsToFile(FullPath);
        #endregion

		#region disposable pattern
		public void Dispose()
		{
			if (AutoCommit) Commit();
			GC.SuppressFinalize(this);
		}

		~SongFile()
		{
			Dispose();
		}
		#endregion
    }
}

