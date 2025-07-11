# EPR PRN REP EXP Frontend

A frontend application for the **Environment Agency**'s Electronic Packaging Reporting system. This project facilitates reporting and management of **Packaging Recovery Notes (PRNs)** and **Export Recovery Notes (PERNs)**, under the **Extended Producer Responsibility (EPR)** regulations.

---

## 🧰 Technologies Used

- **dotnet 8** (runtime)
- **MVC** (frontend framework)
- **GOV.UK Frontend** (UI components)
- **Azure AD B2C** (authentication)
- **Redis** (session management)

---

## Technical Overview

### Development

The application is written in dotnet 8 using the MVC framework for the frontend whilst following the GDS Frontend Design System for designing and creating UI elements.

```text
├── 📁 src/
│   ├── 📁 Epr.Reprocessor.Exporter.UI/
│   ├── 📁 Epr.Reprocessor.Exporter.UI.App/
│   ├── 📁 Epr.Reprocessor.Exporter.UI.App.UnitTests/
│   └── 📁 Epr.Reprocessor.Exporter.UI.UnitTests/
│── 📄 README.md 
├── 📄 LICENSE   
└── 📄 .gitignore
```

## 🚀 Getting Started

### Prerequisites

- .NET SDK & Runtime (minimum version 8) - [Download Here](https://dotnet.microsoft.com/en-us/download/dotnet/8.0)
- Editor of choice - Visual Studio, VS Code, Rider etc
- Azure Artifacts NuGet Credentials
- Azure Artifacts Credentials Provider - if using command line, follow Setup instructions below (step 1)
- Credentials in Azure B2C
- Postman
- Clone depedant repos

### Installation

In order to run the end to end solution all the below 3 repos need cloning using the below commands

```bash
git clone https://github.com/DEFRA/epr-prn-rep-exp-frontend.git
git clone https://github.com/DEFRA/epr-prn-rep-exp-facade.git
git clone https://github.com/DEFRA/epr-prn-common-backend.git
```

### Setup

#### 1. Azure Artifacts Credential Provider

in the setup folder at the root of this repo, run the 'install-azure-credentials-provider.ps1', this will install the required tool that will allow interactive authentication with the defra Azure Artifacts Feed.

#### 2. Build Code

This readme focuses on setting up the frontend with the assumption that you have already followed the steps to setup the facade and backend layers.

Assuming a root folder of repos, navigate to the epr-prn-rep-exp-frontend\src under repos and run the below commands.

```pwsh
dotnet restore --interactive
dotnet build --force
```

The '--interactive' leverages the previously installed Azure Artifacts Credentials Provider to allow for interactive authentication to Azure. When you run that you'll be asked to follow a url and be provided a code that will then allow you to use your defra account to authenticate with the Defra Azure Artifacts Private Feed that is required to pull down the NuGet packages.

#### 3. App Settings

The below settings are required, for where values are empty, these need to be provided by a member of the team and stored in user secrets.

| Key                                                    | Value                |
|---------------------------------------------------------|----------------------|
| AzureAdB2C:Instance                                     |                      |
| AzureAdB2C:Domain                                       |                      |
| AzureAdB2C:ClientSecret                                 |                      |
| AzureAdB2C:ClientId                                     |                      |
| Redis:ConnectionString                                  | localhost:6379       |
| AccountsFacadeAPI:BaseEndpoint                          |                      |
| AccountsFacadeAPI:DownstreamScope                       |                      |
| UseLocalSession                                         | true                 |

#### 4. Dev Certs

Dev certs are required to enable proper running of https locally, run the below commands and accept any prompts that are displayed.

```pwsh
dotnet dev-certs https
dotnet dev-certs https --trust
```

#### 5. Run Application

Assuming you have followed the setup for the facade and backend, you can now run the frontend application.

```pwsh
dotnet run --project .\Epr.Reprocessor.Exporter.UI.csproj --launch-profile https
```

The frontend will now be listening on https://localhost:7068/

Alternatively, 

In the tools folder, there is a RunBackend.ps1 script that when ran with the -RunFrontFrond switch, will start up the backend, facade and the frontend, assuming the first two repos have been setup.


## Helpful tools

| .gitaliases

Within this file is shared git aliases that can be setup for all users of the repo, it contains helpful git commands that can be configured within the git config on your local machine so that you can run custom git commands behind a shorthand alias. 

### Setup

To setup, simply run the setup-git-aliases.bat script in the folder and this will include the file into your git global config

### Current aliases

The below will give a quick one liner about whether your current branch is upto date with the main branch, if it is not, it will print out the commits that are in main that are NOT in your branch.
```git
git mainstatus 
```

## Contributing to this project

Please read the [contribution guidelines](CONTRIBUTING.md) before submitting a pull request.

## Licence

[Licence information](LICENCE.md).