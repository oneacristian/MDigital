using MDigital.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MDigital.Controllers
{
    [ApiController]
    [Route("api/articles")]
    public class ArticlesController : ControllerBase
    {
        private readonly MDigitalDBContext _context;

        public ArticlesController(MDigitalDBContext context)
        {
            _context = context;
        }

        // GET: api/articles
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Article>>> GetArticles()
        {
            try
            {
                var articles = await _context.Articles.ToListAsync();
                return Ok(articles);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred while retrieving articles: {ex.Message}");
            }
        }

        // GET: api/articles/1
        [HttpGet("{id}")]
        public async Task<ActionResult<Article>> GetArticle(long id)
        {
            try
            {
                var article = await _context.Articles.FindAsync(id);

                if (article == null)
                {
                    return NotFound();
                }

                return Ok(article);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred while retrieving the Article with ID {id}: {ex.Message}");
            }
        }

        // POST: api/articles
        [HttpPost]
        public async Task<ActionResult<Article>> CreateArticle(Article article)
        {
            try
            {
                _context.Articles.Add(article);
                await _context.SaveChangesAsync();

                return CreatedAtAction(nameof(GetArticle), new { id = article.Id }, article);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred while creating the Article: {ex.Message}");
            }
        }

        // PUT: api/articles/1
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateArticle(long id, Article article)
        {
            try
            {
                if (id != article.Id)
                {
                    return BadRequest();
                }

                _context.Entry(article).State = EntityState.Modified;

                try
                {
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException uce)
                {
                    if (!ArticleExists(id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw new Exception(uce.Message);
                    }
                }

                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred while updating the Article: {ex.Message}");
            }
        }

        // DELETE: api/articles/1
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteArticle(long id)
        {
            try
            {
                var article = await _context.Articles.FindAsync(id);
                if (article == null)
                {
                    return NotFound();
                }

                _context.Articles.Remove(article);
                await _context.SaveChangesAsync();

                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred while deleting the Article: {ex.Message}");
            }
        }

        private bool ArticleExists(long id)
        {
            return _context.Articles.Any(a => a.Id == id);
        }
    }
}
