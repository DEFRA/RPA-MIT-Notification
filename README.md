# rpa-mit-notification

This repository contains an azure function with a Service Bus trigger, the messages to the service bus are sent via other services, its use is as a method of sending email notifications to recipients at key times in the manual invoice process, such as when a user action is required.
## Requirements

Amend as needed for your distribution, this assumes you are using windows with WSL. 

- <details>
    <summary> .NET 8 SDK </summary>
    
    #### Basic instructions for installing the .NET 8 SDK on a debian based system.
  
    Amend as needed for your distribution.

    ```bash
    wget https://packages.microsoft.com/config/debian/12/packages-microsoft-prod.deb -O packages-microsoft-prod.deb
    sudo dpkg -i packages-microsoft-prod.deb
    sudo apt-get update && sudo apt-get install -y dotnet-sdk-8.0
    ```
</details>

- <details>
    <summary> Azure Functions Core Tools </summary>
    
    ```bash
    sudo apt-get install azure-functions-core-tools-4
    ```
</details>

- [Docker](https://docs.docker.com/desktop/install/linux-install/)
- GOV.UK Notify credentials
- Service Bus Queue

---
## Local Setup

To run this service locally complete the following steps.
### Create Local Settings

Create a local.setttings.json file with the following content.

```json
{
    "IsEncrypted": false,
    "Values": {
        "AzureWebJobsStorage": "UseDevelopmentStorage=true",
        "FUNCTIONS_WORKER_RUNTIME": "dotnet-isolated"
    }
}
```

### Set up user secrets

Use the secrets-template to create a "secrets.json" in the same folder location as the [RPA.MIT.Notification.Function.csproj](RPA.MIT.Notification.Function/RPA.MIT.Notification.Function.csproj) file.

Once this is done run the following command to add the projects user secrets

```bash
cat secrets.json | dotnet user-secrets set
```

These values can also be added to the local settings file, but the preferred method is via user secrets.

Pay special attention to the following keys, these values must be taken from[ GOV.UK Notify](https://www.notifications.service.gov.uk). 

The API key from your Notify Service.

```json
{
	"NotifyApiKey": ""
}
```

These are the template ids setup in the service, they will be in standard GUID format. 

```json 
{
    "templatesRequesterApproval": "",
    "templatesRequesterApproved": "",
    "templatesRequesterRejected": "",
    "templatesApproverApproval": "",
    "templatesApproverApproved": "",
    "templatesApproverRejected": "",
    "templatesError": "",
    "templatesUploaded": ""
}
```

### Create emulated table storage

You need to create a local emulation of azure table storage, this can be done using [azurite](https://github.com/Azure/Azurite).

In your console run the following commands.

```bash
docker pull mcr.microsoft.com/azure-storage/azurite
```

```bash
docker run --name azurite -p 10000:10000 -p 10001:10001 -p 10002:10002 mcr.microsoft.com/azure-storage/azurite
```

You can view the emulated storage using a tool such as [Azure Storage Explorer](https://github.com/microsoft/AzureStorageExplorer).
### Startup

To start the function locally.

```bash
func start
```

If running multiple function apps locally you might encounter a port conflict error as they all try to run on port 7071. If this happens use a command such as this entering a port that is free.

```bash
func start --port 7072
```


---
## Running in Docker

To create the application as a docker container run the following command in the parent directory.

```bash
docker compose up
```

---
## Usage / Endpoints

### Queue

Below is an **encoded** example message that can be added to the service bus queue to test functionality. Json messages format must be encoded as base64 to be accepted.

```base64
ewogICAgIkFjdGlvbiI6ICJSZXF1ZXN0ZXJBcHByb3ZhbCIsCiAgICAiRGF0YSI6CiAgICAgICAgewogICAgICAgICAgICAiaW52b2ljZUlkIjogIjEyMzQ1IiwKICAgICAgICAgICAgImxpbmsiOiAiaHR0cHM6Ly9nb29nbGUuY29tIiwKICAgICAgICAgICAgIm5hbWUiOiAiTG9ybmEgQ29sZSIsCiAgICAgICAgICAgICJzY2hlbWVUeXBlIjogImJwcyIsCiAgICAgICAgICAgICJ2YWx1ZSI6ICIyNTAiCiAgICAgICAgfSwKICAgICJJZCI6ICIxMjM0NTY3ODkiLAogICAgIlNjaGVtZSI6ICJicHMiLAogICAgIkVtYWlsUmVjaXBpZW50IjoibG9ybmEuY29sZUBkb21haW4udGxkIgp9
```

