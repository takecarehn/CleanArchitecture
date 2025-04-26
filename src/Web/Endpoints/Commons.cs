using CleanArchitecture.Application.Common.Models;
using CleanArchitecture.Domain.Constants;
using CleanArchitecture.Infrastructure.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CleanArchitecture.Web.Endpoints;

public class Commons : EndpointGroupBase
{
    public override void Map(WebApplication app)
    {
        var group = app.MapGroup("/api/common").WithTags("Commons"); ;

        group.MapPost("/upload", UploadFilesAsync)
             .RequirePermission(ClaimValues.PermissionUpload);
    }

    private static async Task<Result> UploadFilesAsync(
        [FromServices] IConfiguration configuration,
        HttpRequest request)
    {
        if (!request.HasFormContentType)
        {
            return Result.Failure("Invalid form data.");
        }

        var form = request.Form;
        var files = form.Files;

        if (files.Count == 0)
        {
            return Result.Failure("No files uploaded.");
        }

        var settings = configuration.GetSection("FileUploadSettings");
        var allowedExtensions = settings.GetSection("AllowedExtensions").Get<string[]>() ?? Array.Empty<string>();
        var maxFileSizeMB = settings.GetValue<int>("MaxFileSizeMB", 10);
        var maxFilesPerUpload = settings.GetValue<int>("MaxFilesPerUpload", 5);

        if (files.Count > maxFilesPerUpload)
        {
            return Result.Failure($"Only {maxFilesPerUpload} files can be uploaded at once.");
        }

        var uploadRoot = Path.Combine(Directory.GetCurrentDirectory(), "uploads");
        Directory.CreateDirectory(uploadRoot);

        var fileUrls = new List<string>();

        foreach (var file in files)
        {
            var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
            var contentType = file.ContentType;

            // Check extension
            if (!allowedExtensions.Contains(extension))
            {
                continue; // Skip file invalid extension
            }

            // Check file size
            if (file.Length > maxFileSizeMB * 1024 * 1024)
            {
                continue; // Skip file over file size limit
            }

            // Check ContentType sơ bộ
            if (!IsAllowedContentType(extension, contentType))
            {
                continue; // Skip file MIME type not allowed
            }

            // determine save folder
            var subFolder = extension switch
            {
                ".png" or ".jpg" or ".jpeg" => "avatars",
                ".txt" or ".doc" or ".docx" or ".pdf" or ".ppt" or ".pptx" or ".xls" or ".xlsx" => "documents",
                _ => "others"
            };

            var targetFolder = Path.Combine(uploadRoot, subFolder);
            Directory.CreateDirectory(targetFolder);

            var fileName = $"{Guid.NewGuid()}{extension}";
            var filePath = Path.Combine(targetFolder, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            var relativePath = Path.Combine("uploads", subFolder, fileName).Replace("\\", "/");
            fileUrls.Add("/" + relativePath);
        }

        if (fileUrls.Count == 0)
        {
            return Result.Failure("No valid files uploaded.");
        }

        return Result.Success(new { Files = fileUrls });
    }

    private static bool IsAllowedContentType(string extension, string contentType)
    {
        // Some MIME common type mapping
        return extension switch
        {
            ".png" => contentType == "image/png",
            ".jpg" or ".jpeg" => contentType == "image/jpeg",
            ".txt" => contentType == "text/plain",
            ".doc" => contentType == "application/msword",
            ".docx" => contentType == "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
            ".pdf" => contentType == "application/pdf",
            ".ppt" => contentType == "application/vnd.ms-powerpoint",
            ".pptx" => contentType == "application/vnd.openxmlformats-officedocument.presentationml.presentation",
            ".xls" => contentType == "application/vnd.ms-excel",
            ".xlsx" => contentType == "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
            _ => false
        };
    }
}
