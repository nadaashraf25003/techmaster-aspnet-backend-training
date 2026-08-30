using System.ComponentModel.DataAnnotations;

namespace StudentManagementApi.DTOs;

/// <summary>
/// Query parameters for searching, filtering, and paginating student profiles.
/// </summary>
public class StudentQueryParametersDto
{
    /// <summary>
    /// Search term matched against FullName and Email (case-insensitive substring match).
    /// </summary>
    public string? Search { get; set; }

    /// <summary>
    /// Filter students by TrackName (case-insensitive).
    /// </summary>
    public string? TrackName { get; set; }

    /// <summary>
    /// Filter students by Active status (true/false).
    /// </summary>
    public bool? IsActive { get; set; }

    /// <summary>
    /// Page number (1-based index). Defaults to 1.
    /// </summary>
    [Range(1, int.MaxValue, ErrorMessage = "PageNumber must be at least 1.")]
    public int PageNumber { get; set; } = 1;

    /// <summary>
    /// Number of items per page. Defaults to 10 (maximum 100).
    /// </summary>
    [Range(1, 100, ErrorMessage = "PageSize must be between 1 and 100.")]
    public int PageSize { get; set; } = 10;
}
