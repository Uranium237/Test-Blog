using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Blog.Data;
using Blog.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;

namespace Blog.Controllers
{
    public class CommentsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public CommentsController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: Comments
        public async Task<IActionResult> Index()
        {
            return View(await _context.Comments.ToListAsync());
        }

        // GET: Comments/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var comment = await _context.Comments.SingleOrDefaultAsync(m => m.CommentId == id);
            if (comment == null)
            {
                return NotFound();
            }

            return View(comment);
        }

        // GET: Comments/Create
        [Authorize(Roles = "Admin,User")]
        public IActionResult Create(int PostId)
        {
            var com = new Comment();
            com.PostId = PostId;
            //ViewData["PostId"] = PostId;
            //int PostId = Request.Params["PostId"];
            return View(com);
        }

        // POST: Comments/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [Authorize(Roles = "Admin,User")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("CommentId,PostId,ReleaseDate,Text,Author,UserId")] Comment comment)
        {
            var user = await _userManager.GetUserAsync(HttpContext.User);
            var userId = user?.Id;
            var userName = user?.UserName;

            comment.UserId = userId;
            comment.Author = userName;


            if (ModelState.IsValid)
            {
                _context.Add(comment);
                await _context.SaveChangesAsync();
                return RedirectToAction("Details", "Posts", new { @Id = comment.PostId });
            }
            return View(comment);
        }

        // GET: Comments/Edit/5
        [Authorize(Roles = "Admin,User")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var comment = await _context.Comments.SingleOrDefaultAsync(m => m.CommentId == id);
            if (comment == null)
            {
                return NotFound();
            }
            return View(comment);
        }

        // POST: Comments/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [Authorize(Roles = "Admin,User")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("CommentId,PostId,ReleaseDate,Text,Author,UserId")] Comment comment)
        {
            var user = await _userManager.GetUserAsync(HttpContext.User);
            var userId = user?.Id;
            var userR = await _userManager.GetRolesAsync(user);

            if ( userR.All(u => u != "Admin"))
            {
                if (userId != comment.UserId)
                { return RedirectToAction("AccessDenied", "Account"); }
            }

                if (id != comment.CommentId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(comment);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CommentExists(comment.CommentId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction("Details", "Posts", new { @Id = comment.PostId });
            }
            return View(comment);
        }

        // GET: Comments/Delete/5
        [Authorize(Roles = "Admin,User")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var comment = await _context.Comments.SingleOrDefaultAsync(m => m.CommentId == id);
            if (comment == null)
            {
                return NotFound();
            }

            return View(comment);
        }

        // POST: Comments/Delete/5
        [HttpPost, ActionName("Delete")]
        [Authorize(Roles = "Admin,User")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var comment = await _context.Comments.SingleOrDefaultAsync(m => m.CommentId == id);

            var user = await _userManager.GetUserAsync(HttpContext.User);
            var userId = user?.Id;
            var userR = await _userManager.GetRolesAsync(user);

            if (userR.All(u => u != "Admin"))
            {
                if (userId != comment.UserId)
                { return RedirectToAction("AccessDenied", "Account"); }
            }
            _context.Comments.Remove(comment);
            await _context.SaveChangesAsync();
            return RedirectToAction("Details", "Posts", new { @Id = comment.PostId });
        }

        private bool CommentExists(int id)
        {
            return _context.Comments.Any(e => e.CommentId == id);
        }
    }
}
