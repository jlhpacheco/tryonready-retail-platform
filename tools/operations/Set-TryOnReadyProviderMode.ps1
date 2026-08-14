param(
    [Parameter(Mandatory = $true)]
    [ValidateSet("Enable", "Disable")]
    [string]$Mode
)

$ErrorActionPreference = "Stop"
$appName = "tryonready-demo"
$flyPath = "C:\Users\jlhpa\.fly\bin\fly.exe"
$statusUri = "https://tryonready-demo.fly.dev/api/status"
$logDirectory = Join-Path $env:LOCALAPPDATA "TryOnReady"
$logPath = Join-Path $logDirectory "provider-mode-automation.log"

New-Item -ItemType Directory -Path $logDirectory -Force | Out-Null

function Write-OperationLog {
    param([string]$Message)

    $timestamp = Get-Date -Format "yyyy-MM-ddTHH:mm:ssK"
    Add-Content -LiteralPath $logPath -Value "$timestamp $Message"
}

function Invoke-FlyModeChange {
    param(
        [string]$Enabled,
        [string]$SimulationEnabled
    )

    $attempt = 0
    while ($attempt -lt 3) {
        $attempt++
        & $flyPath secrets set `
            --app $appName `
            "YouCam__Enabled=$Enabled" `
            "YouCam__SimulationEnabled=$SimulationEnabled"

        if ($LASTEXITCODE -eq 0) {
            return
        }

        Write-OperationLog "Fly mode update attempt $attempt failed."
        Start-Sleep -Seconds 30
    }

    throw "Fly mode update failed after three attempts."
}

function Wait-ForExpectedStatus {
    param(
        [string]$ExpectedProviderMode,
        [bool]$ExpectedLiveIntegration
    )

    for ($attempt = 1; $attempt -le 12; $attempt++) {
        try {
            $status = Invoke-RestMethod -Uri $statusUri -TimeoutSec 30
            if (
                $status.providerMode -eq $ExpectedProviderMode -and
                [bool]$status.liveYouCamIntegration -eq $ExpectedLiveIntegration
            ) {
                return $status
            }
        }
        catch {
            Write-OperationLog "Health verification attempt $attempt failed: $($_.Exception.Message)"
        }

        Start-Sleep -Seconds 15
    }

    throw "TryOnReady did not reach the expected provider state."
}

try {
    Write-OperationLog "Starting $Mode operation for $appName."

    if ($Mode -eq "Enable") {
        Invoke-FlyModeChange -Enabled "true" -SimulationEnabled "false"
        $status = Wait-ForExpectedStatus `
            -ExpectedProviderMode "YouCamLive" `
            -ExpectedLiveIntegration $true
        Write-OperationLog "Enable verified: provider=$($status.providerMode), live=$($status.liveYouCamIntegration)."
    }
    else {
        Invoke-FlyModeChange -Enabled "false" -SimulationEnabled "true"
        $status = Wait-ForExpectedStatus `
            -ExpectedProviderMode "StoredReplay" `
            -ExpectedLiveIntegration $false
        Write-OperationLog "Disable verified: provider=$($status.providerMode), live=$($status.liveYouCamIntegration), newRequests=$($status.playbackMakesNewProviderRequests)."
    }

    exit 0
}
catch {
    Write-OperationLog "ERROR during $Mode operation: $($_.Exception.Message)"
    exit 1
}
