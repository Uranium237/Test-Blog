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
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Net.Http.Headers;
using System.IO;

namespace Blog.Controllers
{
    public class PostsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        private IHostingEnvironment _environment;

        public PostsController(
            ApplicationDbContext context,
            UserManager<ApplicationUser> userManager,
            IHostingEnvironment environment)
        {
            _context = context;
            _userManager = userManager;
            _environment = environment; 
        }

        public IActionResult UploadFiles()
        {
            return View();
        }

        // GET: Posts
        public async Task<IActionResult> Index()
        {
            return View(await _context.Posts.ToListAsync());
        }

        // GET: Posts/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var post = await _context.Posts.Include(p => p.Comments).Include(k => k.Images).SingleOrDefaultAsync(m => m.PostId == id);
            if (post == null)
            {
                return NotFound();
            }

            return View(post);
        }

        // GET: Posts/Create
        [Authorize(Roles = "Admin")]
        public IActionResult Create()
        {
            return View();
        }

        // POST: Posts/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("PostId,ReleaseDate,Text,Title,Author,UserId")] Post post)
        {
            var user = await _userManager.GetUserAsync(HttpContext.User);
            var userId = user?.Id;
            var userName = user?.UserName;

            post.UserId = userId;
            post.Author = userName;


            if (ModelState.IsValid)
            {

                var files = HttpContext.Request.Form.Files;
                foreach (var Image in files)
                {
                    if (Image != null && Image.Length > 0)
                    {

                        var file = Image;
                        var uploads = Path.Combine(_environment.WebRootPath, "");

                        if (file.Length > 0)
                        {
                            var fileName = ContentDispositionHeaderValue.Parse(file.ContentDisposition).FileName.Trim('"');

                            //System.Console.WriteLine(fileName);
                            using (var fileStream = new FileStream(Path.Combine(uploads, file.FileName), FileMode.Create))
                            {
                                var image = new Image();
                                
                                await file.CopyToAsync(fileStream);
                                image.ImageName = file.FileName;
                                post.Images = new List<Image>();
                                post.Images.Add(image);
                            }


                        }
                    }
                }

                _context.Add(post);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(post);
        }

    


        // GET: Posts/Edit/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int? id)
        {
            var user = await _userManager.GetUserAsync(HttpContext.User);
            var userId = user?.Id;
            var userR = await _userManager.GetRolesAsync(user);

            var post = await _context.Posts.Include(k => k.Images).SingleOrDefaultAsync(m => m.PostId == id);

            if (userR.All(u => u != "Admin"))
            {
                if (userId != post.UserId)
                { return RedirectToAction("AccessDenied", "Account"); }
            }

            if (id == null)
            {
                return NotFound();
            }

            
            if (post == null)
            {
                return NotFound();
            }
            return View(post);
        }

        // POST: Posts/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int id, [Bind("PostId,ReleaseDate,Text,Title,Author,UserId")] Post post)
        {

            //var post1 = await post.Include(k => k.Images);
            var user = await _userManager.GetUserAsync(HttpContext.User);
            var userId = user?.Id;
            var userR = await _userManager.GetRolesAsync(user);

            if (userR.All(u => u != "Admin"))
            {
                if (userId != post.UserId)
                { return RedirectToAction("AccessDenied", "Account"); }
            }

            if (id != post.PostId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var files = HttpContext.Request.Form.Files;
                    foreach (var Image in files)
                    {
                        if (Image != null && Image.Length > 0)
                        {

                            var file = Image;
                            var uploads = Path.Combine(_environment.WebRootPath, "");

                            if (file.Length > 0)
                            {
                                var fileName = ContentDispositionHeaderValue.Parse(file.ContentDisposition).FileName.Trim('"');

                                //System.Console.WriteLine(fileName);
                                using (var fileStream = new FileStream(Path.Combine(uploads, file.FileName), FileMode.Create))
                                {
                                    var image = new Image();

                                    await file.CopyToAsync(fileStream);
                                    image.ImageName = file.FileName;
                                    post.Images = new List<Image>();
                                    post.Images.Add(image);
                                }


                            }
                        }
                    }


                    _context.Update(post);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PostExists(post.PostId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction("Index");
            }
            return View(post);
        }

        // GET: Posts/Delete/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            var user = await _userManager.GetUserAsync(HttpContext.User);
            var userId = user?.Id;
            var userR = await _userManager.GetRolesAsync(user);

            var post = await _context.Posts.Include(k => k.Images).SingleOrDefaultAsync(m => m.PostId == id);

            if (userR.All(u => u != "Admin"))
            {
                if (userId != post.UserId)
                { return RedirectToAction("AccessDenied", "Account"); }
            }


            if (id == null)
            {
                return NotFound();
            }

            //var post = await _context.Posts.SingleOrDefaultAsync(m => m.PostId == id);
            if (post == null)
            {
                return NotFound();
            }

            return View(post);
        }

        // POST: Posts/Delete/5
        [HttpPost, ActionName("Delete")]
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var user = await _userManager.GetUserAsync(HttpContext.User);
            var userId = user?.Id;
            var userR = await _userManager.GetRolesAsync(user);

            var post = await _context.Posts.Include(k => k.Images).SingleOrDefaultAsync(m => m.PostId == id);

            if (userR.All(u => u != "Admin"))
            {
                if (userId != post.UserId)
                { return RedirectToAction("AccessDenied", "Account"); }
            }

            _context.Posts.Remove(post);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        //[HttpPost]
        [Authorize(Roles = "Admin")]
        //[ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteImage(int id)
        {
            var im = await _context.Image.SingleOrDefaultAsync(m => m.ImageId == id);
            var pid = im.PostId;
            var post = await _context.Posts.SingleOrDefaultAsync(m => m.PostId == pid);
            _context.Image.Remove(im);
            var uploads = Path.Combine(_environment.WebRootPath, im.ImageName);
            System.IO.File.Delete(uploads);
            await _context.SaveChangesAsync();
            //return View(post);
            return RedirectToAction("Edit",new { id = pid });

        }



        private bool PostExists(int id)
        {
            return _context.Posts.Any(e => e.PostId == id);
        }
    }
}
