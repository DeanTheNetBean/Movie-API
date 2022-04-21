using Microsoft.AspNetCore.Mvc;
using Movie_API.Models;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Movie_API.Controllers
{
    
    [ApiController]
    public class MetadataController : ControllerBase
    {
        [HttpPost]
        [Route("[controller]")]
        public ActionResult<string> Post(Metadata metadata)
        {

            List<Metadata> database = new List<Metadata>();
            database.Add(metadata);

            return Ok("Saved to a database");
        }


        [HttpGet]
        [Route("[controller]/:{movieId}")]
        public ActionResult<IEnumerable<Metadata>> Get(int movieId)
        {
            var metadataCsv = new List<Metadata>();

            var lines = System.IO.File.ReadAllLines(@"metadata.csv");

            foreach (string line in lines.Skip(1))
            {
                var lineSplit = Regex.Split(line, ",(?=(?:[^\"]*\"[^\"]*\")*(?![^\"]*\"))");
                metadataCsv.Add(new Metadata { Id = int.Parse(lineSplit[0]), MovieId = int.Parse(lineSplit[1]), Title = lineSplit[2].ToString(), Language = lineSplit[3].ToString(), Duration = lineSplit[4].ToString(), ReleaseYear = int.Parse(lineSplit[5]) });
            }

            var filteredList = metadataCsv
                .Where(md => md.MovieId == movieId)
                .GroupBy(g => g.Language)
                .Select(g => g.OrderByDescending(md => md.Id).Last())
                .Where(md => !string.IsNullOrEmpty(md.Title) && !string.IsNullOrEmpty(md.Duration) && !string.IsNullOrEmpty(md.Language) && md.ReleaseYear != 0)
                .OrderBy(md => md.Language);

            if (filteredList.Count() < 1)
                return NotFound();

            return Ok(filteredList);
        }
    }
}
