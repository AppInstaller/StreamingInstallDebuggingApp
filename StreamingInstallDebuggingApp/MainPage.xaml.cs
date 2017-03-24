using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.Management.Deployment;
using Windows.ApplicationModel;
using System.Diagnostics;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace StreamingInstallDebuggingApp
{
    public sealed partial class MainPage : Page
    {
        PackageManager m_PackageManager;
        PackageManagerDebugSettings m_DebugSettings;
        public MainPage()
        {
            this.InitializeComponent();
            m_PackageManager = new PackageManager();
            m_DebugSettings = m_PackageManager.DebugSettings;

            ApplicationView.PreferredLaunchViewSize = new Size(800, 700);
            ApplicationView.PreferredLaunchWindowingMode = ApplicationViewWindowingMode.PreferredLaunchViewSize;
            this.Loaded += MainPage_Loaded;
        }

        private void MainPage_Loaded(object sender, RoutedEventArgs e)
        {
            this.CGState_combobox.Items.Add(PackageContentGroupState.NotStaged);
            this.CGState_combobox.Items.Add(PackageContentGroupState.Queued);
            this.CGState_combobox.Items.Add(PackageContentGroupState.Staging);
            this.CGState_combobox.Items.Add(PackageContentGroupState.Staged);
            this.CGState_combobox.SelectedIndex = 0;

            FindPackages_button_Click(null, null);
        }

        Package FindPackage()
        {
            this.Status_Text.Text = "";
            Package p = null;
            try
            {
                p = m_PackageManager.FindPackageForUser(this.UserSID_TextBox.Text.Trim(), (string)this.Packages_combobox.SelectedItem);
            }
            catch (Exception ex)
            {
                this.Status_Text.Text = "FindPackage: " + ex.Message;
                return p;
            }

            if (p == null)
            {
                this.Status_Text.Text = "FindPackage: Package not found";
            }

            return p;
        }
        private async void SetCGState_button_Click(object sender, RoutedEventArgs e)
        {
            Package p = FindPackage();
            if (p == null) return;

            var contentGroupName = (string)this.ContentGroups_combobox.SelectedItem;
            var cgState = (PackageContentGroupState)this.CGState_combobox.SelectedItem;

            try
            {
                if (cgState == PackageContentGroupState.Staging)
                {
                    await m_DebugSettings.SetContentGroupStateAsync(p, contentGroupName, cgState, this.Percent_slider.Value);
                }
                else
                {
                    await m_DebugSettings.SetContentGroupStateAsync(p, contentGroupName, cgState);
                }
            }
            catch (Exception ex)
            {
                this.Status_Text.Text = "SetContentGroupState: " + ex.Message;
            }
        }

        private async void FindPackages_button_Click(object sender, RoutedEventArgs e)
        {
            this.Packages_combobox.Items.Clear();
            if (this.ContentGroups_combobox.Items.Count > 0)
            {
                this.ContentGroups_combobox.Items.Clear();
            }
            this.Percent_slider.Value = 0;
            this.CGState_combobox.SelectedIndex = 0;

            try
            {
                var packages = m_PackageManager.FindPackagesForUser(this.UserSID_TextBox.Text.Trim());
                var currentIndex = 0;
                var selectedIndex = 0;
                foreach (var package in packages)
                {
                    if (package.IsDevelopmentMode)
                    {
                        this.Packages_combobox.Items.Add(package.Id.FullName);

                        var groups = await package.GetContentGroupsAsync();
                        if (groups.Count > 0)
                        {
                            selectedIndex = currentIndex;
                        }

                        currentIndex++;
                    }
                }

                if (this.Packages_combobox.Items.Count > 0)
                {
                    this.Packages_combobox.SelectedIndex = selectedIndex;
                }
            }
            catch (Exception ex)
            {
                this.Status_Text.Text = "FindPackages: " + ex.Message;
                return;
            }
        }

        private async void Packages_combobox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            this.ContentGroups_combobox.Items.Clear();
            this.Percent_slider.Value = 0;
            this.CGState_combobox.SelectedIndex = 0;

            Package p = FindPackage();
            if (p == null) return;

            var groups = await p.GetContentGroupsAsync();

            foreach (var group in groups)
            {
                this.ContentGroups_combobox.Items.Add(group.Name);
            }

            if (groups.Count > 0)
            {
                this.ContentGroups_combobox.SelectedIndex = 0;
            }
        }

        private void ChangeUserChecked(object sender, RoutedEventArgs e)
        {
            this.UserSID_TextBlock.Visibility = Visibility.Visible;
            this.UserSID_TextBox.Visibility = Visibility.Visible;
            this.FindPackages_button.Visibility = Visibility.Visible;

            foreach (FrameworkElement i in this.MainGrid.Children)
            {
                if (i.Name != "Title_TextBlock" && i.Name != "UserSID_TextBox" && i.Name != "UserSID_TextBlock" && i.Name != "ChangeUser_TextBlock" && i.Name != "" && i.Name != "ChangeUser_CheckBox" && i.Name != "FindPackages_button")
                {
                    Thickness margin = i.Margin;
                    margin.Top += 50;
                    i.Margin = margin;
                }
            }
        }

        private void ChangeUserUnchecked(object sender, RoutedEventArgs e)
        {
            this.UserSID_TextBlock.Visibility = Visibility.Collapsed;
            this.UserSID_TextBox.Visibility = Visibility.Collapsed;
            this.FindPackages_button.Visibility = Visibility.Collapsed;

            foreach (FrameworkElement i in this.MainGrid.Children)
            {
                if (i.Name != "Title_TextBlock" && i.Name != "UserSID_TextBox" && i.Name != "UserSID_TextBlock" && i.Name != "ChangeUser_TextBlock" && i.Name != "" && i.Name != "ChangeUser_CheckBox" && i.Name != "FindPackages_button")
                {
                    Thickness margin = i.Margin;
                    margin.Top -= 50;
                    i.Margin = margin;
                }
            }
        }
    }
}
