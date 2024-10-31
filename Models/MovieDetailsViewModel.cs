namespace Fall2024_Assignment3_opmcmenaman.Models;

public class MovieDetailsViewModel
{
    public Movie Movie { get; set; }
    public List<Actor> Actors { get; set; } 
    public List<Actor> AllActors { get; set; } 
    public List<string> AIReviews { get; set; }
    public double OverallSentiment { get; set; }
}

