# It's a damn Url shorterner

(what i wrote)
download the nuget packages

replace in
Config.json

```Json
"db_path": "/Users/stefan/projects/csharp/url-shortener/Data/Urls.db"
```

with the path you want to store your data

build and press play i guess

hey it works on my machine atleast

todos
[X] verify that the url that is to be shortened exists

## What CHATGPT WROTE for me

## URL Shortener Web Application

This is a simple URL shortener web application built using C# and ASP.NET Core. It allows users to shorten long URLs into shorter, more manageable links.

## Table of Contents

- [Features](#features)
- [Prerequisites](#prerequisites)
- [Installation](#installation)
- [Usage](#usage)
- [Contributing](#contributing)
- [License](#license)

## Features

- Shorten long URLs into shorter, more convenient links.
- Check the existence and reachability of a URL.
- View a list of all shortened URLs.
- Redirect to the original URL using the shortened link.
- RESTful API for URL shortening.

## Prerequisites

Before you begin, ensure you have met the following requirements:

- **.NET SDK:** Make sure you have the .NET SDK installed on your machine. You can download it from the [official .NET website](https://dotnet.microsoft.com/download).

## Installation

To install and run the project locally, follow these steps:

1. Clone this repository to your local machine:

   ```shell
   git clone https://github.com/2lach/url-shortener.git

```

`cd url-shortener`

```dotnet
dotnet build
dotnet run
```

### Usage

Access the homepage of the web application.
Enter a long URL that you want to shorten and click the "Shorten" button.
The application will generate a shortened URL for you.
You can also use the API endpoint to shorten URLs programmatically.
To check the existence of a URL, provide the URL to the "Check URL" endpoint.
To view all shortened URLs, go to the "/all" route.
Contributing
Contributions are welcome! If you'd like to contribute to this project, please follow these steps:

### Fork the repository

Create a new branch for your feature or bug fix.
Make your changes and test them.
Submit a pull request with a clear description of your changes.
License
This project is licensed under the MIT License. See the LICENSE file for details.
