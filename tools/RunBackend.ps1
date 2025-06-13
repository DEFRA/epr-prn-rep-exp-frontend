[CmdletBinding()]
param(
    [Parameter(Mandatory = $false)]
    [string]$FacadePath = "..\..\epr-prn-rep-exp-facade\src\Epr.Reprocessor.Exporter.Facade.Api\Epr.Reprocessor.Exporter.Facade.Api.csproj",

    [Parameter(Mandatory = $false)]
    [string]$CommonBackendPath = "..\..\epr-prn-common-backend\src",

    [Parameter(Mandatory = $false)]
    [string]$FacadeLaunchProfile = "https",

    [Parameter(Mandatory = $false)]
    [string]$FacadeBranchToCheckout,

    [Parameter(Mandatory = $false)]
    [string]$BackendBranchToCheckout 
)
Process {  

    if ($FacadeBranchToCheckout) {
        Write-Host "Checking out branch '$FacadeBranchToCheckout' for Facade project at $FacadePath"
        git -C $FacadePath checkout $FacadeBranchToCheckout
    }

    if ($BackendBranchToCheckout) {
        Write-Host "Checking out branch '$BackendBranchToCheckout' for Common Backend project at $CommonBackendPath"
        git -C $CommonBackendPath checkout $BackendBranchToCheckout
    }

    Write-Host "Starting Backend project at $CommonBackendPath using launch profile '$FacadeLaunchProfile')"    
    docker compose -f "$CommonBackendPath\docker-compose.yml" -f "$CommonBackendPath\docker-compose.override.yml" up -d --build

    $FacadePath = Resolve-Path -Path $FacadePath

    if (Test-Path -Path $FacadePath) {
        Write-Host "Starting Facade project at $FacadePath using launch profile '$FacadeLaunchProfile')"

        dotnet run --project $FacadePath --launch-profile $FacadeLaunchProfile

        Write-Host "Facade project found at $FacadePath"
    } 
    else {
        Write-Host "Facade project not found at $FacadePath"
        exit 1
    }

    docker compose -f "$CommonBackendPath\docker-compose.yml" -f "$CommonBackendPath\docker-compose.override.yml" up -d --build
}