﻿namespace TimeSince.Avails
{
    [Obsolete("Not needed in this application.  Remove it")]
    public class SearchOptions
    {
        private string _searchTerm;

        public string SearchTerm
        {
            get => _searchTerm ?? string.Empty;
            set => _searchTerm = value;
        }

        public bool   ShowErrors     { get; set; }
        public bool   ShowWarnings   { get; set; }
        public bool   ShowInformation { get; set; }

        public SearchOptions ()
        {
            SearchTerm      = string.Empty;
            ShowErrors      = true;
            ShowWarnings    = true;
            ShowInformation = true;
        }
    }
}
