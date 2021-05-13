// This file is part of JesTpro project.
//
// JesTpro is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (if needed) any later version.
//
// JesTpro has NO WARRANTY!! It is distributed for test, study or 
// personal environments. Any commercial distribution
// has no warranty! 
// See the GNU General Public License in root project folder  
// for more details or  see <http://www.gnu.org/licenses/>

namespace jt.jestpro.Helpers
{
    public class AppSettings
    {
        public string Secret { get; set; }
        public string ImagePath { get; set; }
        public string AttachmentPath { get; set; }
        public string MassiveImportPath { get; set; }
        public string BackupPath { get; set; }
        public string LogPath { get; set; }
        public string DefaultLocale { get; set; }
        public string SystemEmail { get; set; }
        public bool UseSqLite { get; set; }
        public bool ForceExpirationCheckOnStart { get; set; }
        public PdfSettings PdfSettings { get; set; }

    }
}