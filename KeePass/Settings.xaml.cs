﻿using System;
using System.Windows;
using System.Windows.Navigation;
using KeePass.Utils;
using KeePass.I18n;

using Coding4Fun.Phone.Controls.Data;
using Microsoft.Phone.Tasks;
using System.Collections.Generic;
using System.Threading;
using System.Globalization;

namespace KeePass
{
    class LanguageProxy : CultureInfo
    {
        public LanguageProxy(string culture) : base(culture) { }
        public override string ToString()
        {
            return base.Parent.DisplayName;
        }

    }
    public partial class Settings
    {
        string page = string.Empty;
        string[] cultures = new[] { "en-US", "ka-GE", "de-DE", "ru-RU", "el-GR", "es-ES", "fr-FR", "hu-HU", "nl-NL", "pl-PL", };
        public Settings()
        {
            InitializeComponent();
            lstAutoSyncSettings.Items.Add(Strings.Settings_AutoUpdateInactive);
            lstAutoSyncSettings.Items.Add(Strings.Settings_AutoUpdateActive);
            lstAutoSyncSettings.Items.Add(Strings.Settings_AutoUpdateSWLAN);

            var version = PhoneHelper
                .GetAppAttribute("Version");

            lblVersion.Text = string.Format(lblVersion.Text, version);

            Array.ForEach(cultures, item =>
            {
                var lang = new LanguageProxy(item);
                listpickerLanguage.Items.Add(lang);
                if (lang.Name == Thread.CurrentThread.CurrentCulture.Name)
                    listpickerLanguage.SelectedItem = lang;
            });
        }

        protected override void OnNavigatedTo(
            bool cancelled, NavigationEventArgs e)
        {
            if (cancelled)
                return;

            var settings = AppSettings.Instance;
            chkBrowser.IsChecked = settings.UseIntBrowser;
            chkRecycleBin.IsChecked = settings.HideRecycleBin;
            chkPWSearch.IsChecked = settings.SearchInPW;
            chkSyncToast.IsChecked = settings.SyncToast;

            chkPassword.IsChecked = !string
                .IsNullOrEmpty(settings.Password);

            if (AppSettings.Instance.AutoUpdate == true)
            {
                if (AppSettings.Instance.AutoUpdateWLAN == true)
                    lstAutoSyncSettings.SelectedIndex = 2;
                else
                    lstAutoSyncSettings.SelectedIndex = 1;
            }
            else
                lstAutoSyncSettings.SelectedIndex = 0;

            if (NavigationContext.QueryString.TryGetValue("page", out page))
            {
                page = NavigationContext.QueryString["page"];
                Pivot.SelectedIndex = Convert.ToInt32(page);
                NavigationContext.QueryString.Remove("page");
            }
        }

        private void chkBrowser_CheckedChanged(
            object sender, RoutedEventArgs e)
        {
            AppSettings.Instance.UseIntBrowser =
                chkBrowser.IsChecked == true;
        }

        private void chkPassword_Checked(
            object sender, RoutedEventArgs e)
        {
            this.NavigateTo<GlobalPass>();
        }

        private void chkPassword_Loaded(
            object sender, RoutedEventArgs e)
        {
            chkPassword.Checked += chkPassword_Checked;
            chkPassword.Unchecked += chkPassword_Unchecked;
        }

        private static void chkPassword_Unchecked(
            object sender, RoutedEventArgs e)
        {
            AppSettings.Instance.Password = string.Empty;
        }

        private void chkRecycleBin_CheckedChanged(
            object sender, RoutedEventArgs e)
        {
            AppSettings.Instance.HideRecycleBin =
                chkRecycleBin.IsChecked == true;
        }

        private void chkPWSearch_CheckedChanged(object sender, RoutedEventArgs e)
        {
            AppSettings.Instance.SearchInPW =
                chkPWSearch.IsChecked == true;
        }

        private void chkSyncToast_CheckedChanged(object sender, RoutedEventArgs e)
        {
            AppSettings.Instance.SyncToast =
                chkSyncToast.IsChecked == true;
        }

        private void setWLAN(object sender, RoutedEventArgs e)
        {
            if (lstAutoSyncSettings.SelectedIndex == 0)
            {
                AppSettings.Instance.AutoUpdate = false;
                AppSettings.Instance.AutoUpdateWLAN = false;
            }
            else if (lstAutoSyncSettings.SelectedIndex == 1)
            {
                AppSettings.Instance.AutoUpdate = true;
                AppSettings.Instance.AutoUpdateWLAN = false;
            }
            else if (lstAutoSyncSettings.SelectedIndex == 2)
            {
                AppSettings.Instance.AutoUpdateWLAN = true;
                AppSettings.Instance.AutoUpdate = true;
            }
        }

        private void lnkReview_Click(object sender, RoutedEventArgs e)
        {
            new MarketplaceReviewTask().Show();
        }

        private void ListPicker_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            var si = listpickerLanguage.SelectedItem as LanguageProxy;
            AppSettings.Instance.Language = si.Name;
            App.SetCulture(si.Name);
            this.UpdateLayout();
        }
    }
}