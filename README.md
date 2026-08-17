# Todo List Application

This repository contains a full-stack Todo List application featuring a .NET backend and an Angular frontend client.

## Project Structure

* **`TodoAPI/`**: The backend .NET Web API project.
* **`TodoAPI.Tests/`**: The test project containing unit tests specifically for the API controllers.
* **`TodoClient/`**: The frontend Angular client application.

## Getting Started

Follow these steps to get the application up and running locally.

### Step 1: Start the Backend API

The backend API must be running first, as the Angular client relies on it to fetch and save tasks. It is configured to run on port `5001`.

1. Open your terminal or command prompt.
2. Navigate to the API directory:
```bash
cd TodoAPI
```

3. Start the application:
```bash
dotnet run
```

*Note: The API will listen on `http://localhost:5001`, and the frontend communicates with the endpoint at `http://localhost:5001/todo`.*

### Step 2: Start the Frontend Client

Once the .NET API is running, you can launch the Angular client.

1. Open a **new** terminal window.
2. Navigate to the client directory:
```bash
cd TodoClient
```

3. Start the Angular development server:
```bash
ng serve
```

4. Open your web browser and navigate to **[http://localhost:4200/](http://localhost:4200/)** to view and use the application.
