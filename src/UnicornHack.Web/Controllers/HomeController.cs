using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnicornHack.Data;
using UnicornHack.Hubs;
using UnicornHack.Utils;
using UnicornHack.Models.GameState;
using UnicornHack.Models.GameViewModels;
using UnicornHack.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking.Internal;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace UnicornHack.Controllers
{
    public class HomeController : Controller
    {
        private readonly GameDbContext _dbContext;
        private readonly IHubContext<GameHub, IGameClient> _hubContext;
        private readonly GameServices _gameServices;

        public HomeController(GameDbContext dbContext, IHubContext<GameHub, IGameClient> hubContext, GameServices gameServices)
        {
            _dbContext = dbContext;
            _hubContext = hubContext;
            _gameServices = gameServices;
        }

        public IActionResult Index()
        {
            return View(new Character());
        }

        public IActionResult Contact()
        {
            return View();
        }

        //
        // GET: /Home/Game?Name
        [HttpGet]
        [ResponseCache(Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Game(Character model)
        {
            return View(FindOrCreateCharacter(model.Name));
        }

        //
        // POST: /Home/PerformAction
        [HttpPost]
        [ResponseCache(Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult PerformAction(string name, string action, string target)
        {
            var character = FindOrCreateCharacter(name);
            if (character.Game.ActingActor == null)
            {
                if (!character.Game.PlayerCharacters.Any(pc => pc.IsAlive))
                {
                    Delete(character.Game);
                    _dbContext.SaveChanges();
                    character = FindOrCreateCharacter(name);
                }
            }

            character.NextAction = action;
            character.NextActionTarget = string.IsNullOrEmpty(target) ? 0 : Int32.Parse(target);

            if (character.Game.ActingActor == character)
            {
                character.Game.Turn();
            }

            var level = character.Level;

            if (!character.IsAlive)
            {
                character.Act();
            }

            _dbContext.SaveChanges();

            // TODO: #6067
            if (!level.Actors.Contains(character))
            {
                character.Level = level;
                level.Actors.Add(character);
                character.Game.Actors.Add(character);
            }

            return PartialView(nameof(Game), character);
        }

        public IActionResult Error()
        {
            return View();
        }

        private PlayerCharacter FindOrCreateCharacter(string name)
        {
            var character = FindCharacter(name);
            if (character == null)
            {
                var game = new Game
                {
                    CurrentTurn = -1,
                    RandomSeed = Environment.TickCount,
                    Services = _gameServices
                };
                character = PlayerCharacter.CreateCharacter(game, name);
                _dbContext.Characters.Add(character);

                var items = character.Level.Items.ToList();
                var downStairs = character.Level.DownStairs.ToList();
                var upStairs = character.Level.UpStairs.ToList();
                _dbContext.SaveChanges();

                game.ActingActor = character;
                _dbContext.SaveChanges();

                // TODO: #6067
                character.Level.Items.AddRange(items);
                character.Level.DownStairs.AddRange(downStairs);
                character.Level.UpStairs.AddRange(upStairs);
            }

            return character;
        }

        private PlayerCharacter FindCharacter(string name)
        {
            var character = _dbContext.Characters
                .Include(c => c.Level.Game)
                .FirstOrDefault(c => c.GivenName.Equals(name, StringComparison.OrdinalIgnoreCase));

            if (character == null)
            {
                return null;
            }

            _dbContext.Characters
                .Include(a => a.Inventory)
                .Include(a => a.Log)
                .Include(c => c.Level.DownStairs)
                .Include(c => c.Level.UpStairs)
                .Include(c => c.Level.Game)
                .Where(c => c.Game == character.Game).ToList();

            var currentLevel = character.Level;
            var levelsToLoad = new List<int> { currentLevel.Id };

            // Preload adjacent levels
            var upStairs = character.Level.UpStairs.SingleOrDefault(s =>
                s.DownLevelX == character.LevelX
                && s.DownLevelY == character.LevelY);
            if (upStairs != null)
            {
                if (upStairs.UpId.HasValue)
                {
                    levelsToLoad.Add(upStairs.UpId.Value);
                }
                else
                {
                    Level.CreateUpLevel(upStairs);
                }
            }

            var downStairs = character.Level.DownStairs.SingleOrDefault(s =>
                s.UpLevelX == character.LevelX
                && s.UpLevelY == character.LevelY);
            if (downStairs != null)
            {
                if (downStairs.DownId.HasValue)
                {
                    levelsToLoad.Add(downStairs.DownId.Value);
                }
                else
                {
                    Level.CreateDownLevel(downStairs);
                }
            }

            var levels = _dbContext.Levels
                .Include(l => l.Actors).ThenInclude(a => a.Inventory)
                .Include(l => l.Items)
                .Include(l => l.Game)
                .Where(l => levelsToLoad.Contains(l.Id)).ToList();

            character.Game.Services = _gameServices;

            return character;
        }

        private void Delete(Game game)
        {
            game = _dbContext.Games
                .Include(g => g.Actors)
                .Include(g => g.Items)
                .Include(g => g.Levels)
                .Include(g => g.Stairs)
                .Single(g => g.Id == game.Id);

            foreach (var playerCharacter in game.PlayerCharacters)
            {
                foreach (var log in playerCharacter.Log.ToList())
                {
                    _dbContext.LogEntries.Remove(log);
                }
            }
            foreach (var item in game.Items.ToList())
            {
                _dbContext.Items.Remove(item);
            }
            foreach (var actor in game.Actors.ToList())
            {
                _dbContext.Actors.Remove(actor);
            }
            foreach (var stairs in game.Stairs.ToList())
            {
                _dbContext.Stairs.Remove(stairs);
            }
            foreach (var level in game.Levels.ToList())
            {
                _dbContext.Levels.Remove(level);
            }

            _dbContext.Games.Remove(game);
            _dbContext.SaveChanges();
        }
    }
}