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
    private readonly IDocumentAddVersionFacade _documentAddVersionFacade;
    private readonly IDocumentPublishFacade _documentPublishFacade;
    private readonly IDocumentDownloadFacade _documentDownloadFacade;

    public DocumentsController(
        IDocumentUploadFacade documentUploadFacade,
        IDocumentListFacade documentListFacade,
        IDocumentAddVersionFacade documentAddVersionFacade,
        IDocumentPublishFacade documentPublishFacade,
        IDocumentDownloadFacade documentDownloadFacade)
    {
        _documentUploadFacade = documentUploadFacade;
        _documentListFacade = documentListFacade;
        _documentAddVersionFacade = documentAddVersionFacade;
        _documentPublishFacade = documentPublishFacade;
        _documentDownloadFacade = documentDownloadFacade;
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

    [HttpPost("{documentId:guid}/versions")]
    [Consumes("multipart/form-data")]
    public async Task<IActionResult> AddVersion(
        [FromRoute] Guid documentId,
        [FromForm] AddDocumentVersionFormRequest request,
        CancellationToken cancellationToken)
    {
        await using var content = request.File.OpenReadStream();
        var result = await _documentAddVersionFacade.AddVersionAsync(
            new AddDocumentVersionRequest { DocumentId = documentId },
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
            DocumentAddVersionFacadeError.NoTenantContext => StatusCode(StatusCodes.Status401Unauthorized),
            DocumentAddVersionFacadeError.DocumentNotFound => StatusCode(StatusCodes.Status404NotFound),
            DocumentAddVersionFacadeError.InvalidFileName => Problem(
                detail: "The file name is not valid.",
                statusCode: StatusCodes.Status400BadRequest),
            DocumentAddVersionFacadeError.UnsupportedMediaType => Problem(
                detail: "The file type is not supported.",
                statusCode: StatusCodes.Status415UnsupportedMediaType),
            DocumentAddVersionFacadeError.PayloadTooLarge => Problem(
                detail: "The file exceeds the maximum allowed size.",
                statusCode: StatusCodes.Status413PayloadTooLarge),
            _ => Problem(statusCode: StatusCodes.Status500InternalServerError),
        };
    }

    [HttpPost("{documentId:guid}/versions/{versionId:guid}/publish")]
    public async Task<IActionResult> Publish(
        [FromRoute] Guid documentId,
        [FromRoute] Guid versionId,
        CancellationToken cancellationToken)
    {
        var result = await _documentPublishFacade.PublishAsync(
            new PublishDocumentVersionRequest { DocumentId = documentId, VersionId = versionId },
            cancellationToken);

        if (result.IsSuccess)
        {
            return Ok(result.Result);
        }

        return result.Error switch
        {
            DocumentPublishFacadeError.NoTenantContext => StatusCode(StatusCodes.Status401Unauthorized),
            DocumentPublishFacadeError.Forbidden => StatusCode(StatusCodes.Status403Forbidden),
            DocumentPublishFacadeError.DocumentNotFound => StatusCode(StatusCodes.Status404NotFound),
            DocumentPublishFacadeError.VersionNotFound => StatusCode(StatusCodes.Status404NotFound),
            DocumentPublishFacadeError.NotEligible => Problem(
                detail: "The target version is not eligible for publication.",
                statusCode: StatusCodes.Status409Conflict),
            _ => Problem(statusCode: StatusCodes.Status500InternalServerError),
        };
    }

    [HttpGet("{documentId:guid}/versions/{versionId:guid}/download")]
    public async Task<IActionResult> Download(
        [FromRoute] Guid documentId,
        [FromRoute] Guid versionId,
        CancellationToken cancellationToken)
    {
        var result = await _documentDownloadFacade.DownloadAsync(documentId, versionId, cancellationToken);

        if (result.IsSuccess)
        {
            // Streams the authorized binary directly through this service —
            // never a durable/public Blob URL the caller could reuse or
            // share without re-authorization.
            return File(result.Content!, result.Descriptor!.ContentType, result.Descriptor.FileName);
        }

        return result.Error switch
        {
            DocumentDownloadFacadeError.NoTenantContext => StatusCode(StatusCodes.Status401Unauthorized),
            DocumentDownloadFacadeError.NotFound => StatusCode(StatusCodes.Status404NotFound),
            DocumentDownloadFacadeError.Forbidden => StatusCode(StatusCodes.Status403Forbidden),
            _ => Problem(statusCode: StatusCodes.Status500InternalServerError),
        };
    }
}
