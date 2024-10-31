using Azure.AI.OpenAI;
using OpenAI.Chat;
using System;
using System.ClientModel;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using VaderSharp2;
using static System.Net.Mime.MediaTypeNames;
using Fall2024_Assignment3_opmcmenaman.Models;


public class OpenAIService
{
    private readonly string _apiKey;
    private readonly string _apiEndpoint;
    private static ApiKeyCredential _apiCredential;
    private readonly SentimentIntensityAnalyzer _analyzer;

    public OpenAIService(string apiKey, string endpoint)
    {
        _apiKey = apiKey;
        _apiEndpoint = endpoint;
        _apiCredential = new ApiKeyCredential(_apiKey);
        _analyzer = new SentimentIntensityAnalyzer();
    }

    private ChatClient CreateClient()
    {
        var baseClient = new AzureOpenAIClient(new Uri(_apiEndpoint), _apiCredential);
        return baseClient.GetChatClient("gpt-35-turbo");
    }

    public async Task<List<string>> GenerateReviewsAsync(string title, string year)
    {
        var client = CreateClient();

        string[] personas = { "is harsh", "loves romance", "loves comedy", "loves thrillers", "loves fantasy",
                          "is nostalgic", "is a critic of visuals", "is a fan of soundtracks", "prefers indie films", "is a big blockbuster fan" };

        var messages = new ChatMessage[]
        {
            new SystemChatMessage($"You represent a group of {personas.Length} film critics who have the following personalities: {string.Join(",", personas)}. When you receive a question, respond as each member of the group with each response separated by a '|', but don't indicate which member you are"),
            new UserChatMessage($"How would you rate the movie {title} released in {year} out of 10 in 150 words or less?")
        };

        var result = await client.CompleteChatAsync(messages);
        string[] reviews = result.Value.Content[0].Text.Split('|').Select(s => s.Trim()).ToArray();
        return reviews.ToList();
    }

    public async Task<List<Tweet>> GenerateTweetsAsync(string actorName)
    {

        var client = CreateClient();
        var messages = new ChatMessage[]
        {
        new SystemChatMessage("You are Twitter. Only respond with a valid JSON array of tweet objects, each containing 'username' and 'tweet' keys, and starting with '['. Do not add any additional text or explanations."),
        new UserChatMessage($"Generate 20 tweets from various users about the actor {actorName}.")
        };

        ClientResult<ChatCompletion> result = await client.CompleteChatAsync(messages);
        string tweetsJsonString = result.Value.Content.FirstOrDefault()?.Text ?? "[]";

        var tweetsList = new List<Tweet>();
        var analyzer = new SentimentIntensityAnalyzer();

        try
        {
            var jsonArray = JsonNode.Parse(tweetsJsonString)?.AsArray();
            foreach (var json in jsonArray)
            {
                var tweetText = json["tweet"]?.ToString() ?? "";
                var sentiment = analyzer.PolarityScores(tweetText).Compound;
                tweetsList.Add(new Tweet
                {
                    Username = json["username"]?.ToString() ?? "",
                    Text = tweetText,
                    SentimentScore = sentiment
                });
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error parsing tweets JSON: {ex.Message}. Raw JSON: {tweetsJsonString}");
        }

        return tweetsList;
    }






    public SentimentAnalysisResults AnalyzeSentiment(string text)
    {
        return _analyzer.PolarityScores(text);
    }
}
