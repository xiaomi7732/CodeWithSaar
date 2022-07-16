# Example code for understanding CORS

## Project structure

src - source files;
thunber-tests - http requests

Under `src`:

- BE: Backend - that used to demo defining CORS policies;
- FE: Frontend - a simple static content host, hosts a simple html page with some javascript trying to hit the backend;
- HC: HttpClient - a simple httpClient code to access backend - to see that CORS could be bypassed;

## To run the demo

1. Start the backend:

```shell
dotnet run --project src/BE --urls="https://localhost:8080"
```

1. Start the frontend:

```shell
dotnet run --project src/FE --urls="https://localhost:8081"
```

Notice, the url for backend is hard-coded to `https://localhost:8080` in the frontend.

1. At any moment, try hit the backend using the http client that does not implement CORS

```shell
dotnet run --project src/HC
```