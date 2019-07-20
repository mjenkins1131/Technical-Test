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
        IDictionary<String, dynamic> CompanyFeeds = new Dictionary<String, dynamic>();

        //List Collection for Company URLs
        List<String> BBCUrl = new List<String>();
        List<String> RealTimeUrl = new List<String>();
        List<String> BillSimmUrl = new List<String>();
        List<String> DianeUrl = new List<String>();

        //List Collection for the slacking companies
        List<String> SlackingCompanies = new List<String>();



        public MainWindow()
        {
            InitializeComponent();
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            
        }

        //Required Function
        public List<String> InactiveCompanies(SelectedDatesCollection dates)
        {
            //Iterate through dictionary
            foreach (KeyValuePair<string, dynamic> entry in CompanyFeeds)
            {
                //Load Feed
                var r = XmlReader.Create(entry.Value);
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
                        //Add the company name to the slacking group if they havent posted 
                        SlackingCompanies.Add(entry.Key);
                    }

                }

            }
            

            return SlackingCompanies;
        }

        //BBC Radio Button Function
        private void BBC_RadioButton_Checked(object sender, RoutedEventArgs e)
        {
            BBCUrl.Add("https://www.bbc.com/news/10628494");
            if (BBCTrack.IsChecked == true)
            {
                CompanyFeeds.Add("BBC", BBCUrl);
            }
                
        }
    
        //Real Time Radio Button Function
        private void RealTime_RadioButton_Checked(object sender, RoutedEventArgs e)
        {
            RealTimeUrl.Add("http://billmaher.hbo.libsynpro.com/rss");
            if (RealTimeTrack.IsChecked == true)
            {
                foreach (String i in RealTimeUrl){
                    CompanyFeeds.Add("Real Time", RealTimeUrl.IndexOf(i));
                }
            }

        }

        //Bill Simmons Radio Button Function
        private void BillSim_RadioButton_Checked(object sender, RoutedEventArgs e)
        {
            BillSimmUrl.Add("https://rss.art19.com/the-bill-simmons-podcast");
            if (BillSimTrack.IsChecked == true)
            {
                foreach (String i in BillSimmUrl)
                {
                    CompanyFeeds.Add("Bill Simmons", BillSimmUrl.IndexOf(i));
                }
            }

        }

        //Diane Rehm Radio Button Function
        private void Diane_RadioButton_Checked(object sender, RoutedEventArgs e)
        {
            DianeUrl.Add("https://dianerehm.org/rss/npr/dr_podcast.xml");
            if (DianeTrack.IsChecked == true)
            {
                foreach (String i in DianeUrl)
                {
                    CompanyFeeds.Add("Diane Rehm", DianeUrl.IndexOf(i));
                }
            }


        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            SelectedDatesCollection dates = DateRange.SelectedDates;
            InactiveCompanies(dates);
        }
    }
}
