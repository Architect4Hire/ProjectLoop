using Microsoft.AspNetCore.Mvc;
using ProjectLoop.Documents.Contracts;
using ProjectLoop.Documents.Core;

namespace ProjectLoop.Documents.Controllers;

[ApiController]
[Route("documents")]
public sealed class DocumentsController : ControllerBase
{
    private readonly IDocumentUploadFacade _documentUploadFacade;
    private readonly IDocumentListFacade _documentListFacade;

    public DocumentsController(IDocumentUploadFacade documentUploadFacade, IDocumentListFacade documentListFacade)
    {
        _documentUploadFacade = documentUploadFacade;
        _documentListFacade = documentListFacade;
    }

    [HttpGet]
    public async Task<IActionResult> List([FromQuery] DocumentListQuery query, CancellationToken cancellationToken)
    {
        var result = await _documentListFacade.ListAsync(query, cancellationToken);

        if (result.IsSuccess)
        {
            return Ok(result.Response);
        }

        return result.Error switch
        {
            DocumentListFacadeError.NoTenantContext => StatusCode(StatusCodes.Status401Unauthorized),
            _ => Problem(statusCode: StatusCodes.Status500InternalServerError),
        };
    }

    [HttpPost]
    [Consumes("multipart/form-data")]
    public async Task<IActionResult> Upload([FromForm] UploadDocumentFormRequest request, CancellationToken cancellationToken)
    {
        var metadata = new UploadDocumentMetadata
        {
            ProjectId = request.ProjectId,
            Title = request.Title,
            Category = request.Category,
            Visibility = request.Visibility,
        };

        await using var content = request.File.OpenReadStream();
        var result = await _documentUploadFacade.UploadAsync(
            metadata,
            content,
            request.File.FileName,
            request.File.ContentType,
            cancellationToken);

        if (result.IsSuccess)
        {
            return StatusCode(StatusCodes.Status201Created, result.Result);
        }

        return result.Error switch
        {
            DocumentUploadFacadeError.NoTenantContext => StatusCode(StatusCodes.Status401Unauthorized),
            DocumentUploadFacadeError.InvalidMetadata => Problem(
                detail: "The document metadata is not valid.",
                statusCode: StatusCodes.Status400BadRequest),
            DocumentUploadFacadeError.InvalidFileName => Problem(
                detail: "The file name is not valid.",
                statusCode: StatusCodes.Status400BadRequest),
            DocumentUploadFacadeError.UnsupportedMediaType => Problem(
                detail: "The file type is not supported.",
                statusCode: StatusCodes.Status415UnsupportedMediaType),
            DocumentUploadFacadeError.PayloadTooLarge => Problem(
                detail: "The file exceeds the maximum allowed size.",
                statusCode: StatusCodes.Status413PayloadTooLarge),
            _ => Problem(statusCode: StatusCodes.Status500InternalServerError),
        };
    }
}
