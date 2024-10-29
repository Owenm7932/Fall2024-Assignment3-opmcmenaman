namespace Fall2024_Assignment3_opmcmenaman.Models;

public class MovieDetailsViewModel
{
    public Movie Movie { get; set; }
    public List<Actor> Actors { get; set; } // Associated actors
    public List<Actor> AllActors { get; set; } // All available actors for dropdown
    public List<string> AIReviews { get; set; } // Reviews with sentiment analysis
    public double OverallSentiment { get; set; }
}

