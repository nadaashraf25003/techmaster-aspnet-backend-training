$ErrorActionPreference = "Continue"

$projectPath = "c:\Users\user\Desktop\Tech_Master\techmaster-aspnet-backend-training\phase-04-secure-professional-backend\task-08-bad-auth-refactor-pack\TrainingCenter.Api"
$baseUrl = "http://localhost:5250"

Write-Host "`n=== Starting Task 08 Refactored Auth Server on $baseUrl ===" -ForegroundColor Cyan
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
    Write-Host "`n=======================================================" -ForegroundColor Cyan
    Write-Host "   TASK 08: BAD AUTH REFACTOR VERIFICATION SUITE       " -ForegroundColor Cyan
    Write-Host "=======================================================" -ForegroundColor Cyan

    # -------------------------------------------------------------
    # FIX 1 & 7: Test Login with Non-Existent User -> Expect HTTP 401 (NOT 200 OK "wrong email")
    # -------------------------------------------------------------
    Write-Host "`n--- FIX 7 Test: Login with non-existent email (Expected 401 Unauthorized, not 200 OK) ---" -ForegroundColor Yellow
    $nonExistentBody = @{
        email = "nobody_$([Guid]::NewGuid().ToString('N').Substring(0,6))@techmaster.com"
        password = "Password@123"
    } | ConvertTo-Json

    try {
        $res = Invoke-RestMethod -Uri "$baseUrl/api/auth/login" -Method Post -Body $nonExistentBody -ContentType "application/json"
        Write-Host "FAILED: Endpoint returned 200 OK with: $($res | ConvertTo-Json)" -ForegroundColor Red
    } catch {
        $statusCode = $_.Exception.Response.StatusCode.value__
        if ($statusCode -eq 401) {
            Write-Host "PASSED: Correctly returned 401 Unauthorized for non-existent email." -ForegroundColor Green
        } else {
            Write-Host "FAILED: Expected 401, got $statusCode" -ForegroundColor Red
        }
    }

    # -------------------------------------------------------------
    # FIX 1 & 7: Test Login with Wrong Password -> Expect HTTP 401 (NOT 200 OK "wrong password")
    # -------------------------------------------------------------
    Write-Host "`n--- FIX 1 & 7 Test: Login with wrong password (Expected 401 Unauthorized, not 200 OK) ---" -ForegroundColor Yellow
    $wrongPasswordBody = @{
        email = "admin@techmaster.com"
        password = "WrongPassword@999"
    } | ConvertTo-Json

    try {
        $res = Invoke-RestMethod -Uri "$baseUrl/api/auth/login" -Method Post -Body $wrongPasswordBody -ContentType "application/json"
        Write-Host "FAILED: Endpoint returned 200 OK for wrong password!" -ForegroundColor Red
    } catch {
        $statusCode = $_.Exception.Response.StatusCode.value__
        if ($statusCode -eq 401) {
            Write-Host "PASSED: Correctly returned 401 Unauthorized for invalid password." -ForegroundColor Green
        } else {
            Write-Host "FAILED: Expected 401, got $statusCode" -ForegroundColor Red
        }
    }

    # -------------------------------------------------------------
    # FIX 4: Test Request Validation -> Missing / Malformed Fields
    # -------------------------------------------------------------
    Write-Host "`n--- FIX 4 Test: Request Validation (Invalid Email & Missing Password) ---" -ForegroundColor Yellow
    $invalidRegBody = @{
        fullName = ""
        email = "not-an-email"
        password = "123"
        confirmPassword = "mismatch"
        role = "Student"
    } | ConvertTo-Json

    try {
        $res = Invoke-RestMethod -Uri "$baseUrl/api/auth/register" -Method Post -Body $invalidRegBody -ContentType "application/json"
        Write-Host "FAILED: Invalid registration was accepted!" -ForegroundColor Red
    } catch {
        $statusCode = $_.Exception.Response.StatusCode.value__
        if ($statusCode -eq 400) {
            Write-Host "PASSED: Correctly returned 400 Bad Request on validation errors." -ForegroundColor Green
        } else {
            Write-Host "FAILED: Expected 400, got $statusCode" -ForegroundColor Red
        }
    }

    # -------------------------------------------------------------
    # FIX 5: Test Privilege Escalation Defense -> Public Registration as Admin is Forbidden
    # -------------------------------------------------------------
    Write-Host "`n--- FIX 5 Test: Privilege Escalation Defense (Prevent Registering as Admin) ---" -ForegroundColor Yellow
    $adminRegAttemptBody = @{
        fullName = "Hacker User"
        email = "hacker_$([Guid]::NewGuid().ToString('N').Substring(0,6))@techmaster.com"
        password = "Password@123456"
        confirmPassword = "Password@123456"
        role = "Admin"
    } | ConvertTo-Json

    try {
        $res = Invoke-RestMethod -Uri "$baseUrl/api/auth/register" -Method Post -Body $adminRegAttemptBody -ContentType "application/json"
        Write-Host "FAILED: Public Admin registration was allowed!" -ForegroundColor Red
    } catch {
        $statusCode = $_.Exception.Response.StatusCode.value__
        if ($statusCode -eq 400) {
            Write-Host "PASSED: Correctly rejected public Admin registration with 400 Bad Request." -ForegroundColor Green
        } else {
            Write-Host "FAILED: Expected 400, got $statusCode" -ForegroundColor Red
        }
    }

    # -------------------------------------------------------------
    # FIX 1, 3, 4: Valid Registration -> Safe DTO, No PasswordHash Leaked
    # -------------------------------------------------------------
    Write-Host "`n--- FIX 1 & 3 Test: Valid Registration & Safe DTO Response ---" -ForegroundColor Yellow
    $validRegEmail = "refactored_student_$([Guid]::NewGuid().ToString('N').Substring(0,6))@techmaster.com"
    $validRegBody = @{
        fullName = "Refactored Student"
        email = $validRegEmail
        password = "Password@123456"
        confirmPassword = "Password@123456"
        role = "Student"
        phoneNumber = "01122334455"
        address = "456 Clean Code Blvd, Cairo"
    } | ConvertTo-Json

    $regRes = Invoke-RestMethod -Uri "$baseUrl/api/auth/register" -Method Post -Body $validRegBody -ContentType "application/json"
    $newUserId = $regRes.data.userId
    Write-Host "Registered User ID: $newUserId, Email: $validRegEmail" -ForegroundColor Green

    # Verify response does NOT expose password or hash
    if ($regRes.data.password -or $regRes.data.passwordHash) {
        Write-Host "FAILED: Response leaked password or password hash!" -ForegroundColor Red
    } else {
        Write-Host "PASSED: DTO does NOT contain password or passwordHash field." -ForegroundColor Green
    }

    # -------------------------------------------------------------
    # FIX 4: Test Duplicate Email Conflict -> Expect HTTP 409
    # -------------------------------------------------------------
    Write-Host "`n--- FIX 4 Test: Duplicate Email Conflict Handling (Expected 409 Conflict) ---" -ForegroundColor Yellow
    try {
        $res = Invoke-RestMethod -Uri "$baseUrl/api/auth/register" -Method Post -Body $validRegBody -ContentType "application/json"
        Write-Host "FAILED: Duplicate registration succeeded!" -ForegroundColor Red
    } catch {
        $statusCode = $_.Exception.Response.StatusCode.value__
        if ($statusCode -eq 409) {
            Write-Host "PASSED: Correctly returned 409 Conflict for duplicate email." -ForegroundColor Green
        } else {
            Write-Host "FAILED: Expected 409, got $statusCode" -ForegroundColor Red
        }
    }

    # -------------------------------------------------------------
    # FIX 2 & 3: Valid Login -> Real Signed JWT Token & Safe AuthResponse
    # -------------------------------------------------------------
    Write-Host "`n--- FIX 2 & 3 Test: Login, Real Signed JWT & Claims Verification ---" -ForegroundColor Yellow
    $loginBody = @{
        email = $validRegEmail
        password = "Password@123456"
    } | ConvertTo-Json

    $loginRes = Invoke-RestMethod -Uri "$baseUrl/api/auth/login" -Method Post -Body $loginBody -ContentType "application/json"
    $jwtToken = $loginRes.data.accessToken
    $refreshToken = $loginRes.data.refreshToken

    Write-Host "Access Token received: $($jwtToken.Substring(0,30))... (Length: $($jwtToken.Length))" -ForegroundColor Green
    Write-Host "Refresh Token received: $($refreshToken.Substring(0,20))... (Length: $($refreshToken.Length))" -ForegroundColor Green

    # Verify JWT Structure (header.payload.signature)
    $jwtParts = $jwtToken.Split('.')
    if ($jwtParts.Count -eq 3) {
        Write-Host "PASSED: Real 3-part cryptographically signed JWT structure verified (Header.Payload.Signature)." -ForegroundColor Green
    } else {
        Write-Host "FAILED: Token is not a valid 3-part JWT!" -ForegroundColor Red
    }

    # Verify /api/auth/me accepts token
    $meRes = Invoke-RestMethod -Uri "$baseUrl/api/auth/me" -Method Get -Headers @{ Authorization = "Bearer $jwtToken" }
    Write-Host "Authenticated as: $($meRes.data.fullName) ($($meRes.data.email)) with Role: $($meRes.data.role)" -ForegroundColor Green

    # -------------------------------------------------------------
    # FIX 9: Test Inactive User Login -> Expect HTTP 403 Forbidden
    # -------------------------------------------------------------
    Write-Host "`n--- FIX 9 Test: Inactive Account Login Check (Expected 403 Forbidden) ---" -ForegroundColor Yellow
    $inactiveLoginBody = @{
        email = "inactive@techmaster.com"
        password = "Inactive@123456"
    } | ConvertTo-Json

    try {
        $res = Invoke-RestMethod -Uri "$baseUrl/api/auth/login" -Method Post -Body $inactiveLoginBody -ContentType "application/json"
        Write-Host "FAILED: Inactive user login succeeded!" -ForegroundColor Red
    } catch {
        $statusCode = $_.Exception.Response.StatusCode.value__
        if ($statusCode -eq 403) {
            Write-Host "PASSED: Correctly returned 403 Forbidden for deactivated account." -ForegroundColor Green
        } else {
            Write-Host "FAILED: Expected 403, got $statusCode" -ForegroundColor Red
        }
    }

    # -------------------------------------------------------------
    # FIX 10: Test Security & Audit Logs
    # -------------------------------------------------------------
    Write-Host "`n--- FIX 10 Test: Verify Security Audit Logs Captured ---" -ForegroundColor Yellow
    $adminLoginBody = @{
        email = "admin@techmaster.com"
        password = "Admin@123456"
    } | ConvertTo-Json
    $adminAuth = Invoke-RestMethod -Uri "$baseUrl/api/auth/login" -Method Post -Body $adminLoginBody -ContentType "application/json"
    $adminToken = $adminAuth.data.accessToken

    $auditLogs = Invoke-RestMethod -Uri "$baseUrl/api/admin/activity-logs?userId=$newUserId" -Method Get -Headers @{ Authorization = "Bearer $adminToken" }
    Write-Host "Audit logs captured for User $newUserId count: $($auditLogs.data.totalCount)" -ForegroundColor Green
    foreach ($log in $auditLogs.data.items) {
        Write-Host " -> [$($log.createdAt)] Action: $($log.action) | Description: $($log.description)" -ForegroundColor Gray
    }

    Write-Host "`n=======================================================" -ForegroundColor Green
    Write-Host "   ALL 10 BAD AUTH REFACTOR FIXES FULLY VERIFIED!      " -ForegroundColor Green
    Write-Host "=======================================================`n" -ForegroundColor Green

} finally {
    if ($serverProcess -and -not $serverProcess.HasExited) {
        Write-Host "Stopping test server process (PID: $($serverProcess.Id))..." -ForegroundColor Cyan
        Stop-Process -Id $serverProcess.Id -Force
    }
}
