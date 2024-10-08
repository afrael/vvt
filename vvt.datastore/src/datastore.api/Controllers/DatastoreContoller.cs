using Microsoft.AspNetCore.Mvc;
using vvt.common.lib.dtos;
using vvt.services.lib;

namespace vvt.app.api.Controllers;

[ApiController]
[Route("[controller]")]
public class DatastoreController : ControllerBase
{
    private readonly ILogger<DatastoreController> _logger;
    private readonly IUploadService _uploadService;

    public DatastoreController(ILogger<DatastoreController> logger, IUploadService uploadService)
    {
        _logger = logger;
        _uploadService = uploadService;
    }

    [HttpPost("/DataStore")]
    public async Task<ActionResult> PostAsynch([FromForm] IFormFile file)
    {
        try
        {
            _logger.LogInformation($"vvt.controller: File Received at {DateTime.Now}");

            // check if user uploaded empty file
            if(file.Length == 0)
            {
                return BadRequest("The File cannot be empty.");
            }

            using (var streamReader = new StreamReader(file.OpenReadStream()))
            {
                return await ProcessFileUpload(streamReader);
            }
        }
        catch (Exception ex)
        {
            return BadRequest($"Upload of File Failed. Exception {ex.Message}");
        }
    }

    private async Task<ActionResult> ProcessFileUpload(StreamReader streamReader)
    {
        var content = await streamReader.ReadToEndAsync();
        _logger.LogInformation($"vvt.controller: Received {content.Length} bytes");
        var clearResult = await _uploadService.ClearUploadData();
        if (clearResult.Result != vvt.common.lib.enums.OperationResults.Sucessfull)
        {
            return BadRequest(clearResult.Message);
        }
        var result = await _uploadService.SaveUpload(new RawUploadDto(content));
        return Ok(result.Message);
    }
}
