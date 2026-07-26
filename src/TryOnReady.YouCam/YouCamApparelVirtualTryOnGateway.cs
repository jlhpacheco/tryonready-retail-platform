using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using TryOnReady.Application.VirtualTryOn;

namespace TryOnReady.YouCam;

internal sealed class YouCamApparelVirtualTryOnGateway(
    HttpClient httpClient,
    IOptions<YouCamOptions> options,
    ILogger<YouCamApparelVirtualTryOnGateway> logger)
    : IApparelVirtualTryOnGateway
{
    private const int MaximumProviderResultBytes = 15 * 1024 * 1024;
    private static readonly JsonSerializerOptions JsonOptions =
        new(JsonSerializerDefaults.Web);

    private readonly YouCamOptions options = options.Value;

    public async Task<VirtualTryOnSubmission> SubmitAsync(
        VirtualTryOnRequest request,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);

        try
        {
            var personUploadTask = ReserveAndUploadAsync(
                request.PersonImage,
                cancellationToken);
            var garmentUploadTask = ReserveAndUploadAsync(
                request.GarmentImage,
                cancellationToken);
            await Task.WhenAll(personUploadTask, garmentUploadTask);

            var payload = new CreateTaskRequest(
                personUploadTask.Result,
                garmentUploadTask.Result,
                NormalizeGarmentCategory(request.GarmentCategory));
            using var message = CreateProviderRequest(
                HttpMethod.Post,
                "s2s/v2.0/task/cloth-v3");
            message.Content = JsonContent.Create(payload, options: JsonOptions);

            using var response = await httpClient.SendAsync(
                message,
                HttpCompletionOption.ResponseHeadersRead,
                cancellationToken);
            if (!response.IsSuccessStatusCode)
            {
                return new VirtualTryOnSubmission(
                    VirtualTryOnSubmissionStatus.Rejected,
                    ProviderReference: null,
                    $"YouCam rejected task creation with HTTP {(int)response.StatusCode}.",
                    ConsumesApiUnits: false);
            }

            var body = await response.Content.ReadFromJsonAsync<ApiEnvelope<TaskData>>(
                JsonOptions,
                cancellationToken);
            if (body?.Data?.TaskId is not { Length: > 0 } taskId)
            {
                return new VirtualTryOnSubmission(
                    VirtualTryOnSubmissionStatus.Rejected,
                    ProviderReference: null,
                    "YouCam did not return a task identifier.",
                    ConsumesApiUnits: false);
            }

            logger.LogInformation(
                "Virtual try-on job {JobId} was accepted by YouCam.",
                request.JobId);
            return new VirtualTryOnSubmission(
                VirtualTryOnSubmissionStatus.Accepted,
                taskId,
                "YouCam accepted the secure server-side task.",
                ConsumesApiUnits: true);
        }
        catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
        {
            throw;
        }
        catch (HttpRequestException exception)
        {
            logger.LogWarning(
                exception,
                "YouCam task submission failed at the HTTP boundary.");
            return new VirtualTryOnSubmission(
                VirtualTryOnSubmissionStatus.Rejected,
                ProviderReference: null,
                "YouCam could not be reached securely.",
                ConsumesApiUnits: false);
        }
        catch (JsonException exception)
        {
            logger.LogWarning(
                exception,
                "YouCam returned an unreadable task response.");
            return new VirtualTryOnSubmission(
                VirtualTryOnSubmissionStatus.Rejected,
                ProviderReference: null,
                "YouCam returned an unreadable task response.",
                ConsumesApiUnits: false);
        }
    }

    public async Task<VirtualTryOnProgress> GetProgressAsync(
        string providerReference,
        CancellationToken cancellationToken)
    {
        using var request = CreateProviderRequest(
            HttpMethod.Get,
            $"s2s/v2.0/task/cloth-v3/{Uri.EscapeDataString(providerReference)}");
        using var response = await httpClient.SendAsync(
            request,
            HttpCompletionOption.ResponseHeadersRead,
            cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            return new VirtualTryOnProgress(
                VirtualTryOnProgressStatus.Failed,
                $"YouCam status polling returned HTTP {(int)response.StatusCode}.",
                $"http_{(int)response.StatusCode}",
                Result: null);
        }

        var body = await response.Content.ReadFromJsonAsync<ApiEnvelope<ProgressData>>(
            JsonOptions,
            cancellationToken);
        var status = body?.Data?.TaskStatus?.Trim().ToLowerInvariant();
        if (status is "pending" or "queued")
        {
            return new VirtualTryOnProgress(
                VirtualTryOnProgressStatus.Pending,
                "YouCam queued the generation.",
                ProviderErrorCode: null,
                Result: null);
        }

        if (status is "running" or "processing")
        {
            return new VirtualTryOnProgress(
                VirtualTryOnProgressStatus.Running,
                "YouCam is generating the virtual try-on.",
                ProviderErrorCode: null,
                Result: null);
        }

        if (status == "success" && body?.Data?.Results?.Url is { Length: > 0 } url)
        {
            var result = await DownloadResultAsync(url, cancellationToken);
            return new VirtualTryOnProgress(
                VirtualTryOnProgressStatus.Succeeded,
                "YouCam completed the virtual try-on.",
                ProviderErrorCode: null,
                result);
        }

        var providerError = ExtractProviderError(body?.Data?.Error);
        return new VirtualTryOnProgress(
            VirtualTryOnProgressStatus.Failed,
            "YouCam could not generate this virtual try-on.",
            providerError,
            Result: null);
    }

    public async Task DeleteResourcesAsync(
        string providerReference,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(options.DeleteResourcesPathTemplate))
        {
            return;
        }

        var path = options.DeleteResourcesPathTemplate.Replace(
            "{task_id}",
            Uri.EscapeDataString(providerReference),
            StringComparison.Ordinal);
        using var request = CreateProviderRequest(HttpMethod.Delete, path);
        using var response = await httpClient.SendAsync(
            request,
            HttpCompletionOption.ResponseHeadersRead,
            cancellationToken);
        response.EnsureSuccessStatusCode();
    }

    private async Task<string> ReserveAndUploadAsync(
        VirtualTryOnAsset asset,
        CancellationToken cancellationToken)
    {
        var payload = new FileReservationRequest(
        [
            new FileReservationItem(
                NormalizeMediaType(asset.MediaType),
                Path.GetFileName(asset.FileName),
                asset.Content.LongLength),
        ]);
        using var reservationRequest = CreateProviderRequest(
            HttpMethod.Post,
            "s2s/v2.0/file/cloth-v3");
        reservationRequest.Content = JsonContent.Create(payload, options: JsonOptions);
        using var reservationResponse = await httpClient.SendAsync(
            reservationRequest,
            HttpCompletionOption.ResponseHeadersRead,
            cancellationToken);
        reservationResponse.EnsureSuccessStatusCode();

        var reservation = await reservationResponse.Content
            .ReadFromJsonAsync<ApiEnvelope<FileReservationData>>(
                JsonOptions,
                cancellationToken);
        var file = reservation?.Data?.Files?.SingleOrDefault()
            ?? throw new JsonException("YouCam did not return one file reservation.");
        var upload = file.Requests?.SingleOrDefault()
            ?? throw new JsonException("YouCam did not return one upload request.");
        var uploadUri = new Uri(upload.Url, UriKind.Absolute);
        ValidateSignedAssetUri(uploadUri);

        using var uploadRequest = new HttpRequestMessage(HttpMethod.Put, uploadUri);
        uploadRequest.Content = new ByteArrayContent(asset.Content);
        uploadRequest.Content.Headers.ContentType = MediaTypeHeaderValue.Parse(
            NormalizeMediaType(asset.MediaType));
        uploadRequest.Content.Headers.ContentLength = asset.Content.LongLength;
        using var uploadResponse = await httpClient.SendAsync(
            uploadRequest,
            HttpCompletionOption.ResponseHeadersRead,
            cancellationToken);
        uploadResponse.EnsureSuccessStatusCode();
        return file.FileId;
    }

    private async Task<VirtualTryOnResult> DownloadResultAsync(
        string url,
        CancellationToken cancellationToken)
    {
        var resultUri = new Uri(url, UriKind.Absolute);
        ValidateSignedAssetUri(resultUri);
        using var request = new HttpRequestMessage(HttpMethod.Get, resultUri);
        using var response = await httpClient.SendAsync(
            request,
            HttpCompletionOption.ResponseHeadersRead,
            cancellationToken);
        response.EnsureSuccessStatusCode();

        if (response.Content.Headers.ContentLength > MaximumProviderResultBytes)
        {
            throw new HttpRequestException("The provider result exceeded the safe limit.");
        }

        await using var source = await response.Content.ReadAsStreamAsync(
            cancellationToken);
        using var destination = new MemoryStream();
        var buffer = new byte[81920];
        var total = 0;
        while (true)
        {
            var read = await source.ReadAsync(buffer, cancellationToken);
            if (read == 0)
            {
                break;
            }

            total += read;
            if (total > MaximumProviderResultBytes)
            {
                throw new HttpRequestException(
                    "The provider result exceeded the safe limit.");
            }

            await destination.WriteAsync(buffer.AsMemory(0, read), cancellationToken);
        }

        var mediaType = response.Content.Headers.ContentType?.MediaType
            ?? "image/png";
        return new VirtualTryOnResult(
            $"youcam-{Guid.NewGuid():N}{GetExtension(mediaType)}",
            mediaType,
            destination.ToArray());
    }

    private HttpRequestMessage CreateProviderRequest(HttpMethod method, string path)
    {
        var request = new HttpRequestMessage(method, path);
        request.Headers.Authorization = new AuthenticationHeaderValue(
            "Bearer",
            options.ApiKey);
        request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue(
            "application/json"));
        return request;
    }

    private static string NormalizeGarmentCategory(string category) =>
        category.Trim().ToLowerInvariant() switch
        {
            "top" or "upper_body" => "upper_body",
            "bottom" or "lower_body" => "lower_body",
            "full-body outfit" or "full_body" => "full_body",
            _ => "auto",
        };

    private static string NormalizeMediaType(string mediaType) =>
        mediaType.Equals("image/jpeg", StringComparison.OrdinalIgnoreCase)
            ? "image/jpg"
            : mediaType.ToLowerInvariant();

    private static string GetExtension(string mediaType) =>
        mediaType.ToLowerInvariant() switch
        {
            "image/jpeg" or "image/jpg" => ".jpg",
            "image/webp" => ".webp",
            _ => ".png",
        };

    private static void ValidateSignedAssetUri(Uri uri)
    {
        if (uri.Scheme != Uri.UriSchemeHttps
            || !(uri.Host.EndsWith(".amazonaws.com", StringComparison.OrdinalIgnoreCase)
                || uri.Host.EndsWith(".makeupar.com", StringComparison.OrdinalIgnoreCase)
                || uri.Host.EndsWith(".perfectcorp.com", StringComparison.OrdinalIgnoreCase)))
        {
            throw new HttpRequestException(
                "The provider returned an untrusted signed asset URL.");
        }
    }

    private static string ExtractProviderError(JsonElement? error)
    {
        if (error is null
            || error.Value.ValueKind is JsonValueKind.Null
                or JsonValueKind.Undefined)
        {
            return "provider_error";
        }

        if (error.Value.ValueKind == JsonValueKind.String)
        {
            return error.Value.GetString() ?? "provider_error";
        }

        if (error.Value.ValueKind == JsonValueKind.Object
            && error.Value.TryGetProperty("code", out var code)
            && code.ValueKind == JsonValueKind.String)
        {
            return code.GetString() ?? "provider_error";
        }

        return "provider_error";
    }

    private sealed record ApiEnvelope<T>(
        [property: JsonPropertyName("status")] int Status,
        [property: JsonPropertyName("data")] T? Data);

    private sealed record FileReservationRequest(
        [property: JsonPropertyName("files")]
        IReadOnlyList<FileReservationItem> Files);

    private sealed record FileReservationItem(
        [property: JsonPropertyName("content_type")] string ContentType,
        [property: JsonPropertyName("file_name")] string FileName,
        [property: JsonPropertyName("file_size")] long FileSize);

    private sealed record FileReservationData(
        [property: JsonPropertyName("files")]
        IReadOnlyList<FileReservationFile>? Files);

    private sealed record FileReservationFile(
        [property: JsonPropertyName("file_id")] string FileId,
        [property: JsonPropertyName("requests")]
        IReadOnlyList<FileUploadRequest>? Requests);

    private sealed record FileUploadRequest(
        [property: JsonPropertyName("method")] string Method,
        [property: JsonPropertyName("url")] string Url);

    private sealed record CreateTaskRequest(
        [property: JsonPropertyName("src_file_id")] string SourceFileId,
        [property: JsonPropertyName("ref_file_id")] string ReferenceFileId,
        [property: JsonPropertyName("garment_category")] string GarmentCategory);

    private sealed record TaskData(
        [property: JsonPropertyName("task_id")] string? TaskId);

    private sealed record ProgressData(
        [property: JsonPropertyName("task_status")] string? TaskStatus,
        [property: JsonPropertyName("error")] JsonElement? Error,
        [property: JsonPropertyName("results")] ProgressResults? Results);

    private sealed record ProgressResults(
        [property: JsonPropertyName("url")] string? Url);
}
