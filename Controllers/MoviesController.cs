using Microsoft.AspNetCore.Mvc;
using Movie_API.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Movie_API.Controllers
{

    [ApiController]
    public class MoviesController : ControllerBase
    {

        [HttpGet]
        [Route("[controller]/[action]")]
        public ActionResult<IEnumerable<Stat>> Stats()
        {
            var metadataCsv = new List<Metadata>();

            var lines = System.IO.File.ReadAllLines(@"metadata.csv");

            foreach (string line in lines.Skip(1))
            {
                var lineSplit = Regex.Split(line, ",(?=(?:[^\"]*\"[^\"]*\")*(?![^\"]*\"))");
                metadataCsv.Add(new Metadata { Id = int.Parse(lineSplit[0]), MovieId = int.Parse(lineSplit[1]), Title = lineSplit[2].ToString(), Language = lineSplit[3].ToString(), Duration = lineSplit[4].ToString(), ReleaseYear = int.Parse(lineSplit[5]) });
            }

            lines = System.IO.File.ReadAllLines(@"stats.csv");

            var statsCsv = lines.Skip(1)
                        .Select(l => Regex.Split(l, ",(?=(?:[^\"]*\"[^\"]*\")*(?![^\"]*\"))"))
                        .GroupBy(g => g[0])
                        .Select(g => new Stat
                        {
                            MovieId = int.Parse(g.First()[0]),
                            AverageWatchDurationS = Math.Round(TimeSpan.FromMilliseconds(g.Average(s => long.Parse(s[1]))).TotalSeconds),
                            Watches = g.Count(),
                            Title = metadataCsv.FirstOrDefault(md => md.MovieId == int.Parse(g.First()[0]))?.Title,
                            ReleaseYear = metadataCsv.FirstOrDefault(md => md.MovieId == int.Parse(g.First()[0]))?.ReleaseYear ?? 0
                        });

            return Ok(statsCsv.Where(s => !string.IsNullOrEmpty(s.Title)).OrderByDescending(s => s.Watches).ThenByDescending(s => s.ReleaseYear));
        }
    }
}
