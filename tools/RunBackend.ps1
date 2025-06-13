[CmdletBinding()]
param(
    [Parameter(Mandatory = $false)]
    [string]$FacadePath = "..\..\epr-prn-rep-exp-facade\src\Epr.Reprocessor.Exporter.Facade.Api\Epr.Reprocessor.Exporter.Facade.Api.csproj",

    [Parameter(Mandatory = $false)]
    [string]$CommonBackendPath = "..\..\epr-prn-common-backend\src",

    [Parameter(Mandatory = $false)]
    [string]$FacadeLaunchProfile = "https"
)
Process {  

    Register-EngineEvent PowerShell.Exiting -Action {
        docker-compose down
    }
    
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