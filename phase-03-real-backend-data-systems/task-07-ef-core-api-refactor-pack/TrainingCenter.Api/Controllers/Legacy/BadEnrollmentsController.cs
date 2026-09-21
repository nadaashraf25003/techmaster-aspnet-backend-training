using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TrainingCenter.Api.Common;
using TrainingCenter.Api.Data;
using TrainingCenter.Api.Entities;

namespace TrainingCenter.Api.Controllers.Legacy;

/// <summary>
/// LEGACY BAD CODE PRESERVED FOR COMPARISON AND REFACTORING AUDIT.
/// Contains fat controller logic, synchronous blocking calls, missing DTOs,
/// missing domain validation, hard deletes, and incorrect HTTP status codes.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class BadEnrollmentsController : ControllerBase
{
    private readonly AppDbContext _db;

    public BadEnrollmentsController(AppDbContext db)
    {
        _db = db;
    }

    [HttpGet]
    public IActionResult GetAll()
    {
        // Problem: returns full EF entities with navigation properties.
        // Problem: no pagination, no filtering, no projection.
        var data = _db.Enrollments
            .Include(e => e.Student)
            .Include(e => e.TrainingTrack)
            .Include(e => e.Payments)
            .ToList();

        return Ok(data);
    }

    [HttpPost]
    public IActionResult Create(Enrollment enrollment)
    {
        // Problem: accepts entity directly from request body.
        // Problem: no validation.
        // Problem: duplicate active enrollments are allowed.
        // Problem: track capacity is ignored.
        enrollment.EnrollmentDate = DateTime.Now;
        enrollment.Status = EnrollmentStatus.Active;

        _db.Enrollments.Add(enrollment);
        _db.SaveChanges();

        return Ok(enrollment);
    }

    [HttpPost("pay")]
    public IActionResult Pay(int enrollmentId, decimal amount)
    {
        // Problem: query duplicated and not async.
        var enrollment = _db.Enrollments
            .Include(x => x.Payments)
            .FirstOrDefault(x => x.Id == enrollmentId);

        if (enrollment == null)
        {
            return Ok("not found"); // wrong status code
        }

        // Problem: no validation for negative or zero amount.
        var payment = new Payment
        {
            EnrollmentId = enrollmentId,
            Amount = amount,
            PaymentDate = DateTime.Now,
            Status = PaymentStatus.Paid,
            ReferenceNumber = $"LEGACY-{Guid.NewGuid().ToString("N")[..8].ToUpper()}"
        };

        _db.Payments.Add(payment);
        _db.SaveChanges();

        return Ok(payment);
    }

    [HttpDelete("{id}")]
    public IActionResult Delete(int id)
    {
        var item = _db.Enrollments.Find(id);
        if (item == null)
        {
            return Ok("missing"); // wrong status code
        }

        // Problem: hard delete loses historical data.
        _db.Enrollments.Remove(item);
        _db.SaveChanges();

        return Ok("deleted");
    }
}
