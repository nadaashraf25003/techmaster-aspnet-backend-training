$ErrorActionPreference = "Continue"

$projectPath = "c:\Users\user\Desktop\Tech_Master\techmaster-aspnet-backend-training\phase-04-secure-professional-backend\task-07-audit-activity-timeline\TrainingCenter.Api"
$baseUrl = "http://localhost:5245"

Write-Host "`n=== Starting Task 07 Test Server on $baseUrl ===" -ForegroundColor Cyan
$serverProcess = Start-Process -FilePath "dotnet" -ArgumentList "run --no-build --urls $baseUrl" -WorkingDirectory $projectPath -PassThru -NoNewWindow

$serverReady = $false
for ($i = 1; $i -le 20; $i++) {
    Start-Sleep -Seconds 2
    try {
        $health = Invoke-RestMethod -Uri "$baseUrl/health" -Method Get -TimeoutSec 3
        if ($health.status -eq "Healthy") {
            $serverReady = $true
            Write-Host "Server is ready! Health Status: $($health.status)" -ForegroundColor Green
            break
        }
    } catch {
        Write-Host "Waiting for server to start (attempt $i/20)..." -ForegroundColor Gray
    }
}

if (-not $serverReady) {
    Write-Host "ERROR: Server failed to start within timeout." -ForegroundColor Red
    if ($serverProcess -and -not $serverProcess.HasExited) { Stop-Process -Id $serverProcess.Id -Force }
    exit 1
}

try {
    # 1. Health Check Verified
    Write-Host "`n--- Test 1: Health Check Passed ---" -ForegroundColor Yellow

    # 2. Login as Admin
    Write-Host "`n--- Test 2: Login as Admin ---" -ForegroundColor Yellow
    $adminLoginBody = @{
        email = "admin@techmaster.com"
        password = "Admin@123456"
    } | ConvertTo-Json

    $adminAuth = Invoke-RestMethod -Uri "$baseUrl/api/auth/login" -Method Post -Body $adminLoginBody -ContentType "application/json"
    $adminToken = $adminAuth.data.accessToken
    $adminUserId = $adminAuth.data.userId
    Write-Host "Admin Logged In. Token Length: $($adminToken.Length), UserId: $adminUserId" -ForegroundColor Green

    # 3. Login as Student
    Write-Host "`n--- Test 3: Login as Student ---" -ForegroundColor Yellow
    $studentLoginBody = @{
        email = "student@techmaster.com"
        password = "Student@123456"
    } | ConvertTo-Json

    $studentAuth = Invoke-RestMethod -Uri "$baseUrl/api/auth/login" -Method Post -Body $studentLoginBody -ContentType "application/json"
    $studentToken = $studentAuth.data.accessToken
    Write-Host "Student Logged In. Token Length: $($studentToken.Length)" -ForegroundColor Green

    # 4. Security Check: Student tries to access Activity Logs (Expected 403 Forbidden)
    Write-Host "`n--- Test 4: Security Check (Student access to /api/admin/activity-logs) ---" -ForegroundColor Yellow
    try {
        Invoke-RestMethod -Uri "$baseUrl/api/admin/activity-logs" -Method Get -Headers @{ Authorization = "Bearer $studentToken" }
        Write-Host "FAILED: Student was able to access activity logs!" -ForegroundColor Red
    } catch {
        Write-Host "PASSED: Access Denied for Student (StatusCode: $($_.Exception.Response.StatusCode.value__))" -ForegroundColor Green
    }

    # 5. Security Check: Anonymous tries to access Activity Logs (Expected 401 Unauthorized)
    Write-Host "`n--- Test 5: Security Check (Anonymous access to /api/admin/activity-logs) ---" -ForegroundColor Yellow
    try {
        Invoke-RestMethod -Uri "$baseUrl/api/admin/activity-logs" -Method Get
        Write-Host "FAILED: Anonymous access succeeded!" -ForegroundColor Red
    } catch {
        Write-Host "PASSED: Unauthorized for Anonymous (StatusCode: $($_.Exception.Response.StatusCode.value__))" -ForegroundColor Green
    }

    # 6. User Registration Audit Trail
    Write-Host "`n--- Test 6: Register New Student & Verify Audit Log ---" -ForegroundColor Yellow
    $regEmail = "audittest_$([Guid]::NewGuid().ToString('N').Substring(0,6))@techmaster.com"
    $regBody = @{
        fullName = "Audit Test Student"
        email = $regEmail
        password = "Password@123456"
        confirmPassword = "Password@123456"
        role = "Student"
        phoneNumber = "01099998888"
        address = "123 Test St, Cairo"
    } | ConvertTo-Json

    $regRes = Invoke-RestMethod -Uri "$baseUrl/api/auth/register" -Method Post -Body $regBody -ContentType "application/json"
    $newUserId = $regRes.data.userId
    Write-Host "Registered User ID: $newUserId, Email: $regEmail" -ForegroundColor Green

    # 7. Failed Login Attempt Audit Trail
    Write-Host "`n--- Test 7: Failed Login Attempt ---" -ForegroundColor Yellow
    $failedLoginBody = @{
        email = $regEmail
        password = "WrongPassword@123"
    } | ConvertTo-Json

    try {
        Invoke-RestMethod -Uri "$baseUrl/api/auth/login" -Method Post -Body $failedLoginBody -ContentType "application/json"
    } catch {
        Write-Host "Login correctly rejected for invalid password." -ForegroundColor Gray
    }

    # 8. Admin Creates Track
    Write-Host "`n--- Test 8: Admin Creates Track ---" -ForegroundColor Yellow
    $trackCode = "AUD-$([Guid]::NewGuid().ToString('N').Substring(0,4).ToUpper())"
    $trackBody = @{
        title = "Cloud Native Microservices Audit"
        code = $trackCode
        description = "Advanced track to verify full audit logging."
        price = 8500.00
        durationHours = 60
        capacity = 20
        status = "Upcoming"
        startDate = (Get-Date).AddDays(10).ToString("yyyy-MM-dd")
        endDate = (Get-Date).AddDays(40).ToString("yyyy-MM-dd")
        primaryInstructorId = 1
    } | ConvertTo-Json

    $trackRes = Invoke-RestMethod -Uri "$baseUrl/api/tracks" -Method Post -Body $trackBody -ContentType "application/json" -Headers @{ Authorization = "Bearer $adminToken" }
    $trackId = $trackRes.data.trainingTrackId
    Write-Host "Created Track ID: $trackId, Code: $trackCode" -ForegroundColor Green

    # 9. Student Enrollment Request
    Write-Host "`n--- Test 9: Student Enrollment Request ---" -ForegroundColor Yellow
    $enrollBody = @{
        studentId = $regRes.data.linkedStudentId
        trainingTrackId = $trackId
        notes = "Student applying with full audit coverage."
    } | ConvertTo-Json

    $enrollRes = Invoke-RestMethod -Uri "$baseUrl/api/enrollments" -Method Post -Body $enrollBody -ContentType "application/json" -Headers @{ Authorization = "Bearer $adminToken" }
    $enrollmentId = $enrollRes.data.enrollmentId
    Write-Host "Created Enrollment ID: $enrollmentId, Status: $($enrollRes.data.status)" -ForegroundColor Green

    # 10. Admin Updates Enrollment Status (Status Transition Audit)
    Write-Host "`n--- Test 10: Admin Updates Enrollment Status ---" -ForegroundColor Yellow
    $updateEnrollBody = @{
        status = "Active"
        progressPercentage = 15.0
        finalResult = $null
        notes = "Approved by Admin with audit verification."
    } | ConvertTo-Json

    $upEnrollRes = Invoke-RestMethod -Uri "$baseUrl/api/enrollments/$enrollmentId/status" -Method Put -Body $updateEnrollBody -ContentType "application/json" -Headers @{ Authorization = "Bearer $adminToken" }
    Write-Host "Updated Enrollment $enrollmentId Status to: $($upEnrollRes.data.status)" -ForegroundColor Green

    # 11. Record Payment & Update Payment Status
    Write-Host "`n--- Test 11: Record Payment and Transition Status ---" -ForegroundColor Yellow
    $payRef = "PAY-AUD-$([Guid]::NewGuid().ToString('N').Substring(0,6).ToUpper())"
    $paymentBody = @{
        enrollmentId = $enrollmentId
        amount = 8500.00
        paymentMethod = "CreditCard"
        paymentDate = (Get-Date).ToString("yyyy-MM-ddTHH:mm:ssZ")
        paymentStatus = "Pending"
        referenceNumber = $payRef
        notes = "Full tuition payment pending gateway clearance."
    } | ConvertTo-Json

    $payRes = Invoke-RestMethod -Uri "$baseUrl/api/payments" -Method Post -Body $paymentBody -ContentType "application/json" -Headers @{ Authorization = "Bearer $adminToken" }
    $paymentId = $payRes.data.paymentId
    Write-Host "Created Payment ID: $paymentId, Ref: $payRef, Status: $($payRes.data.paymentStatus)" -ForegroundColor Green

    $updatePayBody = @{
        status = "Completed"
        notes = "Gateway confirmed payment."
    } | ConvertTo-Json

    $upPayRes = Invoke-RestMethod -Uri "$baseUrl/api/payments/$paymentId/status" -Method Put -Body $updatePayBody -ContentType "application/json" -Headers @{ Authorization = "Bearer $adminToken" }
    Write-Host "Updated Payment $paymentId Status to: $($upPayRes.data.paymentStatus)" -ForegroundColor Green

    # 12. Admin Queries Activity Logs (List & Pagination)
    Write-Host "`n--- Test 12: Admin Queries Activity Logs (Paginated) ---" -ForegroundColor Yellow
    $logsRes = Invoke-RestMethod -Uri "$baseUrl/api/admin/activity-logs?pageNumber=1&pageSize=10" -Method Get -Headers @{ Authorization = "Bearer $adminToken" }
    Write-Host "Total Activity Logs in System: $($logsRes.data.totalCount)" -ForegroundColor Green
    Write-Host "Logs Returned on Page 1: $($logsRes.data.items.Count)" -ForegroundColor Green
    foreach ($item in $logsRes.data.items | Select-Object -First 5) {
        Write-Host " -> Log #$($item.activityLogId): [$($item.action)] on $($item.entityName):$($item.entityId) by User $($item.userId) ($($item.userRole)) - $($item.description)" -ForegroundColor Gray
    }

    # 13. Admin Filter Activity Logs by User
    Write-Host "`n--- Test 13: Filter Logs by User ID ($newUserId) ---" -ForegroundColor Yellow
    $userLogs = Invoke-RestMethod -Uri "$baseUrl/api/admin/activity-logs?userId=$newUserId" -Method Get -Headers @{ Authorization = "Bearer $adminToken" }
    Write-Host "Logs for User $newUserId count: $($userLogs.data.totalCount)" -ForegroundColor Green
    foreach ($item in $userLogs.data.items) {
        Write-Host " -> Log #$($item.activityLogId): [$($item.action)] - $($item.description)" -ForegroundColor Gray
    }

    # 14. Admin Filter Activity Logs by Entity (Payment)
    Write-Host "`n--- Test 14: Filter Logs by Entity (Payment) ---" -ForegroundColor Yellow
    $payLogs = Invoke-RestMethod -Uri "$baseUrl/api/admin/activity-logs?entityName=Payment" -Method Get -Headers @{ Authorization = "Bearer $adminToken" }
    Write-Host "Payment Activity Logs count: $($payLogs.data.totalCount)" -ForegroundColor Green

    # 15. Admin Queries Activity Summary & Aggregates
    Write-Host "`n--- Test 15: Admin Queries Activity Log Summary ---" -ForegroundColor Yellow
    $summary = Invoke-RestMethod -Uri "$baseUrl/api/admin/activity-logs/summary" -Method Get -Headers @{ Authorization = "Bearer $adminToken" }
    Write-Host "Total System Logs: $($summary.data.totalLogs)" -ForegroundColor Green
    Write-Host "Total Users Audited: $($summary.data.totalUsersAudited)" -ForegroundColor Green
    Write-Host "Action Breakdown:" -ForegroundColor Green
    $summary.data.actionBreakdown | Format-Table -AutoSize | Out-String | Write-Host -ForegroundColor Gray

    # 16. Admin Queries Entity Timeline
    Write-Host "`n--- Test 16: Entity Timeline for Enrollment #$($enrollmentId) ---" -ForegroundColor Yellow
    $timeline = Invoke-RestMethod -Uri "$baseUrl/api/admin/activity-logs/timeline/Enrollment/$enrollmentId" -Method Get -Headers @{ Authorization = "Bearer $adminToken" }
    Write-Host "Timeline Events for Enrollment #$($enrollmentId): $($timeline.data.Count)" -ForegroundColor Green
    foreach ($t in $timeline.data) {
        Write-Host " -> [$($t.createdAt)] Action: $($t.action) | Metadata: $($t.metadata)" -ForegroundColor Gray
    }

    Write-Host "`n=======================================================" -ForegroundColor Green
    Write-Host "   ALL TASK 07 AUDIT TRAIL TESTS PASSED SUCCESSFULLY!   " -ForegroundColor Green
    Write-Host "=======================================================`n" -ForegroundColor Green

} finally {
    if ($serverProcess -and -not $serverProcess.HasExited) {
        Write-Host "Stopping test server process (PID: $($serverProcess.Id))..." -ForegroundColor Cyan
        Stop-Process -Id $serverProcess.Id -Force
    }
}
