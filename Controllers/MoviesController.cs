using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Fall2024_Assignment3_opmcmenaman.Data;
using Fall2024_Assignment3_opmcmenaman.Models;

namespace Fall2024_Assignment3_opmcmenaman.Controllers
{
    public class MoviesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly OpenAIService _openAIService;

        public MoviesController(ApplicationDbContext context, OpenAIService openAIService)
        {
            _context = context;
            _openAIService = openAIService;
        }

        // GET: Movies
        public async Task<IActionResult> Index()
        {
            return View(await _context.Movies.ToListAsync());
        }

        // GET: Movies/Details/5
        public async Task<IActionResult> Details(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var movie = await _context.Movies
                .Include(m => m.MovieActors)
                    .ThenInclude(ma => ma.Actor)
                .FirstOrDefaultAsync(m => m.MovieId == id);

            if (movie == null)
            {
                return NotFound();
            }

            var reviews = await _openAIService.GenerateReviewsAsync(movie.Title, movie.YearOfRelease);

            double sentimentSum = 0;
            int reviewCount = 0;
            var reviewsWithSentiments = new List<string>();

            foreach (var review in reviews)
            {
                var sentiment = _openAIService.AnalyzeSentiment(review);
                sentimentSum += sentiment.Compound;
                reviewCount++;
                reviewsWithSentiments.Add($"{review} (Sentiment Score: {sentiment.Compound:0.###})");
            }

            double overallSentimentAverage = reviewCount > 0 ? sentimentSum / reviewCount : 0;

            var allActors = await _context.Actors.ToListAsync();

            var viewModel = new MovieDetailsViewModel
            {
                Movie = movie,
                AIReviews = reviewsWithSentiments,
                OverallSentiment = overallSentimentAverage,
                Actors = movie.MovieActors.Select(ma => ma.Actor).ToList(),
                AllActors = allActors
            };

            return View(viewModel);
        }

        public async Task<IActionResult> ManageActors(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var movie = await _context.Movies
                .Include(m => m.MovieActors)
                    .ThenInclude(ma => ma.Actor)
                .FirstOrDefaultAsync(m => m.MovieId == id);

            if (movie == null)
            {
                return NotFound();
            }

            var assignedActorIds = movie.MovieActors.Select(ma => ma.ActorId).ToList();

            var availableActors = await _context.Actors
                .Where(a => !assignedActorIds.Contains(a.ActorId))
                .ToListAsync();

            var viewModel = new ManageMovieActorsViewModel
            {
                Movie = movie,
                AssignedActors = movie.MovieActors.Select(ma => ma.Actor).ToList(),
                AvailableActors = availableActors
            };

            return View(viewModel);
        }



        [HttpPost]
        public async Task<IActionResult> AddActor(string movieId, string actorId)
        {
            if (string.IsNullOrEmpty(movieId) || string.IsNullOrEmpty(actorId))
            {
                return BadRequest("MovieId and ActorId must be provided.");
            }

            if (!_context.MovieActors.Any(ma => ma.MovieId == movieId && ma.ActorId == actorId))
            {
                var movieActor = new MovieActor
                {
                    MovieId = movieId,
                    ActorId = actorId
                };
                _context.MovieActors.Add(movieActor);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction("ManageActors", new { id = movieId });
        }

        [HttpPost]
        public async Task<IActionResult> RemoveActor(string movieId, string actorId)
        {
            if (string.IsNullOrEmpty(movieId) || string.IsNullOrEmpty(actorId))
            {
                return BadRequest("MovieId and ActorId must be provided.");
            }

            var movieActor = await _context.MovieActors
                .FirstOrDefaultAsync(ma => ma.MovieId == movieId && ma.ActorId == actorId);

            if (movieActor != null)
            {
                _context.MovieActors.Remove(movieActor);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction("ManageActors", new { id = movieId });
        }








        // GET: Movies/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Movies/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("MovieId,Title,IMDBLink,Genre,YearOfRelease,PosterURL")] Movie movie)
        {
            if (ModelState.IsValid)
            {
                _context.Add(movie);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(movie);
        }

        // GET: Movies/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var movie = await _context.Movies.FindAsync(id);
            if (movie == null)
            {
                return NotFound();
            }
            return View(movie);
        }

        // POST: Movies/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("MovieId,Title,IMDBLink,Genre,YearOfRelease,PosterURL")] Movie movie)
        {
            if (id != movie.MovieId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(movie);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!MovieExists(movie.MovieId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(movie);
        }

        // GET: Movies/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var movie = await _context.Movies
                .FirstOrDefaultAsync(m => m.MovieId == id);
            if (movie == null)
            {
                return NotFound();
            }

            return View(movie);
        }

        // POST: Movies/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var movie = await _context.Movies.FindAsync(id);
            if (movie != null)
            {
                _context.Movies.Remove(movie);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool MovieExists(string id)
        {
            return _context.Movies.Any(e => e.MovieId == id);
        }
    }
}
