# YouCam Account and Secret Setup

This runbook intentionally contains no API key, secret key, redemption code, or
real customer image.

## Security rule

Any key pasted into chat, email, an issue, a screenshot, source code, or a
terminal transcript must be treated as compromised. Delete or rotate it in the
YouCam console before using TryOnReady.

TryOnReady reads the provider credential only from server-side configuration:

```text
YouCam__Enabled=true
YouCam__ApiKey=<rotated key>
```

Do not put the real value in `appsettings.json`, `.env.example`, Markdown, a
frontend environment variable, or a GitHub Actions file.

## Redeem the hackathon units

1. Sign in to the YouCam API console with the account registered for the
   hackathon.
2. Open **Redeem Code** in the account navigation.
3. Copy the redemption code from the official hackathon email.
4. Paste it into the YouCam console and confirm the redemption.
5. Open **Usage** or the account balance and confirm that 1,000 API units were
   added.
6. Do not copy the redemption code into this repository.

Redeeming the code changes the YouCam account only. It does not configure the
application.

## Rotate and save a server key locally

The key previously shown in conversation must not be reused.

1. Open **API Keys** in the YouCam console.
2. Delete the exposed key.
3. Select **Generate new API Key** and give it a description such as
   `TryOnReady local and Fly demo`.
4. Copy the new key once. Do not paste it into chat.
5. In Visual Studio, right-click `TryOnReady.Api`, choose
   **Manage User Secrets**, and add:

   ```json
   {
     "YouCam": {
       "Enabled": true,
       "ApiKey": "PASTE_THE_ROTATED_KEY_HERE"
     }
   }
   ```

6. Save the file. Visual Studio User Secrets are stored outside the repository.
7. Restart the API so configuration is reloaded.

For command-line development, use .NET User Secrets rather than a repository
`.env` file:

```powershell
dotnet user-secrets set "YouCam:Enabled" "true" --project .\src\TryOnReady.Api
dotnet user-secrets set "YouCam:ApiKey" "PASTE_THE_ROTATED_KEY_HERE" --project .\src\TryOnReady.Api
```

The second command can be entered directly in a private terminal. Never paste
its completed command or output into a task, issue, or document.

## Fly.io secret

After the Fly application exists, set the same rotated key as a Fly secret:

```powershell
fly secrets set YouCam__Enabled=true YouCam__ApiKey=PASTE_THE_ROTATED_KEY_HERE --app tryonready
```

Fly injects secrets as runtime environment variables. The value does not belong
in `fly.toml`.

## Controlled live test

1. Pass all unit, integration, and Playwright tests in simulated-provider mode.
2. Confirm the dashboard's starting unit count.
3. Use one authorized synthetic garment image and one authorized synthetic
   person image.
4. Submit once and do not repeatedly click the action.
5. Confirm the task reaches `Succeeded`, a result appears, and one task is
   recorded.
6. Confirm the duplicate-request test returns the existing job without creating
   another provider task.
7. Compare the TryOnReady unit record with the YouCam **Usage** page.
8. Record the date, test asset names, result, and units in
   `docs/IMPLEMENTATION-LOG.md`; never record the key or signed image URL.

