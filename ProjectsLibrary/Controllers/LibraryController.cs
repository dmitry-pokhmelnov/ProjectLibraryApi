using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.Extensions.Logging;
using ProjectsLibrary.Models;

namespace ProjectsLibrary.Controllers
{
    
    [ApiController]
    [Route("[controller]")]
    public class LibraryController : ControllerBase
    {
        private readonly ILogger<LibraryController> _logger;

        public LibraryController(ILogger<LibraryController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        public List<ProjectResponse> Get()
        {
            using (LibraryContext libraryContext = new LibraryContext())
            {
                return libraryContext.Projects
                    .Select(c => new ProjectResponse { Id = c.Id, ProjectName = c.ProjectName })
                    .ToList();
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ProjectResponse>> Get(int id)
        {
            using (LibraryContext libraryContext = new LibraryContext())
            {
                var result = await libraryContext.Projects.FindAsync(id);
                
                if (result == null)
                {
                    return NotFound();
                }

                return new ProjectResponse { Id = result.Id, ProjectName = result.ProjectName };
            }
        }

        [HttpPost]
        public async Task<ActionResult<string>> Post([FromBody] string projectName)
        {
            using (LibraryContext libraryContext = new LibraryContext())
            {
                List<Geometry> geometries = new List<Geometry>();
                List<Models.Attribute> attributes = new List<Models.Attribute>();

                //TODO: slow perfomance, need to move somewhere
                for (int i = 0; i < 100000; i++)
                {
                    geometries.Add(new Geometry { GeometryName = $"Geom{i}"});
                    attributes.Add(new Models.Attribute { AttributeName = $"Attr{i}" });
                }

                libraryContext.Projects.Add(
                    new Project 
                    {
                        ProjectName = projectName,
                        Geometries = geometries,
                        Attributes = attributes
                    }
                );
                await libraryContext.SaveChangesAsync();
                return $"Project: {projectName} added.";
            }
        }

        [HttpDelete]
        public async Task<ActionResult<string>> Delete(int id)
        {
            using (LibraryContext libraryContext = new LibraryContext())
            {
                libraryContext.Remove(await libraryContext.Projects.FindAsync(id));
                await libraryContext.SaveChangesAsync();
                return $"Project with id: {id} deleted.";
            }
        }

        [HttpPut]
        public async Task<ActionResult<string>> Update(int id, string name)
        {
            using (LibraryContext libraryContext = new LibraryContext())
            {
                var project = await libraryContext.Projects.FindAsync(id);
                project.ProjectName = name;
                libraryContext.Update(project);
                await libraryContext.SaveChangesAsync();
                return $"Successfully updated name of project id: {id}";
            }
        }
    }
}
