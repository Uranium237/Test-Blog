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
using System.Text;

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

        [HttpPost]
        public async Task<IActionResult> Index(string categoryName, string searchString)
        {
            IQueryable<string> categoryQuery = from m in _context.Categories
                                               orderby m.CategoryName
                                               select m.CategoryName;


            var tagsL = from m in _context.Tags
                       select m;
            //var post = new Post();
            var postms = new List<PostModel>();

            var postmv = new PostModelView();

            if (!String.IsNullOrEmpty(categoryName))
            {
                var category = await _context.Categories.Include(k => k.Posts).SingleOrDefaultAsync(m => m.CategoryName == categoryName);
                var posts = category.Posts;

                foreach (var post in posts)
                {
                    var postmodel = new PostModel
                    {
                        Author = post.Author,
                        PostId = post.PostId,
                        UserId = post.UserId,
                        Text = post.Text,
                        Title = post.Title,
                        ReleaseDate = post.ReleaseDate

                    };
                    //var category = await _context.Categories.SingleOrDefaultAsync(m => m.CategoryId == post.CategoryId);
                    postmodel.CategoryName = category.CategoryName;
                    //StringBuilder sb = new StringBuilder();
                    var tags = await _context.PostTags.Where(m => m.PostId == postmodel.PostId).Select(pt => pt.Tag.TagName).ToArrayAsync();
                    ////var tagS = await _context.Tags.ToListAsync();
                    //for (int i = 0; i < tags.Count(); i++)
                    //{
                    //    var taG = tags[i];
                    //    //var tAg = tagS.FirstOrDefault(t => t.TagId == taG.TagId);
                    //    sb.Append(taG.TagName);
                    //    if (i != tags.Count() - 1)
                    //    {
                    //        sb.Append(",");
                    //    }
                    //}
                    postmodel.TagStr = String.Join(",", tags);//sb.ToString();
                    //postmodel.CategoryNames = new SelectList(await categoryQuery.Distinct().ToListAsync());
                    postms.Add(postmodel);
                }
                
                postmv.CategoryNames = new SelectList(await categoryQuery.Distinct().ToListAsync());
                postmv.PostModels = postms;

            }

            if (!String.IsNullOrEmpty(searchString))
            {
                tagsL = tagsL.Where(s => s.TagName.Contains(searchString));
                //var tagsL = tags.ToList();

                foreach (var tag in tagsL)
                {
                    var tagd = await _context.PostTags.ToListAsync();
                    if (tagd != null)
                    {
                        foreach (var tago in tagd.Where(m => m.TagId == tag.TagId))
                        {
                            var post = await _context.Posts.SingleOrDefaultAsync(m => m.PostId == tago.PostId);

                        
                            var postmodel = new PostModel
                            {
                                Author = post.Author,
                                PostId = post.PostId,
                                UserId = post.UserId,
                                Text = post.Text,
                                Title = post.Title,
                                ReleaseDate = post.ReleaseDate

                            };
                            var category = await _context.Categories.SingleOrDefaultAsync(m => m.CategoryId == post.CategoryId);
                            postmodel.CategoryName = category.CategoryName;
                            //StringBuilder sb = new StringBuilder();
                            var tags = await _context.PostTags.Where(m => m.PostId == postmodel.PostId).Select(pt => pt.Tag.TagName).ToArrayAsync();
                            ////var tagS = await _context.Tags.ToListAsync();
                            //for (int i = 0; i < tags.Count(); i++)
                            //{
                            //    var taG = tags[i];
                            //    //var tAg = tagS.FirstOrDefault(t => t.TagId == taG.TagId);
                            //    sb.Append(taG.TagName);
                            //    if (i != tags.Count() - 1)
                            //    {
                            //        sb.Append(",");
                            //    }
                            //}
                            postmodel.TagStr = String.Join(",", tags);//sb.ToString();

                            //postmodel.CategoryNames = new SelectList(await categoryQuery.Distinct().ToListAsync());

                            postms.Add(postmodel);
                        }
                        
                    }
                    else { break; }
                }
                //var postmv = new PostModelView();
                postmv.CategoryNames = new SelectList(await categoryQuery.Distinct().ToListAsync());
                postmv.PostModels = postms;
            }


            return View(postmv);
        }

        // GET: Posts
        public async Task<IActionResult> Index()
        {
            IQueryable<string> categoryQuery = from m in _context.Categories
                                               orderby m.CategoryName
                                               select m.CategoryName;

            var posts = from m in _context.Posts
                        select m;
            var postms = new List<PostModel>();

            var postmv = new PostModelView();

            foreach (var post in posts)
            {
                var postmodel = new PostModel
                {
                    Author = post.Author,
                    PostId = post.PostId,
                    UserId = post.UserId,
                    Text = post.Text,
                    Title = post.Title,
                    ReleaseDate = post.ReleaseDate
                    
                };
                var category = await _context.Categories.SingleOrDefaultAsync(m => m.CategoryId == post.CategoryId);
                postmodel.CategoryName = category.CategoryName;
                //StringBuilder sb = new StringBuilder();
                var tags = await _context.PostTags.Where(m => m.PostId == postmodel.PostId).Select(pt => pt.Tag.TagName).ToArrayAsync();
                ////var tagS = await _context.Tags.ToListAsync();
                //for (int i = 0; i < tags.Count(); i++)
                //{
                //    var taG = tags[i];
                //    //var tAg = tagS.FirstOrDefault(t => t.TagId == taG.TagId);
                //    sb.Append(taG.TagName);
                //    if (i != tags.Count() - 1)
                //    {
                //        sb.Append(",");
                //    }
                //}
                postmodel.TagStr = String.Join(",", tags);//sb.ToString();

                postms.Add(postmodel);
            }

            postmv.CategoryNames = new SelectList(await categoryQuery.Distinct().ToListAsync());
            postmv.PostModels = postms;

            return View(postmv);
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

            var postmodel = new PostModel
            {
                Author = post.Author,
                PostId = post.PostId,
                UserId = post.UserId,
                Text = post.Text,
                Title = post.Title,
                ReleaseDate = post.ReleaseDate
            };
            postmodel.Comments = new List<Comment>();
            postmodel.Comments = post.Comments;
            postmodel.Images = new List<Image>();
            postmodel.Images = post.Images;
            var category = await _context.Categories.SingleOrDefaultAsync(m => m.CategoryId == post.CategoryId);
            postmodel.CategoryName = category.CategoryName;
            //StringBuilder sb = new StringBuilder();
            var tags = await _context.PostTags.Where(m => m.PostId == postmodel.PostId).Select(pt => pt.Tag.TagName).ToArrayAsync();
            ////var tagS = await _context.Tags.ToListAsync();
            //for (int i = 0; i < tags.Count(); i++)
            //{
            //    var taG = tags[i];
            //    //var tAg = tagS.FirstOrDefault(t => t.TagId == taG.TagId);
            //    sb.Append(taG.TagName);
            //    if (i != tags.Count() - 1)
            //    {
            //        sb.Append(",");
            //    }
            //}
            postmodel.TagStr = String.Join(",", tags);//sb.ToString();

            return View(postmodel);
        }

        // GET: Posts/Create
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create()
        {
            IQueryable<string> categoryQuery = from m in _context.Categories
                                               orderby m.CategoryName
                                               select m.CategoryName;
            var postmodel = new PostModel();
            postmodel.CategoryNames = new SelectList(await categoryQuery.Distinct().ToListAsync());
            return View(postmodel);
        }

        // POST: Posts/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("PostId,ReleaseDate,Text,Title,TagStr,Author,UserId,CategoryName,CategoryNames")] PostModel postmodel)
        {
            var user = await _userManager.GetUserAsync(HttpContext.User);
            var userId = user?.Id;
            var userName = user?.UserName;

            var post = new Post();
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

                post.Title = postmodel.Title;
                post.Text = postmodel.Text;
                //post.PostId = postmodel.PostId;
                post.ReleaseDate = postmodel.ReleaseDate;
                //var tags = new List<Tag>();
                //post.Tags = new List<Tag>();

                IQueryable<string> categoryQuery = from m in _context.Categories
                                                   orderby m.CategoryName
                                                   select m.CategoryName;
                postmodel.CategoryNames = new SelectList(await categoryQuery.Distinct().ToListAsync());

                var category = await _context.Categories.SingleOrDefaultAsync(m => m.CategoryName == postmodel.CategoryName);
                post.CategoryId = category.CategoryId;

                Char delimiter = ',';
                var Atags = postmodel.TagStr.Split(delimiter).ToList();
                var tags = await _context.Tags.ToListAsync();
                
                foreach (var str in Atags)
                {
                    var tag = new Tag();
                    if (!tags.Exists(x => x.TagName == str))
                    {
                        tag.TagName = str;
                        _context.Add(tag);
                    }
                    
                }
                //post.Tags = tags;
                _context.Add(post);
                await _context.SaveChangesAsync();
                
                foreach (var str in Atags)
                {

                    var tag = tags.FirstOrDefault(t => t.TagName == str);
                    var taG = new PostTag();
                    taG.TagId = tag.TagId;
                    taG.PostId = post.PostId;
                    _context.Add(taG);
                }
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

            var postmodel = new PostModel
            {
                Author = post.Author,
                PostId = post.PostId,
                UserId = post.UserId,
                Text = post.Text,
                Title = post.Title,
                ReleaseDate = post.ReleaseDate
            };
            postmodel.Images = new List<Image>();
            postmodel.Images = post.Images;
            var category = await _context.Categories.SingleOrDefaultAsync(m => m.CategoryId == post.CategoryId);
            postmodel.CategoryName = category.CategoryName;

            IQueryable<string> categoryQuery = from m in _context.Categories
                                               orderby m.CategoryName
                                               select m.CategoryName;
            postmodel.CategoryNames = new SelectList(await categoryQuery.Distinct().ToListAsync());

            //StringBuilder sb = new StringBuilder();
            var tags = await _context.PostTags.Where(m => m.PostId == postmodel.PostId).Select(pt => pt.Tag.TagName).ToArrayAsync();
            ////var tagS = await _context.Tags.ToListAsync();
            //for (int i = 0; i < tags.Count(); i++)
            //{
            //    var taG = tags[i];
            //    //var tAg = tagS.FirstOrDefault(t => t.TagId == taG.TagId);
            //    sb.Append(taG.TagName);
            //    if (i != tags.Count() - 1)
            //    {
            //        sb.Append(",");
            //    }
            //}
            postmodel.TagStr = String.Join(",", tags);//sb.ToString();

            return View(postmodel);
        }

        // POST: Posts/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int id, [Bind("PostId,ReleaseDate,Text,Title,TagStr,Author,UserId,CategoryName,CategoryNames")] PostModel postmodel)
        {

            //var post1 = await post.Include(k => k.Images);
            var user = await _userManager.GetUserAsync(HttpContext.User);
            var userId = user?.Id;
            var userR = await _userManager.GetRolesAsync(user);

            var post = await _context.Posts.Include(k => k.Images).SingleOrDefaultAsync(m => m.PostId == id);

            if (userR.All(u => u != "Admin"))
            {
                if (userId != postmodel.UserId)
                { return RedirectToAction("AccessDenied", "Account"); }
            }

            if (id != postmodel.PostId)
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

                    post.Title = postmodel.Title;
                    post.Text = postmodel.Text;
                    //post.PostId = postmodel.PostId;
                    post.ReleaseDate = postmodel.ReleaseDate;
                    //var tags = new List<Tag>();
                    //post.Tags = new List<Tag>();

                    IQueryable<string> categoryQuery = from m in _context.Categories
                                                       orderby m.CategoryName
                                                       select m.CategoryName;
                    postmodel.CategoryNames = new SelectList(await categoryQuery.Distinct().ToListAsync());

                    var category = await _context.Categories.SingleOrDefaultAsync(m => m.CategoryName == postmodel.CategoryName);
                    post.CategoryId = category.CategoryId;

                    Char delimiter = ',';
                    var atags = postmodel.TagStr.Split(delimiter).ToList();
                    var tags = await _context.Tags.ToListAsync();
                    foreach (var str in atags)
                    {
                        var tag = new Tag();
                        if (!tags.Exists(x => x.TagName == str))
                        {
                            tag.TagName = str;
                            _context.Add(tag);
                            
                        }
                    }
                    //post.Tags = tags;
                    _context.Update(post);
                    await _context.SaveChangesAsync();
                    var tagsp = await _context.PostTags.ToListAsync();
                    foreach (var str in atags)
                    {
                        
                            var tag = tags.FirstOrDefault(t => t.TagName == str);
                        if (!tagsp.Exists(x => x.TagId == tag.TagId))
                        {
                            var taG = new PostTag();
                            taG.TagId = tag.TagId;
                            taG.PostId = post.PostId;
                            _context.Add(taG);
                        }
                    }
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
            return RedirectToAction("Edit", new { id = pid });

        }



        private bool PostExists(int id)
        {
            return _context.Posts.Any(e => e.PostId == id);
        }
    }
}
