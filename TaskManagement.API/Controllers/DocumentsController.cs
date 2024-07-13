using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaskManagement.API.Data;
using TaskManagement.API.DTOs;

namespace TaskManagement.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    [Authorize]
    public class DocumentsController(AppDbContext dbContext) : ControllerBase
    {
        /// <summary>
        /// Downloads the document by id.
        /// </summary>
        /// <param name="documentId">The document id.</param>
        [HttpGet("{documentId}")]
        public async Task<IActionResult> GetDocument(int documentId)
        {
            var document = await dbContext.Documents.FindAsync(documentId);

            if (document is null)
            {
                return NotFound(new ApiResponse(404, "Document doesn't exist"));
            }

            return File(document.Content, "application/octet-stream", document.FileName);
        }

        /// <summary>
        /// Attach a document to a task.
        /// </summary>
        /// <param name="taskId">The task id.</param>
        /// <param name="file">The file to be uploaded.</param>
        /// <returns></returns>
        [HttpPost("{taskId}/attach")]
        public async Task<IActionResult> AttachDocument(int taskId, IFormFile file)
        {
            var task = await dbContext.Tasks.FindAsync(taskId);

            if (task == null)
            {
                return NotFound(new ApiResponse(404, "Task not found."));
            }

            if (file == null || file.Length == 0)
            {
                return BadRequest(new ApiResponse(400, "No file uploaded."));
            }

            using var memoryStream = new MemoryStream();
            await file.CopyToAsync(memoryStream);

            var document = new Document
            {
                FileName = file.FileName,
                Content = memoryStream.ToArray(),
                TaskId = taskId
            };

            dbContext.Documents.Add(document);
            await dbContext.SaveChangesAsync();

            return Created();
        }

    }
}
