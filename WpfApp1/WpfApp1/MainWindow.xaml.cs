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

        public static MainWindow MainCS;
        public MainWindow()
        {
            InitializeComponent();
            MainCS = this;
        }

        //Required Function
        public void InactiveCompanies(SelectedDatesCollection dates)
        {
            bool isInactive = true;

            //Iterate through dictionary
            foreach (KeyValuePair<string, List<String>> company in CompanyFeeds)
            {

                foreach (String url in company.Value)
                {

                    //Load Feed
                    XmlReaderSettings settings = new XmlReaderSettings();
                    settings.DtdProcessing = DtdProcessing.Parse;
                    var r = XmlReader.Create(url, settings);
                    var feed = SyndicationFeed.Load(r);

                    //Add the company name to the slacking group if they havent posted 
                    if (checkInactive(feed) == true)
                    {
                        SlackingCompanies.Add(company.Key);
                        continue;
                    }

                    else
                    {
                        //If they have been active in different URL then we will remove 
                        //them from the slacking list if they are already in it
                        if (SlackingCompanies.Contains(company.Key))
                        {
                            SlackingCompanies.Remove(company.Key);
                        }

                        //reset booleon
                        isInactive = true;
                        break;
                    }


                }

            }

            //pass they slacking companies to the grid to be displayed
            results.CompanyGrid.ItemsSource = SlackingCompanies;
            results.Show();

            bool checkInactive(SyndicationFeed feed)
            {
                //Iterate through retrieved posts
                foreach (SyndicationItem post in feed.Items)
                {

                    //If there is a post published in the date range continue
                    //If the company hasnt posted in one feed but has in another we will remove it from the slacking pile
                    foreach (DateTime date in dates)
                    {

                        // MessageBox.Show(date.ToString());
                        // MessageBox.Show(post.PublishDate.Date.ToString());

                        if (date.Date == post.PublishDate.Date)
                        {
                            isInactive = false;
                            break;
                        }

                    }

                    if(isInactive == false) { break; }
                }
                return isInactive;
            }
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
            else if(BillSimmTracker.IsChecked == false)
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
                CompanyFeeds.Remove("Diane Rehm");
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

        public void ResetAll()
        {
            //Reset all the collections
            SlackingCompanies.Clear();
            CompanyFeeds.Clear();

            //Reset all the box checks
            BillSimmTracker.IsChecked = false;
            DianeTracker.IsChecked = false;
            RealTimeTracker.IsChecked = false;
            BBCTracker.IsChecked = false;
        }


    }
}
