using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.ServiceModel.Syndication;
using System.Xml;

namespace WpfApp1
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        //Dictionary for Companies and their URLs
        IDictionary<String, List<String>> CompanyFeeds = new Dictionary<String, List<String>>();

        //List Collection for Company URLs
        List<String> BBCUrl = new List<String>();
        List<String> RealTimeUrl = new List<String>();
        List<String> BillSimmUrl = new List<String>();
        List<String> DianeUrl = new List<String>();

        //List Collection for the slacking companies
        List<String> SlackingCompanies = new List<String>();

        Window1 results = new Window1();


        public MainWindow()
        {
            InitializeComponent();
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            
        }

        //Required Function
        public void InactiveCompanies(SelectedDatesCollection dates)
        {
            //Iterate through dictionary
            foreach (KeyValuePair<string, List<String>> entry in CompanyFeeds)
            {

                foreach (String url in entry.Value) {

                    //Load Feed
                    XmlReaderSettings settings = new XmlReaderSettings();
                    settings.DtdProcessing = DtdProcessing.Parse;
                    var r = XmlReader.Create(url,settings);
                    var feed = SyndicationFeed.Load(r);

                    //Iterate through retrieved posts
                    foreach (SyndicationItem post in feed.Items)
                    {
                        //If there is a post published in the date range continue
                        //If the company hasnt posted in one feed but has in another we will remove it from the slacking pile
                        if (dates.Contains(post.PublishDate.UtcDateTime))
                        {
                            if (SlackingCompanies.Contains(entry.Key))
                            {
                                SlackingCompanies.Remove(entry.Key);
                                
                            }
                            
                            continue;
                        }
                        else
                        {
                           
                            if (SlackingCompanies.Contains(entry.Key)) { continue; }else
                            //Add the company name to the slacking group if they havent posted 
                            SlackingCompanies.Add(entry.Key);
                            
                            
                        }
                    }
                }
               
            }
            results.CompanyGrid.ItemsSource = SlackingCompanies;
            results.Show();
        }

        //BBC Checked Button Function
        private void BBCTracker_Checked(object sender, RoutedEventArgs e)
        {
            BBCUrl.Add("https://www.bbc.com/news/10628494");
            if (BBCTracker.IsChecked == true)
            {
                CompanyFeeds.Add("BBC", BBCUrl);
            }
            else
            {
                CompanyFeeds.Remove("BBC");
            }
        }

        //Real Time Checked Button Function
        private void RealTimeTracker_Checked(object sender, RoutedEventArgs e)
        {
            RealTimeUrl.Add("http://billmaher.hbo.libsynpro.com/rss");
            if (RealTimeTracker.IsChecked == true)
            {
                CompanyFeeds.Add("Real Time", RealTimeUrl);
            }
            else
            {
                CompanyFeeds.Remove("Real Time");
            }
        }

        //Bill Simmons Checked Button Function
        private void BillSimmTracker_Checked(object sender, RoutedEventArgs e)
        {
            BillSimmUrl.Add("https://rss.art19.com/the-bill-simmons-podcast");
            if (BillSimmTracker.IsChecked == true)
            {
                CompanyFeeds.Add("Bill Simmons", BillSimmUrl);
            }
            else
            {
                CompanyFeeds.Remove("Bill Simmons");
            }
        }

        //Diane Rehm Checked Button Function
        private void DianeTracker_Changed(object sender, RoutedEventArgs e)
        {
            DianeUrl.Add("https://dianerehm.org/rss/npr/dr_podcast.xml");
            if (DianeTracker.IsChecked == true)
            {
                CompanyFeeds.Add("Diane Rehm", DianeUrl);
            }
            else if(DianeTracker.IsChecked == false)
            {
                
            }
        }

        private void DianeTracker_UnChecked(object sender, RoutedEventArgs e)
        {
            
            CompanyFeeds.Remove("Diane Rehm");
           
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            SelectedDatesCollection dates = DateRange.SelectedDates;
            InactiveCompanies(dates);
        }

       
    }
}
