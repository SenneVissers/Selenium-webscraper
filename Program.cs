using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Interactions;

namespace YouTubeScraper
{
    public class Vacancy
    {
        public string Title { get; set; }
        public string Company { get; set; }
        public string Location { get; set; }
        public string Keywords { get; set; }
        public string DetailsUrl { get; set; }
    }

    class Program
    {
        static void Main(string[] args)
        {
            // Choose the scraping option
            Console.WriteLine("what do you want to scrape (yt / jobs / crypto): ");
            string scrapingChoice = Console.ReadLine();

            if (scrapingChoice == "yt")
            {

                // Set the search term
                Console.WriteLine("Enter a search term: ");
                string searchTerm = Console.ReadLine();

                // Initialize the ChromeDriver
                using (var driver = new ChromeDriver())
                {
                    // Go to the YouTube website
                    driver.Navigate().GoToUrl("https://www.youtube.com/");

                    // Wait for the page to load
                    System.Threading.Thread.Sleep(2000);

                    // Find the accept terms button and click it
                    IWebElement acceptButton = driver.FindElement(By.CssSelector("button.yt-spec-button-shape-next.yt-spec-button-shape-next--filled.yt-spec-button-shape-next--call-to-action.yt-spec-button-shape-next--size-m"));
                    acceptButton.Click();

                    // Find the search box element and enter the search term
                    IWebElement searchBox = driver.FindElement(By.Id("search-input"));
                    searchBox.Click();

                    IWebElement searchBox2 = driver.FindElement(By.Name("search_query"));
                    searchBox2.SendKeys(searchTerm);

                    // Find the search button element and click it
                    IWebElement searchButton = driver.FindElement(By.Id("search-icon-legacy"));
                    searchButton.Click();

                    // Wait for the search results to load
                    System.Threading.Thread.Sleep(2000);

                    // Filter on upload date
                    IWebElement filterButton = driver.FindElement(By.CssSelector("button.yt-spec-button-shape-next.yt-spec-button-shape-next--text.yt-spec-button-shape-next--mono.yt-spec-button-shape-next--size-m.yt-spec-button-shape-next--icon-leading.yt-spec-button-shape-next--align-by-text[aria-pressed='false'][aria-label='Zoekfilters'][style='']"));
                    filterButton.Click();

                    IWebElement filterOptionButton = driver.FindElement(By.CssSelector("a#endpoint.yt-simple-endpoint.style-scope.ytd-search-filter-renderer"));
                    filterOptionButton.Click();

                    // Wait for the search results to update
                    System.Threading.Thread.Sleep(3000);

                    // Find the list of search results
                    IWebElement searchResults = driver.FindElement(By.Id("contents"));

                    // Find all the video titles in the search results
                    IReadOnlyCollection<IWebElement> videoTitles = searchResults.FindElements(By.XPath("//a[@id='video-title']"));

                    // Print the titles of the first 5 videos
                    int i = 1;
                    List<string> titles = new List<string>();
                    foreach (IWebElement videoTitle in videoTitles)
                    {
                        if (i > 5)
                        {
                            break;
                        }
                        Console.WriteLine($"{i}. {videoTitle.Text}");
                        titles.Add(videoTitle.Text);
                        i++;
                    }

                    // Write the titles to a CSV file
                    using (var writer = new StreamWriter("./RecentYT.csv"))
                    {
                        for (int j = 0; j < 5; j++)
                        {
                            writer.WriteLine(titles[j]);
                        }
                        writer.WriteLine("\n");
                    }

                    // Write the titles to a JSON file
                    using (var writer = new StreamWriter("./RecentYT.json"))
                    {
                        JsonSerializer serializer = new JsonSerializer();
                        serializer.Serialize(writer, titles.Take(5));
                    }

                }// Chromedriver
            }// Scrapingchoice if
            else if (scrapingChoice == "jobs")
            {

                // Set the search term
                Console.WriteLine("Enter a search term: ");
                string searchTerm = Console.ReadLine();

                // Initialize the ChromeDriver
                using (var driver = new ChromeDriver())
                {
                    // Go to the ICTjob website
                    driver.Navigate().GoToUrl("https://www.ictjob.be/nl/");

                    // Find the search box element and enter the search term
                    IWebElement searchBox = driver.FindElement(By.Name("keywords"));
                    searchBox.SendKeys(searchTerm);
                    searchBox.SendKeys(Keys.Enter);

                    // Wait for the search results to load
                    System.Threading.Thread.Sleep(4000);

                    // Find the list of search results
                    IWebElement searchResults = driver.FindElement(By.CssSelector("ul.search-result-list.clearfix"));

                    // Find all the vacancy elements in the search results
                    IReadOnlyCollection<IWebElement> vacancyElements = searchResults.FindElements(By.CssSelector("li.search-item.clearfix"));

                    // Create a list of vacancies
                    List<Vacancy> vacancies = new List<Vacancy>();

                    foreach (IWebElement vacancyElement in vacancyElements)
                    {
                        // Find the title element and extract the text
                        IWebElement titleElement = vacancyElement.FindElement(By.CssSelector("h2.job-title"));
                        string title = titleElement.Text;

                        // Find the company element and extract the text
                        IWebElement companyElement = vacancyElement.FindElement(By.CssSelector("span.job-company"));
                        string company = companyElement.Text;

                        // Find the location element and extract the text
                        IWebElement locationElement = vacancyElement.FindElement(By.CssSelector("span.job-location"));
                        string location = locationElement.Text;

                        // Find the keywords element and extract the text
                        IWebElement keywordsElement = vacancyElement.FindElement(By.CssSelector("span.job-keywords"));
                        string keywords = keywordsElement.Text;

                        // Find the link to the details page and extract the URL
                        IWebElement detailsLinkElement = vacancyElement.FindElement(By.TagName("a"));
                        string detailsUrl = detailsLinkElement.GetAttribute("href");

                        // Create a new vacancy object and add it to the list
                        Vacancy vacancy = new Vacancy
                        {
                            Title = title,
                            Company = company,
                            Location = location,
                            Keywords = keywords,
                            DetailsUrl = detailsUrl
                        };
                        vacancies.Add(vacancy);
                    }

                    // Print the vacancies
                    foreach (Vacancy vacancy in vacancies)
                    {
                        Console.WriteLine($"Title: {vacancy.Title}");
                        Console.WriteLine($"Company: {vacancy.Company}");
                        Console.WriteLine($"Location: {vacancy.Location}");
                        Console.WriteLine($"Keywords: {vacancy.Keywords}");
                        Console.WriteLine($"Details URL: {vacancy.DetailsUrl}");
                        Console.WriteLine();
                    }

                    // Write to csv
                    using (var writer = new StreamWriter("./RecentICTJobs.csv"))
                    {
                        // Write the header row
                        writer.WriteLine("Title,Company,Location,Keywords,DetailsURL");

                        // Write the data rows
                        foreach (Vacancy vacancy in vacancies)
                        {
                            writer.WriteLine($"{vacancy.Title},{vacancy.Company},{vacancy.Location},{vacancy.Keywords},{vacancy.DetailsUrl}");
                        }
                        writer.WriteLine("\n");
                    }

                    // write to JSON
                    using (var writer = new StreamWriter("./RecentICTJobs.json"))
                    {
                        // Serialize the data to JSON
                        string json = JsonConvert.SerializeObject(vacancies);

                        // Write the JSON to the file
                        writer.Write(json);
                    }

                }// Chromedriver
            }// Scrapingchoice else if
            else if (scrapingChoice == "crypto")
            {

                // Prompt the user to input a cryptocurrency
                Console.WriteLine("Enter a cryptocurrency: ");
                string crypto = Console.ReadLine();

                // Create a new ChromeDriver instance
                using (var driver = new ChromeDriver())
                {
                    // Navigate to CoinMarketCap
                    driver.Navigate().GoToUrl("https://coinmarketcap.com/");

                    IWebElement body = driver.FindElement(By.TagName("body"));
                    body.Click();

                    // Wait for the page to load
                    System.Threading.Thread.Sleep(1000);

                    // Search for the cryptocurrency on CoinMarketCap
                    IWebElement searchBox = driver.FindElement(By.ClassName("search-input-static"));
                    searchBox.Click();

                    // Wait for the page to load
                    System.Threading.Thread.Sleep(1000);

                    IWebElement searchBox2 = driver.FindElement(By.ClassName("desktop-input"));
                    searchBox2.SendKeys(crypto.Substring(0,3));
                    System.Threading.Thread.Sleep(1000);
                    searchBox2.SendKeys(crypto.Substring(3));

                    // Wait for the page to load
                    driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(10);

                    // Get the cryptocurrency's price, market cap, and volume from the last 24 hours
                    IWebElement priceElement = driver.FindElement(By.CssSelector("div.priceValue > span"));
                    string price = priceElement.Text;

                    IWebElement marketCapElement = driver.FindElement(By.CssSelector("div.statsValue"));
                    string marketCap = marketCapElement.Text;

                    IList<IWebElement> volumeElement = driver.FindElements(By.CssSelector("div.statsValue"));
                    IWebElement thirdElement = volumeElement.Skip(2).First();
                    string volume = thirdElement.Text;

                    // Print the cryptocurrency's information
                    Console.WriteLine($"Price: {price}");
                    Console.WriteLine($"Market cap: {marketCap}");
                    Console.WriteLine($"Volume (24h): {volume}");

                    // Write the cryptocurrency's information to a CSV file
                    using (StreamWriter sw = new StreamWriter("Crypto.csv"))
                    {
                        sw.WriteLine($"Cryptocurrency: {crypto}");
                        sw.WriteLine($"Price: {price}");
                        sw.WriteLine($"Market cap: {marketCap}");
                        sw.WriteLine($"Volume (24h): {volume}");
                    }

                    // Write the cryptocurrency's information to a JSON file
                    using (StreamWriter sw = new StreamWriter("Crypto.json"))
                    {
                        var data = new Dictionary<string, string>
                    {
                        { "Cryptocurrency", crypto },
                        { "Price", price },
                        { "Market cap", marketCap },
                        { "Volume (24h)", volume }
                    };
                        string json = JsonConvert.SerializeObject(data);
                        sw.WriteLine(json);
                    }
                }// Chromedriver
            }// Scrapingchoice else if
        }// Static void
    }// Class
}// Namespace

