using MyFixIt.Persistence;
using System.Configuration;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace MyFixIt.Controllers
{
    [Authorize]
    public class TasksController : Controller
    {
        private readonly IFixItTaskRepository _fixItRepository = null;
        private readonly IPhotoService _photoService = null;
        private readonly IFixItQueueManager _queueManager = null;

        public TasksController(IFixItTaskRepository repository, IPhotoService photoStore, IFixItQueueManager queueManager)
        {
            _fixItRepository = repository;
            _photoService = photoStore;
            _queueManager = queueManager;
        }

        // GET: /FixIt/
        public async Task<ActionResult> Status()
        {
            string currentUser = User.Identity.Name;
            var result = await _fixItRepository.FindTasksByCreatorAsync(currentUser);

            return View(result);
        }

        //
        // GET: /Tasks/Create
        public ActionResult Create()
        {
            return View();
        }

        //
        // POST: /Tasks/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "FixItTaskId,CreatedBy,Owner,Title,Notes,PhotoUrl,IsDone")]FixItTask fixittask, HttpPostedFileBase photo)
        {
            if (ModelState.IsValid)
            {
                fixittask.CreatedBy = User.Identity.Name;
                fixittask.PhotoUrl = await _photoService.UploadPhotoAsync(photo);

                if (ConfigurationManager.AppSettings["UseQueues"] == "true")
                {
                    await _queueManager.SendMessageAsync(fixittask);
                }
                else
                {
                    await _fixItRepository.CreateAsync(fixittask);
                }

                return RedirectToAction("Success");
            }

            return View(fixittask);
        }

        //
        // GET: /Tasks/Success
        public ActionResult Success()
        {
            return View();
        }
    }
}