# GitHub Hosted App

This repository is an illustrative template for deploying a web application using GitHub exclusively. It demonstrates a straightforward and effective method to automate the deployment process without relying on external hosting, leveraging GitHub's built-in capabilities.

## Key Features:

1. No external hosting required
2. Free for Public repositories

## How it works

Frontend is a Blazor WASM app compiled into a static website, and any frontend like React, Angular, etc., can be used in its place. It is deployed using [GitHub Pages](https://pages.github.com/). You also can use [custom domain](https://docs.github.com/en/pages/configuring-a-custom-domain-for-your-github-pages-site) here.

With the backend part, it's a bit more complex. We launch it using GitHub Actions, so there are a few considerations to keep in mind: the job execution cannot exceed 6 hours, we don't have a static IP, and there's no external port within the workflow execution scope. Thus, we need to restart the application periodically, preferably without downtime, and also utilize an external HTTP tunnel. Backend can also be written using any languages or frameworks.

The main logic of operation is summarized in the following sequence:
1. Publish Frontend to GitHub Pages
2. Run Backend app
3. Launch an HTTPS tunnel and obtain its public URL
4. Publish the public URL to a text file in a dedicated branch of the repository
5. Frontend requests the public URL from the text file before accessing to backend
6. For Backend, a restart schedule is set every 4 hours
7. Backend continuously checks that this instance is available at the address from the text file. If it's not, it terminates its operation


## Project Structure:

- `.github/workflows/`:
  - `ci.yaml`: CI workflow. Includes steps for building, publishing the application
  - `hosting.yaml`: Hosting workflow. Includes steps for hosting backend part of the application

- `.hosting/`:
  - `docker-compose.yaml`: docker compose for Backend and an HTTPs tunnel
  - `start-tunnel.sh`: launch the HTTPs tunnel, track changes in the public URL and publish it to Git
  - `monitor-app.sh`: monitor that Backend is still running, so if the app is terminated, then terminate the tunnel too

- `src/`:
  - `Frontend/`: Frontend app based on [Blazor WebAssembly](https://learn.microsoft.com/en-us/aspnet/core/blazor/#blazor-webassembly)
  - `Backend/`: Backend app based on [Minimal API WebApplication](https://learn.microsoft.com/en-us/aspnet/core/fundamentals/minimal-apis/webapplication#webapplication)

## HTTPS Tunnel:

This template uses https://github.com/localtunnel/localtunnel to organize a gateway to the backend. You can use any other service like ngrok or something else. However, keep in mind that ngrok allows only one tunnel per region in the free plan, so smooth migration between backend instances is not possible.

## Usage Instructions:

TODO
